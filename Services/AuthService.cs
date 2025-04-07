using Microsoft.EntityFrameworkCore;
using sge_api.Data;
using sge_api.Models;
using System.Security.Cryptography;
using BCrypt.Net;

namespace sge_api.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AuthService(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Generar código de verificación
        public async Task<string> GenerarCodigoVerificacion(string numeroIdentificacion)
        {
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.NumeroIdentificacion == numeroIdentificacion);

            if (empleado == null)
                return "NOT_FOUND"; // No existe el empleado en la base de datos

            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.EmpleadoId == empleado.Id);

            try
            {
                if (existingUser == null)
                {
                    existingUser = new Users
                    {
                      EmpleadoId = empleado.Id,
                      // NumeroIdentificacion = empleado.NumeroIdentificacion,
                      Usuario = await GenerarUsuarioUnico(empleado),
                      // PasswordHash = BCrypt.Net.BCrypt.HashPassword(GenerarContraseñaAleatoria()),
                      PasswordHash = GenerarContraseñaAleatoria(),
                      Estado = "Inactivo",
                      FechaRegistro = DateTime.UtcNow
                    };

                    _context.Usuarios.Add(existingUser);
                    await _context.SaveChangesAsync();

                    // Asignar Email Corporativo
                    await GenerarEmailCorporativo(empleado, existingUser.Usuario);

                    // Registrar evento: usuario creado
                    await RegistrarEventoUsuario(
                        existingUser.Id,
                        empleado.Id,
                        "registro",
                        true,
                        razon: "Usuario creado exitosamente"
                    );
                }
            }
            catch (DbUpdateException ex)
            {
                // Aquí puedes revisar si la excepción es por duplicado
                if (ex.InnerException?.Message.Contains("duplicate") == true ||
                    ex.Message.Contains("IX_Usuarios_NumeroIdentificacion")) // por ejemplo
                {
                    return "USER_ALREADY_EXISTS";
                }

                // O cualquier otro tipo de excepción
                return "ERROR_CREATING_USER";
            }

            // Verifica si tiene un código usado
            var usedCode = await _context.CodigosVerificacion
                .Where(c => c.UsuarioId == existingUser.Id && c.Tipo == "Registro" && c.Usado)
                .FirstOrDefaultAsync();

            if (usedCode != null)
                return "ACCOUNT_ALREADY_ACTIVE";

            // Verifica si tiene un código aún válido
            var existingCode = await _context.CodigosVerificacion
              .Where(c => c.UsuarioId == existingUser.Id && c.Tipo == "Registro")
              .OrderByDescending(c => c.FechaGeneracion)
              .FirstOrDefaultAsync();

            if (existingCode != null && !existingCode.Usado && existingCode.Expiracion > DateTime.UtcNow)
                return "CODE_ALREADY_SENT";

            // Verifica si Tiene código anterior expirado, Lo elimina y genera uno nuevo
            if (existingCode != null)
            {
                _context.CodigosVerificacion.Remove(existingCode);
                await _context.SaveChangesAsync();
            }

            var verificationCode = GenerateVerificationCode();
            var codeEntry = new CodigosVerificacion
            {
                UsuarioId = existingUser.Id,
                Codigo = verificationCode,
                Tipo = "Registro",
                FechaGeneracion = DateTime.UtcNow,
                Expiracion = DateTime.UtcNow.AddMinutes(15),
                Usado = false
            };

            _context.CodigosVerificacion.Add(codeEntry);
            await _context.SaveChangesAsync();

            // Enviar el correo en segundo plano
            _ = Task.Run(async () =>
            {
                await _emailService.SendVerificationEmail(empleado.EmailPersonal, verificationCode);
            });

            return "OK";
        }

        // Completar el registro tras verificar el código
        public async Task<string> CompletarRegistro(string numeroIdentificacion, string codigo)
        {
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.NumeroIdentificacion == numeroIdentificacion);
            if (empleado == null)
                return "NOT_FOUND";

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.EmpleadoId == empleado.Id);
            if (user == null)
                return "NO_USER";

            var usedCode = await _context.CodigosVerificacion
                .Where(c => c.UsuarioId == user.Id && c.Tipo == "Registro" && c.Usado)
                .FirstOrDefaultAsync();

            if (usedCode != null)
                return "ACCOUNT_ALREADY_ACTIVE";

            var codeEntry = await _context.CodigosVerificacion
                .Where(c => c.UsuarioId == user.Id && c.Tipo == "Registro" && !c.Usado && c.Expiracion > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (codeEntry == null || codeEntry.Codigo != codigo)
            {
                await RegistrarEventoUsuario(user.Id, empleado.Id, "registro", false, razon: "Código inválido");
                return "INVALID_CODE";
            }

            codeEntry.Usado = true;
            user.Estado = "Activo";
            await _context.SaveChangesAsync();

            await RegistrarEventoUsuario(user.Id, empleado.Id, "registro", true, razon: "Cuenta activada correctamente");

            _ = Task.Run(() =>
            {
                _emailService.SendUserCredentials(
                    empleado.EmailPersonal,
                    user.Usuario,
                    "Tu contraseña fue establecida en el registro."
                );
            });

            return "OK";
        }

        // Autenticación de usuario
        public async Task<Users> AuthenticateUser(string usuario, string password)
        {
            var user = await _context.Usuarios
                .Include(u => u.Empleado)
                .ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(u => u.Usuario == usuario);

            if (user == null || user.PasswordHash != password)
            {
                if (user != null)
                {
                    await RegistrarEventoUsuario(
                        user.Id,
                        user.EmpleadoId,
                        "intento_acceso",
                        false,
                        ip: "127.0.0.1",
                        navegador: "Desconocido",
                        razon: "Credenciales inválidas"
                    );
                }

                return null;
            }

            if (user.Estado != "Activo")
            {
                await RegistrarEventoUsuario(
                    user.Id,
                    user.EmpleadoId,
                    "intento_acceso",
                    false,
                    ip: "127.0.0.1",
                    navegador: "Desconocido",
                    razon: "Usuario inactivo"
                );

                return null;
            }

            if (user.FechaUltimoLogin == null)
            {
                user.FechaUltimoLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            await RegistrarEventoUsuario(
                user.Id,
                user.EmpleadoId,
                "sesion",
                true,
                ip: "127.0.0.1",
                navegador: "Desconocido"
            );

            return user;
        }

        // Generar usuario único
        private async Task<string> GenerarUsuarioUnico(Empleado empleado)
        {
            string usuario;
            int intento = 1;

            do
            {
                switch (intento)
                {
                    case 1:
                        usuario = $"{empleado.PrimerNombre.ToLower()}.{empleado.ApellidoPaterno.ToLower()}";
                        break;
                    case 2:
                        usuario = !string.IsNullOrEmpty(empleado.SegundoNombre)
                            ? $"{empleado.SegundoNombre.ToLower()}.{empleado.ApellidoPaterno.ToLower()}"
                            : $"{empleado.PrimerNombre.Substring(0, 1).ToLower()}.{empleado.ApellidoPaterno.ToLower()}";
                        break;
                    case 3:
                        usuario = $"{empleado.PrimerNombre.Substring(0, 1).ToLower()}.{empleado.ApellidoPaterno.ToLower()}";
                        break;
                    default:
                        usuario = $"{empleado.PrimerNombre.ToLower()}.{empleado.ApellidoPaterno.ToLower()}{intento}";
                        break;
                }

                intento++;

            } while (await _context.Usuarios.AnyAsync(u => u.Usuario == usuario));

            return usuario;
        }

        // Generar Email Corporativo
        private async Task GenerarEmailCorporativo(Empleado empleado, string usuario)
        {
            if (!string.IsNullOrEmpty(empleado.EmailCorporativo))
                return;

            var empresa = await _context.Empresas
                .Where(e => e.Id == empleado.EmpresaId)
                .Select(e => e.Sitio_Web)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(empresa))
            {
                string dominio = ExtraerDominio(empresa);
                if (!string.IsNullOrEmpty(dominio))
                {
                    empleado.EmailCorporativo = $"{usuario}@{dominio}".ToLower();
                    _context.Empleados.Update(empleado);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // Extraer dominio del sitio web
        private string ExtraerDominio(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return "";

                Uri uri = new Uri(url);
                return uri.Host.Replace("www.", "");
            }
            catch
            {
                return "";
            }
        }

        // Generar Contraseña Aleatoria
        private string GenerarContraseñaAleatoria()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Generar Código de Verificación Aleatorio
        private string GenerateVerificationCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            return (Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000).ToString("D6");
        }

        // Registrar eventos en historial_eventos_usuario
        private async Task RegistrarEventoUsuario(int usuarioId, int empleadoId, string tipoEvento, bool exito, string ip = null, string navegador = null, string razon = null, string motivo = null)
        {
            var evento = new HistorialEventosUsuario
            {
                UsuarioId = usuarioId,
                EmpleadoId = empleadoId,
                TipoEvento = tipoEvento,
                Exito = exito,
                Ip = ip,
                Navegador = navegador,
                Razon = razon,
                Motivo = motivo,
                FechaCambio = DateTime.UtcNow
            };

            _context.HistorialEventosUsuario.Add(evento);
            await _context.SaveChangesAsync();
        }
    }
}

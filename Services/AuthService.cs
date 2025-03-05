using Microsoft.EntityFrameworkCore;
using sge_api.Data;
using sge_api.Models;
using System.Security.Cryptography;
using System.Text;
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

        // 📌 **Paso 1: Generar código de verificación después de crear el usuario**
        public async Task<string> GenerarCodigoVerificacion(string numeroIdentificacion)
        {
            // 🔹 **Verificar si el empleado está registrado**
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.NumeroIdentificacion == numeroIdentificacion);

            if (empleado == null)
                return "NOT_FOUND"; // No existe el empleado en la base de datos

            // 🔹 **Verificar si el usuario ya está registrado**
            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.EmpleadoId == empleado.Id);

            if (existingUser == null)
            {
                // ✅ **Si no existe, crear usuario en la tabla `users`**
                existingUser = new Users
                {
                    EmpleadoId = empleado.Id,
                    NumeroIdentificacion = empleado.NumeroIdentificacion,
                    Usuario = await GenerarUsuarioUnico(empleado),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(GenerarContraseñaAleatoria()),
                    Estado = "Activo",
                    FechaRegistro = DateTime.UtcNow
                };

                _context.Usuarios.Add(existingUser);
                await _context.SaveChangesAsync();

                // ✅ **Asignar Email Corporativo**
                await GenerarEmailCorporativo(empleado, existingUser.Usuario);
            }

            // 🔹 **Verificar si ya tiene un código de verificación**
            var existingCode = await _context.CodigosVerificacion
                .Where(c => c.UsuarioId == existingUser.Id && c.Tipo == "Registro")
                .OrderByDescending(c => c.FechaGeneracion)
                .FirstOrDefaultAsync();

            if (existingCode != null)
            {
                // ✅ **Si el código es válido y no ha expirado, NO generar otro**
                if (!existingCode.Usado && existingCode.Expiracion > DateTime.UtcNow)
                    return "CODE_ALREADY_SENT"; // Código aún válido

                // ✅ **Si el código ya expiró, eliminarlo**
                _context.CodigosVerificacion.Remove(existingCode);
                await _context.SaveChangesAsync();
            }

            // ✅ **Generar un nuevo código de verificación**
            var verificationCode = GenerateVerificationCode();
            var codeEntry = new CodigosVerificacion
            {
                UsuarioId = existingUser.Id,
                Codigo = verificationCode,
                Tipo = "Registro",
                FechaGeneracion = DateTime.UtcNow,
                Expiracion = DateTime.UtcNow.AddMinutes(15), // Expira en 15 minutos
                Usado = false
            };

            _context.CodigosVerificacion.Add(codeEntry);
            await _context.SaveChangesAsync();

            // ✅ **Enviar el código de verificación al correo personal**
            await _emailService.SendVerificationEmail(empleado.EmailPersonal, verificationCode);

            return "OK"; // Registro completado
        }




        // 📌 **Paso 2: Completar el registro tras verificar el código**
        public async Task<string> CompletarRegistro(string numeroIdentificacion, string codigo)
        {
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.NumeroIdentificacion == numeroIdentificacion);
            if (empleado == null)
                return "NOT_FOUND";

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.EmpleadoId == empleado.Id);
            if (user == null)
                return "NO_USER";

            var codeEntry = await _context.CodigosVerificacion
                .Where(c => c.UsuarioId == user.Id && c.Tipo == "Registro" && !c.Usado && c.Expiracion > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (codeEntry == null || codeEntry.Codigo != codigo)
                return "INVALID_CODE";

            // ✅ **Marcar código como usado**
            codeEntry.Usado = true;
            await _context.SaveChangesAsync();

            // ✅ **Enviar credenciales de acceso**
            await _emailService.SendUserCredentials(empleado.EmailPersonal, user.Usuario, "Tu contraseña fue establecida en el registro.");

            return "OK";
        }

       


        // 📌 **Generar usuario único**
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
                    case 4:
                        usuario = $"{empleado.PrimerNombre.Substring(0, 1).ToLower()}.{empleado.ApellidoPaterno.ToLower()}.{empleado.ApellidoMaterno?.Substring(0, 1).ToLower() ?? ""}";
                        break;
                    default:
                        usuario = $"{empleado.PrimerNombre.ToLower()}.{empleado.ApellidoPaterno.ToLower()}{intento}";
                        break;
                }

                intento++;

            } while (await _context.Usuarios.AnyAsync(u => u.Usuario == usuario));

            return usuario;
        }

        // 📌 **Generar Email Corporativo**
        private async Task GenerarEmailCorporativo(Empleado empleado, string usuario)
        {
            if (!string.IsNullOrEmpty(empleado.EmailCorporativo))
                return; // Ya tiene un email corporativo asignado

            var empresa = await _context.Empresas
                .Where(e => e.Id == empleado.EmpresaId)
                .Select(e => e.Sitio_Web)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(empresa))
            {
                string dominio = ExtraerDominio(empresa);
                if (!string.IsNullOrEmpty(dominio))
                {
                    // ✅ **Asignar email corporativo**
                    empleado.EmailCorporativo = $"{usuario}@{dominio}".ToLower();

                    // ✅ **Actualizar en la base de datos**
                    _context.Empleados.Update(empleado);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // 📌 **Extraer dominio del sitio web**
        private string ExtraerDominio(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return "";

                // 🔹 Eliminar 'http://', 'https://', 'www.' y dejar solo el dominio
                Uri uri = new Uri(url);
                string dominio = uri.Host.Replace("www.", "");
                return dominio;
            }
            catch
            {
                return ""; // En caso de error, devuelve vacío
            }
        }

        // 📌 **Generar Contraseña Aleatoria**
        private string GenerarContraseñaAleatoria()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // 📌 **Generar Código de Verificación Aleatorio**
        private string GenerateVerificationCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            int code = BitConverter.ToInt32(bytes, 0) % 1000000; // Asegurar un número de 6 dígitos
            code = Math.Abs(code);
            return code.ToString("D6");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using sge_api.Services;
using sge_api.Models.Requests;
using sge_api.Models;
using System.Threading.Tasks;
using System;
using sge_api.Data;
using Microsoft.EntityFrameworkCore;

namespace sge_api.Controllers
{
    [Route("api/register")]
    [ApiController]
    [Tags("Register")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _context;

        public AuthController(AuthService authService, AppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("generate-verification")]
        public async Task<IActionResult> GenerateVerification([FromBody] GenerateVerificationRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var navegador = Request.Headers["User-Agent"].ToString();
            var result = await _authService.GenerarCodigoVerificacion(request.NumeroIdentificacion);

            // Buscar usuario por cédula
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NumeroIdentificacion == request.NumeroIdentificacion);

            // Buscar empleado si aplica
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.NumeroIdentificacion == request.NumeroIdentificacion);

            // Registrar evento
            var evento = new HistorialEventosUsuario
            {
                UsuarioId = usuario?.Id,
                EmpleadoId = empleado?.Id,
                NumeroIdentificacion = request.NumeroIdentificacion,
                TipoEvento = "GENERACION DE CODIGO",
                FechaEvento = DateTime.UtcNow,
                Exito = result == "OK",
                Ip = ip,
                Navegador = navegador,
                Razon = result switch
                {
                    "NOT_FOUND" => "Intento de generación de codigo con empleado no registrado",
                    "CODE_ALREADY_SENT" => "Ya tiene un código activo",
                    "NO_CORPORATE_EMAIL" => "Sin email personal asignado",
                    "OK" => "Verificación exitosa",
                    _ => "Error interno durante la verificación"
                },
                Motivo = result
            };

            _context.HistorialEventosUsuario.Add(evento);
            await _context.SaveChangesAsync();

            return result switch
            {
                "NOT_FOUND" => NotFound(new { status = "NOT_FOUND", message = "Su cédula no esta registrada" }),
                "USER_ALREADY_EXISTS" => Conflict(new { status = "USER_ALREADY_EXISTS", message = "El usuario ya está registrado" }),
                "ACCOUNT_ALREADY_ACTIVE" => BadRequest(new { status = "ACCOUNT_ALREADY_ACTIVE", message = "La cuenta ya está activa" }),
                "CODE_ALREADY_SENT" => StatusCode(208, new { status = "CODE_ALREADY_SENT", message = "Ya se envió un código válido" }),
                "ERROR_CREATING_USER" => StatusCode(500, new { status = "ERROR_CREATING_USER", message = "Error al crear usuario" }),
                "OK" => Ok(new { status = "OK", message = "Código generado y enviado" }),
                _ => BadRequest(new { status = "UNKNOWN", message = "Error desconocido" })
            };
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var navegador = Request.Headers["User-Agent"].ToString();

            var result = await _authService.CompletarRegistro(request.NumeroIdentificacion, request.Codigo);

            // Buscar usuario por cédula
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NumeroIdentificacion == request.NumeroIdentificacion);

            // Buscar empleado si aplica
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.NumeroIdentificacion == request.NumeroIdentificacion);

            // Registrar evento
            var evento = new HistorialEventosUsuario
            {
                UsuarioId = usuario?.Id,
                EmpleadoId = empleado?.Id,
                NumeroIdentificacion = request.NumeroIdentificacion,
                TipoEvento = "REGISTRO",
                FechaEvento = DateTime.UtcNow,
                Exito = result == "OK",
                Ip = ip,
                Navegador = navegador,
                Razon = result switch
                {
                    "NOT_FOUND" => "Intento de verificación con empleado no registrado",
                    "NO_USER" => "Intento de verificación sin usuario asociado",
                    "INVALID_CODE" => "Código inválido o expirado",
                    "NO_CORPORATE_EMAIL" => "Sin email personal asignado",
                    "OK" => "Verificación exitosa",
                    _ => "Error interno durante la verificación"
                },
                Motivo = result
            };

            _context.HistorialEventosUsuario.Add(evento);
            await _context.SaveChangesAsync();

            return result switch
            {
                "OK" => Ok("Registro completado. Credenciales enviadas al email personal."),
                "NOT_FOUND" => BadRequest("El empleado no está registrado."),
                "NO_USER" => BadRequest("No se encontró un usuario asociado."),
                "INVALID_CODE" => BadRequest("Código de verificación inválido o expirado."),
                "NO_PERSONAL_EMAIL" => BadRequest("El usuario no tiene email personal asignado."),
                _ => StatusCode(500, "Error interno del servidor.")
            };
        }

        public class GenerateVerificationRequest
        {
            public string? NumeroIdentificacion { get; set; }
        }
    }
}
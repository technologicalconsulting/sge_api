using Microsoft.AspNetCore.Mvc;
using sge_api.Services;
using sge_api.Models.Requests;
using sge_api.Models;
using System.Threading.Tasks;
using System;
using sge_api.Data;

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

            // Registrar evento
            var evento = new HistorialEventosUsuario
            {
                TipoEvento = "intento_de_registro",
                FechaEvento = DateTime.UtcNow,
                Exito = result == "OK",
                Ip = ip,
                Navegador = navegador,
                Razon = result == "NOT_FOUND" ? "Intento de acceso con cédula no registrada" : "Generación de código de verificación",
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
            var result = await _authService.CompletarRegistro(request.NumeroIdentificacion, request.Codigo);

            return result switch
            {
                "OK" => Ok("Registro completado. Credenciales enviadas al email corporativo."),
                "NOT_FOUND" => BadRequest("El empleado no está registrado."),
                "NO_USER" => BadRequest("No se encontró un usuario asociado."),
                "INVALID_CODE" => BadRequest("Código de verificación inválido o expirado."),
                "NO_CORPORATE_EMAIL" => BadRequest("El usuario no tiene email corporativo asignado."),
                _ => StatusCode(500, "Error interno del servidor.")
            };
        }

        public class GenerateVerificationRequest
        {
            public string? NumeroIdentificacion { get; set; }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using sge_api.Services;
using sge_api.Models.Requests;
using System.Threading.Tasks;
using sge_api.Models;

namespace sge_api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// **Generar código de verificación para un usuario pre-registrado.**
        [HttpPost("generate-verification")]
        public async Task<IActionResult> GenerateVerification([FromBody] GenerateVerificationRequest request)
        {
            var result = await _authService.GenerarCodigoVerificacion(request.NumeroIdentificacion);

            return result switch
            {
                "OK" => Ok("Código de verificación enviado."),
                "NOT_FOUND" => BadRequest("El empleado no está registrado."),
                "USER_NOT_FOUND" => BadRequest("No se pudo registrar automáticamente al usuario."),
                "CODE_EXISTS" => Conflict("Ya existe un código de verificación activo."),
                _ => StatusCode(500, "Error interno del servidor.")
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

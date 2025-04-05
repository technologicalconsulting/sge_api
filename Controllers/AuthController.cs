using Microsoft.AspNetCore.Mvc;
using sge_api.Services;
using sge_api.Models.Requests;
using System.Threading.Tasks;
using sge_api.Models;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.AuthenticateUser(request.Usuario, request.Password);

            if (user == null)
                return Unauthorized("Credenciales incorrectas o usuario inactivo.");

            if (user.Empleado == null)
                return BadRequest("El usuario no tiene un empleado asociado.");

            if (user.Empleado.Empresa == null)
                return BadRequest("El empleado no tiene una empresa asociada.");

            bool requiereCambioPassword = user.FechaUltimoLogin == null;

            return Ok(new
            {
                Usuario = user.Usuario,
                NombreCompleto = $"{user.Empleado.PrimerNombre} {user.Empleado.SegundoNombre ?? ""} {user.Empleado.ApellidoPaterno} {user.Empleado.ApellidoMaterno ?? ""}".Trim(),
                Empresa = user.Empleado.Empresa.Nombre_Comercial,
                RequiereCambioPassword = requiereCambioPassword
            });
        }




        /// **Generar código de verificación para un usuario pre-registrado.**
        [HttpPost("generate-verification")]
        public async Task<IActionResult> GenerateVerification([FromBody] GenerateVerificationRequest request)
        {
            var result = await _authService.GenerarCodigoVerificacion(request.NumeroIdentificacion);

            return result switch
            {
                "NOT_FOUND" => NotFound("El usuario no existe"),
                "USER_ALREADY_EXISTS" => Conflict("El usuario ya está registrado"),
                "ACCOUNT_ALREADY_ACTIVE" => BadRequest("La cuenta ya está activa"),
                "CODE_ALREADY_SENT" => StatusCode(208, "Ya se envió un código válido"), // 208: Already Reported
                "ERROR_CREATING_USER" => StatusCode(500, "Error al crear usuario"),
                "OK" => Ok("Código generado y enviado"),
                _ => BadRequest("Error desconocido")
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

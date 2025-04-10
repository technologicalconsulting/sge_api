using Microsoft.AspNetCore.Mvc;
using sge_api.Services;
using sge_api.Models.Requests;
using System.Threading.Tasks;

namespace sge_api.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _LoginService;

        public LoginController(LoginService LoginService)
        {
            _LoginService = LoginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _LoginService.AuthenticateUser(request.Usuario, request.Password);

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
    }
}
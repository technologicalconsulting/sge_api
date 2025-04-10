using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sge_api.Data;

namespace sge_api.Controllers
{
    [Route("api/dbtest")]
    [ApiController]
    public class DbTestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DbTestController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Prueba la conexión a la base de datos consultando las empresas.
        /// </summary>
        [HttpGet("empresas")]
        public async Task<IActionResult> GetEmpresas()
        {
            try
            {
                var empresas = await _context.Empresas.ToListAsync();
                return Ok(empresas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
            }
        }
    }
}

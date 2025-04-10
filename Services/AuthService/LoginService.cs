using Microsoft.EntityFrameworkCore;
using sge_api.Data;
using sge_api.Models;
using System.Threading.Tasks;
using System.Security.Cryptography;
using BCrypt.Net;

namespace sge_api.Services
{
    public class LoginService
    {
        private readonly AppDbContext _context;

        public LoginService(AppDbContext context, EmailService emailService)
        {
            _context = context;
        }


        // Autenticación de usuario
        public async Task<Users> AuthenticateUser(string usuario, string password)
        {
            var user = await _context.Usuarios
                .Include(u => u.Empleado)
                .ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(u => u.Usuario == usuario);

            return user;
        }

    }
}

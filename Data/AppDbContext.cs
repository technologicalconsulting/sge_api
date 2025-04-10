using Microsoft.EntityFrameworkCore;
using sge_api.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace sge_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Users> Usuarios { get; set; }
        public DbSet<CodigosVerificacion> CodigosVerificacion { get; set; }
        public DbSet<HistorialEventosUsuario> HistorialEventosUsuario { get; set; }



        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var empleados = ChangeTracker.Entries<Empleado>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in empleados)
            {
                var empleado = entry.Entity;

                // Obtener el usuario de la tabla users
                var usuario = await Usuarios
                    .Where(u => u.EmpleadoId == empleado.Id)
                    .Select(u => u.Usuario)
                    .FirstOrDefaultAsync();

                // Obtener el dominio desde la tabla empresa
                var empresa = await Empresas
                    .Where(e => e.Id == empleado.EmpresaId)
                    .Select(e => e.Sitio_Web)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(empresa))
                {
                    string dominio = ExtraerDominio(empresa);
                    if (!string.IsNullOrEmpty(dominio))
                    {
                        empleado.EmailCorporativo = $"{usuario}@{dominio}";
                    }
                }


            }
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Empleado empleado)
                {
                    if (empleado.FechaRegistro.Kind == DateTimeKind.Unspecified)
                    {
                        empleado.FechaRegistro = DateTime.SpecifyKind(empleado.FechaRegistro, DateTimeKind.Utc);
                    }

                    if (empleado.FechaNacimiento.HasValue && empleado.FechaNacimiento.Value.Kind == DateTimeKind.Unspecified)
                    {
                        empleado.FechaNacimiento = DateTime.SpecifyKind(empleado.FechaNacimiento.Value, DateTimeKind.Utc);
                    }
                }

                if (entry.Entity is CodigosVerificacion codigo)
                {
                    if (codigo.FechaGeneracion.Kind == DateTimeKind.Unspecified)
                    {
                        codigo.FechaGeneracion = DateTime.SpecifyKind(codigo.FechaGeneracion, DateTimeKind.Utc);
                    }

                    if (codigo.Expiracion.Kind == DateTimeKind.Unspecified)
                    {
                        codigo.Expiracion = DateTime.SpecifyKind(codigo.Expiracion, DateTimeKind.Utc);
                    }
                }
            }


            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Empresa)
                .WithMany()  // Si en `Empresa` hay una lista de empleados, usa `.WithMany(emp => emp.Empleados)`
                .HasForeignKey(e => e.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistorialEventosUsuario>()
                .ToTable("historialeventosusuario");
        }

        // 🔹 Método para extraer el dominio del sitio web
        private string ExtraerDominio(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                return uri.Host.Replace("www.", ""); // Extrae solo el dominio sin www.
            }
            catch
            {
                return ""; // En caso de error, devuelve vacío
            }
        }
    }
}

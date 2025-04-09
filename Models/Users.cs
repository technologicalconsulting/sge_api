using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sge_api.Models
{
    [Table("users")]
    public class Users
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Empleado")]
        [Column("empleado_id")]
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }

       [Column("numero_identificacion")]
       public string NumeroIdentificacion { get; set; }

        [Column("usuario")]
        public string Usuario { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Column("intentos_fallidos")]
        public int IntentosFallidos { get; set; } = 0;

        [Column("bloqueado")]
        public bool Bloqueado { get; set; } = false;

        [Column("estado")]
        public string Estado { get; set; } = "Activo";

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Column("fecha_ultimo_login")]
        public DateTime? FechaUltimoLogin { get; set; }
    }
}

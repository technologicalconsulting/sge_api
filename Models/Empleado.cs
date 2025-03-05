using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sge_api.Models
{
    [Table("empleado")]
    public class Empleado
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("primer_nombre")]
        public string PrimerNombre { get; set; }

        [Column("segundo_nombre")]
        public string? SegundoNombre { get; set; }

        [Column("apellido_paterno")]
        public string ApellidoPaterno { get; set; }

        [Column("apellido_materno")]
        public string? ApellidoMaterno { get; set; }

        [Column("tipo_documento")]
        public string TipoDocumento { get; set; }

        [Column("numero_identificacion")]
        public string NumeroIdentificacion { get; set; }

        [Column("email_personal")]
        public string EmailPersonal { get; set; }

        [Column("email_corporativo")]
        public string? EmailCorporativo { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("fecha_nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Column("genero")]
        public string Genero { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = "Activo";

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}

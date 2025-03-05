using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sge_api.Models
{
    [Table("empresa")]
    public class Empresa
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("razon_social")]
        public string Razon_Social { get; set; }

        [Column("nombre_comercial")]
        public string? Nombre_Comercial { get; set; }

        [Column("ruc")]
        public string RUC { get; set; }

        [Column("tipo_empresa")]
        public string Tipo_Empresa { get; set; }

        [Column("sector")]
        public string? Sector { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("ciudad")]
        public string? Ciudad { get; set; }

        [Column("pais")]
        public string Pais { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("sitio_web")]
        public string? Sitio_Web { get; set; }

        [Column("logo_url")]
        public string? Logo_URL { get; set; }

        [Column("fecha_fundacion")]
        public DateTime? Fecha_Fundacion { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("fecha_registro")]
        public DateTime Fecha_Registro { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sge_api.Models
{
    [Table("codigos_verificacion")]
    public class CodigosVerificacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Users")]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        public Users Usuario { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("fecha_generacion")]
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

        [Column("expiracion")]
        public DateTime Expiracion { get; set; }

        [Column("usado")]
        public bool Usado { get; set; } = false;
    }
}

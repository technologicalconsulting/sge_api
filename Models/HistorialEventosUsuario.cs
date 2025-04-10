using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("historial_eventos_usuario")]
public class HistorialEventosUsuario
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("usuario_id")]
    public int? UsuarioId { get; set; }

    [Column("empleado_id")]
    public int? EmpleadoId { get; set; }

    [Column("tipo_evento")]
    public string TipoEvento { get; set; }

    [Column("fecha_evento")]
    public DateTime? FechaEvento { get; set; }

    [Column("exito")]
    public bool? Exito { get; set; }

    [Column("ip")]
    public string Ip { get; set; }

    [Column("navegador")]
    public string Navegador { get; set; }

    [Column("razon")]
    public string Razon { get; set; }

    [Column("motivo")]
    public string Motivo { get; set; }

    [Column("fecha_cambio")]
    public DateTime? FechaCambio { get; set; }
}

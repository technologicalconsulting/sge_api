namespace sge_api.Models
{
    public class HistorialEventosUsuario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EmpleadoId { get; set; }
        public string TipoEvento { get; set; }
        public DateTime FechaEvento { get; set; } = DateTime.UtcNow;
        public bool Exito { get; set; }
        public string Ip { get; set; }
        public string Navegador { get; set; }
        public string Razon { get; set; }
        public string Motivo { get; set; }
        public DateTime? FechaCambio { get; set; }

        public Users Usuario { get; set; }
        public Empleado Empleado { get; set; }
    }
}

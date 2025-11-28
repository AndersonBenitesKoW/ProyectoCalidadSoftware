namespace CapaEntidad
{
    public class entResultadoDiagnostico
    {
        public int IdResultado { get; set; }
        public int IdAyuda { get; set; }
        public DateTime FechaResultado { get; set; }
        public string? Resumen { get; set; }
        public bool Critico { get; set; }
        public string Estado { get; set; } = null!;

        // --- Propiedades de JOINs para mostrar información del control prenatal ---
        public int? IdControlPrenatal { get; set; }
        public string? NombrePaciente { get; set; }
        public string? NombreProfesional { get; set; }
        public DateTime? FechaControlPrenatal { get; set; }
        public string? DescripcionAyuda { get; set; }
        public string? TipoAyuda { get; set; }
        public bool? Urgente { get; set; }

        // Detalle de parámetros del examen
        public List<entResultadoItem> Items { get; set; } = new();
    }
}

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
    }
}

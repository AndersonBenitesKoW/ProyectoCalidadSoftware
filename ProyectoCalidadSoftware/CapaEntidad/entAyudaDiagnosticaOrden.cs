namespace CapaEntidad
{
    public class entAyudaDiagnosticaOrden
    {
        public int IdAyuda { get; set; }
        public int IdPaciente { get; set; }
        public int? IdEmbarazo { get; set; }
        public int? IdProfesional { get; set; }
        public short? IdTipoAyuda { get; set; }
        public string? Descripcion { get; set; }
        public bool Urgente { get; set; }
        public DateTime FechaOrden { get; set; }
        public string Estado { get; set; } = null!;
    }
}

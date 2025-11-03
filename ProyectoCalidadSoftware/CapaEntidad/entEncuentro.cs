namespace CapaEntidad
{
    public class entEncuentro
    {
        public int IdEncuentro { get; set; }
        public int IdEmbarazo { get; set; }

        public int IdProfesional { get; set; }
        public short IdTipoEncuentro { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public string Estado { get; set; } = null!;
        public string? Notas { get; set; }
    }
}




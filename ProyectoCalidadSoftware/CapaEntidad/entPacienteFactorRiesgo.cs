namespace CapaEntidad
{
    public class entPacienteFactorRiesgo
    {
        public int IdPacienteFactor { get; set; }
        public int IdPaciente { get; set; }
        public int IdFactorCat { get; set; }
        public string? Detalle { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estado { get; set; }
    }
}

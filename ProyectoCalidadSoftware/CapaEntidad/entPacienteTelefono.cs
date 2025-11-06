namespace CapaEntidad
{
    public class entPacienteTelefono
    {
        public int IdPacienteTelefono { get; set; }
        public int IdPaciente { get; set; }
        public string Telefono { get; set; } = null!;
        public string? Tipo { get; set; }
        public bool EsPrincipal { get; set; }
    }
}

namespace CapaEntidad
{
    public class entPacienteEmail
    {
        public int IdPacienteEmail { get; set; }
        public int IdPaciente { get; set; }
        public string Email { get; set; } = null!;
        public bool EsPrincipal { get; set; }
    }
}

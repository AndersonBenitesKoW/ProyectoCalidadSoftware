namespace CapaEntidad
{
    public class entPaciente
    {
        public int IdPaciente { get; set; }
        public int? IdUsuario { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string? DNI { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool Estado { get; set; }
        public string NombreCompleto => $"{Apellidos}, {Nombres}";
        public string EmailPrincipal { get; set; }
        public string TelefonoPrincipal { get; set; }
    }
}

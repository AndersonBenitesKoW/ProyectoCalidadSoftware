namespace CapaEntidad
{
    public class entProfesionalSalud
    {
        public int IdProfesional { get; set; }
        public int? IdUsuario { get; set; }
        public string CMP { get; set; } = null!;
        public string? Especialidad { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public bool Estado { get; set; }
    }
}

namespace CapaEntidad
{
    public class entUsuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string ClaveHash { get; set; } = null!;
        public bool Estado { get; set; }
    }
}

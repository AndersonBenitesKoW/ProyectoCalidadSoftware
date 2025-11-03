using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class entUsuario
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreUsuario { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string ClaveHash { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public string Email { get; set; } = null!;

        public bool Estado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = null!;

    }
}

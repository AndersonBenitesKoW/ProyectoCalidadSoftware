using System;
using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class entAyudaDiagnosticaOrden
    {
        public int IdAyuda { get; set; }
        public int IdPaciente { get; set; }
        public int? IdEmbarazo { get; set; }
        public int? IdProfesional { get; set; }

        [Display(Name = "Tipo de Ayuda")]
        public short? IdTipoAyuda { get; set; }

        [Display(Name = "Descripción Específica")]
        public string? Descripcion { get; set; }

        public bool Urgente { get; set; }

        [Display(Name = "Fecha de Orden")]
        public DateTime FechaOrden { get; set; }

        public string Estado { get; set; } = string.Empty;

        // --- Propiedades de JOINs (para Listas y Detalles) ---
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreProfesional { get; set; } = string.Empty;
        public string NombreTipoAyuda { get; set; } = string.Empty;
    }
}
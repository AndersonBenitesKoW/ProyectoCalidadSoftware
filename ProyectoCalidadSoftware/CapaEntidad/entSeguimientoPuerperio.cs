using System;
using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class entSeguimientoPuerperio
    {
        public int IdPuerperio { get; set; }
        public int IdEmbarazo { get; set; }
        public int? IdEncuentro { get; set; }
        public int? IdProfesional { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Display(Name = "PA Sistólica")]
        public byte? PA_Sistolica { get; set; }

        [Display(Name = "PA Diastólica")]
        public byte? PA_Diastolica { get; set; }

        [Display(Name = "Temp (°C)")]
        public decimal? Temp_C { get; set; }

        [Display(Name = "Altura Uterina (cm)")]
        public decimal? AlturaUterinaPP_cm { get; set; }

        public string? Loquios { get; set; }
        public string? Lactancia { get; set; }

        [Display(Name = "Signos de Infección")]
        public bool? SignosInfeccion { get; set; }

        [Display(Name = "Tamizaje Depresión")]
        public string? TamizajeDepresion { get; set; }

        [Display(Name = "Método PF")]
        public short? IdMetodoPF { get; set; }

        public string? Observaciones { get; set; }
        public bool Estado { get; set; }

        // Propiedades de JOINs (para Listas y Detalles)
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreProfesional { get; set; } = string.Empty;
        public string NombreMetodoPF { get; set; } = string.Empty;
    }
}
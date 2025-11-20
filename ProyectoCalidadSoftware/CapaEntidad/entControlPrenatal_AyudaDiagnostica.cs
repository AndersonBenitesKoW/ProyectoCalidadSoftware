using System;
using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class entControlPrenatal_AyudaDiagnostica
    {
        public int IdCP_AD { get; set; }
        public int IdControl { get; set; }
        public int IdAyuda { get; set; }

        [Display(Name = "Tipo de Ayuda")]
        public short? IdTipoAyuda { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? FechaOrden { get; set; }

        public string? Comentario { get; set; }
        public bool Estado { get; set; }

        // --- Propiedades de JOINs (para Listas y Detalles) ---
        public string NombreTipoAyuda { get; set; } = string.Empty;
        public string DescripcionAyuda { get; set; } = string.Empty;
        public bool Urgente { get; set; }
    }
}
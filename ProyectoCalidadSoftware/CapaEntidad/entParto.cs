using System;
using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class entParto
    {
        public int IdParto { get; set; }
        public int IdEmbarazo { get; set; }
        public int? IdEncuentro { get; set; }
        public int? IdProfesional { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [DataType(DataType.Time)]
        public DateTime? HoraIngreso { get; set; }

        [DataType(DataType.Time)]
        public DateTime? HoraInicioTrabajo { get; set; }

        public string? Membranas { get; set; } // (Rotas, Integras)

        [Display(Name = "Líquido Amniótico")]
        public short? IdLiquido { get; set; }

        public string? Analgesia { get; set; }

        [Display(Name = "Vía de Parto")]
        public short? IdViaParto { get; set; }

        [Display(Name = "Indicación Cesárea")]
        public string? IndicacionCesarea { get; set; }

        [Display(Name = "Pérdida (ml)")]
        public int? PerdidasML { get; set; }

        public string? Desgarro { get; set; } // (Grado I, II, etc.)
        public string? Complicaciones { get; set; }
        public bool Estado { get; set; }

        // --- Propiedades de JOINs (para Vistas) ---
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreProfesional { get; set; } = string.Empty;
        public string NombreViaParto { get; set; } = string.Empty;
        public string NombreLiquido { get; set; } = string.Empty;
    }
}
using System;
using System.ComponentModel.DataAnnotations; // Para DataAnnotations

namespace CapaEntidad
{
    public class entControlPrenatal
    {
        // --- Campos de la tabla ControlPrenatal ---
        public int IdControlPrenatal { get; set; } // Tu DA usa este nombre
        public int IdEmbarazo { get; set; }
        public int? IdEncuentro { get; set; }
        public int? IdProfesional { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        public decimal? PesoKg { get; set; }
        public decimal? TallaM { get; set; }

        [Display(Name = "PA Sistólica")]
        public byte? PA_Sistolica { get; set; }

        [Display(Name = "PA Diastólica")]
        public byte? PA_Diastolica { get; set; }

        [Display(Name = "Altura Uterina (cm)")]
        public decimal? AlturaUterina_cm { get; set; }

        [Display(Name = "FCF (lpm)")]
        public byte? FCF_bpm { get; set; }

        public string? Presentacion { get; set; }
        public string? Proteinuria { get; set; }

        [Display(Name = "Movimientos Fetales")]
        public bool? MovFetales { get; set; }

        public string? Consejerias { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }

        // --- Propiedades de JOINs (para Listas y Detalles) ---
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreProfesional { get; set; } = string.Empty;
    }
}
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

        public int? NumeroControl { get; set; }
        public int? EdadGestSemanas { get; set; }
        public int? EdadGestDias { get; set; }
        public string? MetodoEdadGest { get; set; }

        public decimal? PesoKg { get; set; }
        public decimal? PesoPreGestacionalKg { get; set; }
        public decimal? TallaM { get; set; }
        public decimal? IMCPreGestacional { get; set; }

        [Display(Name = "PA Sistólica")]
        public byte? PA_Sistolica { get; set; }

        [Display(Name = "PA Diastólica")]
        public byte? PA_Diastolica { get; set; }

        public short? Pulso { get; set; }
        public short? FrecuenciaRespiratoria { get; set; }
        public decimal? Temperatura { get; set; }

        [Display(Name = "Altura Uterina (cm)")]
        public decimal? AlturaUterina_cm { get; set; }

        public string? DinamicaUterina { get; set; }
        public string? Presentacion { get; set; }
        public string? TipoEmbarazo { get; set; }

        [Display(Name = "FCF (lpm)")]
        public byte? FCF_bpm { get; set; }

        public string? LiquidoAmniotico { get; set; }
        public decimal? IndiceLiquidoAmniotico { get; set; }
        public string? PerfilBiofisico { get; set; }
        public string? Proteinuria { get; set; }
        public string? Edemas { get; set; }
        public string? Reflejos { get; set; }
        public decimal? Hemoglobina { get; set; }
        public string? ResultadoVIH { get; set; }
        public string? ResultadoSifilis { get; set; }
        public string? GrupoSanguineoRh { get; set; }

        public bool EcografiaRealizada { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaEcografia { get; set; }

        public string? LugarEcografia { get; set; }
        public bool PlanPartoEntregado { get; set; }
        public string? MicronutrientesEntregados { get; set; }
        public bool ViajoUltSemanas { get; set; }
        public bool ReferenciaObstetrica { get; set; }

        public string? Consejerias { get; set; }
        public string? Observaciones { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ProximaCitaFecha { get; set; }

        public string? EstablecimientoAtencion { get; set; }
        public bool Estado { get; set; }

        // --- Propiedades de JOINs (para Listas y Detalles) ---
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreProfesional { get; set; } = string.Empty;

        // --- Ayudas Diagnósticas asociadas ---
        public List<entControlPrenatal_AyudaDiagnostica> AyudasDiagnosticas { get; set; } = new List<entControlPrenatal_AyudaDiagnostica>();
    }
}
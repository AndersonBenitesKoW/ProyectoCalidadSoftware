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

        // Campos adicionales de la tabla Parto
        [DataType(DataType.Time)]
        public DateTime? HoraExpulsion { get; set; }

        public string? TipoParto { get; set; }

        public int? TiempoRoturaMembranasHoras { get; set; }

        public string? AspectoLiquido { get; set; }

        public string? PosicionMadre { get; set; }

        public bool Acompanante { get; set; }

        public string? LugarNacimiento { get; set; }

        public int? DuracionSegundaEtapaMinutos { get; set; }

        public bool Episiotomia { get; set; }

        public string? ComplicacionesMaternas { get; set; }

        public bool Derivacion { get; set; }

        public string? SeguroTipo { get; set; }

        public int? NumeroHijosPrevios { get; set; }

        public int? NumeroCesareasPrevias { get; set; }

        public bool EmbarazoMultiple { get; set; }

        public int? NumeroGemelos { get; set; }

        public string? Observaciones { get; set; }

        // --- Propiedades de JOINs (para Vistas) ---
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreProfesional { get; set; } = string.Empty;
        public string NombreViaParto { get; set; } = string.Empty;
        public string NombreLiquido { get; set; } = string.Empty;

        // --- Relaciones ---
        public List<entPartoIntervencion> Intervenciones { get; set; } = new List<entPartoIntervencion>();
        public List<entBebe> Bebes { get; set; } = new List<entBebe>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ControlPrenatal
    {
        public int IdControl { get; set; }
        public int IdEmbarazo { get; set; }
        public int? IdEncuentro { get; set; }
        public int? IdProfesional { get; set; }
        public DateTime Fecha { get; set; }
        public decimal? PesoKg { get; set; }          // (5,2)
        public decimal? TallaM { get; set; }          // (3,2)
        public byte? PA_Sistolica { get; set; }
        public byte? PA_Diastolica { get; set; }
        public decimal? AlturaUterina_cm { get; set; } // (4,1)
        public byte? FCF_bpm { get; set; }
        public string? Presentacion { get; set; }
        public string? Proteinuria { get; set; }
        public bool? MovFetales { get; set; }
        public string? Consejerias { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }
    }
}

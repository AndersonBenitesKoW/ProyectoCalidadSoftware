using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Bebe
    {
        public int IdBebe { get; set; }
        public int IdParto { get; set; }
        public string EstadoBebe { get; set; } = null!;
        public string? Sexo { get; set; } // 'F' / 'M'
        public byte? Apgar1 { get; set; }
        public byte? Apgar5 { get; set; }
        public int? PesoGr { get; set; }
        public decimal? TallaCm { get; set; }          // (4,1)
        public decimal? PerimetroCefalico { get; set; }// (4,1)
        public decimal? EG_Semanas { get; set; }       // (4,1)
        public bool? Reanimacion { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }
    }
}

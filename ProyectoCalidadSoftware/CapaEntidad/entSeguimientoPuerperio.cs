using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class entSeguimientoPuerperio
    {
        public int IdSeguimientoPuerperio { get; set; }
        public int IdEmbarazo { get; set; }
        public int? IdEncuentro { get; set; }
        public int? IdProfesional { get; set; }
        public DateTime Fecha { get; set; }
        public byte? PA_Sistolica { get; set; }
        public byte? PA_Diastolica { get; set; }
        public decimal? Temp_C { get; set; }            // (4,1)
        public decimal? AlturaUterinaPP_cm { get; set; }// (4,1)
        public string? Loquios { get; set; }
        public string? Lactancia { get; set; }
        public bool? SignosInfeccion { get; set; }
        public string? TamizajeDepresion { get; set; }
        public short? IdMetodoPF { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }
    }
}

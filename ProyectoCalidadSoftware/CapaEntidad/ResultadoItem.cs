using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ResultadoItem
    {
        public int IdResultadoItem { get; set; }
        public int IdResultado { get; set; }
        public string Parametro { get; set; } = null!;
        public decimal? ValorNumerico { get; set; }  // (12,4)
        public string? ValorTexto { get; set; }
        public string? Unidad { get; set; }
        public string? RangoRef { get; set; }
    }
}

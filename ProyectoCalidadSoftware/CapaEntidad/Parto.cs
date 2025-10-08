using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Parto
    {
        public int IdParto { get; set; }
        public int IdEmbarazo { get; set; }
        public int? IdEncuentro { get; set; }
        public int? IdProfesional { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? HoraIngreso { get; set; }
        public DateTime? HoraInicioTrabajo { get; set; }
        public string? Membranas { get; set; }
        public short? IdLiquido { get; set; }
        public string? Analgesia { get; set; }
        public short? IdViaParto { get; set; }
        public string? IndicacionCesarea { get; set; }
        public int? PerdidasML { get; set; }
        public string? Desgarro { get; set; }
        public string? Complicaciones { get; set; }
        public bool Estado { get; set; }
    }
}

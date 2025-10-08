using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class AntecedenteObstetrico
    {
        public int IdAntecedente { get; set; }
        public int IdPaciente { get; set; }
        public short? Menarquia { get; set; }
        public short? CicloDias { get; set; }
        public short? Gestas { get; set; }
        public short? Partos { get; set; }
        public short? Abortos { get; set; }
        public string? Observacion { get; set; }
        public bool Estado { get; set; }
    }
}

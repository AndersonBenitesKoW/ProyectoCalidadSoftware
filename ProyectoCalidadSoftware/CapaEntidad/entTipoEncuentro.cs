using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class entTipoEncuentro
    {
        public short IdTipoEncuentro { get; set; }
        public string Codigo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class PartoIntervencion
    {
        public int IdPartoIntervencion { get; set; }
        public int IdParto { get; set; }
        public string Intervencion { get; set; } = null!;
    }
}

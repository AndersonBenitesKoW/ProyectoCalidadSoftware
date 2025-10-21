using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class entRol
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}

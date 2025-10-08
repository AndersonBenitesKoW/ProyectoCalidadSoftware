using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Auditoria
    {
        public int IdAuditoria { get; set; }
        public int? IdUsuario { get; set; }
        public string Accion { get; set; } = null!;
        public string? Entidad { get; set; }
        public int? IdRegistro { get; set; }
        public string? Antes { get; set; }   // JSON/XML
        public string? Despues { get; set; } // JSON/XML
        public string? IpCliente { get; set; }
        public DateTime FechaHora { get; set; }
    }
}

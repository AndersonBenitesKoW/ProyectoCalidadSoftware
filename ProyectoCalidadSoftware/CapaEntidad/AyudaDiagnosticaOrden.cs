using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class AyudaDiagnosticaOrden
    {
        public int IdAyuda { get; set; }
        public int IdPaciente { get; set; }
        public int? IdEmbarazo { get; set; }
        public int? IdProfesional { get; set; }
        public short? IdTipoAyuda { get; set; }
        public string? Descripcion { get; set; }
        public bool Urgente { get; set; }
        public DateTime FechaOrden { get; set; }
        public string Estado { get; set; } = null!;
    }
}

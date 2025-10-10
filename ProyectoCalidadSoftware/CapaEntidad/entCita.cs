using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class entCita
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int? IdRecepcionista { get; set; }
        public int? IdProfesional { get; set; }
        public int? IdEmbarazo { get; set; }
        public DateTime FechaCita { get; set; }
        public string? Motivo { get; set; }
        public short IdEstadoCita { get; set; }
        public string? Observacion { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public string? MotivoAnulacion { get; set; }
    }
}

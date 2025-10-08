using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class PacienteEmail
    {
        public int IdPacienteEmail { get; set; }
        public int IdPaciente { get; set; }
        public string Email { get; set; } = null!;
        public bool EsPrincipal { get; set; }
    }
}

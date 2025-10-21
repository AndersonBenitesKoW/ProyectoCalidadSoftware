using System;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
=======
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89

namespace CapaEntidad
{
    public class entEmbarazo
    {
        public int IdEmbarazo { get; set; }
        public int IdPaciente { get; set; }
        public DateTime? FUR { get; set; }
        public DateTime? FPP { get; set; }
        public string? Riesgo { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public bool Estado { get; set; }
<<<<<<< HEAD
    }
}
=======

        public string NombrePaciente { get; set; } = string.Empty;
    }
}
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89

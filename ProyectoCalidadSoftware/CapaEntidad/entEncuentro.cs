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
    public class entEncuentro
    {
        public int IdEncuentro { get; set; }
        public int IdEmbarazo { get; set; }
<<<<<<< HEAD
        public int IdProfesional { get; set; }
        public short IdTipoEncuentro { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public string Estado { get; set; } = null!;
        public string? Notas { get; set; }
    }
}
=======
        public int? IdProfesional { get; set; } 
        public short IdTipoEncuentro { get; set; } 
        public DateTime FechaHoraInicio { get; set; }
        public DateTime? FechaHoraFin { get; set; }
        public string Estado { get; set; } = null!; 
        public string? Notas { get; set; }
    }
}
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89

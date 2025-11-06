using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class entProfesionalTelefono
    {
        public int IdProfesionalTelefono { get; set; }
        public int IdProfesional { get; set; }
        public string Telefono { get; set; } = null!;
        public string? Tipo { get; set; }
        public bool EsPrincipal { get; set; }


    }
}

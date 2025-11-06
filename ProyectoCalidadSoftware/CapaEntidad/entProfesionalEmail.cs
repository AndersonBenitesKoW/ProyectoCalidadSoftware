using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class entProfesionalEmail
    {

        public int IdProfesionalEmail { get; set; }
        public int IdProfesional { get; set; }
        public string Email { get; set; } = null!;
        public bool EsPrincipal { get; set; }   



    }
}

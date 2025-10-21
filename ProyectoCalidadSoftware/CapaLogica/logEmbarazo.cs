using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;  
namespace CapaLogica
{
    public class logEmbarazo
    {

        #region Singleton
        private static readonly logEmbarazo UnicaInstancia = new logEmbarazo();
        public static logEmbarazo Instancia
        {
            get { return logEmbarazo.UnicaInstancia; }
        }
        private logEmbarazo() { }
        #endregion

        // LISTAR
<<<<<<< HEAD
        public List<entEmbarazo> ListarEmbarazo()
        {
            return DA_Embarazo.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarEmbarazo(entEmbarazo entidad)
        {
            return DA_Embarazo.Instancia.Insertar(entidad);
=======
        public List<entEmbarazo> ListarEmbarazosPorEstado(bool estado)
        {
            return DA_Embarazo.Instancia.ListarPorEstado(estado);
        }

        // INSERTAR
        public int RegistrarEmbarazo(entEmbarazo embarazo)
        {
            if (embarazo.IdPaciente <= 0)
            {
                throw new ApplicationException("El IdPaciente es obligatorio para registrar un embarazo.");
            }

            if (embarazo.FUR.HasValue && !embarazo.FPP.HasValue)
            {
                embarazo.FPP = embarazo.FUR.Value.AddDays(7).AddMonths(-3).AddYears(1);
            }

            return DA_Embarazo.Instancia.Insertar(embarazo);
        }
        public entEmbarazo? BuscarEmbarazoPorId(int idEmbarazo)
        {
            if (idEmbarazo <= 0)
            {
                return null; 
            }
            return DA_Embarazo.Instancia.BuscarPorId(idEmbarazo);
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
        }

    }
}

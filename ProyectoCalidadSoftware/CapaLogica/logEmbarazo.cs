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
        public List<entEmbarazo> ListarEmbarazo()
        {
            return DA_Embarazo.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarEmbarazo(entEmbarazo entidad)
        {
            return DA_Embarazo.Instancia.Insertar(entidad);
        }

    }
}

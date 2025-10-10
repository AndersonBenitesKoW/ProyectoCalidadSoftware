using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaAccesoDatos;
using CapaEntidad; 

namespace CapaLogica
{
    public class logAntecedenteObstetrico
    {

        #region Singleton
        private static readonly logAntecedenteObstetrico UnicaInstancia = new logAntecedenteObstetrico();
        public static logAntecedenteObstetrico Instancia
        {
            get { return logAntecedenteObstetrico.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        public List<entAntecedenteObstetrico> ListarAntecedenteObstetrico()
        {
            return DA_AntecedenteObstetrico.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarAntecedenteObstetrico(entAntecedenteObstetrico entidad)
        {
            return DA_AntecedenteObstetrico.Instancia.Insertar(entidad);
        }


    }
}

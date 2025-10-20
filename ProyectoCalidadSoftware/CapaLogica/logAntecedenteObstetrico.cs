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


        public entAntecedenteObstetrico BuscarAntecedenteObstetrico(int id)
          => DA_AntecedenteObstetrico.Instancia.BuscarPorId(id);

        public bool ActualizarAntecedenteObstetrico(entAntecedenteObstetrico entidad)
            => DA_AntecedenteObstetrico.Instancia.Actualizar(entidad);

        public bool AnularAntecedenteObstetrico(int id)
            => DA_AntecedenteObstetrico.Instancia.Anular(id);

        public bool EliminarAntecedenteObstetrico(int id)
            => DA_AntecedenteObstetrico.Instancia.Eliminar(id);




    }
}

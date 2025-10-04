using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaAccesoDatos;

namespace CapaLogica
{
    public class logAntecedenteObstetrico
    {

        #region Singleton
        private static readonly logAntecedenteObstetrico _instancia = new logAntecedenteObstetrico();
        public static logAntecedenteObstetrico Instancia
        {
            get { return logAntecedenteObstetrico._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable ListarAntecedenteObstetrico()
        {
            return DA_AntecedenteObstetrico.Instancia.Listar();
        }

        public bool InsertarAntecedenteObstetrico(int idPaciente, short? menarquia, short? cicloDias, short? gestas,
                                                  short? partos, short? abortos, string observacion, bool estado)
        {
            return DA_AntecedenteObstetrico.Instancia.Insertar(
                idPaciente, menarquia, cicloDias, gestas, partos, abortos, observacion, estado
            );
        }

        #endregion


    }
}

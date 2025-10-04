using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
   public class logAuditoria
    {

        #region Singleton
        private static readonly logAuditoria _instancia = new logAuditoria();
        public static logAuditoria Instancia
        {
            get { return logAuditoria._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable ListarAuditoria()
        {
            return DA_Auditoria.Instancia.Listar();
        }

        public bool InsertarAuditoria(int? idUsuario, string accion, string entidad, int? idRegistro,
                                      string antes, string despues, string ipCliente)
        {
            return DA_Auditoria.Instancia.Insertar(idUsuario, accion, entidad, idRegistro, antes, despues, ipCliente);
        }

        #endregion


    }
}

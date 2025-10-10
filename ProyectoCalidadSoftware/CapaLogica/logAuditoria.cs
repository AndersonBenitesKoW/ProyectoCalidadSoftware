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
   public class logAuditoria
    {

        #region Singleton
        private static readonly logAuditoria UnicaInstancia = new logAuditoria();
        public static logAuditoria Instancia
        {
            get { return logAuditoria.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        public List<entAuditoria> ListarAuditoria()
        {
            return DA_Auditoria.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarAuditoria(entAuditoria entidad)
        {
            return DA_Auditoria.Instancia.Insertar(entidad);
        }

    }
}

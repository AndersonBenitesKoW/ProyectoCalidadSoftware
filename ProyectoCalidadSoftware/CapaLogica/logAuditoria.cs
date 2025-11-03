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
        public bool InsertarAuditoria(entAuditoria entidad)
        {
            return DA_Auditoria.Instancia.Insertar(entidad);
        }

    }
}

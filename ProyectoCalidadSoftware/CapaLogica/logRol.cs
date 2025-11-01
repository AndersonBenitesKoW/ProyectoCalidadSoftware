using CapaAccesoDatos;
using CapaEntidad;
namespace CapaLogica
{
    internal class logRol
    {
        #region Singleton
        private static readonly logRol UnicaInstancia = new logRol();
        public static logRol Instancia
        {
            get { return logRol.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        public List<entRol> ListarRol()
        {
            return DA_Rol.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarRol(entRol entidad)
        {
            return DA_Rol.Instancia.Insertar(entidad);
        }

    }
}

using CapaAccesoDatos;
using CapaEntidad;
namespace CapaLogica
{
    public class logUsuario
    {
        #region Singleton
        private static readonly logUsuario UnicaInstancia = new logUsuario();
        public static logUsuario Instancia
        {
            get { return logUsuario.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        public List<entUsuario> ListarUsuario()
        {
            return DA_Usuario.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarUsuario(entUsuario entidad)
        {
            return DA_Usuario.Instancia.Insertar(entidad);
        }

    }
}

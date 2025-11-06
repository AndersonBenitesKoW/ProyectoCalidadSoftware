using CapaAccesoDatos;
using CapaEntidad;
namespace CapaLogica
{
    public class logUsuarioRol
    {

        #region Singleton
        private static readonly logUsuarioRol UnicaInstancia = new logUsuarioRol();
        public static logUsuarioRol Instancia
        {
            get { return logUsuarioRol.UnicaInstancia; }
        }
        public logUsuarioRol() { }
        #endregion

        // LISTAR
        public List<entUsuarioRol> ListarUsuario()
        {
            return DA_UsuarioRol.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarUsuario(entUsuarioRol entidad)
        {
            return DA_UsuarioRol.Instancia.Insertar(entidad);
        }

    }
}

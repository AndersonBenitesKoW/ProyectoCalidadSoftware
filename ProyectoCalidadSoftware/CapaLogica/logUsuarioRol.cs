using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

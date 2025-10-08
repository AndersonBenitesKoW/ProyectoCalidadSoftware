using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class logUsuarioRol
    {

        #region Singleton
        private static readonly logUsuario UnicaInstancia = new logUsuario();
        public static logUsuario Instancia
        {
            get { return logUsuario.UnicaInstancia; }
        }
        private logUsuario() { }
        #endregion

        // LISTAR
        public List<entUsuario> ListarUsuario()
        {
            return DA_Usuario.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarUsuario(entUsuario entidad)
        {
            return DA_Usuario.Instancia.Insertar(entidad);
        }

    }
}

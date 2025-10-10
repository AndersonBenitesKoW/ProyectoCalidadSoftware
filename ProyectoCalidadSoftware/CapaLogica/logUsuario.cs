using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // VERIFICAR LOGIN
        public entUsuario VerificarUsuario(string username, string password)
        {
            // Hash the password (assuming SHA256)
            string passwordHash = HashPassword(password);
            return DA_Usuario.Instancia.VerificarLogin(username, passwordHash);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash).Substring(0, 50); // Truncate to 50 chars as per DB
            }
        }

    }
}

using CapaAccesoDatos;
using CapaEntidad;
namespace CapaLogica
{
    public class logPacienteEmail
    {
        #region Singleton
        private static readonly logPacienteEmail UnicaInstancia = new logPacienteEmail();
        public static logPacienteEmail Instancia
        {
            get { return logPacienteEmail.UnicaInstancia; }
        }
        private logPacienteEmail() { }
        #endregion

        // LISTAR
        public List<entPacienteEmail> ListarPacienteEmail()
        {
            return DA_PacienteEmail.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarPacienteEmail(entPacienteEmail entidad)
        {
            return DA_PacienteEmail.Instancia.Insertar(entidad);
        }

    }
}

using CapaAccesoDatos;
using CapaEntidad;
namespace CapaLogica
{
    public class logProfesionalEmail
    {
        #region Singleton
        private static readonly logProfesionalEmail UnicaInstancia = new logProfesionalEmail();
        public static logProfesionalEmail Instancia
        {
            get { return logProfesionalEmail.UnicaInstancia; }
        }
        private logProfesionalEmail() { }
        #endregion

        // LISTAR
        public List<entProfesionalEmail> ListarProfesionalEmail()
        {
            return DA_ProfesionalEmail.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarProfesionalEmail(entProfesionalEmail entidad)
        {
            return DA_ProfesionalEmail.Instancia.Insertar(entidad);
        }

    }
}
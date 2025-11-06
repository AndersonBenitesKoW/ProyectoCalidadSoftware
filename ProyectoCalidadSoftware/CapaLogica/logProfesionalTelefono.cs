using CapaAccesoDatos;
using CapaEntidad;

namespace CapaLogica
{
    public class logProfesionalTelefono
    {

        #region Singleton
        private static readonly logProfesionalTelefono UnicaInstancia = new logProfesionalTelefono();
        public static logProfesionalTelefono Instancia
        {
            get { return logProfesionalTelefono.UnicaInstancia; }
        }
        private logProfesionalTelefono() { }
        #endregion

        #region Métodos
        // LISTAR
        public List<entProfesionalTelefono> ListarProfesionalTelefono()
        {
            return DA_ProfesionalTelefono.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarProfesionalTelefono(entProfesionalTelefono entidad)
        {
            return DA_ProfesionalTelefono.Instancia.Insertar(entidad);
        }
        #endregion



    }
}
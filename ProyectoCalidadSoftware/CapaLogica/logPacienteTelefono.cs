using CapaAccesoDatos;
using CapaEntidad;

namespace CapaLogica
{
    public class logPacienteTelefono
    {

        #region Singleton
        private static readonly logPacienteTelefono UnicaInstancia = new logPacienteTelefono();
        public static logPacienteTelefono Instancia
        {
            get { return logPacienteTelefono.UnicaInstancia; }
        }
        private logPacienteTelefono() { }
        #endregion

        #region Métodos
        // LISTAR
        public List<entPacienteTelefono> ListarPacienteTelefono()
        {
            return DA_PacienteTelefono.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarPacienteTelefono(entPacienteTelefono entidad)
        {
            return DA_PacienteTelefono.Instancia.Insertar(entidad);
        }
        #endregion



    }
}

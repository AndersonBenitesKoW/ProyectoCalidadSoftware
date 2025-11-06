using CapaAccesoDatos;
using CapaEntidad;
namespace CapaLogica
{
    public class logEstadoCita
    {
        #region Singleton
        private static readonly logEstadoCita UnicaInstancia = new logEstadoCita();
        public static logEstadoCita Instancia
        {
            get { return logEstadoCita.UnicaInstancia; }
        }
        private logEstadoCita() { }
        #endregion

        // LISTAR
        public List<entEstadoCita> ListarEstadoCita()
        {
            return DA_EstadoCita.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarEstadoCita(entEstadoCita entidad)
        {
            return DA_EstadoCita.Instancia.Insertar(entidad);
        }

    }
}

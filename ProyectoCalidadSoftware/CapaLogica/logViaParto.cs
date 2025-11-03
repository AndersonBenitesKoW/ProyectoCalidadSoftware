using CapaAccesoDatos;
using CapaEntidad;

namespace CapaLogica
{
    public class logViaParto
    {

        #region Singleton
        private static readonly logViaParto UnicaInstancia = new logViaParto();
        public static logViaParto Instancia
        {
            get { return logViaParto.UnicaInstancia; }
        }
        private logViaParto() { }
        #endregion

        // LISTAR
        public List<entViaParto> Listar()
        {
            return DA_ViaParto.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarViaParto(entViaParto entidad)
        {
            return DA_ViaParto.Instancia.Insertar(entidad);
        }

    }
}

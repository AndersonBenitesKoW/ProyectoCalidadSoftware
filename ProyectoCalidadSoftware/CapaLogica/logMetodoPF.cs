using CapaAccesoDatos;
using CapaEntidad;

namespace CapaLogica
{
    public class logMetodoPF
    {

        #region Singleton
        private static readonly logMetodoPF UnicaInstancia = new logMetodoPF();
        public static logMetodoPF Instancia
        {
            get { return logMetodoPF.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        public List<entMetodoPF> ListarMetodoPF()
        {
            return DA_MetodoPF.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarMetodoPF(entMetodoPF entidad)
        {
            return DA_MetodoPF.Instancia.Insertar(entidad);
        }

    }
}

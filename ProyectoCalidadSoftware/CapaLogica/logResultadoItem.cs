using CapaAccesoDatos;
using CapaEntidad;
namespace CapaLogica
{
    public class logResultadoItem
    {
        #region Singleton
        private static readonly logResultadoItem UnicaInstancia = new logResultadoItem();
        public static logResultadoItem Instancia
        {
            get { return logResultadoItem.UnicaInstancia; }
        }
        private logResultadoItem() { }
        #endregion

        // LISTAR
        public List<entResultadoItem> ListarResultadoItem()
        {
            return DA_ResultadoItem.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarResultadoItem(entResultadoItem entidad)
        {
            return DA_ResultadoItem.Instancia.Insertar(entidad);
        }

        // ELIMINAR POR RESULTADO
        public bool EliminarResultadoItemPorResultado(int idResultado)
        {
            return DA_ResultadoItem.Instancia.EliminarPorResultado(idResultado);
        }

    }
}

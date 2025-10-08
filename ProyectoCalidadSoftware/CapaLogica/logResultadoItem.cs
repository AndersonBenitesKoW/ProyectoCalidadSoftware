using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int InsertarResultadoItem(entResultadoItem entidad)
        {
            return DA_ResultadoItem.Instancia.Insertar(entidad);
        }

    }
}

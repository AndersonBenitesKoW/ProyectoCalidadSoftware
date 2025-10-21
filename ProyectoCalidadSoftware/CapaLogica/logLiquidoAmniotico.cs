using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaLogica
{
    public class logLiquidoAmniotico
    {
        #region Singleton
        private static readonly logLiquidoAmniotico UnicaInstancia = new logLiquidoAmniotico();
        public static logLiquidoAmniotico Instancia
        {
            get { return logLiquidoAmniotico.UnicaInstancia; }
        }
        private logLiquidoAmniotico() { }
        #endregion

        // LISTAR
        public List<entLiquidoAmniotico> ListarLiquidoAmniotico()
        {
            return DA_LiquidoAmniotico.Instancia.Listar();
        }

        // LISTAR (alias para compatibilidad)
        public List<entLiquidoAmniotico> Listar()
        {
            return ListarLiquidoAmniotico();
        }

        // INSERTAR
        public bool InsertarLiquidoAmniotico(entLiquidoAmniotico entidad)
        {
            return DA_LiquidoAmniotico.Instancia.Insertar(entidad);
        }

    }
}

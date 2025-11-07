using CapaAccesoDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logLiquidoAmniotico
    {
        #region Singleton
        private static readonly logLiquidoAmniotico _instancia = new logLiquidoAmniotico();
        public static logLiquidoAmniotico Instancia { get { return _instancia; } }
        #endregion

        public List<entLiquidoAmniotico> ListarLiquidos()
        {
            return DA_LiquidoAmniotico.Instancia.Listar();
        }
    }
}
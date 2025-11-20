using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logTipoEncuentro
    {
        #region Singleton
        private static readonly logTipoEncuentro _instancia = new logTipoEncuentro();
        public static logTipoEncuentro Instancia
        {
            get { return logTipoEncuentro._instancia; }
        }
        private logTipoEncuentro() { }
        #endregion

        // Renombrado para que EncuentroController lo encuentre
        public List<entTipoEncuentro> ListarTiposEncuentro()
        {
            try
            {
                return DA_TipoEncuentro.Instancia.Listar();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar tipos de encuentro: " + ex.Message, ex);
            }
        }

        public short ObtenerIdPorCodigo(string codigo)
        {
            try
            {
                return DA_TipoEncuentro.Instancia.ObtenerIdPorCodigo(codigo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener ID del tipo de encuentro: " + ex.Message, ex);
            }
        }
    }
}
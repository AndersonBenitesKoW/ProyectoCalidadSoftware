using System;
using System.Collections.Generic;
using CapaAccesoDatos;
using CapaEntidad;

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
        #endregion

        /// <summary>
        /// Llama a la Capa de Datos para listar Tipos de Encuentro
        /// </summary>
        public List<entTipoEncuentro> Listar()
        {
            return DA_TipoEncuentro.Instancia.Listar();
        }
    }
}
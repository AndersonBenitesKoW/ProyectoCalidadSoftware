using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logTipoAyudaDiagnostica
    {
        #region Singleton
        private static readonly logTipoAyudaDiagnostica _instancia = new logTipoAyudaDiagnostica();
        public static logTipoAyudaDiagnostica Instancia
        {
            get { return logTipoAyudaDiagnostica._instancia; }
        }
        #endregion

        public List<entTipoAyudaDiagnostica> ListarTiposAyuda()
        {
            try
            {
                return DA_TipoAyudaDiagnostica.Instancia.Listar();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar tipos de ayuda: " + ex.Message, ex);
            }
        }
    }
}
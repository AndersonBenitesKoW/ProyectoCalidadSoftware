using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logMetodoPF
    {
        #region Singleton
        private static readonly logMetodoPF _instancia = new logMetodoPF();
        public static logMetodoPF Instancia
        {
            get { return logMetodoPF._instancia; }
        }
        #endregion

        public List<entMetodoPF> ListarMetodosPF()
        {
            try
            {
                return DA_MetodoPF.Instancia.Listar();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar métodos PF: " + ex.Message, ex);
            }
        }
    }
}
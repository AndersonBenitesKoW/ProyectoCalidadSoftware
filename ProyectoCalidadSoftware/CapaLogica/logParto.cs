using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logParto
    {
        #region Singleton
        private static readonly logParto UnicaInstancia = new logParto();
        public static logParto Instancia { get { return logParto.UnicaInstancia; } }
        private logParto() { }
        #endregion

        public List<entParto> ListarPartos(bool estado)
        {
            try
            {
                return DA_Parto.Instancia.Listar(estado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar partos: " + ex.Message, ex);
            }
        }

        public bool RegistrarParto(entParto entidad)
        {
            try
            {
                if (entidad.IdEmbarazo <= 0)
                    throw new ApplicationException("El IdEmbarazo es obligatorio.");
                entidad.Estado = true;
                return DA_Parto.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al registrar parto: " + ex.Message, ex);
            }
        }

        public bool EditarParto(entParto entidad)
        {
            try
            {
                return DA_Parto.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al editar parto: " + ex.Message, ex);
            }
        }

        public entParto? BuscarParto(int id)
        {
            try
            {
                return DA_Parto.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al buscar parto: " + ex.Message, ex);
            }
        }

        public bool AnularParto(int id)
        {
            try
            {
                return DA_Parto.Instancia.Anular(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al anular parto: " + ex.Message, ex);
            }
        }
    }
}
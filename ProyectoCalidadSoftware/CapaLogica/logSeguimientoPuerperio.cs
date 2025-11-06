using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logSeguimientoPuerperio
    {
        #region Singleton
        private static readonly logSeguimientoPuerperio UnicaInstancia = new logSeguimientoPuerperio();
        public static logSeguimientoPuerperio Instancia
        {
            get { return logSeguimientoPuerperio.UnicaInstancia; }
        }
        private logSeguimientoPuerperio() { }
        #endregion

        public List<entSeguimientoPuerperio> ListarSeguimiento(bool estado)
        {
            try
            {
                return DA_SeguimientoPuerperio.Instancia.Listar(estado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar seguimientos: " + ex.Message, ex);
            }
        }

        public bool InsertarSeguimiento(entSeguimientoPuerperio entidad)
        {
            try
            {
                if (entidad.IdEmbarazo <= 0)
                    throw new ApplicationException("El IdEmbarazo es obligatorio.");
                entidad.Estado = true; // Aseguramos que se inserte como activo
                return DA_SeguimientoPuerperio.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar seguimiento: " + ex.Message, ex);
            }
        }

        public bool EditarSeguimiento(entSeguimientoPuerperio entidad)
        {
            try
            {
                return DA_SeguimientoPuerperio.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al editar seguimiento: " + ex.Message, ex);
            }
        }

        public entSeguimientoPuerperio? BuscarSeguimiento(int id)
        {
            try
            {
                return DA_SeguimientoPuerperio.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al buscar seguimiento: " + ex.Message, ex);
            }
        }

        public bool InhabilitarSeguimiento(int id)
        {
            try
            {
                return DA_SeguimientoPuerperio.Instancia.Inhabilitar(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al inhabilitar seguimiento: " + ex.Message, ex);
            }
        }
    }
}
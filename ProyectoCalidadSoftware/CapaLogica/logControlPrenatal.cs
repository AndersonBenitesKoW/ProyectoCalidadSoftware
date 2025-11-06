using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logControlPrenatal
    {
        #region Singleton
        private static readonly logControlPrenatal UnicaInstancia = new logControlPrenatal();
        public static logControlPrenatal Instancia
        {
            get { return logControlPrenatal.UnicaInstancia; }
        }
        private logControlPrenatal() { }
        #endregion

        public List<entControlPrenatal> ListarControlPrenatal()
        {
            try
            {
                return DA_ControlPrenatal.Instancia.Listar();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar controles: " + ex.Message, ex);
            }
        }

        public bool InsertarControlPrenatal(entControlPrenatal entidad)
        {
            try
            {
                // Aquí puedes añadir validaciones de negocio
                if (entidad.IdEmbarazo <= 0)
                    throw new ApplicationException("El IdEmbarazo es obligatorio.");
                if (entidad.Fecha > DateTime.Now.AddDays(1))
                    throw new ApplicationException("La fecha del control no puede ser futura.");

                entidad.Estado = true; // Aseguramos que se inserte como activo
                return DA_ControlPrenatal.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar control: " + ex.Message, ex);
            }
        }

        public bool EditarControlPrenatal(entControlPrenatal entidad)
        {
            try
            {
                return DA_ControlPrenatal.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al editar control: " + ex.Message, ex);
            }
        }

        public entControlPrenatal? BuscarControlPrenatal(int id)
        {
            try
            {
                return DA_ControlPrenatal.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al buscar control: " + ex.Message, ex);
            }
        }

        public bool InhabilitarControlPrenatal(int id)
        {
            try
            {
                return DA_ControlPrenatal.Instancia.Inhabilitar(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al inhabilitar control: " + ex.Message, ex);
            }
        }
    }
}
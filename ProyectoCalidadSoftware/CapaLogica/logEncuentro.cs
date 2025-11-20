using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logEncuentro
    {
        #region Singleton
        private static readonly logEncuentro UnicaInstancia = new logEncuentro();
        public static logEncuentro Instancia
        {
            get { return logEncuentro.UnicaInstancia; }
        }
        private logEncuentro() { }
        #endregion

        public List<entEncuentro> ListarEncuentros()
        {
            try
            {
                return DA_Encuentro.Instancia.Listar();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar encuentros: " + ex.Message, ex);
            }
        }

        public int InsertarEncuentro(entEncuentro entidad)
        {
            try
            {
                if (entidad.IdEmbarazo <= 0)
                    throw new ApplicationException("El IdEmbarazo es obligatorio.");
                if (entidad.IdTipoEncuentro <= 0)
                    throw new ApplicationException("El Tipo de Encuentro es obligatorio.");

                return DA_Encuentro.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar encuentro: " + ex.Message, ex);
            }
        }

        public bool EditarEncuentro(entEncuentro entidad)
        {
            try
            {
                return DA_Encuentro.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al editar encuentro: " + ex.Message, ex);
            }
        }

        public entEncuentro? BuscarEncuentro(int id)
        {
            try
            {
                return DA_Encuentro.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al buscar encuentro: " + ex.Message, ex);
            }
        }

        public bool AnularEncuentro(int id)
        {
            try
            {
                return DA_Encuentro.Instancia.Eliminar(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al anular encuentro: " + ex.Message, ex);
            }
        }
        public List<object> ListarEncuentrosPorEmbarazo(int idEmbarazo)
        {
            try
            {
                return DA_Encuentro.Instancia.ListarPorEmbarazo(idEmbarazo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar encuentros por embarazo: " + ex.Message, ex);
            }
        }
    }
}
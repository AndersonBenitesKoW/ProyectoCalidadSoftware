using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logProfesionalSalud
    {
        #region Singleton
        private static readonly logProfesionalSalud UnicaInstancia = new logProfesionalSalud();
        public static logProfesionalSalud Instancia
        {
            get { return logProfesionalSalud.UnicaInstancia; }
        }
        private logProfesionalSalud() { }
        #endregion

        public List<entProfesionalSalud> ListarProfesionalSalud(bool estado)
        {
            return DA_ProfesionalSalud.Instancia.Listar(estado);
        }

        public int InsertarProfesionalSalud(entProfesionalSalud entidad)
        {
            try
            {
                // Validación de negocio (ej. CMP no puede estar vacío)
                if (string.IsNullOrWhiteSpace(entidad.CMP))
                    throw new ArgumentException("El CMP es obligatorio.");
                if (string.IsNullOrWhiteSpace(entidad.EmailPrincipal))
                    throw new ArgumentException("El Email Principal es obligatorio.");
                if (string.IsNullOrWhiteSpace(entidad.TelefonoPrincipal))
                    throw new ArgumentException("El Teléfono Principal es obligatorio.");

                return DA_ProfesionalSalud.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el profesional: " + ex.Message);
            }
        }

        public entProfesionalSalud BuscarProfesionalSaludPorId(int id)
        {
            try
            {
                return DA_ProfesionalSalud.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar el profesional: " + ex.Message);
            }
        }

        public bool EditarProfesionalSalud(entProfesionalSalud entidad)
        {
            try
            {
                return DA_ProfesionalSalud.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el profesional: " + ex.Message);
            }
        }

        // --- MÉTODO NUEVO ---
        public bool InhabilitarProfesionalSalud(int idProfesional)
        {
            try
            {
                return DA_ProfesionalSalud.Instancia.Eliminar(idProfesional);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inhabilitar el profesional: " + ex.Message);
            }
        }

        public entProfesionalSalud BuscarProfesionalPorCMP(string cmp)
        {
            try
            {
                return DA_ProfesionalSalud.Instancia.BuscarPorCMP(cmp);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar el profesional por CMP: " + ex.Message);
            }
        }

    }
}
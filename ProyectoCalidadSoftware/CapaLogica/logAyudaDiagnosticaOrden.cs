using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logAyudaDiagnosticaOrden
    {
        #region Singleton
        private static readonly logAyudaDiagnosticaOrden UnicaInstancia = new logAyudaDiagnosticaOrden();
        public static logAyudaDiagnosticaOrden Instancia
        {
            get { return logAyudaDiagnosticaOrden.UnicaInstancia; }
        }
        private logAyudaDiagnosticaOrden() { }
        #endregion

        // ==== CORRECCIÓN ====
        // Renombrado de "ListarOrdenes" a "ListarAyudaDiagnosticaOrden"
        public List<entAyudaDiagnosticaOrden> ListarAyudaDiagnosticaOrden()
        {
            try
            {
                return DA_AyudaDiagnosticaOrden.Instancia.Listar();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar órdenes: " + ex.Message, ex);
            }
        }

        // ==== CORRECCIÓN ====
        // Renombrado de "InsertarOrden" a "InsertarAyudaDiagnosticaOrden"
        public bool InsertarAyudaDiagnosticaOrden(entAyudaDiagnosticaOrden entidad)
        {
            try
            {
                if (entidad.IdPaciente <= 0)
                    throw new ApplicationException("El IdPaciente es obligatorio.");
                if (entidad.IdTipoAyuda <= 0 && string.IsNullOrWhiteSpace(entidad.Descripcion))
                    throw new ApplicationException("Debe seleccionar un Tipo de Ayuda o ingresar una Descripción.");

                return DA_AyudaDiagnosticaOrden.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar orden: " + ex.Message, ex);
            }
        }

        // (Métodos restantes, asegúrate de que tu controlador los llame con estos nombres)
        public bool EditarOrden(entAyudaDiagnosticaOrden entidad)
        {
            try
            {
                return DA_AyudaDiagnosticaOrden.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al editar orden: " + ex.Message, ex);
            }
        }

        public entAyudaDiagnosticaOrden? BuscarOrden(int id)
        {
            try
            {
                return DA_AyudaDiagnosticaOrden.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al buscar orden: " + ex.Message, ex);
            }
        }

        public bool AnularOrden(int id)
        {
            try
            {
                return DA_AyudaDiagnosticaOrden.Instancia.Anular(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al anular orden: " + ex.Message, ex);
            }
        }
    }
}
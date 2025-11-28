using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logControlPrenatal_AyudaDiagnostica
    {
        #region Singleton
        private static readonly logControlPrenatal_AyudaDiagnostica UnicaInstancia = new logControlPrenatal_AyudaDiagnostica();
        public static logControlPrenatal_AyudaDiagnostica Instancia
        {
            get { return logControlPrenatal_AyudaDiagnostica.UnicaInstancia; }
        }
        private logControlPrenatal_AyudaDiagnostica() { }
        #endregion

        public List<entControlPrenatal_AyudaDiagnostica> ListarAyudasPorControl(int idControl)
        {
            try
            {
                return DA_ControlPrenatal_AyudaDiagnostica.Instancia.ListarPorIdControl(idControl);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al listar ayudas diagnósticas del control: " + ex.Message, ex);
            }
        }

        public bool InsertarAyudaDiagnosticaAlControl(entControlPrenatal_AyudaDiagnostica entidad)
        {
            try
            {
                if (entidad.IdControl <= 0)
                    throw new ApplicationException("El IdControl es obligatorio.");
                if (entidad.IdAyuda <= 0)
                    throw new ApplicationException("El IdAyuda es obligatorio.");

                return DA_ControlPrenatal_AyudaDiagnostica.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar ayuda diagnóstica al control: " + ex.Message, ex);
            }
        }

        public bool EditarAyudaDiagnosticaDelControl(entControlPrenatal_AyudaDiagnostica entidad)
        {
            try
            {
                return DA_ControlPrenatal_AyudaDiagnostica.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al editar ayuda diagnóstica del control: " + ex.Message, ex);
            }
        }

        public bool InhabilitarAyudaDiagnosticaDelControl(int idCP_AD)
        {
            try
            {
                return DA_ControlPrenatal_AyudaDiagnostica.Instancia.Inhabilitar(idCP_AD);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al inhabilitar ayuda diagnóstica del control: " + ex.Message, ex);
            }
        }
    }
}
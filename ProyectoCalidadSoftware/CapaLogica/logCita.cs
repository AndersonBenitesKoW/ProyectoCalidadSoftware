using CapaAccesoDatos;
using CapaEntidad;
using System; // Para Exception
using System.Collections.Generic; // Para List<>

namespace CapaLogica
{
    public class logCita
    {
        #region Singleton
        private static readonly logCita UnicaInstancia = new logCita();
        public static logCita Instancia
        {
            get { return logCita.UnicaInstancia; }
        }
        private logCita() { }
        #endregion

        // LISTAR
        public List<entCita> ListarCita()
        {
            try
            {
                return DA_Cita.Instancia.Listar();
            }
            catch (Exception ex)
            {
                // Es buena idea registrar el error
                throw new ApplicationException("Error al listar citas: " + ex.Message, ex);
            }
        }

        // INSERTAR
        public bool InsertarCita(entCita entidad)
        {
            try
            {
                // Aquí podrías agregar reglas de negocio
                // Ej: if (entidad.FechaCita < DateTime.Now) throw new Exception("...");
                return DA_Cita.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al insertar cita: " + ex.Message, ex);
            }
        }

        // EDITAR (NUEVO)
        public bool EditarCita(entCita entidad)
        {
            try
            {
                return DA_Cita.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al editar cita: " + ex.Message, ex);
            }
        }

        // BUSCAR (NUEVO)
        public entCita? BuscarCita(int id)
        {
            try
            {
                return DA_Cita.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al buscar cita: " + ex.Message, ex);
            }
        }

        // ANULAR (Tu código ya era correcto)
        // ANULAR
        public bool AnularCita(int id, string motivoAnulacion) // 1. Añadido parámetro
        {
            if (id <= 0)
            {
                throw new ApplicationException("El ID de la cita no es válido.");
            }
            if (string.IsNullOrWhiteSpace(motivoAnulacion))
            {
                throw new ApplicationException("El motivo de anulación es obligatorio.");
            }
            // 2. Pasamos ambos parámetros
            return DA_Cita.Instancia.Eliminar(id, motivoAnulacion);
        }
    }
}
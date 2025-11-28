using CapaAccesoDatos;
using CapaEntidad;
using System; // Agregado para Exception
using System.Collections.Generic; // Agregado para List<>
using System.Linq; // Agregado para .Where()

namespace CapaLogica
{
    public class logPaciente
    {
        #region Singleton
        private static readonly logPaciente UnicaInstancia = new logPaciente();
        public static logPaciente Instancia
        {
            get { return logPaciente.UnicaInstancia; }
        }
        private logPaciente() { }
        #endregion

        // LISTAR
        public List<entPaciente> ListarPaciente()
        {
            return DA_Paciente.Instancia.Listar();
        }

        // LISTAR PACIENTES ACTIVOS
        public List<entPaciente> ListarPacientesActivos()
        {
            return ListarPaciente().Where(p => p.Estado).ToList();
        }

        // INSERTAR
        public bool InsertarPaciente(entPaciente entidad)
        {
            try
            {
                return DA_Paciente.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en logPaciente.Insertar: {ex.Message}");
                // Relanzamos la excepción para que el controlador la atrape
                throw;
            }
        }

        // BUSCAR (CORREGIDO)
        public entPaciente? BuscarPaciente(int id)
        {
            // CORRECCIÓN: Tu DA_Paciente.BuscarPorId(id) ahora devuelve
            // un 'entPaciente' directamente, no un 'DataTable'.
            // Ya no necesitamos convertir de 'dt.Rows'.
            try
            {
                return DA_Paciente.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en logPaciente.Buscar: {ex.Message}");
                return null;
            }
        }

        // ACTUALIZAR (CORREGIDO)
        public bool ActualizarPaciente(entPaciente entidad)
        {
            // CORRECCIÓN: Llamamos al nuevo método 'Editar' que
            // recibe el objeto 'entPaciente' completo.
            // Esto evita el error de 'FechaNacimiento.Value'.
            try
            {
                return DA_Paciente.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en logPaciente.Actualizar: {ex.Message}");
                // Relanzamos para que el controlador muestre el error
                throw;
            }
        }

        // BUSCAR POR DNI
        public entPaciente? BuscarPacientePorDNI(string dni)
        {
            try
            {
                return DA_Paciente.Instancia.BuscarPorDNI(dni);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en logPaciente.BuscarPorDNI: {ex.Message}");
                return null;
            }
        }

        // INHABILITAR
        public bool InhabilitarPaciente(int id)
        {
            try
            {
                // (Esto asume que DA_Paciente.Eliminar llama a sp_InhabilitarPaciente)
                return DA_Paciente.Instancia.Eliminar(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en logPaciente.Inhabilitar: {ex.Message}");
                throw;
            }
        }
    }
}
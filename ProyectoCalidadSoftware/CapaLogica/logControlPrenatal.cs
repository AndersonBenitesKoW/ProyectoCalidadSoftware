using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Transactions;

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

        public int InsertarControlPrenatal(entControlPrenatal entidad)
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

        public int RegistrarControlPrenatalConEncuentro(entControlPrenatal control, int idProfesional)
        {
            Console.WriteLine("LOG LOGICA: Iniciando RegistrarControlPrenatalConEncuentro");
            Console.WriteLine($"LOG LOGICA: IdEmbarazo={control.IdEmbarazo}, idProfesional={idProfesional}");
            using (var scope = new TransactionScope())
            {
                try
                {
                    // 1. Obtener IdTipoEncuentro para "ANC"
                    short idTipoEncuentro = logTipoEncuentro.Instancia.ObtenerIdPorCodigo("ANC");
                    Console.WriteLine($"LOG LOGICA: idTipoEncuentro={idTipoEncuentro}");
                    if (idTipoEncuentro == 0)
                        throw new ApplicationException("Tipo de encuentro 'ANC' no encontrado.");

                    // 2. Crear Encuentro
                    var enc = new entEncuentro
                    {
                        IdEmbarazo = control.IdEmbarazo,
                        IdProfesional = idProfesional,
                        IdTipoEncuentro = idTipoEncuentro,
                        FechaHoraInicio = DateTime.UtcNow,
                        Estado = "Cerrado"
                    };
                    int idEncuentro = logEncuentro.Instancia.InsertarEncuentro(enc);
                    Console.WriteLine($"LOG LOGICA: idEncuentro insertado={idEncuentro}");

                    // 3. Asignar el IdEncuentro al control
                    control.IdEncuentro = idEncuentro;

                    // 4. Insertar el Control Prenatal
                    int idControl = DA_ControlPrenatal.Instancia.Insertar(control);
                    Console.WriteLine($"LOG LOGICA: idControl insertado={idControl}");

                    scope.Complete();
                    Console.WriteLine("LOG LOGICA: Transacción completada");
                    return idControl;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LOG LOGICA: Error en RegistrarControlPrenatalConEncuentro: {ex.Message}");
                    throw new ApplicationException("Error al registrar control prenatal con encuentro: " + ex.Message, ex);
                }
            }
        }
    }
}

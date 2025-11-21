using CapaAccesoDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Transactions;

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
            using (var scope = new TransactionScope())
            {
                try
                {
                    if (entidad.IdEmbarazo <= 0)
                        throw new ApplicationException("El IdEmbarazo es obligatorio.");

                    entidad.Estado = true;

                    // Crear encuentro automáticamente para el parto
                    short idTipoEncuentro = logTipoEncuentro.Instancia.ObtenerIdPorCodigo("INTRAPARTO");
                    if (idTipoEncuentro == 0)
                        throw new ApplicationException("Tipo de encuentro 'INTRAPARTO' no encontrado.");

                    var encuentro = new entEncuentro
                    {
                        IdEmbarazo = entidad.IdEmbarazo,
                        IdProfesional = entidad.IdProfesional,
                        IdTipoEncuentro = idTipoEncuentro,
                        FechaHoraInicio = DateTime.UtcNow,
                        Estado = "Cerrado",
                        Notas = $"Encuentro generado automáticamente al registrar el parto - Fecha: {entidad.Fecha.ToShortDateString()}"
                    };

                    int idEncuentro = logEncuentro.Instancia.InsertarEncuentro(encuentro);
                    entidad.IdEncuentro = idEncuentro;

                    // Insertar el parto
                    bool resultado = DA_Parto.Instancia.Insertar(entidad);

                    scope.Complete();
                    return resultado;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error al registrar parto: " + ex.Message, ex);
                }
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

        public int RegistrarPartoConEncuentro(entParto parto, int idProfesional)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    // 1. Obtener IdTipoEncuentro para "INTRAPARTO" (Trabajo de parto/Parto)
                    short idTipoEncuentro = logTipoEncuentro.Instancia.ObtenerIdPorCodigo("INTRAPARTO");
                    if (idTipoEncuentro == 0)
                        throw new ApplicationException("Tipo de encuentro 'INTRAPARTO' no encontrado.");

                    // 2. Crear Encuentro
                    var enc = new entEncuentro
                    {
                        IdEmbarazo = parto.IdEmbarazo,
                        IdProfesional = idProfesional,
                        IdTipoEncuentro = idTipoEncuentro,
                        FechaHoraInicio = DateTime.UtcNow,
                        Estado = "Cerrado"
                    };
                    int idEncuentro = logEncuentro.Instancia.InsertarEncuentro(enc);

                    // 3. Asignar el IdEncuentro al parto
                    parto.IdEncuentro = idEncuentro;

                    // 4. Insertar el Parto
                    int idParto = DA_Parto.Instancia.InsertarConId(parto);

                    scope.Complete();
                    return idParto;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error al registrar parto con encuentro: " + ex.Message, ex);
                }
            }
        }
    }
}
using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Parto
    {
        #region Singleton
        private static readonly DA_Parto _instancia = new DA_Parto();
        public static DA_Parto Instancia
        {
            get { return DA_Parto._instancia; }
        }
        #endregion

        #region Métodos

        /// <summary>
        /// Registra un Parto y sus Intervenciones usando una Transacción.
        /// </summary>
        /// <param name="parto">Entidad Parto (que contiene la lista de Intervenciones)</param>
        /// <returns>El IdParto generado si fue exitoso, 0 si falló.</returns>
        public int RegistrarParto(entParto parto)
        {
            int idPartoGenerado = 0;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                cn.Open();
                SqlTransaction transaccion = cn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarParto", cn, transaccion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@IdEmbarazo", parto.IdEmbarazo);
                        cmd.Parameters.AddWithValue("@IdEncuentro", (object)parto.IdEncuentro ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IdProfesional", (object)parto.IdProfesional ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Fecha", parto.Fecha.Date);
                        cmd.Parameters.AddWithValue("@HoraIngreso", (object)parto.HoraIngreso ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HoraInicioTrabajo", (object)parto.HoraInicioTrabajo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Membranas", (object)parto.Membranas ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IdLiquido", (object)parto.IdLiquido ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Analgesia", (object)parto.Analgesia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IdViaParto", (object)parto.IdViaParto ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IndicacionCesarea", (object)parto.IndicacionCesarea ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PerdidasML", (object)parto.PerdidasML ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Desgarro", (object)parto.Desgarro ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Complicaciones", (object)parto.Complicaciones ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", parto.Estado);

                        idPartoGenerado = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (idPartoGenerado > 0 && parto.Intervenciones != null)
                    {
                        foreach (var intervencion in parto.Intervenciones)
                        {
                            using (SqlCommand cmdDetalle = new SqlCommand("sp_InsertarPartoIntervencion", cn, transaccion))
                            {
                                cmdDetalle.CommandType = CommandType.StoredProcedure;
                                cmdDetalle.Parameters.AddWithValue("@IdParto", idPartoGenerado);
                                cmdDetalle.Parameters.AddWithValue("@Intervencion", intervencion.Intervencion);

                                cmdDetalle.ExecuteNonQuery();
                            }
                        }
                    }

                    transaccion.Commit();
                }
                catch (Exception)
                {
                    transaccion.Rollback();
                    idPartoGenerado = 0;
                    throw;
                }
            }

            return idPartoGenerado;
        }

        /// <summary>
        /// Realiza una anulación lógica del registro de Parto (Estado = 0).
        /// </summary>
        /// <param name="idParto">ID del Parto a anular</param>
        /// <returns>True si fue exitoso, False si falló.</returns>
        public bool AnularParto(int idParto)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_AnularParto", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdParto", idParto);

                    cn.Open();
                    // ExecuteNonQuery devuelve el número de filas afectadas
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public List<entParto> Listar(bool estado)
        {
            List<entParto> lista = new List<entParto>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarPartos", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new entParto
                            {
                                IdParto = Convert.ToInt32(dr["IdParto"]),
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                Estado = Convert.ToBoolean(dr["Estado"]),
                                IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                                // Cargamos las propiedades adicionales
                                NombrePaciente = dr["NombrePaciente"].ToString() ?? string.Empty,
                                DescripcionViaParto = dr["DescripcionViaParto"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public entParto BuscarPorId(int idParto)
        {
            entParto? parto = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarPartoPorId", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdParto", idParto);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // Leer el PRIMER conjunto de resultados (Datos del Parto)
                        if (dr.Read())
                        {
                            parto = new entParto
                            {
                                IdParto = Convert.ToInt32(dr["IdParto"]),
                                IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                                IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? Convert.ToInt32(dr["IdEncuentro"]) : (int?)null,
                                IdProfesional = dr["IdProfesional"] != DBNull.Value ? Convert.ToInt32(dr["IdProfesional"]) : (int?)null,
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                HoraIngreso = dr["HoraIngreso"] != DBNull.Value ? Convert.ToDateTime(dr["HoraIngreso"]) : (DateTime?)null,
                                HoraInicioTrabajo = dr["HoraInicioTrabajo"] != DBNull.Value ? Convert.ToDateTime(dr["HoraInicioTrabajo"]) : (DateTime?)null,
                                Membranas = dr["Membranas"]?.ToString(),
                                IdLiquido = dr["IdLiquido"] != DBNull.Value ? Convert.ToInt16(dr["IdLiquido"]) : (short?)null,
                                Analgesia = dr["Analgesia"]?.ToString(),
                                IdViaParto = dr["IdViaParto"] != DBNull.Value ? Convert.ToInt16(dr["IdViaParto"]) : (short?)null,
                                IndicacionCesarea = dr["IndicacionCesarea"]?.ToString(),
                                PerdidasML = dr["PerdidasML"] != DBNull.Value ? Convert.ToInt32(dr["PerdidasML"]) : (int?)null,
                                Desgarro = dr["Desgarro"]?.ToString(),
                                Complicaciones = dr["Complicaciones"]?.ToString(),
                                Estado = Convert.ToBoolean(dr["Estado"]),
                                NombrePaciente = dr["NombrePaciente"]?.ToString() ?? string.Empty,
                                DescripcionViaParto = dr["DescripcionViaParto"]?.ToString() ?? string.Empty,
                                NombreProfesional = dr["NombreProfesional"]?.ToString() ?? string.Empty,
                                DescripcionLiquido = dr["DescripcionLiquido"]?.ToString() ?? string.Empty,
                            };
                        }

                        if (parto != null && dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                parto.Intervenciones.Add(new entPartoIntervencion
                                {
                                    IdPartoIntervencion = Convert.ToInt32(dr["IdPartoIntervencion"]),
                                    IdParto = idParto, // Ya lo sabemos
                                    Intervencion = dr["Intervencion"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            return parto;
        }


        #endregion
    }
}
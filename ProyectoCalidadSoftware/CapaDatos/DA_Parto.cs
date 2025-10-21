using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

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

<<<<<<< HEAD
        public List<entParto> Listar()
        {
            List<entParto> lista = new List<entParto>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarParto", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var parto = new entParto
                        {
                            IdParto = Convert.ToInt32(dr["IdParto"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEncuentro"]) : null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            HoraIngreso = dr["HoraIngreso"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HoraIngreso"]) : null,
                            HoraInicioTrabajo = dr["HoraInicioTrabajo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HoraInicioTrabajo"]) : null,
                            Membranas = dr["Membranas"].ToString(),
                            IdLiquido = dr["IdLiquido"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdLiquido"]) : null,
                            Analgesia = dr["Analgesia"].ToString(),
                            IdViaParto = dr["IdViaParto"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdViaParto"]) : null,
                            IndicacionCesarea = dr["IndicacionCesarea"].ToString(),
                            PerdidasML = dr["PerdidasML"] != DBNull.Value ? (int?)Convert.ToInt32(dr["PerdidasML"]) : null,
                            Desgarro = dr["Desgarro"].ToString(),
                            Complicaciones = dr["Complicaciones"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(parto);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entParto entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
                cmd.Parameters.AddWithValue("@FechaHora", entidad.Fecha);
                cmd.Parameters.AddWithValue("@TipoParto", (object)entidad.Membranas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Lugar", (object)entidad.Analgesia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.Complicaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idParto, int idEmbarazo, DateTime fechaHora, string tipoParto, string lugar, string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);
                cmd.Parameters.AddWithValue("@FechaHora", fechaHora);
                cmd.Parameters.AddWithValue("@TipoParto", (object)tipoParto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Lugar", (object)lugar ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idParto)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", idParto);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idParto)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", idParto);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


=======
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
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
}
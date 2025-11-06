using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Encuentro
    {
        #region Singleton
        private static readonly DA_Encuentro _instancia = new DA_Encuentro();
        public static DA_Encuentro Instancia
        {
            get { return DA_Encuentro._instancia; }
        }
        #endregion

        public List<entEncuentro> Listar()
        {
            List<entEncuentro> lista = new List<entEncuentro>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarEncuentros", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entEncuentro
                        {
                            IdEncuentro = Convert.ToInt32(dr["IdEncuentro"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? Convert.ToInt32(dr["IdProfesional"]) : (int?)null,
                            IdTipoEncuentro = Convert.ToInt16(dr["IdTipoEncuentro"]),
                            FechaHoraInicio = Convert.ToDateTime(dr["FechaHoraInicio"]),
                            FechaHoraFin = dr["FechaHoraFin"] != DBNull.Value ? Convert.ToDateTime(dr["FechaHoraFin"]) : (DateTime?)null,
                            Estado = dr["Estado"].ToString(),
                            Notas = dr["Notas"].ToString(),
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString(),
                            TipoEncuentroDesc = dr["TipoEncuentroDesc"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public bool Insertar(entEncuentro entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarEncuentro", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdTipoEncuentro", entidad.IdTipoEncuentro);
                cmd.Parameters.AddWithValue("@Notas", (object)entidad.Notas ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool Editar(entEncuentro entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarEncuentro", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEncuentro", entidad.IdEncuentro);
                cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdTipoEncuentro", entidad.IdTipoEncuentro);
                cmd.Parameters.AddWithValue("@FechaHoraFin", (object)entidad.FechaHoraFin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);
                cmd.Parameters.AddWithValue("@Notas", (object)entidad.Notas ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public entEncuentro? BuscarPorId(int idEncuentro)
        {
            entEncuentro? entidad = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarEncuentro", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEncuentro", idEncuentro);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            entidad = new entEncuentro
                            {
                                IdEncuentro = Convert.ToInt32(dr["IdEncuentro"]),
                                IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                                IdProfesional = dr["IdProfesional"] != DBNull.Value ? Convert.ToInt32(dr["IdProfesional"]) : (int?)null,
                                IdTipoEncuentro = Convert.ToInt16(dr["IdTipoEncuentro"]),
                                FechaHoraInicio = Convert.ToDateTime(dr["FechaHoraInicio"]),
                                FechaHoraFin = dr["FechaHoraFin"] != DBNull.Value ? Convert.ToDateTime(dr["FechaHoraFin"]) : (DateTime?)null,
                                Estado = dr["Estado"].ToString(),
                                Notas = dr["Notas"].ToString(),
                                NombrePaciente = dr["NombrePaciente"].ToString(),
                                NombreProfesional = dr["NombreProfesional"].ToString(),
                                TipoEncuentroDesc = dr["TipoEncuentroDesc"].ToString()
                            };
                        }
                    }
                }
            }
            return entidad;
        }

        // Método de Anulación
        public bool Eliminar(int idEncuentro)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarEncuentro", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEncuentro", idEncuentro);

                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
        public List<object> ListarPorEmbarazo(int idEmbarazo)
        {
            var lista = new List<object>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarEncuentrosPorEmbarazo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // Creamos un objeto anónimo simple para el JSON
                            lista.Add(new
                            {
                                IdEncuentro = Convert.ToInt32(dr["IdEncuentro"]),
                                EncuentroDesc = dr["EncuentroDesc"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}
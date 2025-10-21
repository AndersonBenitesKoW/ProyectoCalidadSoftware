using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

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

        #region Métodos

        public List<entEncuentro> Listar()
        {
            List<entEncuentro> lista = new List<entEncuentro>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarEncuentro", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var encuentro = new entEncuentro
                        {
                            IdEncuentro = Convert.ToInt32(dr["IdEncuentro"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            IdTipoEncuentro = Convert.ToInt16(dr["IdTipoEncuentro"]),
                            FechaHoraInicio = Convert.ToDateTime(dr["FechaHoraInicio"]),
                            FechaHoraFin = Convert.ToDateTime(dr["FechaHoraFin"]),
                            Estado = dr["Estado"].ToString(),
                            Notas = dr["Notas"].ToString()
                        };

                        lista.Add(encuentro);
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
                cmd.Parameters.AddWithValue("@IdProfesional", entidad.IdProfesional);
                cmd.Parameters.AddWithValue("@IdTipoEncuentro", entidad.IdTipoEncuentro);
                cmd.Parameters.AddWithValue("@FechaHoraInicio", entidad.FechaHoraInicio);
                cmd.Parameters.AddWithValue("@FechaHoraFin", entidad.FechaHoraFin);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);
                cmd.Parameters.AddWithValue("@Notas", (object)entidad.Notas ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idEncuentro, int idEmbarazo, int? idProfesional, short idTipoEncuentro,
                           DateTime? fechaHoraInicio, DateTime? fechaHoraFin,
                           string estado, string notas)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarEncuentro", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEncuentro", idEncuentro);
                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)idProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdTipoEncuentro", idTipoEncuentro);
                cmd.Parameters.AddWithValue("@FechaHoraInicio", (object)fechaHoraInicio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaHoraFin", (object)fechaHoraFin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", (object)estado ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notas", (object)notas ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idEncuentro)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarEncuentro", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEncuentro", idEncuentro);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idEncuentro)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarEncuentro", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEncuentro", idEncuentro);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
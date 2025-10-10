using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_ProfesionalSalud
    {
        #region Singleton
        private static readonly DA_ProfesionalSalud _instancia = new DA_ProfesionalSalud();
        public static DA_ProfesionalSalud Instancia
        {
            get { return DA_ProfesionalSalud._instancia; }
        }
        #endregion

        #region Métodos

        public List<entProfesionalSalud> Listar()
        {
            List<entProfesionalSalud> lista = new List<entProfesionalSalud>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarProfesionalSalud", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var profesional = new entProfesionalSalud
                        {
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            IdUsuario = dr["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdUsuario"]) : null,
                            CMP = dr["CMP"].ToString(),
                            Especialidad = dr["Especialidad"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(profesional);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entProfesionalSalud entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);
                cmd.Parameters.AddWithValue("@Especialidad", (object)entidad.Especialidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Colegiatura", entidad.CMP);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idProfesional, string nombres, string apellidos, string especialidad, string colegiatura, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);
                cmd.Parameters.AddWithValue("@Nombres", (object)nombres ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellidos", (object)apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Especialidad", (object)especialidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Colegiatura", (object)colegiatura ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idProfesional)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idProfesional)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
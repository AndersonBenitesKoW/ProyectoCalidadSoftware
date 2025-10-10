using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_Usuario
    {
        #region Singleton
        private static readonly DA_Usuario _instancia = new DA_Usuario();
        public static DA_Usuario Instancia
        {
            get { return DA_Usuario._instancia; }
        }
        #endregion

        #region Métodos

        public List<entUsuario> Listar()
        {
            List<entUsuario> lista = new List<entUsuario>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarUsuario", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var usuario = new entUsuario
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            ClaveHash = dr["ClaveHash"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(usuario);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entUsuario entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", entidad.NombreUsuario);
                cmd.Parameters.AddWithValue("@PasswordHash", entidad.ClaveHash);
                cmd.Parameters.AddWithValue("@Correo", "");
                cmd.Parameters.AddWithValue("@IdRol", 1);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idUsuario, string username, string passwordHash, string correo, int idRol, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@Username", (object)username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PasswordHash", (object)passwordHash ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", (object)correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdRol", idRol);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idUsuario)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idUsuario)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
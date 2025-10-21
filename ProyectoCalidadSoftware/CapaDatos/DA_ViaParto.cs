using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_ViaParto
    {
        #region Singleton
        private static readonly DA_ViaParto _instancia = new DA_ViaParto();
        public static DA_ViaParto Instancia
        {
            get { return DA_ViaParto._instancia; }
        }
        #endregion

        #region Métodos

        public List<entViaParto> Listar()
        {
            List<entViaParto> lista = new List<entViaParto>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarViaParto", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var via = new entViaParto
                        {
                            IdViaParto = Convert.ToInt16(dr["IdViaParto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        };

                        lista.Add(via);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entViaParto entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarViaParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", entidad.Codigo);
                cmd.Parameters.AddWithValue("@Descripcion", entidad.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", true);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idViaParto, string nombre, string descripcion, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarViaParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdViaParto", idViaParto);
                cmd.Parameters.AddWithValue("@Nombre", (object)nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idViaParto)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarViaParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdViaParto", idViaParto);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idViaParto)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarViaParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdViaParto", idViaParto);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
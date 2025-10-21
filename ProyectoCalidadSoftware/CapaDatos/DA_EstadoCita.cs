using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_EstadoCita
    {
        #region Singleton
        private static readonly DA_EstadoCita _instancia = new DA_EstadoCita();
        public static DA_EstadoCita Instancia
        {
            get { return DA_EstadoCita._instancia; }
        }
        #endregion

        #region Métodos

        public List<entEstadoCita> Listar()
        {
            List<entEstadoCita> lista = new List<entEstadoCita>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarEstadoCita", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var estadoCita = new entEstadoCita
                        {
                            IdEstadoCita = Convert.ToInt16(dr["IdEstadoCita"]),
                            Codigo = dr["Codigo"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        };

                        lista.Add(estadoCita);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entEstadoCita entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarEstadoCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Codigo", entidad.Codigo);
                cmd.Parameters.AddWithValue("@Descripcion", entidad.Descripcion);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idEstadoCita, string codigo, string descripcion)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarEstadoCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEstadoCita", idEstadoCita);
                cmd.Parameters.AddWithValue("@Codigo", codigo);
                cmd.Parameters.AddWithValue("@Descripcion", descripcion);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idEstadoCita)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarEstadoCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoCita", idEstadoCita);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idEstadoCita)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarEstadoCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoCita", idEstadoCita);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
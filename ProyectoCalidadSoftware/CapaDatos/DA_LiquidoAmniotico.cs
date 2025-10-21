using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_LiquidoAmniotico
    {
        #region Singleton
        private static readonly DA_LiquidoAmniotico _instancia = new DA_LiquidoAmniotico();
        public static DA_LiquidoAmniotico Instancia
        {
            get { return DA_LiquidoAmniotico._instancia; }
        }
        #endregion

        #region Métodos

        public List<entLiquidoAmniotico> Listar()
        {
            List<entLiquidoAmniotico> lista = new List<entLiquidoAmniotico>();
<<<<<<< HEAD

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarLiquidoAmniotico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
=======
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarLiquidoAmniotico", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
<<<<<<< HEAD
                        var liquido = new entLiquidoAmniotico
                        {
                            IdLiquido = Convert.ToInt16(dr["IdLiquido"]),
                            Codigo = dr["Codigo"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        };

                        lista.Add(liquido);
                    }
                }
            }

=======
                        lista.Add(new entLiquidoAmniotico
                        {
                            IdLiquido = Convert.ToInt16(dr["IdLiquido"]),
                            Codigo = dr["Codigo"].ToString() ?? string.Empty,
                            Descripcion = dr["Descripcion"].ToString() ?? string.Empty
                        });
                    }
                }
            }
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
            return lista;
        }

        public bool Insertar(entLiquidoAmniotico entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarLiquidoAmniotico", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Descripcion", entidad.Descripcion);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idLiquidoAmniotico, string descripcion)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarLiquidoAmniotico", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdLiquidoAmniotico", idLiquidoAmniotico);
                cmd.Parameters.AddWithValue("@Descripcion", descripcion);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idLiquidoAmniotico)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarLiquidoAmniotico", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLiquidoAmniotico", idLiquidoAmniotico);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idLiquidoAmniotico)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarLiquidoAmniotico", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLiquidoAmniotico", idLiquidoAmniotico);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
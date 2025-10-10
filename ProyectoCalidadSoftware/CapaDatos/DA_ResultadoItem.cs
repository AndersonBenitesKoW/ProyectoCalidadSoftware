using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_ResultadoItem
    {
        #region Singleton
        private static readonly DA_ResultadoItem _instancia = new DA_ResultadoItem();
        public static DA_ResultadoItem Instancia
        {
            get { return DA_ResultadoItem._instancia; }
        }
        #endregion

        #region Métodos

        public List<entResultadoItem> Listar()
        {
            List<entResultadoItem> lista = new List<entResultadoItem>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarResultadoItem", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var item = new entResultadoItem
                        {
                            IdResultadoItem = Convert.ToInt32(dr["IdResultadoItem"]),
                            IdResultado = Convert.ToInt32(dr["IdResultado"]),
                            Parametro = dr["Parametro"].ToString(),
                            ValorNumerico = dr["ValorNumerico"] != DBNull.Value ? Convert.ToDecimal(dr["ValorNumerico"]) : null,
                            ValorTexto = dr["ValorTexto"].ToString(),
                            Unidad = dr["Unidad"].ToString(),
                            RangoRef = dr["RangoRef"].ToString()
                        };

                        lista.Add(item);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entResultadoItem entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarResultadoItem", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdResultado", entidad.IdResultado);
                cmd.Parameters.AddWithValue("@Nombre", entidad.Parametro);
                cmd.Parameters.AddWithValue("@Valor", (object)entidad.ValorTexto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Unidad", (object)entidad.Unidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.RangoRef ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", true);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idResultadoItem, int idResultado, string nombre, string valor, string unidad, string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarResultadoItem", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdResultadoItem", idResultadoItem);
                cmd.Parameters.AddWithValue("@IdResultado", idResultado);
                cmd.Parameters.AddWithValue("@Nombre", (object)nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Valor", (object)valor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Unidad", (object)unidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idResultadoItem)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarResultadoItem", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultadoItem", idResultadoItem);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idResultadoItem)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarResultadoItem", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultadoItem", idResultadoItem);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
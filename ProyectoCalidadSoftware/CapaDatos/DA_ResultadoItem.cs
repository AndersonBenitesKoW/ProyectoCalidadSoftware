using System;
using System.Data;
using System.Data.SqlClient;

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

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarResultadoItem", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(int idResultado, string nombre, string valor, string unidad, string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarResultadoItem", cn);
                cmd.CommandType = CommandType.StoredProcedure;

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
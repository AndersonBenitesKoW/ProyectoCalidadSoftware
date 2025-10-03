using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Rol
    {
        #region Singleton
        private static readonly DA_Rol _instancia = new DA_Rol();
        public static DA_Rol Instancia
        {
            get { return DA_Rol._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(string nombre, string descripcion, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", (object)nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idRol, string nombre, string descripcion, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdRol", idRol);
                cmd.Parameters.AddWithValue("@Nombre", (object)nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idRol)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRol", idRol);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idRol)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRol", idRol);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
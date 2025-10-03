using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Embarazo
    {
        #region Singleton
        private static readonly DA_Embarazo _instancia = new DA_Embarazo();
        public static DA_Embarazo Instancia
        {
            get { return DA_Embarazo._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarEmbarazo", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(int idPaciente, DateTime? fur, DateTime? fpp,
                             string riesgo, DateTime? fechaApertura,
                             DateTime? fechaCierre, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarEmbarazo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@FUR", (object)fur ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FPP", (object)fpp ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Riesgo", (object)riesgo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaApertura", (object)fechaApertura ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCierre", (object)fechaCierre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idEmbarazo, int idPaciente, DateTime? fur, DateTime? fpp,
                           string riesgo, DateTime? fechaApertura,
                           DateTime? fechaCierre, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarEmbarazo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@FUR", (object)fur ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FPP", (object)fpp ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Riesgo", (object)riesgo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaApertura", (object)fechaApertura ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCierre", (object)fechaCierre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idEmbarazo)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarEmbarazo", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idEmbarazo)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarEmbarazo", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
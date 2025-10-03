using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_PacienteFactorRiesgo
    {
        #region Singleton
        private static readonly DA_PacienteFactorRiesgo _instancia = new DA_PacienteFactorRiesgo();
        public static DA_PacienteFactorRiesgo Instancia
        {
            get { return DA_PacienteFactorRiesgo._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(int idPaciente, int idFactorCat, string detalle, DateTime? fechaRegistro, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@IdFactorCat", idFactorCat);
                cmd.Parameters.AddWithValue("@Detalle", (object)detalle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaRegistro", (object)fechaRegistro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idPacienteFactor, int idPaciente, int idFactorCat, string detalle, DateTime? fechaRegistro, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPacienteFactor", idPacienteFactor);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@IdFactorCat", idFactorCat);
                cmd.Parameters.AddWithValue("@Detalle", (object)detalle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaRegistro", (object)fechaRegistro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idPacienteFactor)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteFactor", idPacienteFactor);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idPacienteFactor)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteFactor", idPacienteFactor);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
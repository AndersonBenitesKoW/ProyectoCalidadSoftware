using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Cita
    {
        #region Singleton
        private static readonly DA_Cita _instancia = new DA_Cita();
        public static DA_Cita Instancia
        {
            get { return DA_Cita._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(int idPaciente, int? idRecepcionista, int? idProfesional,
                             int? idEmbarazo, DateTime fechaCita, string motivo,
                             short idEstadoCita, string observacion,
                             DateTime? fechaAnulacion, string motivoAnulacion)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@IdRecepcionista", (object)idRecepcionista ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)idProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)idEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCita", fechaCita);
                cmd.Parameters.AddWithValue("@Motivo", (object)motivo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEstadoCita", idEstadoCita);
                cmd.Parameters.AddWithValue("@Observacion", (object)observacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaAnulacion", (object)fechaAnulacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MotivoAnulacion", (object)motivoAnulacion ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idCita, int idPaciente, int? idRecepcionista, int? idProfesional,
                           int? idEmbarazo, DateTime fechaCita, string motivo,
                           short idEstadoCita, string observacion,
                           DateTime? fechaAnulacion, string motivoAnulacion)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCita", idCita);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@IdRecepcionista", (object)idRecepcionista ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)idProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)idEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCita", fechaCita);
                cmd.Parameters.AddWithValue("@Motivo", (object)motivo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEstadoCita", idEstadoCita);
                cmd.Parameters.AddWithValue("@Observacion", (object)observacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaAnulacion", (object)fechaAnulacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MotivoAnulacion", (object)motivoAnulacion ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idCita)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idCita)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
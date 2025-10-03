using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_ControlPrenatal
    {
        #region Singleton
        private static readonly DA_ControlPrenatal _instancia = new DA_ControlPrenatal();
        public static DA_ControlPrenatal Instancia
        {
            get { return DA_ControlPrenatal._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(int idEmbarazo, int? idEncuentro, int? idProfesional, DateTime fecha,
                             decimal? pesoKg, decimal? tallaM, byte? paSistolica, byte? paDiastolica,
                             decimal? alturaUterinaCm, byte? fcfBpm, string presentacion,
                             string proteinuria, bool? movFetales, string consejerias,
                             string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);
                cmd.Parameters.AddWithValue("@IdEncuentro", (object)idEncuentro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)idProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@PesoKg", (object)pesoKg ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TallaM", (object)tallaM ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PA_Sistolica", (object)paSistolica ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PA_Diastolica", (object)paDiastolica ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AlturaUterina_cm", (object)alturaUterinaCm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FCF_bpm", (object)fcfBpm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Presentacion", (object)presentacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Proteinuria", (object)proteinuria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MovFetales", (object)movFetales ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Consejerias", (object)consejerias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idControl, int idEmbarazo, int? idEncuentro, int? idProfesional, DateTime fecha,
                           decimal? pesoKg, decimal? tallaM, byte? paSistolica, byte? paDiastolica,
                           decimal? alturaUterinaCm, byte? fcfBpm, string presentacion,
                           string proteinuria, bool? movFetales, string consejerias,
                           string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdControl", idControl);
                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);
                cmd.Parameters.AddWithValue("@IdEncuentro", (object)idEncuentro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)idProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@PesoKg", (object)pesoKg ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TallaM", (object)tallaM ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PA_Sistolica", (object)paSistolica ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PA_Diastolica", (object)paDiastolica ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AlturaUterina_cm", (object)alturaUterinaCm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FCF_bpm", (object)fcfBpm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Presentacion", (object)presentacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Proteinuria", (object)proteinuria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MovFetales", (object)movFetales ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Consejerias", (object)consejerias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idControl)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdControl", idControl);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idControl)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdControl", idControl);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
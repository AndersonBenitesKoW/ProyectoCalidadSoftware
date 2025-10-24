using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

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

        public List<entControlPrenatal> Listar()
        {
            List<entControlPrenatal> lista = new List<entControlPrenatal>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarControlPrenatal", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var control = new entControlPrenatal
                        {
                            IdControlPrenatal = Convert.ToInt32(dr["IdControl"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEncuentro"]) : null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            PesoKg = dr["PesoKg"] != DBNull.Value ? Convert.ToDecimal(dr["PesoKg"]) : null,
                            TallaM = dr["TallaM"] != DBNull.Value ? Convert.ToDecimal(dr["TallaM"]) : null,
                            PA_Sistolica = dr["PA_Sistolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Sistolica"]) : null,
                            PA_Diastolica = dr["PA_Diastolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Diastolica"]) : null,
                            AlturaUterina_cm = dr["AlturaUterina_cm"] != DBNull.Value ? Convert.ToDecimal(dr["AlturaUterina_cm"]) : null,
                            FCF_bpm = dr["FCF_bpm"] != DBNull.Value ? (byte?)Convert.ToByte(dr["FCF_bpm"]) : null,
                            Presentacion = dr["Presentacion"].ToString(),
                            Proteinuria = dr["Proteinuria"].ToString(),
                            MovFetales = dr["MovFetales"] != DBNull.Value ? (bool?)Convert.ToBoolean(dr["MovFetales"]) : null,
                            Consejerias = dr["Consejerias"].ToString(),
                            Observaciones = dr["Observaciones"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(control);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entControlPrenatal entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
                cmd.Parameters.AddWithValue("@IdEncuentro", (object)entidad.IdEncuentro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Fecha", entidad.Fecha);
                cmd.Parameters.AddWithValue("@PesoKg", (object)entidad.PesoKg ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TallaM", (object)entidad.TallaM ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PA_Sistolica", (object)entidad.PA_Sistolica ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PA_Diastolica", (object)entidad.PA_Diastolica ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AlturaUterina_cm", (object)entidad.AlturaUterina_cm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FCF_bpm", (object)entidad.FCF_bpm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Presentacion", (object)entidad.Presentacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Proteinuria", (object)entidad.Proteinuria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MovFetales", (object)entidad.MovFetales ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Consejerias", (object)entidad.Consejerias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.Observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public bool Inhabilitar(int idControl)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InhabilitarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdControl", idControl);

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
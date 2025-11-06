using CapaEntidad;
using System;
using System.Collections.Generic;
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
                        lista.Add(new entControlPrenatal
                        {
                            // Mapeo C# (IdControlPrenatal) a SQL (IdControl)
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
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            // Campos JOIN
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        // Método privado para añadir todos los parámetros
        private void AddControlParameters(SqlCommand cmd, entControlPrenatal entidad)
        {
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
        }

        public bool Insertar(entControlPrenatal entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                AddControlParameters(cmd, entidad); // Usa el método helper
                cn.Open();
                cmd.ExecuteNonQuery();
                return true; // Asumimos éxito
            }
        }

        public bool Editar(entControlPrenatal entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                // Mapeo C# (IdControlPrenatal) a SQL (@IdControl)
                cmd.Parameters.AddWithValue("@IdControl", entidad.IdControlPrenatal);
                AddControlParameters(cmd, entidad); // Usa el método helper
                cn.Open();
                cmd.ExecuteNonQuery();
                return true; // Asumimos éxito
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
                cmd.ExecuteNonQuery();
                return true; // Asumimos éxito
            }
        }

        public entControlPrenatal? BuscarPorId(int idControl)
        {
            entControlPrenatal? control = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarControlPrenatal", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdControl", idControl);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            control = new entControlPrenatal
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
                                Estado = Convert.ToBoolean(dr["Estado"]),
                                NombrePaciente = dr["NombrePaciente"].ToString(),
                                NombreProfesional = dr["NombreProfesional"].ToString()
                            };
                        }
                    }
                }
            }
            return control;
        }

        #endregion
    }
}
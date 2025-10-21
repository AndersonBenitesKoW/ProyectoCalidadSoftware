using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_SeguimientoPuerperio
    {
        #region Singleton
        private static readonly DA_SeguimientoPuerperio _instancia = new DA_SeguimientoPuerperio();
        public static DA_SeguimientoPuerperio Instancia
        {
            get { return DA_SeguimientoPuerperio._instancia; }
        }
        #endregion

        #region Métodos

        public List<entSeguimientoPuerperio> Listar()
        {
            List<entSeguimientoPuerperio> lista = new List<entSeguimientoPuerperio>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarSeguimientoPuerperio", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var seguimiento = new entSeguimientoPuerperio
                        {
                            IdPuerperio = Convert.ToInt32(dr["IdPuerperio"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEncuentro"]) : null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            PA_Sistolica = dr["PA_Sistolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Sistolica"]) : null,
                            PA_Diastolica = dr["PA_Diastolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Diastolica"]) : null,
                            Temp_C = dr["Temp_C"] != DBNull.Value ? Convert.ToDecimal(dr["Temp_C"]) : null,
                            AlturaUterinaPP_cm = dr["AlturaUterinaPP_cm"] != DBNull.Value ? Convert.ToDecimal(dr["AlturaUterinaPP_cm"]) : null,
                            Loquios = dr["Loquios"].ToString(),
                            Lactancia = dr["Lactancia"].ToString(),
                            SignosInfeccion = dr["SignosInfeccion"] != DBNull.Value ? (bool?)Convert.ToBoolean(dr["SignosInfeccion"]) : null,
                            TamizajeDepresion = dr["TamizajeDepresion"].ToString(),
                            IdMetodoPF = dr["IdMetodoPF"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdMetodoPF"]) : null,
                            Observaciones = dr["Observaciones"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(seguimiento);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entSeguimientoPuerperio entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
                cmd.Parameters.AddWithValue("@IdEncuentro", (object)entidad.IdEncuentro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Fecha", entidad.Fecha);
                cmd.Parameters.AddWithValue("@PA_Sistolica", (object)entidad.PA_Sistolica ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PA_Diastolica", (object)entidad.PA_Diastolica ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Temp_C", (object)entidad.Temp_C ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AlturaUterinaPP_cm", (object)entidad.AlturaUterinaPP_cm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Loquios", (object)entidad.Loquios ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Lactancia", (object)entidad.Lactancia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SignosInfeccion", (object)entidad.SignosInfeccion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TamizajeDepresion", (object)entidad.TamizajeDepresion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdMetodoPF", (object)entidad.IdMetodoPF ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.Observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Inhabilitar(int idPuerperio)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_InhabilitarSeguimientoPuerperio", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdPuerperio", idPuerperio);
                    cn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }



        public bool Editar(int idSeguimiento, int idParto, DateTime fecha, string hallazgos, string indicaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdSeguimiento", idSeguimiento);
                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@Hallazgos", (object)hallazgos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Indicaciones", (object)indicaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idSeguimiento)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSeguimiento", idSeguimiento);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idSeguimiento)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSeguimiento", idSeguimiento);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
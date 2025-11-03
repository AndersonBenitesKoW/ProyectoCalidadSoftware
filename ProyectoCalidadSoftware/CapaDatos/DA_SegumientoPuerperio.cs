using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

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


                            IdSeguimientoPuerperio = Convert.ToInt32(dr["IdPuerperio"]),

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
                            SignosInfeccion = dr["SignosInfeccion"] != DBNull.Value && Convert.ToBoolean(dr["SignosInfeccion"]),
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

        public bool Eliminar(int idPuerperio)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuerperio", idPuerperio);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Inhabilitar(int idPuerperio)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InhabilitarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuerperio", idPuerperio);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }




        public entSeguimientoPuerperio? BuscarPorId(int idSeguimiento)
        {
            entSeguimientoPuerperio? seguimiento = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarSeguimientoPuerperio", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSeguimiento", idSeguimiento);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        seguimiento = new entSeguimientoPuerperio
                        {
                            IdSeguimientoPuerperio = Convert.ToInt32(dr["IdPuerperio"]),
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
                            SignosInfeccion = dr["SignosInfeccion"] != DBNull.Value && Convert.ToBoolean(dr["SignosInfeccion"]),

                            TamizajeDepresion = dr["TamizajeDepresion"].ToString(),
                            IdMetodoPF = dr["IdMetodoPF"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdMetodoPF"]) : null,
                            Observaciones = dr["Observaciones"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }

            return seguimiento;
        }

    


        #endregion
    }


}
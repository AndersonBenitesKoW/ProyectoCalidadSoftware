using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_ResultadoDiagnostico
    {
        #region Singleton
        private static readonly DA_ResultadoDiagnostico _instancia = new DA_ResultadoDiagnostico();
        public static DA_ResultadoDiagnostico Instancia
        {
            get { return DA_ResultadoDiagnostico._instancia; }
        }
        #endregion

        #region Métodos

        public List<entResultadoDiagnostico> Listar()
        {
            List<entResultadoDiagnostico> lista = new List<entResultadoDiagnostico>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarResultadoDiagnostico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var resultado = new entResultadoDiagnostico
                        {
                            IdResultado = Convert.ToInt32(dr["IdResultado"]),
                            IdAyuda = Convert.ToInt32(dr["IdAyuda"]),
                            FechaResultado = Convert.ToDateTime(dr["FechaResultado"]),
                            Resumen = dr["Resumen"].ToString(),
                            Critico = Convert.ToBoolean(dr["Critico"]),
                            Estado = dr["Estado"].ToString()
                        };

                        lista.Add(resultado);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entResultadoDiagnostico entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarResultadoDiagnostico", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAyuda", entidad.IdAyuda);
                cmd.Parameters.AddWithValue("@FechaResultado", entidad.FechaResultado);
                cmd.Parameters.AddWithValue("@Resumen", (object)entidad.Resumen ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Critico", entidad.Critico);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }



        // UPDATE/Modificar con OBJETO
        public bool Actualizar(entResultadoDiagnostico entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ActualizarResultadoDiagnostico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdResultado", entidad.IdResultado);
                cmd.Parameters.AddWithValue("@IdAyuda", entidad.IdAyuda);
                cmd.Parameters.AddWithValue("@FechaResultado", entidad?.FechaResultado ?? DateTime.Now);
                cmd.Parameters.AddWithValue("@Resumen", (object)entidad.Resumen ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Critico", entidad.Critico);
                cmd.Parameters.AddWithValue("@Estado", (object)entidad.Estado ?? "ACTIVO");

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Buscar por Id -> ENTIDAD
        public entResultadoDiagnostico BuscarPorId(int idResultado)
        {
            entResultadoDiagnostico entidad = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarResultadoDiagnostico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultado", idResultado);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                        entidad = Map(dr);
                }
            }
            return entidad;
        }

        // Soft delete por estado
        public bool Anular(int idResultado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_AnularResultadoDiagnostico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultado", idResultado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // (Opcional) Eliminar físico si aún lo usas:
        public bool Eliminar(int idResultado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EliminarResultadoDiagnostico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultado", idResultado);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ---------- Helper de mapeo ----------
        private static entResultadoDiagnostico Map(SqlDataReader dr)
        {
            int Ord(string n) => dr.GetOrdinal(n);
            bool Nul(string n) => dr.IsDBNull(Ord(n));

            return new entResultadoDiagnostico
            {
                IdResultado = Nul("IdResultado") ? 0 : dr.GetInt32(Ord("IdResultado")),
                IdAyuda = Nul("IdAyuda") ? 0 : dr.GetInt32(Ord("IdAyuda")),
                FechaResultado = Nul("FechaResultado") ? DateTime.MinValue : Convert.ToDateTime(dr["FechaResultado"]),
                Resumen = Nul("Resumen") ? null : dr.GetString(Ord("Resumen")),
                Critico = !Nul("Critico") && Convert.ToBoolean(dr["Critico"]),
                Estado = Nul("Estado") ? "ACTIVO" : dr.GetString(Ord("Estado"))
            };
        }



        #endregion
    }


}
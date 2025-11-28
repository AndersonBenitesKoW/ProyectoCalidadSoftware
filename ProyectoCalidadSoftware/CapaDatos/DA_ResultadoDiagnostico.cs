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

        private DA_ResultadoDiagnostico() { }
        #endregion

        #region Métodos

        /// <summary>
        /// Lista resultados diagnósticos usando el SP detallado (JOINs en el SP).
        /// </summary>
        public List<entResultadoDiagnostico> Listar()
        {
            var lista = new List<entResultadoDiagnostico>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarResultadoDiagnosticoDetallado", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Para listar todos → @IdResultado = NULL
                var p = cmd.Parameters.Add("@IdResultado", SqlDbType.Int);
                p.Value = DBNull.Value;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(MapDetallado(dr));
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Inserta cabecera de resultado usando el SP.
        /// Devuelve el IdResultado generado.
        /// </summary>
        public int Insertar(entResultadoDiagnostico entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_InsertarResultadoDiagnostico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAyuda", entidad.IdAyuda);

                if (entidad.FechaResultado == default(DateTime))
                    cmd.Parameters.AddWithValue("@FechaResultado", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@FechaResultado", entidad.FechaResultado);

                cmd.Parameters.AddWithValue("@Resumen",
                    (object?)entidad.Resumen ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Critico", entidad.Critico);

                if (string.IsNullOrWhiteSpace(entidad.Estado))
                    cmd.Parameters.AddWithValue("@Estado", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                // El SP devuelve SCOPE_IDENTITY()
                object? escalar = cmd.ExecuteScalar();
                return escalar != null && escalar != DBNull.Value ? Convert.ToInt32(escalar) : 0;
            }
        }

        /// <summary>
        /// Actualiza un resultado diagnóstico usando SP.
        /// </summary>
        public bool Actualizar(entResultadoDiagnostico entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ActualizarResultadoDiagnostico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdResultado", entidad.IdResultado);
                cmd.Parameters.AddWithValue("@IdAyuda", entidad.IdAyuda);

                var fecha = entidad.FechaResultado == default(DateTime)
                    ? DateTime.UtcNow
                    : entidad.FechaResultado;
                cmd.Parameters.AddWithValue("@FechaResultado", fecha);

                cmd.Parameters.AddWithValue("@Resumen",
                    (object?)entidad.Resumen ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Critico", entidad.Critico);

                if (string.IsNullOrWhiteSpace(entidad.Estado))
                    cmd.Parameters.AddWithValue("@Estado", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Busca un resultado por Id usando el SP detallado (JOINs en el SP).
        /// </summary>
        public entResultadoDiagnostico? BuscarPorId(int idResultado)
        {
            entResultadoDiagnostico? entidad = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarResultadoDiagnosticoDetallado", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@IdResultado", SqlDbType.Int).Value = idResultado;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        entidad = MapDetallado(dr);
                    }
                }
            }

            return entidad;
        }

        /// <summary>
        /// Soft delete: marca Estado = 'INACTIVO' usando SP.
        /// </summary>
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

        /// <summary>
        /// Eliminación física (si la sigues usando).
        /// </summary>
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

        // ---------- Helper para mapear resultado detallado ----------
        private static entResultadoDiagnostico MapDetallado(SqlDataReader dr)
        {
            entResultadoDiagnostico e = new entResultadoDiagnostico
            {
                IdResultado = Convert.ToInt32(dr["IdResultado"]),
                IdAyuda = Convert.ToInt32(dr["IdAyuda"]),
                FechaResultado = Convert.ToDateTime(dr["FechaResultado"]),
                Resumen = dr["Resumen"] != DBNull.Value ? dr["Resumen"].ToString() : null,
                Critico = Convert.ToBoolean(dr["Critico"]),
                Estado = dr["Estado"].ToString()
            };

            if (dr["IdControlPrenatal"] != DBNull.Value)
                e.IdControlPrenatal = Convert.ToInt32(dr["IdControlPrenatal"]);

            if (dr["FechaControlPrenatal"] != DBNull.Value)
                e.FechaControlPrenatal = Convert.ToDateTime(dr["FechaControlPrenatal"]);

            if (dr["NombrePaciente"] != DBNull.Value)
                e.NombrePaciente = dr["NombrePaciente"].ToString();

            if (dr["NombreProfesional"] != DBNull.Value)
                e.NombreProfesional = dr["NombreProfesional"].ToString();

            if (dr["DescripcionAyuda"] != DBNull.Value)
                e.DescripcionAyuda = dr["DescripcionAyuda"].ToString();

            if (dr["TipoAyuda"] != DBNull.Value)
                e.TipoAyuda = dr["TipoAyuda"].ToString();

            if (dr["Urgente"] != DBNull.Value)
                e.Urgente = Convert.ToBoolean(dr["Urgente"]);
                e.Urgente = Convert.ToBoolean(dr["Urgente"]);
                e.Urgente = Convert.ToBoolean(dr["Urgente"]);

            return e;
        }

        #endregion
    }
}

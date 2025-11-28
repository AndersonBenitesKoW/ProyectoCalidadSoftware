using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_ResultadoItem
    {
        #region Singleton
        private static readonly DA_ResultadoItem _instancia = new DA_ResultadoItem();
        public static DA_ResultadoItem Instancia
        {
            get { return DA_ResultadoItem._instancia; }
        }

        private DA_ResultadoItem() { }
        #endregion

        #region Métodos

        // LISTAR TODOS LOS ITEMS
        public List<entResultadoItem> Listar()
        {
            var lista = new List<entResultadoItem>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarResultadoItem", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(Map(dr));
                    }
                }
            }

            return lista;
        }

        // LISTAR ITEMS POR RESULTADO
        public List<entResultadoItem> ListarPorResultado(int idResultado)
        {
            var lista = new List<entResultadoItem>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarResultadoItemPorResultado", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultado", idResultado);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(Map(dr));
                    }
                }
            }

            return lista;
        }

        // INSERTAR ITEM
        public bool Insertar(entResultadoItem entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_InsertarResultadoItem", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdResultado", entidad.IdResultado);
                cmd.Parameters.AddWithValue("@Parametro", entidad.Parametro);

                // DECIMAL(12,4)
                var pValorNum = cmd.Parameters.Add("@ValorNumerico", SqlDbType.Decimal);
                pValorNum.Precision = 12;
                pValorNum.Scale = 4;
                pValorNum.Value = (object?)entidad.ValorNumerico ?? DBNull.Value;

                cmd.Parameters.AddWithValue("@ValorTexto",
                    (object?)entidad.ValorTexto ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Unidad",
                    (object?)entidad.Unidad ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@RangoRef",
                    (object?)entidad.RangoRef ?? DBNull.Value);

                cn.Open();
                object? escalar = cmd.ExecuteScalar(); // devuelve IdResultadoItem
                return escalar != null && escalar != DBNull.Value;
            }
        }

        // ACTUALIZAR ITEM
        public bool Actualizar(entResultadoItem entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ActualizarResultadoItem", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdResultadoItem", entidad.IdResultadoItem);
                cmd.Parameters.AddWithValue("@IdResultado", entidad.IdResultado);
                cmd.Parameters.AddWithValue("@Parametro", entidad.Parametro);

                var pValorNum = cmd.Parameters.Add("@ValorNumerico", SqlDbType.Decimal);
                pValorNum.Precision = 12;
                pValorNum.Scale = 4;
                pValorNum.Value = (object?)entidad.ValorNumerico ?? DBNull.Value;

                cmd.Parameters.AddWithValue("@ValorTexto",
                    (object?)entidad.ValorTexto ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Unidad",
                    (object?)entidad.Unidad ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@RangoRef",
                    (object?)entidad.RangoRef ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // BUSCAR UN ITEM POR ID
        public entResultadoItem? BuscarPorId(int idResultadoItem)
        {
            entResultadoItem? entidad = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarResultadoItem", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultadoItem", idResultadoItem);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        entidad = Map(dr);
                    }
                }
            }

            return entidad;
        }

        // ELIMINAR UN ITEM
        public bool Eliminar(int idResultadoItem)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EliminarResultadoItem", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultadoItem", idResultadoItem);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ELIMINAR TODOS LOS ITEMS DE UN RESULTADO (ÚTIL PARA MODIFICAR DETALLE)
        public bool EliminarPorResultado(int idResultado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EliminarResultadoItemPorResultado", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultado", idResultado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ---------- Helper de mapeo ----------
        private static entResultadoItem Map(SqlDataReader dr)
        {
            var item = new entResultadoItem
            {
                IdResultadoItem = Convert.ToInt32(dr["IdResultadoItem"]),
                IdResultado = Convert.ToInt32(dr["IdResultado"]),
                Parametro = dr["Parametro"].ToString() ?? string.Empty,
                ValorNumerico = dr["ValorNumerico"] != DBNull.Value
                    ? Convert.ToDecimal(dr["ValorNumerico"])
                    : (decimal?)null,
                ValorTexto = dr["ValorTexto"] != DBNull.Value
                    ? dr["ValorTexto"].ToString()
                    : null,
                Unidad = dr["Unidad"] != DBNull.Value
                    ? dr["Unidad"].ToString()
                    : null,
                RangoRef = dr["RangoRef"] != DBNull.Value
                    ? dr["RangoRef"].ToString()
                    : null
            };

            return item;
        }

        #endregion
    }
}

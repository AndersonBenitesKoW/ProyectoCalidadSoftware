using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

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

                cmd.Parameters.AddWithValue("@IdEncuentro", entidad.IdAyuda);
                cmd.Parameters.AddWithValue("@Diagnostico", (object)entidad.Resumen ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", entidad.Estado);
                cmd.Parameters.AddWithValue("@Estado", entidad.Critico);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idResultado, int idEncuentro, string diagnostico, string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarResultadoDiagnostico", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdResultado", idResultado);
                cmd.Parameters.AddWithValue("@IdEncuentro", idEncuentro);
                cmd.Parameters.AddWithValue("@Diagnostico", (object)diagnostico ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idResultado)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarResultadoDiagnostico", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultado", idResultado);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idResultado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarResultadoDiagnostico", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResultado", idResultado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
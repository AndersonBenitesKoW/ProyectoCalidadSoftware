using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_TipoAyudaDiagnostica
    {
        #region Singleton
        private static readonly DA_TipoAyudaDiagnostica _instancia = new DA_TipoAyudaDiagnostica();
        public static DA_TipoAyudaDiagnostica Instancia
        {
            get { return DA_TipoAyudaDiagnostica._instancia; }
        }
        #endregion

        #region Métodos

        public List<entTipoAyudaDiagnostica> Listar()
        {
            List<entTipoAyudaDiagnostica> lista = new List<entTipoAyudaDiagnostica>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarTipoAyudaDiagnostica", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var tipoAyuda = new entTipoAyudaDiagnostica
                        {
                            IdTipoAyuda = Convert.ToInt16(dr["IdTipoAyuda"]),
                            Nombre = dr["Nombre"].ToString()
                        };

                        lista.Add(tipoAyuda);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entTipoAyudaDiagnostica entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarTipoAyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", "");
                cmd.Parameters.AddWithValue("@Estado", true);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idTipoAyuda, string nombre, string descripcion, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarTipoAyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdTipoAyuda", idTipoAyuda);
                cmd.Parameters.AddWithValue("@Nombre", (object)nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idTipoAyuda)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarTipoAyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoAyuda", idTipoAyuda);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idTipoAyuda)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarTipoAyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoAyuda", idTipoAyuda);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
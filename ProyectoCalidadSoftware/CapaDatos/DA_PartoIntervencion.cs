using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_PartoIntervencion
    {
        #region Singleton
        private static readonly DA_PartoIntervencion _instancia = new DA_PartoIntervencion();
        public static DA_PartoIntervencion Instancia
        {
            get { return DA_PartoIntervencion._instancia; }
        }
        #endregion

        #region Métodos

        public List<entPartoIntervencion> Listar()
        {
            List<entPartoIntervencion> lista = new List<entPartoIntervencion>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarPartoIntervencion", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var intervencion = new entPartoIntervencion
                        {
                            IdPartoIntervencion = Convert.ToInt32(dr["IdPartoIntervencion"]),
                            IdParto = Convert.ToInt32(dr["IdParto"]),
                            Intervencion = dr["Intervencion"].ToString()
                        };

                        lista.Add(intervencion);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entPartoIntervencion entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarPartoIntervencion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdParto", entidad.IdParto);
                cmd.Parameters.AddWithValue("@Intervencion", entidad.Intervencion);
                cmd.Parameters.AddWithValue("@Descripcion", "");
                cmd.Parameters.AddWithValue("@Estado", true);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idPartoIntervencion, int idParto, string intervencion, string descripcion, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarPartoIntervencion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPartoIntervencion", idPartoIntervencion);
                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cmd.Parameters.AddWithValue("@Intervencion", (object)intervencion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idPartoIntervencion)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarPartoIntervencion", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPartoIntervencion", idPartoIntervencion);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idPartoIntervencion)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarPartoIntervencion", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPartoIntervencion", idPartoIntervencion);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }
}
using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_ProfesionalEmail
    {
        #region Singleton
        private static readonly DA_ProfesionalEmail _instancia = new DA_ProfesionalEmail();
        public static DA_ProfesionalEmail Instancia
        {
            get { return DA_ProfesionalEmail._instancia; }
        }
        #endregion

        #region Métodos

        public List<entProfesionalEmail> Listar()
        {
            List<entProfesionalEmail> lista = new List<entProfesionalEmail>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarProfesionalEmail", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var email = new entProfesionalEmail
                        {
                            IdProfesionalEmail = Convert.ToInt32(dr["IdProfesionalEmail"]),
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            Email = dr["Email"].ToString(),
                            EsPrincipal = Convert.ToBoolean(dr["EsPrincipal"])
                        };

                        lista.Add(email);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entProfesionalEmail entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarProfesionalEmail", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfesional", entidad.IdProfesional);
                cmd.Parameters.AddWithValue("@Email", entidad.Email);
                cmd.Parameters.AddWithValue("@Principal", entidad.EsPrincipal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idProfesionalEmail, int idProfesional, string email, bool principal)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarProfesionalEmail", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfesionalEmail", idProfesionalEmail);
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);
                cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Principal", principal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idProfesionalEmail)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalEmail", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesionalEmail", idProfesionalEmail);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idProfesionalEmail)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarProfesionalEmail", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesionalEmail", idProfesionalEmail);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}

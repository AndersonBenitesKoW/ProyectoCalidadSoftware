using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_ProfesionalTelefono
    {
        #region Singleton
        private static readonly DA_ProfesionalTelefono _instancia = new DA_ProfesionalTelefono();
        public static DA_ProfesionalTelefono Instancia
        {
            get { return DA_ProfesionalTelefono._instancia; }
        }
        #endregion

        #region Métodos

        public List<entProfesionalTelefono> Listar()
        {
            List<entProfesionalTelefono> lista = new List<entProfesionalTelefono>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarProfesionalTelefono", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var telefono = new entProfesionalTelefono
                        {
                            IdProfesionalTelefono = Convert.ToInt32(dr["IdProfesionalTelefono"]),
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            Telefono = dr["Telefono"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            EsPrincipal = Convert.ToBoolean(dr["EsPrincipal"])
                        };

                        lista.Add(telefono);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entProfesionalTelefono entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarProfesionalTelefono", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfesional", entidad.IdProfesional);
                cmd.Parameters.AddWithValue("@Telefono", entidad.Telefono);
                cmd.Parameters.AddWithValue("@Principal", entidad.EsPrincipal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idProfesionalTelefono, int idProfesional, string telefono, bool principal)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarProfesionalTelefono", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfesionalTelefono", idProfesionalTelefono);
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);
                cmd.Parameters.AddWithValue("@Telefono", (object)telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Principal", principal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idProfesionalTelefono)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalTelefono", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesionalTelefono", idProfesionalTelefono);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idProfesionalTelefono)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarProfesionalTelefono", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesionalTelefono", idProfesionalTelefono);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}

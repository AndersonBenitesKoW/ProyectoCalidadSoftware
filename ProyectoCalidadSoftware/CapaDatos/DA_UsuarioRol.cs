using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_UsuarioRol
    {
        #region Singleton
        private static readonly DA_UsuarioRol _instancia = new DA_UsuarioRol();
        public static DA_UsuarioRol Instancia
        {
            get { return DA_UsuarioRol._instancia; }
        }
        #endregion

        #region Métodos

        public List<entUsuarioRol> Listar()
        {
            List<entUsuarioRol> lista = new List<entUsuarioRol>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarUsuarioRol", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var usuarioRol = new entUsuarioRol
                        {
                            IdUsuarioRol = Convert.ToInt32(dr["IdUsuarioRol"]),
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            IdRol = Convert.ToInt32(dr["IdRol"])
                        };

                        lista.Add(usuarioRol);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entUsuarioRol entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarUsuarioRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdUsuario", entidad.IdUsuario);
                cmd.Parameters.AddWithValue("@IdRol", entidad.IdRol);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idUsuarioRol, int idUsuario, int idRol)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarUsuarioRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdUsuarioRol", idUsuarioRol);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@IdRol", idRol);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idUsuarioRol)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarUsuarioRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuarioRol", idUsuarioRol);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idUsuarioRol)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarUsuarioRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuarioRol", idUsuarioRol);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
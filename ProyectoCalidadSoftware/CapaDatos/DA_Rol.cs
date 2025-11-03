using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Rol
    {
        #region Singleton
        private static readonly DA_Rol _instancia = new DA_Rol();
        public static DA_Rol Instancia
        {
            get { return DA_Rol._instancia; }
        }
        #endregion

        #region Métodos

        public List<entRol> Listar()
        {
            List<entRol> lista = new List<entRol>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarRol", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var rol = new entRol
                        {
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            NombreRol = dr["NombreRol"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(rol);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entRol entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", entidad.NombreRol);
                cmd.Parameters.AddWithValue("@Descripcion", (object)entidad.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idRol, string nombre, string descripcion, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdRol", idRol);
                cmd.Parameters.AddWithValue("@Nombre", (object)nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public entRol? BuscarPorId(int idRol)
        {
            entRol? rol = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarRol", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRol", idRol);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        rol = new entRol
                        {
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            NombreRol = dr["NombreRol"].ToString()!,
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString()! : "",
                            Estado = dr["Estado"] != DBNull.Value && Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }
            return rol;
        }

        public entRol? ObtenerPorNombre(string nombreRol)
        {
            entRol? rol = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_Rol_ObtenerPorNombre", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NombreRol", nombreRol);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        rol = new entRol
                        {
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            NombreRol = dr["NombreRol"].ToString()!,
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString()! : "",
                            Estado = dr["Estado"] != DBNull.Value && Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }

            return rol;
        }

        public bool Eliminar(int idRol)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarRol", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRol", idRol);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
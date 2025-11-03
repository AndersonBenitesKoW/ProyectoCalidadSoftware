using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class 
        DA_Usuario
    {
        #region Singleton
        private static readonly DA_Usuario _instancia = new DA_Usuario();
        public static DA_Usuario Instancia
        {
            get { return DA_Usuario._instancia; }
        }
        #endregion

        #region Métodos

        public List<entUsuario> Listar()
        {
            System.Diagnostics.Debug.WriteLine("DA_Usuario.Listar() llamado");

            // usamos diccionario para evitar duplicados
            var dic = new Dictionary<int, entUsuario>();

            try
            {
                using (SqlConnection cn = Conexion.Instancia.Conectar())
                using (SqlCommand cmd = new SqlCommand("sp_ListarUsuario", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int idUsuario = (int)dr["IdUsuario"];

                            // si todavía no lo agregamos, lo creamos
                            if (!dic.TryGetValue(idUsuario, out var usuario))
                            {
                                usuario = new entUsuario
                                {
                                    IdUsuario = idUsuario,
                                    NombreUsuario = dr["NombreUsuario"].ToString(),
                                    ClaveHash = dr["ClaveHash"].ToString(),
                                    Email = dr["Email"].ToString(),
                                    Estado = (bool)dr["Estado"],

                                    // inicializamos en 0 por si esta fila no trae rol
                                    IdRol = 0,
                                    NombreRol = string.Empty
                                };

                                dic.Add(idUsuario, usuario);
                            }

                            // si esta fila sí trae rol y aún no lo hemos puesto, lo ponemos
                            if (dr["IdRol"] != DBNull.Value && usuario.IdRol == 0)
                            {
                                usuario.IdRol = (int)dr["IdRol"];
                                usuario.NombreRol = dr["NombreRol"].ToString();
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Usuarios listados: {dic.Count}");
                return dic.Values.ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en DA_Usuario.Listar: {ex.Message}");
                return new List<entUsuario>();
            }
        }


        public bool Insertar(entUsuario entidad, int idRol)
        {
            System.Diagnostics.Debug.WriteLine($"DA_Usuario.Insertar llamado con usuario: {entidad.NombreUsuario}, rol: {idRol}");

            try
            {
                using var cn = Conexion.Instancia.Conectar();
                using var cmd = new SqlCommand("sp_InsertarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", entidad.NombreUsuario);
                cmd.Parameters.AddWithValue("@PasswordHash", entidad.ClaveHash);
                cmd.Parameters.AddWithValue("@Correo", (object)entidad.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdRol", idRol);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                // OUTPUT
                var pOut = new SqlParameter("@NewIdUsuario", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pOut);

                System.Diagnostics.Debug.WriteLine("Parámetros preparados, abriendo conexión");

                cn.Open();
                cmd.ExecuteNonQuery();

                var newId = (pOut.Value == DBNull.Value) ? 0 : Convert.ToInt32(pOut.Value);
                System.Diagnostics.Debug.WriteLine($"Nuevo IdUsuario devuelto por SP: {newId}");

                return newId > 0;
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error SQL en DA_Usuario.Insertar: {sqlEx.Message} (#{sqlEx.Number})");
                throw new Exception($"Error de base de datos al insertar usuario: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general en DA_Usuario.Insertar: {ex.Message}");
                throw new Exception($"Error al insertar usuario: {ex.Message}", ex);
            }
        }


        public bool Editar(entUsuario entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EditarUsuario", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdUsuario", entidad.IdUsuario);

                // si quieres permitir nulos en nombre/clave/email:
                cmd.Parameters.AddWithValue("@Username",
                    string.IsNullOrWhiteSpace(entidad.NombreUsuario)
                        ? (object)DBNull.Value
                        : entidad.NombreUsuario);

                cmd.Parameters.AddWithValue("@PasswordHash",
                    string.IsNullOrWhiteSpace(entidad.ClaveHash)
                        ? (object)DBNull.Value
                        : entidad.ClaveHash);

                cmd.Parameters.AddWithValue("@Correo",
                    string.IsNullOrWhiteSpace(entidad.Email)
                        ? (object)DBNull.Value
                        : entidad.Email);

                cmd.Parameters.AddWithValue("@IdRol", entidad.IdRol);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public entUsuario? BuscarPorId(int idUsuario)
        {
            entUsuario? usuario = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarUsuario", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        usuario = new entUsuario
                        {
                            IdUsuario = (int)dr["IdUsuario"],
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            ClaveHash = dr["ClaveHash"].ToString(),
                            Email = dr["Email"].ToString(),
                            Estado = (bool)dr["Estado"],
                            IdRol = dr["IdRol"] != DBNull.Value ? (int)dr["IdRol"] : 0,
                            NombreRol = dr["NombreRol"] != DBNull.Value ? dr["NombreRol"].ToString() : string.Empty
                        };
                    }
                }
            }

            return usuario;
        }



        public bool Eliminar(int idUsuario)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EliminarUsuario", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public entUsuario VerificarLogin(string username, string passwordHash)
        {
            entUsuario usuario = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT IdUsuario, NombreUsuario, ClaveHash, Estado FROM Usuario WHERE NombreUsuario = @Username AND ClaveHash = @PasswordHash AND Estado = 1", cn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        usuario = new entUsuario
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            NombreUsuario = dr["NombreUsuario"].ToString(),
                            ClaveHash = dr["ClaveHash"].ToString(),
                            Email = dr["Email"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }

            return usuario;
        }


        //metodos para la gestion de roles y permisos
        public entUsuario? ObtenerPorNombre(string nombreUsuario)
        {
            entUsuario? usuario = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_Usuario_ObtenerPorNombre", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        usuario = new entUsuario
                        {
                            IdUsuario = (int)dr["IdUsuario"],
                            NombreUsuario = dr["NombreUsuario"].ToString()!,
                            ClaveHash = dr["ClaveHash"].ToString()!,
                            Email = dr["Email"].ToString()!,
                            Estado = (bool)dr["Estado"],
                            IdRol = dr["IdRol"] != DBNull.Value ? (int)dr["IdRol"] : 0,
                            NombreRol = dr["NombreRol"] != DBNull.Value ? dr["NombreRol"].ToString()! : ""
                        };
                    }
                }
            }

            return usuario;
        }

        public List<entUsuarioRol> ObtenerRoles(int idUsuario)
        {
            var lista = new List<entUsuarioRol>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_Usuario_ObtenerRoles", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        // Primero construimos el rol
                        var rol = new entRol
                        {
                            IdRol = (int)dr["IdRol"],
                            NombreRol = dr["NombreRol"].ToString()!,
                            Descripcion = dr["Descripcion"].ToString(),
                            Estado = (bool)dr["Estado"]
                        };

                        // Luego construimos la relación UsuarioRol
                        var usuarioRol = new entUsuarioRol
                        {
                            IdUsuarioRol = (int)dr["IdUsuarioRol"], // este viene desde la tabla intermedia
                            IdUsuario = idUsuario,
                            IdRol = rol.IdRol,
                            Rol = rol
                        };

                        lista.Add(usuarioRol);
                    }
                }
            }

            return lista;
        }







        #endregion
    }


}
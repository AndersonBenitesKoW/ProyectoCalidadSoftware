using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_ProfesionalSalud
    {
        #region Singleton
        private static readonly DA_ProfesionalSalud _instancia = new DA_ProfesionalSalud();
        public static DA_ProfesionalSalud Instancia
        {
            get { return DA_ProfesionalSalud._instancia; }
        }
        #endregion

        #region Métodos

<<<<<<< HEAD
        public List<entProfesionalSalud> Listar()
=======
        public List<entProfesionalSalud> Listar(bool estado)
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
        {
            List<entProfesionalSalud> lista = new List<entProfesionalSalud>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
<<<<<<< HEAD
            using (SqlCommand cmd = new SqlCommand("sp_ListarProfesionalSalud", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
=======
            using (SqlCommand cmd = new SqlCommand("sp_ListarProfesionales", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Estado", estado);

>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var profesional = new entProfesionalSalud
                        {
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            IdUsuario = dr["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdUsuario"]) : null,
<<<<<<< HEAD
                            CMP = dr["CMP"].ToString(),
                            Especialidad = dr["Especialidad"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

=======
                            CMP = !Convert.IsDBNull(dr["CMP"]) ? dr["CMP"].ToString() : string.Empty,
                            Especialidad = !Convert.IsDBNull(dr["Especialidad"]) ? dr["Especialidad"].ToString() : string.Empty,
                            Nombres = !Convert.IsDBNull(dr["Nombres"]) ? dr["Nombres"].ToString() : string.Empty,
                            Apellidos = !Convert.IsDBNull(dr["Apellidos"]) ? dr["Apellidos"].ToString() : string.Empty,
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
                        lista.Add(profesional);
                    }
                }
            }
<<<<<<< HEAD

            return lista;
        }

        public bool Insertar(entProfesionalSalud entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);
                cmd.Parameters.AddWithValue("@Especialidad", (object)entidad.Especialidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Colegiatura", entidad.CMP);
=======
            return lista;
        }

        public int Insertar(entProfesionalSalud entidad) 
        {
            int idGenerado = -1; 
            try
            {
                using (SqlConnection cn = Conexion.Instancia.Conectar())
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertarProfesional", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", (object)entidad.IdUsuario ?? DBNull.Value); 
                    cmd.Parameters.AddWithValue("@CMP", entidad.CMP);
                    cmd.Parameters.AddWithValue("@Especialidad", (object)entidad.Especialidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombres", (object)entidad.Nombres ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellidos", (object)entidad.Apellidos ?? DBNull.Value);

                    cn.Open();

                    object objId = cmd.ExecuteScalar();

                    if (objId != null && objId != DBNull.Value)
                    {
                        idGenerado = Convert.ToInt32(objId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; 
            }
            return idGenerado;
        }

        public bool Editar(entProfesionalSalud entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfesional", entidad.IdProfesional);
                cmd.Parameters.AddWithValue("@Nombres", (object)entidad.Nombres ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellidos", (object)entidad.Apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Especialidad", (object)entidad.Especialidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Colegiatura", (object)entidad.CMP ?? DBNull.Value);
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

<<<<<<< HEAD
        public bool Editar(int idProfesional, string nombres, string apellidos, string especialidad, string colegiatura, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);
                cmd.Parameters.AddWithValue("@Nombres", (object)nombres ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellidos", (object)apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Especialidad", (object)especialidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Colegiatura", (object)colegiatura ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idProfesional)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalSalud", cn);
=======
        public entProfesionalSalud BuscarPorId(int idProfesional)
        {
            entProfesionalSalud profesional = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalSalud", cn))
            {
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);

                cn.Open();
<<<<<<< HEAD
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
=======
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        profesional = new entProfesionalSalud
                        {
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            IdUsuario = dr["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdUsuario"]) : null,
                            CMP = !Convert.IsDBNull(dr["CMP"]) ? dr["CMP"].ToString() : string.Empty,
                            Especialidad = !Convert.IsDBNull(dr["Especialidad"]) ? dr["Especialidad"].ToString() : string.Empty,
                            Nombres = !Convert.IsDBNull(dr["Nombres"]) ? dr["Nombres"].ToString() : string.Empty,
                            Apellidos = !Convert.IsDBNull(dr["Apellidos"]) ? dr["Apellidos"].ToString() : string.Empty,
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }
            return profesional;
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
        }

        public bool Eliminar(int idProfesional)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public entProfesionalSalud VerificarProfesionalPorUsuario(int idUsuario)
        {
            entProfesionalSalud profesional = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT IdProfesional, IdUsuario, CMP, Especialidad, Nombres, Apellidos, Estado FROM ProfesionalSalud WHERE IdUsuario = @IdUsuario AND Estado = 1", cn))
            {
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        profesional = new entProfesionalSalud
                        {
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            IdUsuario = dr["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdUsuario"]) : null,
                            CMP = dr["CMP"].ToString(),
                            Especialidad = dr["Especialidad"].ToString(),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }

            return profesional;
        }

        #endregion
    }


}
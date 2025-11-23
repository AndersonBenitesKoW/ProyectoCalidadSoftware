using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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

        public List<entProfesionalSalud> Listar(bool estado)
        {
            List<entProfesionalSalud> lista = new List<entProfesionalSalud>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarProfesionalSalud", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Estado", estado);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entProfesionalSalud
                        {
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            CMP = dr["CMP"].ToString(), // CMP nunca es nulo
                            IdUsuario = dr["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdUsuario"]) : null,
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Especialidad = dr["Especialidad"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            // --- CAMPOS AÑADIDOS ---
                            EmailPrincipal = dr["EmailPrincipal"].ToString(),
                            TelefonoPrincipal = dr["TelefonoPrincipal"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public int Insertar(entProfesionalSalud entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombres", (object)entidad.Nombres ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellidos", (object)entidad.Apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Especialidad", (object)entidad.Especialidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CMP", entidad.CMP);

                // --- PARÁMETROS AÑADIDOS ---
                cmd.Parameters.AddWithValue("@EmailPrincipal", entidad.EmailPrincipal);
                cmd.Parameters.AddWithValue("@TelefonoPrincipal", entidad.TelefonoPrincipal);

                cn.Open();
                object idGenerado = cmd.ExecuteScalar(); // SP devuelve el nuevo ID
                return (idGenerado != null && idGenerado != DBNull.Value) ? Convert.ToInt32(idGenerado) : 0;
            }
        }

        public entProfesionalSalud BuscarPorId(int idProfesional)
        {
            entProfesionalSalud profesional = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    profesional = new entProfesionalSalud
                    {
                        IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                        IdUsuario = dr["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdUsuario"]) : null,
                        Nombres = dr["Nombres"].ToString(),
                        Apellidos = dr["Apellidos"].ToString(),
                        Especialidad = dr["Especialidad"].ToString(),
                        Estado = Convert.ToBoolean(dr["Estado"]),
                        CMP = dr["CMP"].ToString(),
                        // --- CAMPOS AÑADIDOS ---
                        EmailPrincipal = dr["EmailPrincipal"].ToString(),
                        TelefonoPrincipal = dr["TelefonoPrincipal"].ToString()
                    };
                }
            }
            return profesional;
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

                // --- CORRECCIÓN DE PARÁMETRO ---
                cmd.Parameters.AddWithValue("@CMP", (object)entidad.CMP ?? DBNull.Value); // Era @Colegiatura

                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                // --- PARÁMETROS AÑADIDOS ---
                cmd.Parameters.AddWithValue("@EmailPrincipal", (object)entidad.EmailPrincipal ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TelefonoPrincipal", (object)entidad.TelefonoPrincipal ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // --- MÉTODO NUEVO ---
        public bool Eliminar(int idProfesional)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InhabilitarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public entProfesionalSalud BuscarPorCMP(string cmp)
        {
            entProfesionalSalud profesional = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalPorCMP", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CMP", cmp);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        profesional = new entProfesionalSalud
                        {
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            IdUsuario = dr["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdUsuario"]) : null,
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Especialidad = dr["Especialidad"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            CMP = dr["CMP"].ToString(),
                            EmailPrincipal = dr["EmailPrincipal"].ToString(),
                            TelefonoPrincipal = dr["TelefonoPrincipal"].ToString()
                        };
                    }
                }
            }
            return profesional;
        }


        #endregion
    }
}
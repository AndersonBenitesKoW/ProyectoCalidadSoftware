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
                        var profesional = new entProfesionalSalud
                        {
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Especialidad = dr["Especialidad"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(profesional);
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

                cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);
                cmd.Parameters.AddWithValue("@Especialidad", entidad.Especialidad);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public entProfesionalSalud BuscarPorId(int idProfesional)
        {
            entProfesionalSalud? profesional = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        profesional = new entProfesionalSalud
                        {
                            IdProfesional = Convert.ToInt32(dr["IdProfesional"]),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Especialidad = dr["Especialidad"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
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
                cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);
                cmd.Parameters.AddWithValue("@Especialidad", entidad.Especialidad);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }
}

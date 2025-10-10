using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_PacienteEmail
    {
        #region Singleton
        private static readonly DA_PacienteEmail _instancia = new DA_PacienteEmail();
        public static DA_PacienteEmail Instancia
        {
            get { return DA_PacienteEmail._instancia; }
        }
        #endregion

        #region Métodos

        public List<entPacienteEmail> Listar()
        {
            List<entPacienteEmail> lista = new List<entPacienteEmail>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarPacienteEmail", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var email = new entPacienteEmail
                        {
                            IdPacienteEmail = Convert.ToInt32(dr["IdPacienteEmail"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            Email = dr["Email"].ToString(),
                            EsPrincipal = Convert.ToBoolean(dr["EsPrincipal"])
                        };

                        lista.Add(email);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entPacienteEmail entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarPacienteEmail", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@Email", entidad.Email);
                cmd.Parameters.AddWithValue("@Principal", entidad.EsPrincipal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idPacienteEmail, int idPaciente, string email, bool principal)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarPacienteEmail", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPacienteEmail", idPacienteEmail);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Principal", principal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idPacienteEmail)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarPacienteEmail", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteEmail", idPacienteEmail);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idPacienteEmail)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarPacienteEmail", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteEmail", idPacienteEmail);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
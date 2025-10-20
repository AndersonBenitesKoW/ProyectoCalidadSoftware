using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_PacienteTelefono
    {
        #region Singleton
        private static readonly DA_PacienteTelefono _instancia = new DA_PacienteTelefono();
        public static DA_PacienteTelefono Instancia
        {
            get { return DA_PacienteTelefono._instancia; }
        }
        #endregion

        #region Métodos

        public List<entPacienteTelefono> Listar()
        {
            List<entPacienteTelefono> lista = new List<entPacienteTelefono>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarPacienteTelefono", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var telefono = new entPacienteTelefono
                        {
                            IdPacienteTelefono = Convert.ToInt32(dr["IdPacienteTelefono"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
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

        public bool Insertar(entPacienteTelefono entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarPacienteTelefono", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@Telefono", entidad.Telefono);
                cmd.Parameters.AddWithValue("@Principal", entidad.EsPrincipal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idPacienteTelefono, int idPaciente, string telefono, bool principal)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarPacienteTelefono", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPacienteTelefono", idPacienteTelefono);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@Telefono", (object)telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Principal", principal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idPacienteTelefono)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarPacienteTelefono", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteTelefono", idPacienteTelefono);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idPacienteTelefono)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarPacienteTelefono", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteTelefono", idPacienteTelefono);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_PacienteFactorRiesgo
    {
        #region Singleton
        private static readonly DA_PacienteFactorRiesgo _instancia = new DA_PacienteFactorRiesgo();
        public static DA_PacienteFactorRiesgo Instancia
        {
            get { return DA_PacienteFactorRiesgo._instancia; }
        }
        #endregion

        #region Métodos

        public List<entPacienteFactorRiesgo> Listar()
        {
            List<entPacienteFactorRiesgo> lista = new List<entPacienteFactorRiesgo>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarPacienteFactorRiesgo", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var factor = new entPacienteFactorRiesgo
                        {
                            IdPacienteFactor = Convert.ToInt32(dr["IdPacienteFactor"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            IdFactorCat = Convert.ToInt32(dr["IdFactorCat"]),
                            Detalle = dr["Detalle"].ToString(),
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(factor);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entPacienteFactorRiesgo entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@IdFactorCat", entidad.IdFactorCat);
                cmd.Parameters.AddWithValue("@Detalle", (object)entidad.Detalle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaRegistro", entidad.FechaRegistro);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idPacienteFactor, int idPaciente, int idFactorCat, string detalle, DateTime? fechaRegistro, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPacienteFactor", idPacienteFactor);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@IdFactorCat", idFactorCat);
                cmd.Parameters.AddWithValue("@Detalle", (object)detalle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaRegistro", (object)fechaRegistro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idPacienteFactor)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteFactor", idPacienteFactor);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idPacienteFactor)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarPacienteFactorRiesgo", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteFactor", idPacienteFactor);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
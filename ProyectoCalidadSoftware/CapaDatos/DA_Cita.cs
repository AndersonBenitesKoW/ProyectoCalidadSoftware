using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_Cita
    {
        
        
        
        #region Singleton
        private static readonly DA_Cita _instancia = new DA_Cita();
        public static DA_Cita Instancia
        {
            get { return DA_Cita._instancia; }
        }
        #endregion


        public List<entCita> Listar()
        {
            List<entCita> lista = new List<entCita>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarCita", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var cita = new entCita
                        {
                            IdCita = Convert.ToInt32(dr["IdCita"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            IdRecepcionista = dr["IdRecepcionista"] != DBNull.Value ? Convert.ToInt32(dr["IdRecepcionista"]) : (int?)null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? Convert.ToInt32(dr["IdProfesional"]) : (int?)null,
                            IdEmbarazo = dr["IdEmbarazo"] != DBNull.Value ? Convert.ToInt32(dr["IdEmbarazo"]) : (int?)null,
                            FechaCita = Convert.ToDateTime(dr["FechaCita"]),
                            Motivo = dr["Motivo"].ToString(),
                            IdEstadoCita = Convert.ToInt16(dr["IdEstadoCita"]),
                            Observacion = dr["Observacion"].ToString(),
                            FechaAnulacion = dr["FechaAnulacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaAnulacion"]) : (DateTime?)null,
                            MotivoAnulacion = dr["MotivoAnulacion"].ToString()
                        };

                        lista.Add(cita);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entCita entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@IdRecepcionista", (object)entidad.IdRecepcionista ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)entidad.IdEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCita", entidad.FechaCita);
                cmd.Parameters.AddWithValue("@Motivo", (object)entidad.Motivo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEstadoCita", entidad.IdEstadoCita);
                cmd.Parameters.AddWithValue("@Observacion", (object)entidad.Observacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaAnulacion", (object)entidad.FechaAnulacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MotivoAnulacion", (object)entidad.MotivoAnulacion ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idCita, int idPaciente, int? idRecepcionista, int? idProfesional,
                           int? idEmbarazo, DateTime fechaCita, string motivo,
                           short idEstadoCita, string observacion,
                           DateTime? fechaAnulacion, string motivoAnulacion)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCita", idCita);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@IdRecepcionista", (object)idRecepcionista ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)idProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)idEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCita", fechaCita);
                cmd.Parameters.AddWithValue("@Motivo", (object)motivo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEstadoCita", idEstadoCita);
                cmd.Parameters.AddWithValue("@Observacion", (object)observacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaAnulacion", (object)fechaAnulacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MotivoAnulacion", (object)motivoAnulacion ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idCita)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idCita)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

      
    }


}
using CapaEntidad;
using System; // Para DateTime y DBNull
using System.Collections.Generic; // Para List<>
using System.Data;
using System.Data.SqlClient;

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
            using (SqlCommand cmd = new SqlCommand("sp_ListarCitas", cn)) // SP Corregido
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entCita
                        {
                            IdCita = Convert.ToInt32(dr["IdCita"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? Convert.ToInt32(dr["IdProfesional"]) : (int?)null,
                            IdEmbarazo = dr["IdEmbarazo"] != DBNull.Value ? Convert.ToInt32(dr["IdEmbarazo"]) : (int?)null,
                            FechaCita = Convert.ToDateTime(dr["FechaCita"]),
                            Motivo = dr["Motivo"].ToString(),
                            IdEstadoCita = Convert.ToInt16(dr["IdEstadoCita"]),
                            Observacion = dr["Observacion"].ToString(),

                            // Campos de los JOINS
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString(),
                            NombreEstado = dr["NombreEstado"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public bool Insertar(entCita entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarCita", cn); // SP Corregido
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@IdRecepcionista", (object)entidad.IdRecepcionista ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)entidad.IdEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCita", entidad.FechaCita);
                cmd.Parameters.AddWithValue("@Motivo", (object)entidad.Motivo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEstadoCita", entidad.IdEstadoCita);
                cmd.Parameters.AddWithValue("@Observacion", (object)entidad.Observacion ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery(); // Ejecutamos
                return true; // Asumimos éxito
            }
        }

        public bool Editar(entCita entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarCita", cn); // SP Corregido
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCita", entidad.IdCita);
                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@IdRecepcionista", (object)entidad.IdRecepcionista ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)entidad.IdEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCita", entidad.FechaCita);
                cmd.Parameters.AddWithValue("@Motivo", (object)entidad.Motivo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdEstadoCita", entidad.IdEstadoCita);
                cmd.Parameters.AddWithValue("@Observacion", (object)entidad.Observacion ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
                return true; // Asumimos éxito
            }
        }

        public entCita? BuscarPorId(int idCita)
        {
            entCita? cita = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarCita", cn)) // SP Corregido
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCita", idCita);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cita = new entCita
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
                                MotivoAnulacion = dr["MotivoAnulacion"].ToString(),

                                NombrePaciente = dr["NombrePaciente"].ToString(),
                                NombreProfesional = dr["NombreProfesional"].ToString(),
                                NombreEstado = dr["NombreEstado"].ToString()
                            };
                        }
                    }
                }
            }
            return cita;
        }

        // Este es el método para "Anular"
        // (Tu logCita lo llama "Eliminar")
        public bool Eliminar(int idCita, string motivoAnulacion) // 1. Añadido parámetro
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarCita", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCita", idCita);
                cmd.Parameters.AddWithValue("@MotivoAnulacion", motivoAnulacion); // 2. Parámetro nuevo

                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
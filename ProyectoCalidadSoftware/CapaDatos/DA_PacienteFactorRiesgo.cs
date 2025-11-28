using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

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
        /// Actualiza usando un OBJETO (sin DataTable).
        /// </summary>
        public bool Editar(entPacienteFactorRiesgo entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EditarPacienteFactorRiesgo", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPacienteFactor", entidad.IdPacienteFactor);
                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@IdFactorCat", entidad.IdFactorCat);
                cmd.Parameters.AddWithValue("@Detalle", (object)entidad.Detalle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaRegistro", entidad.FechaRegistro == default ? (object)DBNull.Value : entidad.FechaRegistro);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Buscar por Id: devuelve la ENTIDAD (no DataTable).
        /// </summary>
        public entPacienteFactorRiesgo BuscarPorId(int idPacienteFactor)
        {
            entPacienteFactorRiesgo entidad = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarPacienteFactorRiesgo", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteFactor", idPacienteFactor);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        entidad = new entPacienteFactorRiesgo
                        {
                            IdPacienteFactor = Convert.ToInt32(dr["IdPacienteFactor"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            IdFactorCat = Convert.ToInt32(dr["IdFactorCat"]),
                            Detalle = dr["Detalle"] == DBNull.Value ? null : dr["Detalle"].ToString(),
                            FechaRegistro = dr["FechaRegistro"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["FechaRegistro"]),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }

            return entidad;
        }

        /// Eliminar lógico o físico según tu SP. Mantiene parámetro simple (int).
        /// </summary>
        public bool Eliminar(int idPacienteFactor)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EliminarPacienteFactorRiesgo", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPacienteFactor", idPacienteFactor);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }





        #endregion
    }


}
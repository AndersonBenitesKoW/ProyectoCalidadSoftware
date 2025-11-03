using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Paciente
    {
        #region Singleton
        private static readonly DA_Paciente _instancia = new DA_Paciente();
        public static DA_Paciente Instancia
        {
            get { return DA_Paciente._instancia; }
        }
        #endregion

        #region Métodos

        public List<entPaciente> Listar()
        {
            List<entPaciente> lista = new List<entPaciente>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarPacientesActivos", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var paciente = new entPaciente
                        {
                            // --- Campos que no son nulos ---
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            Estado = Convert.ToBoolean(dr["Estado"]),

                            // --- Campos de Texto (más seguro) ---
                            // Comprobamos si son nulos antes de convertirlos a string
                            Nombres = dr["Nombres"] != DBNull.Value ? dr["Nombres"].ToString() : string.Empty,
                            Apellidos = dr["Apellidos"] != DBNull.Value ? dr["Apellidos"].ToString() : string.Empty,
                            DNI = dr["DNI"] != DBNull.Value ? dr["DNI"].ToString() : string.Empty,


                            // --- CAMPOS CON NULOS (LA CORRECCIÓN) ---

                            // 👇 CORRECCIÓN 1: Manejo de nulo para FechaNacimiento
                            // (Asume que 'FechaNacimiento' en 'entPaciente' es de tipo 'DateTime?')
                            FechaNacimiento = dr["FechaNacimiento"] != DBNull.Value
                                ? Convert.ToDateTime(dr["FechaNacimiento"])
                                : (DateTime?)null, // Asigna null de C# si es DBNull

                            // 👇 CORRECCIÓN 2: Manejo de nulo para IdUsuario
                            // (Asume que 'IdUsuario' en 'entPaciente' es de tipo 'int?')
                            IdUsuario = dr["IdUsuario"] != DBNull.Value
                                ? Convert.ToInt32(dr["IdUsuario"])
                                : (int?)null // Asigna null de C# si es DBNull
                        };

                        lista.Add(paciente);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entPaciente entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);
                cmd.Parameters.AddWithValue("@FechaNacimiento", entidad.FechaNacimiento);
                cmd.Parameters.AddWithValue("@DNI", entidad.DNI);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idPaciente, string nombres, string apellidos,
                           DateTime fechaNacimiento, string dni, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@Nombres", nombres);
                cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
                cmd.Parameters.AddWithValue("@DNI", dni);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idPaciente)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idPaciente)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
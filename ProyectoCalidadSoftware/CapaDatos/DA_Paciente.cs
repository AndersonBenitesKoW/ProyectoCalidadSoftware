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

                // --- Parámetros de Paciente ---
                cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);

                // Manejo de Nulos (DBNull.Value)
                cmd.Parameters.AddWithValue("@DNI",
                    string.IsNullOrWhiteSpace(entidad.DNI) ? (object)DBNull.Value : entidad.DNI);

                cmd.Parameters.AddWithValue("@FechaNacimiento",
                    entidad.FechaNacimiento.HasValue ? (object)entidad.FechaNacimiento.Value : (object)DBNull.Value);

                // --- NUEVOS Parámetros de Contacto ---
                cmd.Parameters.AddWithValue("@EmailPrincipal", entidad.EmailPrincipal);
                cmd.Parameters.AddWithValue("@TelefonoPrincipal", entidad.TelefonoPrincipal);

                // (Opcional: puedes agregar TipoTelefono si lo pides en la vista)
                // cmd.Parameters.AddWithValue("@TipoTelefono", "Celular"); 

                cn.Open();

                // --- CORRECCIÓN DE EJECUCIÓN ---
                // El SP devuelve el nuevo ID (NewIdPaciente) con un SELECT.
                // Usamos ExecuteScalar() para leer ese valor.
                // ExecuteNonQuery() devolvería -1 y fallaría.

                object newId = cmd.ExecuteScalar();

                // Si newId no es nulo y es un número, la inserción fue exitosa.
                if (newId != null && newId != DBNull.Value)
                {
                    // Convertimos a int y verificamos que sea mayor a 0
                    return Convert.ToInt32(newId) > 0;
                }

                return false; // Falló la inserción
            }
        }

        // Reemplaza el método que recibía 6 parámetros
        public bool Editar(entPaciente entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros de Paciente
                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                // Manejo de Nulos
                cmd.Parameters.AddWithValue("@DNI",
                    string.IsNullOrWhiteSpace(entidad.DNI) ? (object)DBNull.Value : entidad.DNI);

                cmd.Parameters.AddWithValue("@FechaNacimiento",
                    entidad.FechaNacimiento.HasValue ? (object)entidad.FechaNacimiento.Value : (object)DBNull.Value);

                // Parámetros de Contacto
                cmd.Parameters.AddWithValue("@EmailPrincipal",
                    string.IsNullOrWhiteSpace(entidad.EmailPrincipal) ? (object)DBNull.Value : entidad.EmailPrincipal);

                cmd.Parameters.AddWithValue("@TelefonoPrincipal",
                    string.IsNullOrWhiteSpace(entidad.TelefonoPrincipal) ? (object)DBNull.Value : entidad.TelefonoPrincipal);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Reemplaza el método que devolvía DataTable
        public entPaciente BuscarPorId(int idPaciente)
        {
            entPaciente paciente = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read()) // Si encontró al paciente
                    {
                        paciente = new entPaciente
                        {
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            IdUsuario = dr["IdUsuario"] != DBNull.Value ? Convert.ToInt32(dr["IdUsuario"]) : (int?)null,
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            DNI = dr["DNI"].ToString(),
                            FechaNacimiento = dr["FechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(dr["FechaNacimiento"]) : (DateTime?)null,
                            Estado = Convert.ToBoolean(dr["Estado"]),

                            // Nuevos campos que trae el SP
                            EmailPrincipal = dr["EmailPrincipal"].ToString(),
                            TelefonoPrincipal = dr["TelefonoPrincipal"].ToString()
                        };
                    }
                }
            }
            return paciente; // Devuelve el objeto (o null si no lo encontró)
        }

        // Este método es llamado por logPaciente.InhabilitarPaciente()
        public bool Eliminar(int idPaciente)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                // Llamamos al nuevo SP de inhabilitar
                SqlCommand cmd = new SqlCommand("sp_InhabilitarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
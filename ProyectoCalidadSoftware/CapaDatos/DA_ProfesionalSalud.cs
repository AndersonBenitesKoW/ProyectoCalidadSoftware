using CapaEntidad;
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
            using (SqlCommand cmd = new SqlCommand("sp_ListarProfesionales", cn)) // Asegúrate que el SP se llame así
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
                            // 🚀 LÍNEAS AÑADIDAS: Leer CMP e IdUsuario
                            CMP = dr["CMP"] != DBNull.Value ? dr["CMP"].ToString() : null, // Lee CMP, maneja nulos
                            IdUsuario = dr["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdUsuario"]) : null, // Lee IdUsuario, maneja nulos
                                                                                                                         // --- Fin de líneas añadidas ---
                            Nombres = dr["Nombres"] != DBNull.Value ? dr["Nombres"].ToString() : null, // Añadido manejo de nulos
                            Apellidos = dr["Apellidos"] != DBNull.Value ? dr["Apellidos"].ToString() : null, // Añadido manejo de nulos
                            Especialidad = dr["Especialidad"] != DBNull.Value ? dr["Especialidad"].ToString() : null, // Añadido manejo de nulos
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

                cmd.Parameters.AddWithValue("@Nombres", (object)entidad.Nombres ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellidos", (object)entidad.Apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Especialidad", (object)entidad.Especialidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CMP", entidad.CMP);


                cn.Open();


                object idGenerado = cmd.ExecuteScalar();
                return (idGenerado != null && idGenerado != DBNull.Value) ? Convert.ToInt32(idGenerado) : 0;
            }
        }

        public entProfesionalSalud BuscarPorId(int idProfesional)
        {
            entProfesionalSalud profesional = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                // 1. Llamamos al SP correcto (el que tú me mostraste)
                SqlCommand cmd = new SqlCommand("sp_BuscarProfesionalSalud", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProfesional", idProfesional);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                // 2. Aquí está el punto clave
                if (dr.Read()) // Usamos 'if' porque solo esperamos un resultado
                {
                    profesional = new entProfesionalSalud();
                    profesional.IdProfesional = Convert.ToInt32(dr["IdProfesional"]);
                    profesional.IdUsuario = dr["IdUsuario"] != DBNull.Value ? Convert.ToInt32(dr["IdUsuario"]) : (int?)null;
                    profesional.Nombres = dr["Nombres"].ToString();
                    profesional.Apellidos = dr["Apellidos"].ToString();
                    profesional.Especialidad = dr["Especialidad"].ToString();
                    profesional.Estado = Convert.ToBoolean(dr["Estado"]);

                    // 🚀 ¡AQUÍ ESTÁ LA LÍNEA QUE FALTA! 🚀
                    // Tu código C# probablemente no está leyendo el campo CMP del resultado de la consulta.
                    profesional.CMP = dr["CMP"].ToString();
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
                cmd.Parameters.AddWithValue("@Nombres", (object)entidad.Nombres ?? DBNull.Value); // Manejo de nulos
                cmd.Parameters.AddWithValue("@Apellidos", (object)entidad.Apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Especialidad", (object)entidad.Especialidad ?? DBNull.Value);

                // 🚀 LÍNEA AÑADIDA: El parámetro que faltaba
                cmd.Parameters.AddWithValue("@Colegiatura", (object)entidad.CMP ?? DBNull.Value); // Mapeado desde CMP

                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }
}

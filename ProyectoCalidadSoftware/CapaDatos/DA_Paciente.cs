using System;
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

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(string dni, string nombres, string apellidos, DateTime fechaNacimiento,
                             string direccion, string telefono, string correo, string sexo, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DNI", (object)dni ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Nombres", (object)nombres ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellidos", (object)apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
                cmd.Parameters.AddWithValue("@Direccion", (object)direccion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object)telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", (object)correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sexo", (object)sexo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idPaciente, string dni, string nombres, string apellidos, DateTime fechaNacimiento,
                           string direccion, string telefono, string correo, string sexo, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@DNI", (object)dni ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Nombres", (object)nombres ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellidos", (object)apellidos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
                cmd.Parameters.AddWithValue("@Direccion", (object)direccion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object)telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", (object)correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sexo", (object)sexo ?? DBNull.Value);
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
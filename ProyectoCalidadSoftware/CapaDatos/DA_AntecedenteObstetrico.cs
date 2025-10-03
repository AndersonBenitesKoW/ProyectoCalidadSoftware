using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_AntecedenteObstetrico
    {
        #region Singleton
        private static readonly DA_AntecedenteObstetrico _instancia = new DA_AntecedenteObstetrico();
        public static DA_AntecedenteObstetrico Instancia
        {
            get { return DA_AntecedenteObstetrico._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarAntecedenteObstetrico", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(int idPaciente, short? menarquia, short? cicloDias, short? gestas,
                             short? partos, short? abortos, string observacion, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarAntecedenteObstetrico", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@Menarquia", (object)menarquia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CicloDias", (object)cicloDias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Gestas", (object)gestas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Partos", (object)partos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Abortos", (object)abortos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observacion", (object)observacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idAntecedente, int idPaciente, short? menarquia, short? cicloDias, short? gestas,
                           short? partos, short? abortos, string observacion, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarAntecedenteObstetrico", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAntecedente", idAntecedente);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@Menarquia", (object)menarquia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CicloDias", (object)cicloDias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Gestas", (object)gestas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Partos", (object)partos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Abortos", (object)abortos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observacion", (object)observacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idAntecedente)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarAntecedenteObstetrico", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAntecedente", idAntecedente);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idAntecedente)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarAntecedenteObstetrico", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAntecedente", idAntecedente);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }
}
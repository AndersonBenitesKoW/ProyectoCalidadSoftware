using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Bebe
    {
        #region Singleton
        private static readonly DA_Bebe _instancia = new DA_Bebe();
        public static DA_Bebe Instancia
        {
            get { return DA_Bebe._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarBebe", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(int idParto, string estadoBebe, string sexo,
                             byte? apgar1, byte? apgar5, int? pesoGr, decimal? tallaCm,
                             decimal? perimetroCefalico, decimal? egSemanas,
                             bool? reanimacion, string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarBebe", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cmd.Parameters.AddWithValue("@EstadoBebe", estadoBebe);
                cmd.Parameters.AddWithValue("@Sexo", (object)sexo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apgar1", (object)apgar1 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apgar5", (object)apgar5 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PesoGr", (object)pesoGr ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TallaCm", (object)tallaCm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PerimetroCefalico", (object)perimetroCefalico ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EG_Semanas", (object)egSemanas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Reanimacion", (object)reanimacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idBebe, int idParto, string estadoBebe, string sexo,
                           byte? apgar1, byte? apgar5, int? pesoGr, decimal? tallaCm,
                           decimal? perimetroCefalico, decimal? egSemanas,
                           bool? reanimacion, string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarBebe", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdBebe", idBebe);
                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cmd.Parameters.AddWithValue("@EstadoBebe", estadoBebe);
                cmd.Parameters.AddWithValue("@Sexo", (object)sexo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apgar1", (object)apgar1 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apgar5", (object)apgar5 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PesoGr", (object)pesoGr ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TallaCm", (object)tallaCm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PerimetroCefalico", (object)perimetroCefalico ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EG_Semanas", (object)egSemanas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Reanimacion", (object)reanimacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idBebe)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarBebe", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdBebe", idBebe);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idBebe)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarBebe", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdBebe", idBebe);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
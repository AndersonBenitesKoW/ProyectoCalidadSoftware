using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

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

        public List<entAntecedenteObstetrico> Listar()
        {
            List<entAntecedenteObstetrico> lista = new List<entAntecedenteObstetrico>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarAntecedenteObstetrico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var antecedente = new entAntecedenteObstetrico
                        {
                            IdAntecedente = Convert.ToInt32(dr["IdAntecedente"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            Menarquia = dr["Menarquia"] != DBNull.Value ? (short?)Convert.ToInt16(dr["Menarquia"]) : null,
                            CicloDias = dr["CicloDias"] != DBNull.Value ? (short?)Convert.ToInt16(dr["CicloDias"]) : null,
                            Gestas = dr["Gestas"] != DBNull.Value ? (short?)Convert.ToInt16(dr["Gestas"]) : null,
                            Partos = dr["Partos"] != DBNull.Value ? (short?)Convert.ToInt16(dr["Partos"]) : null,
                            Abortos = dr["Abortos"] != DBNull.Value ? (short?)Convert.ToInt16(dr["Abortos"]) : null,
                            Observacion = dr["Observacion"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(antecedente);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entAntecedenteObstetrico entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarAntecedenteObstetrico", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@Menarquia", (object)entidad.Menarquia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CicloDias", (object)entidad.CicloDias ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Gestas", (object)entidad.Gestas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Partos", (object)entidad.Partos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Abortos", (object)entidad.Abortos ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observacion", (object)entidad.Observacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

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
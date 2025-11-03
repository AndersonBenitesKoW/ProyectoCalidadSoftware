using CapaEntidad;
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
        // EDITAR con OBJETO
        public bool Actualizar(entAntecedenteObstetrico entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EditarAntecedenteObstetrico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAntecedente", entidad.IdAntecedente);
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

        // BUSCAR por Id devuelve ENTIDAD
        public entAntecedenteObstetrico BuscarPorId(int idAntecedente)
        {
            entAntecedenteObstetrico entidad = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarAntecedenteObstetrico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAntecedente", idAntecedente);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                        entidad = Map(dr);
                }
            }
            return entidad;
        }

        // ANULAR (soft delete: Estado = 0)
        public bool Anular(int idAntecedente)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_AnularAntecedenteObstetrico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAntecedente", idAntecedente);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // (Opcional) Eliminar físico
        public bool Eliminar(int idAntecedente)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EliminarAntecedenteObstetrico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAntecedente", idAntecedente);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ---------- Helper ----------
        private static entAntecedenteObstetrico Map(SqlDataReader dr)
        {
            int O(string n) => dr.GetOrdinal(n);
            bool N(string n) => dr.IsDBNull(O(n));

            return new entAntecedenteObstetrico
            {
                IdAntecedente = N("IdAntecedente") ? 0 : dr.GetInt32(O("IdAntecedente")),
                IdPaciente = N("IdPaciente") ? 0 : dr.GetInt32(O("IdPaciente")),
                Menarquia = N("Menarquia") ? (short?)null : Convert.ToInt16(dr["Menarquia"]),
                CicloDias = N("CicloDias") ? (short?)null : Convert.ToInt16(dr["CicloDias"]),
                Gestas = N("Gestas") ? (short?)null : Convert.ToInt16(dr["Gestas"]),
                Partos = N("Partos") ? (short?)null : Convert.ToInt16(dr["Partos"]),
                Abortos = N("Abortos") ? (short?)null : Convert.ToInt16(dr["Abortos"]),
                Observacion = N("Observacion") ? null : dr.GetString(O("Observacion")),
                Estado = !N("Estado") && Convert.ToBoolean(dr["Estado"])
            };
        }

        #endregion
    }
}
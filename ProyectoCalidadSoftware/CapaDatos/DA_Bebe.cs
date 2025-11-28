using CapaEntidad;
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

        public List<entBebe> Listar()
        {
            List<entBebe> lista = new List<entBebe>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarBebe", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var bebe = new entBebe
                        {
                            IdBebe = Convert.ToInt32(dr["IdBebe"]),
                            IdParto = Convert.ToInt32(dr["IdParto"]),
                            EstadoBebe = dr["EstadoBebe"].ToString(),
                            Sexo = dr["Sexo"].ToString(),
                            Apgar1 = dr["Apgar1"] != DBNull.Value ? (byte?)Convert.ToByte(dr["Apgar1"]) : null,
                            Apgar5 = dr["Apgar5"] != DBNull.Value ? (byte?)Convert.ToByte(dr["Apgar5"]) : null,
                            PesoGr = dr["PesoGr"] != DBNull.Value ? (int?)Convert.ToInt32(dr["PesoGr"]) : null,
                            TallaCm = dr["TallaCm"] != DBNull.Value ? Convert.ToDecimal(dr["TallaCm"]) : null,
                            PerimetroCefalico = dr["PerimetroCefalico"] != DBNull.Value ? Convert.ToDecimal(dr["PerimetroCefalico"]) : null,
                            EG_Semanas = dr["EG_Semanas"] != DBNull.Value ? Convert.ToDecimal(dr["EG_Semanas"]) : null,
                            Reanimacion = dr["Reanimacion"] != DBNull.Value ? (bool?)Convert.ToBoolean(dr["Reanimacion"]) : null,
                            Observaciones = dr["Observaciones"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(bebe);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entBebe entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarBebe", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdParto", entidad.IdParto);
                cmd.Parameters.AddWithValue("@NumeroBebe", entidad.NumeroBebe);
                cmd.Parameters.AddWithValue("@EstadoBebe", entidad.EstadoBebe);
                cmd.Parameters.AddWithValue("@Sexo", (object)entidad.Sexo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaHoraNacimiento", (object)entidad.FechaHoraNacimiento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apgar1", (object)entidad.Apgar1 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Apgar5", (object)entidad.Apgar5 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PesoGr", (object)entidad.PesoGr ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TallaCm", (object)entidad.TallaCm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PerimetroCefalico", (object)entidad.PerimetroCefalico ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EG_Semanas", (object)entidad.EG_Semanas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Reanimacion", (object)entidad.Reanimacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.Observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idBebe, int idParto, int numeroBebe, string estadoBebe, string sexo,
                            DateTime? fechaHoraNacimiento, byte? apgar1, byte? apgar5, int? pesoGr, decimal? tallaCm,
                            decimal? perimetroCefalico, decimal? egSemanas,
                            bool? reanimacion, string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarBebe", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdBebe", idBebe);
                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cmd.Parameters.AddWithValue("@NumeroBebe", numeroBebe);
                cmd.Parameters.AddWithValue("@EstadoBebe", estadoBebe);
                cmd.Parameters.AddWithValue("@Sexo", (object)sexo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaHoraNacimiento", (object)fechaHoraNacimiento ?? DBNull.Value);
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

        public entBebe BuscarBebe(int idBebe)
        {
            SqlCommand cmd = null;
            entBebe bebe = null;

            try
            {
                using (SqlConnection cn = Conexion.Instancia.Conectar())
                {
                    cmd = new SqlCommand("sp_BuscarBebe", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdBebe", idBebe);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Helpers para leer nulos de forma segura
                            int Ord(string name) => dr.GetOrdinal(name);
                            bool Nul(string name) => dr.IsDBNull(Ord(name));

                            bebe = new entBebe
                            {
                                IdBebe = Nul("IdBebe") ? 0 : dr.GetInt32(Ord("IdBebe")),
                                IdParto = Nul("IdParto") ? 0 : dr.GetInt32(Ord("IdParto")),
                                EstadoBebe = Nul("EstadoBebe") ? string.Empty : dr.GetString(Ord("EstadoBebe")),
                                Sexo = Nul("Sexo") ? null : dr.GetString(Ord("Sexo")),
                                Apgar1 = Nul("Apgar1") ? (byte?)null : Convert.ToByte(dr["Apgar1"]),
                                Apgar5 = Nul("Apgar5") ? (byte?)null : Convert.ToByte(dr["Apgar5"]),
                                PesoGr = Nul("PesoGr") ? (int?)null : dr.GetInt32(Ord("PesoGr")),
                                TallaCm = Nul("TallaCm") ? (decimal?)null : dr.GetDecimal(Ord("TallaCm")),
                                PerimetroCefalico = Nul("PerimetroCefalico") ? (decimal?)null : dr.GetDecimal(Ord("PerimetroCefalico")),
                                EG_Semanas = Nul("EG_Semanas") ? (decimal?)null : dr.GetDecimal(Ord("EG_Semanas")),
                                Reanimacion = Nul("Reanimacion") ? (bool?)null : dr.GetBoolean(Ord("Reanimacion")),
                                Observaciones = Nul("Observaciones") ? null : dr.GetString(Ord("Observaciones")),
                                Estado = Nul("Estado") ? false : dr.GetBoolean(Ord("Estado"))
                            };
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                throw new Exception("Error al buscar bebé: " + e.Message, e);
            }
            finally
            {
                // Si no usas 'using' en cmd, asegúrate de cerrar la conexión:
                cmd?.Connection?.Close();
            }

            return bebe;
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
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_AyudaDiagnosticaOrden
    {
        #region Singleton
        private static readonly DA_AyudaDiagnosticaOrden _instancia = new DA_AyudaDiagnosticaOrden();
        public static DA_AyudaDiagnosticaOrden Instancia
        {
            get { return DA_AyudaDiagnosticaOrden._instancia; }
        }
        #endregion

        public List<entAyudaDiagnosticaOrden> Listar()
        {
            List<entAyudaDiagnosticaOrden> lista = new List<entAyudaDiagnosticaOrden>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarAyudaDiagnostica", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entAyudaDiagnosticaOrden
                        {
                            IdAyuda = Convert.ToInt32(dr["IdAyuda"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            IdEmbarazo = dr["IdEmbarazo"] != DBNull.Value ? Convert.ToInt32(dr["IdEmbarazo"]) : (int?)null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? Convert.ToInt32(dr["IdProfesional"]) : (int?)null,
                            IdTipoAyuda = dr["IdTipoAyuda"] != DBNull.Value ? Convert.ToInt16(dr["IdTipoAyuda"]) : (short?)null,
                            Descripcion = dr["Descripcion"].ToString(),
                            Urgente = Convert.ToBoolean(dr["Urgente"]),
                            FechaOrden = Convert.ToDateTime(dr["FechaOrden"]),
                            Estado = dr["Estado"].ToString(),
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString(),
                            NombreTipoAyuda = dr["NombreTipoAyuda"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public int Insertar(entAyudaDiagnosticaOrden entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarAyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)entidad.IdEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdTipoAyuda", (object)entidad.IdTipoAyuda ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)entidad.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Urgente", entidad.Urgente);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Editar(entAyudaDiagnosticaOrden entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarAyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAyuda", entidad.IdAyuda);
                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)entidad.IdEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdTipoAyuda", (object)entidad.IdTipoAyuda ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)entidad.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Urgente", entidad.Urgente);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public entAyudaDiagnosticaOrden? BuscarPorId(int idAyuda)
        {
            entAyudaDiagnosticaOrden? entidad = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarAyudaDiagnostica", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdAyuda", idAyuda);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            entidad = new entAyudaDiagnosticaOrden
                            {
                                IdAyuda = Convert.ToInt32(dr["IdAyuda"]),
                                IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                                IdEmbarazo = dr["IdEmbarazo"] != DBNull.Value ? Convert.ToInt32(dr["IdEmbarazo"]) : (int?)null,
                                IdProfesional = dr["IdProfesional"] != DBNull.Value ? Convert.ToInt32(dr["IdProfesional"]) : (int?)null,
                                IdTipoAyuda = dr["IdTipoAyuda"] != DBNull.Value ? Convert.ToInt16(dr["IdTipoAyuda"]) : (short?)null,
                                Descripcion = dr["Descripcion"].ToString(),
                                Urgente = Convert.ToBoolean(dr["Urgente"]),
                                FechaOrden = Convert.ToDateTime(dr["FechaOrden"]),
                                Estado = dr["Estado"].ToString(),
                                NombrePaciente = dr["NombrePaciente"].ToString(),
                                NombreProfesional = dr["NombreProfesional"].ToString(),
                                NombreTipoAyuda = dr["NombreTipoAyuda"].ToString()
                            };
                        }
                    }
                }
            }
            return entidad;
        }

        public bool Anular(int idAyuda)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_AnularAyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAyuda", idAyuda);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_ControlPrenatal_AyudaDiagnostica
    {
        #region Singleton
        private static readonly DA_ControlPrenatal_AyudaDiagnostica _instancia = new DA_ControlPrenatal_AyudaDiagnostica();
        public static DA_ControlPrenatal_AyudaDiagnostica Instancia
        {
            get { return DA_ControlPrenatal_AyudaDiagnostica._instancia; }
        }
        #endregion

        public List<entControlPrenatal_AyudaDiagnostica> ListarPorIdControl(int idControl)
        {
            List<entControlPrenatal_AyudaDiagnostica> lista = new List<entControlPrenatal_AyudaDiagnostica>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarAyudasPorControl", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdControl", idControl);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entControlPrenatal_AyudaDiagnostica
                        {
                            IdCP_AD = Convert.ToInt32(dr["IdCP_AD"]),
                            IdControl = Convert.ToInt32(dr["IdControl"]),
                            IdAyuda = Convert.ToInt32(dr["IdAyuda"]),
                            IdTipoAyuda = dr["IdTipoAyuda"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdTipoAyuda"]) : null,
                            FechaOrden = dr["FechaOrden"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FechaOrden"]) : null,
                            Comentario = dr["Comentario"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            NombreTipoAyuda = dr["NombreTipoAyuda"].ToString(),
                            DescripcionAyuda = dr["Descripcion"].ToString(),
                            Urgente = Convert.ToBoolean(dr["Urgente"])
                        });
                    }
                }
            }
            return lista;
        }

        public bool Insertar(entControlPrenatal_AyudaDiagnostica entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarControlPrenatal_AyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdControl", entidad.IdControl);
                cmd.Parameters.AddWithValue("@IdAyuda", entidad.IdAyuda);
                cmd.Parameters.AddWithValue("@FechaOrden", (object)entidad.FechaOrden ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Comentario", (object)entidad.Comentario ?? DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool Editar(entControlPrenatal_AyudaDiagnostica entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarControlPrenatal_AyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCP_AD", entidad.IdCP_AD);
                cmd.Parameters.AddWithValue("@IdControl", entidad.IdControl);
                cmd.Parameters.AddWithValue("@IdAyuda", entidad.IdAyuda);
                cmd.Parameters.AddWithValue("@FechaOrden", (object)entidad.FechaOrden ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Comentario", (object)entidad.Comentario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool Inhabilitar(int idCP_AD)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InhabilitarControlPrenatal_AyudaDiagnostica", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCP_AD", idCP_AD);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
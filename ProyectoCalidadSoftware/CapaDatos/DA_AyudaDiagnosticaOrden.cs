using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

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

        #region Métodos

        public List<entAyudaDiagnosticaOrden> Listar()
        {
            List<entAyudaDiagnosticaOrden> lista = new List<entAyudaDiagnosticaOrden>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarAyudaDiagnosticaOrden", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var ayuda = new entAyudaDiagnosticaOrden
                        {
                            IdAyuda = Convert.ToInt32(dr["IdAyuda"]),
                            IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                            IdEmbarazo = dr["IdEmbarazo"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEmbarazo"]) : null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                            IdTipoAyuda = dr["IdTipoAyuda"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdTipoAyuda"]) : null,
                            Descripcion = dr["Descripcion"].ToString(),
                            Urgente = Convert.ToBoolean(dr["Urgente"]),
                            FechaOrden = Convert.ToDateTime(dr["FechaOrden"]),
                            Estado = dr["Estado"].ToString()
                        };

                        lista.Add(ayuda);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entAyudaDiagnosticaOrden entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarAyudaDiagnosticaOrden", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)entidad.IdEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdTipoAyuda", (object)entidad.IdTipoAyuda ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)entidad.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Urgente", entidad.Urgente);
                cmd.Parameters.AddWithValue("@FechaOrden", entidad.FechaOrden);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public bool Inhabilitar(int idAyuda)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_InhabilitarAyudaDiagnosticaOrden", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAyuda", idAyuda);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }



        public bool Editar(int idAyuda, int idPaciente, int? idEmbarazo, int? idProfesional, short? idTipoAyuda,
                           string descripcion, bool urgente, DateTime? fechaOrden, string estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarAyudaDiagnosticaOrden", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAyuda", idAyuda);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@IdEmbarazo", (object)idEmbarazo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfesional", (object)idProfesional ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdTipoAyuda", (object)idTipoAyuda ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Urgente", urgente);
                cmd.Parameters.AddWithValue("@FechaOrden", (object)fechaOrden ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idAyuda)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarAyudaDiagnosticaOrden", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAyuda", idAyuda);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idAyuda)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarAyudaDiagnosticaOrden", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAyuda", idAyuda);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
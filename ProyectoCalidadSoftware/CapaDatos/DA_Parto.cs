using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_Parto
    {
        #region Singleton
        private static readonly DA_Parto _instancia = new DA_Parto();
        public static DA_Parto Instancia
        {
            get { return DA_Parto._instancia; }
        }
        #endregion

        #region Métodos

        public List<entParto> Listar()
        {
            List<entParto> lista = new List<entParto>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarParto", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var parto = new entParto
                        {
                            IdParto = Convert.ToInt32(dr["IdParto"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEncuentro"]) : null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            HoraIngreso = dr["HoraIngreso"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HoraIngreso"]) : null,
                            HoraInicioTrabajo = dr["HoraInicioTrabajo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HoraInicioTrabajo"]) : null,
                            Membranas = dr["Membranas"].ToString(),
                            IdLiquido = dr["IdLiquido"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdLiquido"]) : null,
                            Analgesia = dr["Analgesia"].ToString(),
                            IdViaParto = dr["IdViaParto"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdViaParto"]) : null,
                            IndicacionCesarea = dr["IndicacionCesarea"].ToString(),
                            PerdidasML = dr["PerdidasML"] != DBNull.Value ? (int?)Convert.ToInt32(dr["PerdidasML"]) : null,
                            Desgarro = dr["Desgarro"].ToString(),
                            Complicaciones = dr["Complicaciones"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };

                        lista.Add(parto);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entParto entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
                cmd.Parameters.AddWithValue("@FechaHora", entidad.Fecha);
                cmd.Parameters.AddWithValue("@TipoParto", (object)entidad.Membranas ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Lugar", (object)entidad.Analgesia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.Complicaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", entidad.Estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idParto, int idEmbarazo, DateTime fechaHora, string tipoParto, string lugar, string observaciones, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);
                cmd.Parameters.AddWithValue("@FechaHora", fechaHora);
                cmd.Parameters.AddWithValue("@TipoParto", (object)tipoParto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Lugar", (object)lugar ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Observaciones", (object)observaciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idParto)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", idParto);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idParto)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", idParto);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
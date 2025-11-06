using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Parto
    {
        #region Singleton
        private static readonly DA_Parto _instancia = new DA_Parto();
        public static DA_Parto Instancia { get { return _instancia; } }
        #endregion

        public List<entParto> Listar(bool estado)
        {
            var lista = new List<entParto>();
            using (var cn = Conexion.Instancia.Conectar())
            using (var cmd = new SqlCommand("sp_ListarPartos", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Estado", estado);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entParto
                        {
                            IdParto = Convert.ToInt32(dr["IdParto"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString(),
                            NombreViaParto = dr["NombreViaParto"].ToString(),
                            NombreLiquido = dr["NombreLiquido"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        private void AddParameters(SqlCommand cmd, entParto entidad)
        {
            cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
            cmd.Parameters.AddWithValue("@IdEncuentro", (object)entidad.IdEncuentro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fecha", entidad.Fecha);
            cmd.Parameters.AddWithValue("@HoraIngreso", (object)entidad.HoraIngreso ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HoraInicioTrabajo", (object)entidad.HoraInicioTrabajo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Membranas", (object)entidad.Membranas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdLiquido", (object)entidad.IdLiquido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Analgesia", (object)entidad.Analgesia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdViaParto", (object)entidad.IdViaParto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IndicacionCesarea", (object)entidad.IndicacionCesarea ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PerdidasML", (object)entidad.PerdidasML ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Desgarro", (object)entidad.Desgarro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Complicaciones", (object)entidad.Complicaciones ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", entidad.Estado);
        }

        public bool Insertar(entParto entidad)
        {
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_InsertarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameters(cmd, entidad);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool Editar(entParto entidad)
        {
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_EditarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", entidad.IdParto);
                AddParameters(cmd, entidad);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public entParto? BuscarPorId(int idParto)
        {
            entParto? entidad = null;
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_BuscarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        entidad = new entParto
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
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString(),
                            NombreViaParto = dr["NombreViaParto"].ToString(),
                            NombreLiquido = dr["NombreLiquido"].ToString()
                        };
                    }
                }
            }
            return entidad;
        }

        public bool Anular(int idParto)
        {
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_AnularParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
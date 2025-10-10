using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_FactorRiesgoCat
    {
        #region Singleton
        private static readonly DA_FactorRiesgoCat _instancia = new DA_FactorRiesgoCat();
        public static DA_FactorRiesgoCat Instancia
        {
            get { return DA_FactorRiesgoCat._instancia; }
        }
        #endregion

        #region Métodos

        public List<entFactorRiesgoCat> Listar()
        {
            List<entFactorRiesgoCat> lista = new List<entFactorRiesgoCat>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarFactorRiesgoCat", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var factor = new entFactorRiesgoCat
                        {
                            IdFactorCat = Convert.ToInt32(dr["IdFactorCat"]),
                            Nombre = dr["Nombre"].ToString()
                        };

                        lista.Add(factor);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entFactorRiesgoCat entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarFactorRiesgoCat", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idFactorCat, string nombre)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarFactorRiesgoCat", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdFactorCat", idFactorCat);
                cmd.Parameters.AddWithValue("@Nombre", nombre);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idFactorCat)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarFactorRiesgoCat", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdFactorCat", idFactorCat);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idFactorCat)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarFactorRiesgoCat", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdFactorCat", idFactorCat);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
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

        // ACTUALIZAR (con entidad)
        public bool Actualizar(entFactorRiesgoCat entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EditarFactorRiesgoCat", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdFactorCat", entidad.IdFactorCat);
                cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // BUSCAR POR ID → ENTIDAD (sin DataTable)
        public entFactorRiesgoCat BuscarPorId(int idFactorCat)
        {
            entFactorRiesgoCat entidad = null;

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_BuscarFactorRiesgoCat", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdFactorCat", idFactorCat);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        entidad = new entFactorRiesgoCat
                        {
                            IdFactorCat = Convert.ToInt32(dr["IdFactorCat"]),
                            Nombre = dr["Nombre"].ToString()
                        };
                    }
                }
            }
            return entidad;
        }

        // ELIMINAR (por id)
        public bool Eliminar(int idFactorCat)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_EliminarFactorRiesgoCat", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdFactorCat", idFactorCat);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }


}
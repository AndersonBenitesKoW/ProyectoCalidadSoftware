using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_TipoEncuentro
    {
        #region Singleton
        private static readonly DA_TipoEncuentro _instancia = new DA_TipoEncuentro();
        public static DA_TipoEncuentro Instancia
        {
            get { return DA_TipoEncuentro._instancia; }
        }
        #endregion

        #region Métodos

        // Este es el único método que necesitamos para este módulo
        public List<entTipoEncuentro> Listar()
        {
            List<entTipoEncuentro> lista = new List<entTipoEncuentro>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarTipoEncuentro", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new entTipoEncuentro
                            {
                                IdTipoEncuentro = Convert.ToInt16(dr["IdTipoEncuentro"]),
                                Codigo = dr["Codigo"].ToString() ?? string.Empty,
                                Descripcion = dr["Descripcion"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            return lista;
        }

        #endregion
    }
}
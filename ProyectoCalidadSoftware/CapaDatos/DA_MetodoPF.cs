using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_MetodoPF
    {
        #region Singleton
        private static readonly DA_MetodoPF _instancia = new DA_MetodoPF();
        public static DA_MetodoPF Instancia
        {
            get { return DA_MetodoPF._instancia; }
        }
        #endregion

        public List<entMetodoPF> Listar()
        {
            List<entMetodoPF> lista = new List<entMetodoPF>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarMetodoPF", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new entMetodoPF
                            {
                                IdMetodoPF = Convert.ToInt16(dr["IdMetodoPF"]),
                                Nombre = dr["Nombre"].ToString() ?? string.Empty,
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}
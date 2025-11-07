using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_TipoAyudaDiagnostica
    {
        #region Singleton
        private static readonly DA_TipoAyudaDiagnostica _instancia = new DA_TipoAyudaDiagnostica();
        public static DA_TipoAyudaDiagnostica Instancia
        {
            get { return DA_TipoAyudaDiagnostica._instancia; }
        }
        #endregion

        public List<entTipoAyudaDiagnostica> Listar()
        {
            List<entTipoAyudaDiagnostica> lista = new List<entTipoAyudaDiagnostica>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarTipoAyuda", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new entTipoAyudaDiagnostica
                            {
                                IdTipoAyuda = Convert.ToInt16(dr["IdTipoAyuda"]),
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
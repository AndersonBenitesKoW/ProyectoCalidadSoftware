using CapaEntidad;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_LiquidoAmniotico
    {
        #region Singleton
        private static readonly DA_LiquidoAmniotico _instancia = new DA_LiquidoAmniotico();
        public static DA_LiquidoAmniotico Instancia { get { return _instancia; } }
        #endregion

        public List<entLiquidoAmniotico> Listar()
        {
            var lista = new List<entLiquidoAmniotico>();
            using (var cn = Conexion.Instancia.Conectar())
            using (var cmd = new SqlCommand("sp_ListarLiquidoAmniotico", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entLiquidoAmniotico
                        {
                            IdLiquido = Convert.ToInt16(dr["IdLiquido"]),
                            Descripcion = dr["Descripcion"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}
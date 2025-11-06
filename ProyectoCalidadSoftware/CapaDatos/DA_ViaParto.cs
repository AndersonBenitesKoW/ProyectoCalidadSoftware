using CapaEntidad;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_ViaParto
    {
        #region Singleton
        private static readonly DA_ViaParto _instancia = new DA_ViaParto();
        public static DA_ViaParto Instancia { get { return _instancia; } }
        #endregion

        public List<entViaParto> Listar()
        {
            var lista = new List<entViaParto>();
            using (var cn = Conexion.Instancia.Conectar())
            using (var cmd = new SqlCommand("sp_ListarViaParto", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entViaParto
                        {
                            IdViaParto = Convert.ToInt16(dr["IdViaParto"]),
                            Descripcion = dr["Descripcion"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}
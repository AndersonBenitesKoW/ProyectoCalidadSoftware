using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CapaAccesoDatos
    {
        public class Conexion
        {
            #region Singleton
            private static readonly Conexion unicaInstancia = new Conexion();
            public static Conexion Instancia
            {
                get { return Conexion.unicaInstancia; }
            }
        #endregion
        public SqlConnection Conectar()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = "Data Source=LAPTOP-HBAEIJGS\\SQLEXPRESS;" +
                                  "Initial Catalog=ProyectoCalidad;" +
                                  "Integrated Security=True;";
            return cn;
        }

    }


}



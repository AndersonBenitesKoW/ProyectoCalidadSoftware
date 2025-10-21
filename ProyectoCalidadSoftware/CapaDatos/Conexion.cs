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
<<<<<<< HEAD
            cn.ConnectionString = "Data Source=DESKTOP-TMU82JN\\SQLEXPRESS;" +
=======
            cn.ConnectionString = "Data Source=DESKTOP-BLFSTC3\\SQLEXPRESS;" +
>>>>>>> 3d76688d0ae3b9f92704d50a832f9fdb4de0ea89
                                  "Initial Catalog=ProyectoCalidad;" +
                                  "Integrated Security=True;";
            return cn;
        }

    }


}



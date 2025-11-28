using System.Data.SqlClient;


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

            
            cn.ConnectionString = "Data Source=DESKTOP-BLFSTC3\\SQLEXPRESS;" +
                                  "Initial Catalog=ProyectoCalidad;" +
                                  "Integrated Security=True;";

            

            return cn;
        }

    }


}



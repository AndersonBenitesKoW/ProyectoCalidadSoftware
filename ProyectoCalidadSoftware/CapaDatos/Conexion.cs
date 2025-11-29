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

            
            cn.ConnectionString = "Data Source=ANDERSON\\SQL2024;" +
        "Initial Catalog=ProyectoCalidad;" +
        "User ID=sa;" +
        "Password=anderson123;";





            return cn;
        }

    }


}



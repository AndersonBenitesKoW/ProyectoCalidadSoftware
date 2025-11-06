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

            // MODIFICADO: Apunta a TU servidor y usa tu autenticación de Windows.
            cn.ConnectionString = "Data Source=DESKTOP-BLFSTC3\\SQLEXPRESS;" +
                                  "Initial Catalog=ProyectoCalidad;" +
                                  "Integrated Security=True;"; // <-- Esto reemplaza al User ID y Password
            return cn;

        //"Data Source=DESKTOP-TMU82JN\\SQLEXPRESS;" +
        //          "Initial Catalog=ProyectoCalidad;" +
        //          "User ID=tuUsuarioSQL;" +
        //          "Password=tuContraseñaSQL;";

    }

    }


}



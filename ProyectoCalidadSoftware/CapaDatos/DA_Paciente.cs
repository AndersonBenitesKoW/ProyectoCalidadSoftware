using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

        public bool Eliminar(int idPaciente)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
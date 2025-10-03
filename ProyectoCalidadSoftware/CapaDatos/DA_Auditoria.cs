using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_Auditoria
    {
        #region Singleton
        private static readonly DA_Auditoria _instancia = new DA_Auditoria();
        public static DA_Auditoria Instancia
        {
            get { return DA_Auditoria._instancia; }
        }
        #endregion

        #region Métodos

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarAuditoria", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Insertar(int? idUsuario, string accion, string entidad, int? idRegistro,
                             string antes, string despues, string ipCliente)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarAuditoria", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdUsuario", (object)idUsuario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Accion", accion);
                cmd.Parameters.AddWithValue("@Entidad", (object)entidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdRegistro", (object)idRegistro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Antes", (object)antes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Despues", (object)despues ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IpCliente", (object)ipCliente ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idAuditoria, int? idUsuario, string accion, string entidad,
                           int? idRegistro, string antes, string despues, string ipCliente)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarAuditoria", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAuditoria", idAuditoria);
                cmd.Parameters.AddWithValue("@IdUsuario", (object)idUsuario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Accion", accion);
                cmd.Parameters.AddWithValue("@Entidad", (object)entidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdRegistro", (object)idRegistro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Antes", (object)antes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Despues", (object)despues ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IpCliente", (object)ipCliente ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idAuditoria)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarAuditoria", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAuditoria", idAuditoria);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idAuditoria)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarAuditoria", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAuditoria", idAuditoria);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
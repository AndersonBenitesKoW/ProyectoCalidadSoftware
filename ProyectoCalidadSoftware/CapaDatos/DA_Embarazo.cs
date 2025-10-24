using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_Embarazo
    {
        #region Singleton
        private static readonly DA_Embarazo _instancia = new DA_Embarazo();
        public static DA_Embarazo Instancia
        {
            get { return DA_Embarazo._instancia; }
        }
        #endregion

        #region Métodos

        public List<entEmbarazo> ListarPorEstado(bool estado) 
        {
            List<entEmbarazo> lista = new List<entEmbarazo>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarEmbarazosActivos", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new entEmbarazo
                            {
                                IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                                IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                                NombrePaciente = dr["NombrePaciente"].ToString() ?? string.Empty,
                                FPP = dr["FPP"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FPP"]) : null,
                                Estado = Convert.ToBoolean(dr["Estado"]) 
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public int Insertar(entEmbarazo entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertarEmbarazo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdPaciente", entidad.IdPaciente);
                    cmd.Parameters.AddWithValue("@FUR", (object)entidad.FUR ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FPP", (object)entidad.FPP ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Riesgo", (object)entidad.Riesgo ?? DBNull.Value);

                    cn.Open();

                    object idGenerado = cmd.ExecuteScalar();

                    if (idGenerado != null)
                    {
                        return Convert.ToInt32(idGenerado);
                    }
                    else
                    {
                        return 0; 
                    }
                }
            }
        }

        public bool Editar(int idEmbarazo, int idPaciente, DateTime? fur, DateTime? fpp,
                           string riesgo, DateTime? fechaApertura,
                           DateTime? fechaCierre, bool estado)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarEmbarazo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                cmd.Parameters.AddWithValue("@FUR", (object)fur ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FPP", (object)fpp ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Riesgo", (object)riesgo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaApertura", (object)fechaApertura ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCierre", (object)fechaCierre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public entEmbarazo? BuscarPorId(int idEmbarazo)
        {
            entEmbarazo? embarazo = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarEmbarazoPorId", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            embarazo = new entEmbarazo
                            {
                                IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                                IdPaciente = Convert.ToInt32(dr["IdPaciente"]),
                                FUR = dr["FUR"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FUR"]) : null,
                                FPP = dr["FPP"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FPP"]) : null,
                                Riesgo = dr["Riesgo"]?.ToString(),
                                FechaApertura = Convert.ToDateTime(dr["FechaApertura"]),
                                FechaCierre = dr["FechaCierre"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FechaCierre"]) : null,
                                Estado = Convert.ToBoolean(dr["Estado"]),
                                NombrePaciente = dr["NombrePaciente"].ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
            return embarazo; 
        }

        public bool Cerrar(int idEmbarazo)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_CerrarEmbarazo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmbarazo", idEmbarazo);

                    cn.Open();
                    // Devuelve true si se afectó al menos 1 fila
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion
    }


}
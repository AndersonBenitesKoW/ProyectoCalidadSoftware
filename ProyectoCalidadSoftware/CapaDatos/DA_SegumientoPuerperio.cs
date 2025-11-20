using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_SeguimientoPuerperio
    {
        #region Singleton
        private static readonly DA_SeguimientoPuerperio _instancia = new DA_SeguimientoPuerperio();
        public static DA_SeguimientoPuerperio Instancia
        {
            get { return DA_SeguimientoPuerperio._instancia; }
        }
        #endregion

        public List<entSeguimientoPuerperio> Listar(bool estado)
        {
            List<entSeguimientoPuerperio> lista = new List<entSeguimientoPuerperio>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarSeguimientoPuerperio", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Estado", estado);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entSeguimientoPuerperio
                        {
                            IdPuerperio = Convert.ToInt32(dr["IdPuerperio"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                            IdMetodoPF = dr["IdMetodoPF"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdMetodoPF"]) : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            DiasPosparto = dr["DiasPosparto"] != DBNull.Value ? (int?)Convert.ToInt32(dr["DiasPosparto"]) : null,
                            PA_Sistolica = dr["PA_Sistolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Sistolica"]) : null,
                            PA_Diastolica = dr["PA_Diastolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Diastolica"]) : null,
                            Temp_C = dr["Temp_C"] != DBNull.Value ? Convert.ToDecimal(dr["Temp_C"]) : null,
                            AlturaUterinaPP_cm = dr["AlturaUterinaPP_cm"] != DBNull.Value ? Convert.ToDecimal(dr["AlturaUterinaPP_cm"]) : null,
                            InvolucionUterina = dr["InvolucionUterina"].ToString(),
                            Loquios = dr["Loquios"].ToString(),
                            HemorragiaResidual = dr["HemorragiaResidual"].ToString(),
                            Lactancia = dr["Lactancia"].ToString(),
                            ApoyoLactancia = dr["ApoyoLactancia"] != DBNull.Value ? Convert.ToBoolean(dr["ApoyoLactancia"]) : false,
                            SignosInfeccion = dr["SignosInfeccion"] != DBNull.Value ? Convert.ToBoolean(dr["SignosInfeccion"]) : false,
                            TamizajeDepresion = dr["TamizajeDepresion"].ToString(),
                            ConsejoPlanificacion = dr["ConsejoPlanificacion"] != DBNull.Value ? Convert.ToBoolean(dr["ConsejoPlanificacion"]) : false,
                            VisitaDomiciliariaFecha = dr["VisitaDomiciliariaFecha"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["VisitaDomiciliariaFecha"]) : null,
                            SeguroTipo = dr["SeguroTipo"] != DBNull.Value ? dr["SeguroTipo"].ToString() : null,
                            ComplicacionesMaternas = dr["ComplicacionesMaternas"] != DBNull.Value ? dr["ComplicacionesMaternas"].ToString() : null,
                            Derivacion = dr["Derivacion"] != DBNull.Value ? Convert.ToBoolean(dr["Derivacion"]) : false,
                            EstablecimientoAtencion = dr["EstablecimientoAtencion"] != DBNull.Value ? dr["EstablecimientoAtencion"].ToString() : null,
                            Observaciones = dr["Observaciones"] != DBNull.Value ? dr["Observaciones"].ToString() : null,
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString(),
                            NombreMetodoPF = dr["NombreMetodoPF"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        private void AddParameters(SqlCommand cmd, entSeguimientoPuerperio entidad)
        {
            cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
            cmd.Parameters.AddWithValue("@IdEncuentro", (object)entidad.IdEncuentro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fecha", entidad.Fecha);
            cmd.Parameters.AddWithValue("@DiasPosparto", (object)entidad.DiasPosparto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PA_Sistolica", (object)entidad.PA_Sistolica ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PA_Diastolica", (object)entidad.PA_Diastolica ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Temp_C", (object)entidad.Temp_C ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AlturaUterinaPP_cm", (object)entidad.AlturaUterinaPP_cm ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InvolucionUterina", (object)entidad.InvolucionUterina ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Loquios", (object)entidad.Loquios ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HemorragiaResidual", (object)entidad.HemorragiaResidual ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Lactancia", (object)entidad.Lactancia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ApoyoLactancia", entidad.ApoyoLactancia);
            cmd.Parameters.AddWithValue("@SignosInfeccion", entidad.SignosInfeccion);
            cmd.Parameters.AddWithValue("@TamizajeDepresion", (object)entidad.TamizajeDepresion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdMetodoPF", (object)entidad.IdMetodoPF ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ConsejoPlanificacion", entidad.ConsejoPlanificacion);
            cmd.Parameters.AddWithValue("@VisitaDomiciliariaFecha", (object)entidad.VisitaDomiciliariaFecha ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SeguroTipo", (object)entidad.SeguroTipo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ComplicacionesMaternas", (object)entidad.ComplicacionesMaternas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Derivacion", entidad.Derivacion);
            cmd.Parameters.AddWithValue("@EstablecimientoAtencion", (object)entidad.EstablecimientoAtencion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.Observaciones ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", entidad.Estado);
        }

        public bool Insertar(entSeguimientoPuerperio entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameters(cmd, entidad);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool Editar(entSeguimientoPuerperio entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuerperio", entidad.IdPuerperio);
                AddParameters(cmd, entidad);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool Inhabilitar(int idPuerperio)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InhabilitarSeguimientoPuerperio", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPuerperio", idPuerperio);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public entSeguimientoPuerperio? BuscarPorId(int idPuerperio)
        {
            entSeguimientoPuerperio? entidad = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarSeguimientoPuerperio", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdPuerperio", idPuerperio);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            entidad = new entSeguimientoPuerperio
                            {
                                IdPuerperio = Convert.ToInt32(dr["IdPuerperio"]),
                                IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                                IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEncuentro"]) : null,
                                IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                                IdMetodoPF = dr["IdMetodoPF"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdMetodoPF"]) : null,
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                DiasPosparto = dr["DiasPosparto"] != DBNull.Value ? (int?)Convert.ToInt32(dr["DiasPosparto"]) : null,
                                PA_Sistolica = dr["PA_Sistolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Sistolica"]) : null,
                                PA_Diastolica = dr["PA_Diastolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Diastolica"]) : null,
                                Temp_C = dr["Temp_C"] != DBNull.Value ? Convert.ToDecimal(dr["Temp_C"]) : null,
                                AlturaUterinaPP_cm = dr["AlturaUterinaPP_cm"] != DBNull.Value ? Convert.ToDecimal(dr["AlturaUterinaPP_cm"]) : null,
                                InvolucionUterina = dr["InvolucionUterina"].ToString(),
                                Loquios = dr["Loquios"].ToString(),
                                HemorragiaResidual = dr["HemorragiaResidual"].ToString(),
                                Lactancia = dr["Lactancia"].ToString(),
                                ApoyoLactancia = dr["ApoyoLactancia"] != DBNull.Value ? Convert.ToBoolean(dr["ApoyoLactancia"]) : false,
                                SignosInfeccion = dr["SignosInfeccion"] != DBNull.Value ? Convert.ToBoolean(dr["SignosInfeccion"]) : false,
                                TamizajeDepresion = dr["TamizajeDepresion"].ToString(),
                                ConsejoPlanificacion = dr["ConsejoPlanificacion"] != DBNull.Value ? Convert.ToBoolean(dr["ConsejoPlanificacion"]) : false,
                                VisitaDomiciliariaFecha = dr["VisitaDomiciliariaFecha"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["VisitaDomiciliariaFecha"]) : null,
                                SeguroTipo = dr["SeguroTipo"] != DBNull.Value ? dr["SeguroTipo"].ToString() : null,
                                ComplicacionesMaternas = dr["ComplicacionesMaternas"] != DBNull.Value ? dr["ComplicacionesMaternas"].ToString() : null,
                                Derivacion = dr["Derivacion"] != DBNull.Value ? Convert.ToBoolean(dr["Derivacion"]) : false,
                                EstablecimientoAtencion = dr["EstablecimientoAtencion"] != DBNull.Value ? dr["EstablecimientoAtencion"].ToString() : null,
                                Observaciones = dr["Observaciones"] != DBNull.Value ? dr["Observaciones"].ToString() : null,
                                Estado = Convert.ToBoolean(dr["Estado"]),
                                NombrePaciente = dr["NombrePaciente"].ToString(),
                                NombreProfesional = dr["NombreProfesional"].ToString(),
                                NombreMetodoPF = dr["NombreMetodoPF"].ToString()
                            };
                        }
                    }
                }
            }
            return entidad;
        }
    }
}
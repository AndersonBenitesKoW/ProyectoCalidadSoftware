using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    public class DA_ControlPrenatal
    {
        #region Singleton
        private static readonly DA_ControlPrenatal _instancia = new DA_ControlPrenatal();
        public static DA_ControlPrenatal Instancia
        {
            get { return DA_ControlPrenatal._instancia; }
        }
        #endregion

        #region Métodos

        public List<entControlPrenatal> Listar()
        {
            List<entControlPrenatal> lista = new List<entControlPrenatal>();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarControlPrenatal", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entControlPrenatal
                        {
                            // Mapeo C# (IdControlPrenatal) a SQL (IdControl)
                            IdControlPrenatal = Convert.ToInt32(dr["IdControl"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEncuentro"]) : null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            NumeroControl = dr["NumeroControl"] != DBNull.Value ? (int?)Convert.ToInt32(dr["NumeroControl"]) : null,
                            EdadGestSemanas = dr["EdadGestSemanas"] != DBNull.Value ? (int?)Convert.ToInt32(dr["EdadGestSemanas"]) : null,
                            EdadGestDias = dr["EdadGestDias"] != DBNull.Value ? (int?)Convert.ToInt32(dr["EdadGestDias"]) : null,
                            MetodoEdadGest = dr["MetodoEdadGest"] != DBNull.Value ? dr["MetodoEdadGest"].ToString() : null,
                            PesoKg = dr["PesoKg"] != DBNull.Value ? Convert.ToDecimal(dr["PesoKg"]) : null,
                            PesoPreGestacionalKg = dr["PesoPreGestacionalKg"] != DBNull.Value ? Convert.ToDecimal(dr["PesoPreGestacionalKg"]) : null,
                            TallaM = dr["TallaM"] != DBNull.Value ? Convert.ToDecimal(dr["TallaM"]) : null,
                            IMCPreGestacional = dr["IMCPreGestacional"] != DBNull.Value ? Convert.ToDecimal(dr["IMCPreGestacional"]) : null,
                            PA_Sistolica = dr["PA_Sistolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Sistolica"]) : null,
                            PA_Diastolica = dr["PA_Diastolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Diastolica"]) : null,
                            Pulso = dr["Pulso"] != DBNull.Value ? (short?)Convert.ToInt16(dr["Pulso"]) : null,
                            FrecuenciaRespiratoria = dr["FrecuenciaRespiratoria"] != DBNull.Value ? (short?)Convert.ToInt16(dr["FrecuenciaRespiratoria"]) : null,
                            Temperatura = dr["Temperatura"] != DBNull.Value ? Convert.ToDecimal(dr["Temperatura"]) : null,
                            AlturaUterina_cm = dr["AlturaUterina_cm"] != DBNull.Value ? Convert.ToDecimal(dr["AlturaUterina_cm"]) : null,
                            DinamicaUterina = dr["DinamicaUterina"] != DBNull.Value ? dr["DinamicaUterina"].ToString() : null,
                            Presentacion = dr["Presentacion"] != DBNull.Value ? dr["Presentacion"].ToString() : null,
                            TipoEmbarazo = dr["TipoEmbarazo"] != DBNull.Value ? dr["TipoEmbarazo"].ToString() : null,
                            FCF_bpm = dr["FCF_bpm"] != DBNull.Value ? (byte?)Convert.ToByte(dr["FCF_bpm"]) : null,
                            LiquidoAmniotico = dr["LiquidoAmniotico"] != DBNull.Value ? dr["LiquidoAmniotico"].ToString() : null,
                            IndiceLiquidoAmniotico = dr["IndiceLiquidoAmniotico"] != DBNull.Value ? Convert.ToDecimal(dr["IndiceLiquidoAmniotico"]) : null,
                            PerfilBiofisico = dr["PerfilBiofisico"] != DBNull.Value ? dr["PerfilBiofisico"].ToString() : null,
                            Proteinuria = dr["Proteinuria"] != DBNull.Value ? dr["Proteinuria"].ToString() : null,
                            Edemas = dr["Edemas"] != DBNull.Value ? dr["Edemas"].ToString() : null,
                            Reflejos = dr["Reflejos"] != DBNull.Value ? dr["Reflejos"].ToString() : null,
                            Hemoglobina = dr["Hemoglobina"] != DBNull.Value ? Convert.ToDecimal(dr["Hemoglobina"]) : null,
                            ResultadoVIH = dr["ResultadoVIH"] != DBNull.Value ? dr["ResultadoVIH"].ToString() : null,
                            ResultadoSifilis = dr["ResultadoSifilis"] != DBNull.Value ? dr["ResultadoSifilis"].ToString() : null,
                            GrupoSanguineoRh = dr["GrupoSanguineoRh"] != DBNull.Value ? dr["GrupoSanguineoRh"].ToString() : null,
                            EcografiaRealizada = dr["EcografiaRealizada"] != DBNull.Value ? Convert.ToBoolean(dr["EcografiaRealizada"]) : false,
                            FechaEcografia = dr["FechaEcografia"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FechaEcografia"]) : null,
                            LugarEcografia = dr["LugarEcografia"] != DBNull.Value ? dr["LugarEcografia"].ToString() : null,
                            PlanPartoEntregado = dr["PlanPartoEntregado"] != DBNull.Value ? Convert.ToBoolean(dr["PlanPartoEntregado"]) : false,
                            MicronutrientesEntregados = dr["MicronutrientesEntregados"] != DBNull.Value ? dr["MicronutrientesEntregados"].ToString() : null,
                            ViajoUltSemanas = dr["ViajoUltSemanas"] != DBNull.Value ? Convert.ToBoolean(dr["ViajoUltSemanas"]) : false,
                            ReferenciaObstetrica = dr["ReferenciaObstetrica"] != DBNull.Value ? Convert.ToBoolean(dr["ReferenciaObstetrica"]) : false,
                            Consejerias = dr["Consejerias"] != DBNull.Value ? dr["Consejerias"].ToString() : null,
                            Observaciones = dr["Observaciones"] != DBNull.Value ? dr["Observaciones"].ToString() : null,
                            ProximaCitaFecha = dr["ProximaCitaFecha"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["ProximaCitaFecha"]) : null,
                            EstablecimientoAtencion = dr["EstablecimientoAtencion"] != DBNull.Value ? dr["EstablecimientoAtencion"].ToString() : null,
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            // Campos JOIN
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        // Método privado para añadir todos los parámetros
        private void AddControlParameters(SqlCommand cmd, entControlPrenatal entidad)
        {
            cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
            cmd.Parameters.AddWithValue("@IdEncuentro", (object)entidad.IdEncuentro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fecha", entidad.Fecha);
            cmd.Parameters.AddWithValue("@NumeroControl", (object)entidad.NumeroControl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EdadGestSemanas", (object)entidad.EdadGestSemanas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EdadGestDias", (object)entidad.EdadGestDias ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MetodoEdadGest", (object)entidad.MetodoEdadGest ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PesoKg", (object)entidad.PesoKg ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PesoPreGestacionalKg", (object)entidad.PesoPreGestacionalKg ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TallaM", (object)entidad.TallaM ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IMCPreGestacional", (object)entidad.IMCPreGestacional ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PA_Sistolica", (object)entidad.PA_Sistolica ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PA_Diastolica", (object)entidad.PA_Diastolica ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Pulso", (object)entidad.Pulso ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FrecuenciaRespiratoria", (object)entidad.FrecuenciaRespiratoria ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Temperatura", (object)entidad.Temperatura ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AlturaUterina_cm", (object)entidad.AlturaUterina_cm ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DinamicaUterina", (object)entidad.DinamicaUterina ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Presentacion", (object)entidad.Presentacion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoEmbarazo", (object)entidad.TipoEmbarazo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FCF_bpm", (object)entidad.FCF_bpm ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LiquidoAmniotico", (object)entidad.LiquidoAmniotico ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IndiceLiquidoAmniotico", (object)entidad.IndiceLiquidoAmniotico ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PerfilBiofisico", (object)entidad.PerfilBiofisico ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Proteinuria", (object)entidad.Proteinuria ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Edemas", (object)entidad.Edemas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Reflejos", (object)entidad.Reflejos ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Hemoglobina", (object)entidad.Hemoglobina ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ResultadoVIH", (object)entidad.ResultadoVIH ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ResultadoSifilis", (object)entidad.ResultadoSifilis ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GrupoSanguineoRh", (object)entidad.GrupoSanguineoRh ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EcografiaRealizada", (object)entidad.EcografiaRealizada ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaEcografia", (object)entidad.FechaEcografia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LugarEcografia", (object)entidad.LugarEcografia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PlanPartoEntregado", (object)entidad.PlanPartoEntregado ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MicronutrientesEntregados", (object)entidad.MicronutrientesEntregados ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ViajoUltSemanas", (object)entidad.ViajoUltSemanas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReferenciaObstetrica", (object)entidad.ReferenciaObstetrica ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Consejerias", (object)entidad.Consejerias ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.Observaciones ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ProximaCitaFecha", (object)entidad.ProximaCitaFecha ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EstablecimientoAtencion", (object)entidad.EstablecimientoAtencion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", entidad.Estado);
        }

        public int Insertar(entControlPrenatal entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                AddControlParameters(cmd, entidad);

                cn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }


  


        public bool Editar(entControlPrenatal entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                // Mapeo C# (IdControlPrenatal) a SQL (@IdControl)
                cmd.Parameters.AddWithValue("@IdControl", entidad.IdControlPrenatal);
                AddControlParameters(cmd, entidad); // Usa el método helper
                cn.Open();
                cmd.ExecuteNonQuery();
                return true; // Asumimos éxito
            }
        }

        public bool Inhabilitar(int idControl)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InhabilitarControlPrenatal", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdControl", idControl);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true; // Asumimos éxito
            }
        }

        public entControlPrenatal? BuscarPorId(int idControl)
        {
            entControlPrenatal? control = null;
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarControlPrenatal", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdControl", idControl);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            control = new entControlPrenatal
                            {
                                IdControlPrenatal = Convert.ToInt32(dr["IdControl"]),
                                IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                                IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEncuentro"]) : null,
                                IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                NumeroControl = dr["NumeroControl"] != DBNull.Value ? (int?)Convert.ToInt32(dr["NumeroControl"]) : null,
                                EdadGestSemanas = dr["EdadGestSemanas"] != DBNull.Value ? (int?)Convert.ToInt32(dr["EdadGestSemanas"]) : null,
                                EdadGestDias = dr["EdadGestDias"] != DBNull.Value ? (int?)Convert.ToInt32(dr["EdadGestDias"]) : null,
                                MetodoEdadGest = dr["MetodoEdadGest"] != DBNull.Value ? dr["MetodoEdadGest"].ToString() : null,
                                PesoKg = dr["PesoKg"] != DBNull.Value ? Convert.ToDecimal(dr["PesoKg"]) : null,
                                PesoPreGestacionalKg = dr["PesoPreGestacionalKg"] != DBNull.Value ? Convert.ToDecimal(dr["PesoPreGestacionalKg"]) : null,
                                TallaM = dr["TallaM"] != DBNull.Value ? Convert.ToDecimal(dr["TallaM"]) : null,
                                IMCPreGestacional = dr["IMCPreGestacional"] != DBNull.Value ? Convert.ToDecimal(dr["IMCPreGestacional"]) : null,
                                PA_Sistolica = dr["PA_Sistolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Sistolica"]) : null,
                                PA_Diastolica = dr["PA_Diastolica"] != DBNull.Value ? (byte?)Convert.ToByte(dr["PA_Diastolica"]) : null,
                                Pulso = dr["Pulso"] != DBNull.Value ? (short?)Convert.ToInt16(dr["Pulso"]) : null,
                                FrecuenciaRespiratoria = dr["FrecuenciaRespiratoria"] != DBNull.Value ? (short?)Convert.ToInt16(dr["FrecuenciaRespiratoria"]) : null,
                                Temperatura = dr["Temperatura"] != DBNull.Value ? Convert.ToDecimal(dr["Temperatura"]) : null,
                                AlturaUterina_cm = dr["AlturaUterina_cm"] != DBNull.Value ? Convert.ToDecimal(dr["AlturaUterina_cm"]) : null,
                                DinamicaUterina = dr["DinamicaUterina"] != DBNull.Value ? dr["DinamicaUterina"].ToString() : null,
                                Presentacion = dr["Presentacion"] != DBNull.Value ? dr["Presentacion"].ToString() : null,
                                TipoEmbarazo = dr["TipoEmbarazo"] != DBNull.Value ? dr["TipoEmbarazo"].ToString() : null,
                                FCF_bpm = dr["FCF_bpm"] != DBNull.Value ? (byte?)Convert.ToByte(dr["FCF_bpm"]) : null,
                                LiquidoAmniotico = dr["LiquidoAmniotico"] != DBNull.Value ? dr["LiquidoAmniotico"].ToString() : null,
                                IndiceLiquidoAmniotico = dr["IndiceLiquidoAmniotico"] != DBNull.Value ? Convert.ToDecimal(dr["IndiceLiquidoAmniotico"]) : null,
                                PerfilBiofisico = dr["PerfilBiofisico"] != DBNull.Value ? dr["PerfilBiofisico"].ToString() : null,
                                Proteinuria = dr["Proteinuria"] != DBNull.Value ? dr["Proteinuria"].ToString() : null,
                                Edemas = dr["Edemas"] != DBNull.Value ? dr["Edemas"].ToString() : null,
                                Reflejos = dr["Reflejos"] != DBNull.Value ? dr["Reflejos"].ToString() : null,
                                Hemoglobina = dr["Hemoglobina"] != DBNull.Value ? Convert.ToDecimal(dr["Hemoglobina"]) : null,
                                ResultadoVIH = dr["ResultadoVIH"] != DBNull.Value ? dr["ResultadoVIH"].ToString() : null,
                                ResultadoSifilis = dr["ResultadoSifilis"] != DBNull.Value ? dr["ResultadoSifilis"].ToString() : null,
                                GrupoSanguineoRh = dr["GrupoSanguineoRh"] != DBNull.Value ? dr["GrupoSanguineoRh"].ToString() : null,
                                EcografiaRealizada = dr["EcografiaRealizada"] != DBNull.Value ? Convert.ToBoolean(dr["EcografiaRealizada"]) : false,
                                FechaEcografia = dr["FechaEcografia"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FechaEcografia"]) : null,
                                LugarEcografia = dr["LugarEcografia"] != DBNull.Value ? dr["LugarEcografia"].ToString() : null,
                                PlanPartoEntregado = dr["PlanPartoEntregado"] != DBNull.Value ? Convert.ToBoolean(dr["PlanPartoEntregado"]) : false,
                                MicronutrientesEntregados = dr["MicronutrientesEntregados"] != DBNull.Value ? dr["MicronutrientesEntregados"].ToString() : null,
                                ViajoUltSemanas = dr["ViajoUltSemanas"] != DBNull.Value ? Convert.ToBoolean(dr["ViajoUltSemanas"]) : false,
                                ReferenciaObstetrica = dr["ReferenciaObstetrica"] != DBNull.Value ? Convert.ToBoolean(dr["ReferenciaObstetrica"]) : false,
                                Consejerias = dr["Consejerias"] != DBNull.Value ? dr["Consejerias"].ToString() : null,
                                Observaciones = dr["Observaciones"] != DBNull.Value ? dr["Observaciones"].ToString() : null,
                                ProximaCitaFecha = dr["ProximaCitaFecha"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["ProximaCitaFecha"]) : null,
                                EstablecimientoAtencion = dr["EstablecimientoAtencion"] != DBNull.Value ? dr["EstablecimientoAtencion"].ToString() : null,
                                Estado = Convert.ToBoolean(dr["Estado"]),
                                NombrePaciente = dr["NombrePaciente"].ToString(),
                                NombreProfesional = dr["NombreProfesional"].ToString()
                            };
                        }
                    }
                }
            }
            return control;
        }

        #endregion
    }
}
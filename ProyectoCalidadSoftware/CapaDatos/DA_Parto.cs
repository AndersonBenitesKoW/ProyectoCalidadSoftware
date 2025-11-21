using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaAccesoDatos
{
    public class DA_Parto
    {
        #region Singleton
        private static readonly DA_Parto _instancia = new DA_Parto();
        public static DA_Parto Instancia { get { return _instancia; } }
        #endregion

        public List<entParto> Listar(bool estado)
        {
            var lista = new List<entParto>();
            using (var cn = Conexion.Instancia.Conectar())
            using (var cmd = new SqlCommand("sp_ListarPartos", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Estado", estado);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new entParto
                        {
                            IdParto = Convert.ToInt32(dr["IdParto"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString(),
                            NombreViaParto = dr["NombreViaParto"].ToString(),
                            NombreLiquido = dr["NombreLiquido"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        private void AddParameters(SqlCommand cmd, entParto entidad)
        {
            cmd.Parameters.AddWithValue("@IdEmbarazo", entidad.IdEmbarazo);
            cmd.Parameters.AddWithValue("@IdEncuentro", (object)entidad.IdEncuentro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdProfesional", (object)entidad.IdProfesional ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fecha", entidad.Fecha);
            cmd.Parameters.AddWithValue("@HoraIngreso", (object)entidad.HoraIngreso ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HoraInicioTrabajo", (object)entidad.HoraInicioTrabajo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HoraExpulsion", (object)entidad.HoraExpulsion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoParto", (object)entidad.TipoParto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Membranas", (object)entidad.Membranas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TiempoRoturaMembranasHoras", (object)entidad.TiempoRoturaMembranasHoras ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdLiquido", (object)entidad.IdLiquido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AspectoLiquido", (object)entidad.AspectoLiquido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Analgesia", (object)entidad.Analgesia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PosicionMadre", (object)entidad.PosicionMadre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Acompanante", (object)entidad.Acompanante ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdViaParto", (object)entidad.IdViaParto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IndicacionCesarea", (object)entidad.IndicacionCesarea ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LugarNacimiento", (object)entidad.LugarNacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DuracionSegundaEtapaMinutos", (object)entidad.DuracionSegundaEtapaMinutos ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PerdidasML", (object)entidad.PerdidasML ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Desgarro", (object)entidad.Desgarro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Episiotomia", (object)entidad.Episiotomia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ComplicacionesMaternas", (object)entidad.ComplicacionesMaternas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Derivacion", (object)entidad.Derivacion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SeguroTipo", (object)entidad.SeguroTipo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NumeroHijosPrevios", (object)entidad.NumeroHijosPrevios ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NumeroCesareasPrevias", (object)entidad.NumeroCesareasPrevias ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmbarazoMultiple", (object)entidad.EmbarazoMultiple ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NumeroGemelos", (object)entidad.NumeroGemelos ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Observaciones", (object)entidad.Observaciones ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", entidad.Estado);
        }

        public bool Insertar(entParto entidad)
        {
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_InsertarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameters(cmd, entidad);
                cn.Open();
                var idParto = Convert.ToInt32(cmd.ExecuteScalar());
                // Insertar intervenciones

                Console.WriteLine("punto 1 " );
                foreach (var interv in entidad.Intervenciones)
                {
                    DA_PartoIntervencion.Instancia.Insertar(
                        new entPartoIntervencion { IdParto = idParto, Intervencion = interv.Intervencion }
                    );
                }

                Console.WriteLine("punto 2 ");  
                // Insertar bebes
                foreach (var bebe in entidad.Bebes)
                {
                    bebe.IdParto = idParto;
                    DA_Bebe.Instancia.Insertar(bebe);
                }
                Console.WriteLine("punto 3 ");  
                return true;
            }
        }

        public int InsertarConId(entParto entidad)
        {
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_InsertarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameters(cmd, entidad);
                cn.Open();
                var idParto = Convert.ToInt32(cmd.ExecuteScalar());
                // Insertar intervenciones
                foreach (var interv in entidad.Intervenciones)
                {
                    DA_PartoIntervencion.Instancia.Insertar(
                        new entPartoIntervencion { IdParto = idParto, Intervencion = interv.Intervencion }
                    );
                }
                // Insertar bebes
                foreach (var bebe in entidad.Bebes)
                {
                    bebe.IdParto = idParto;
                    DA_Bebe.Instancia.Insertar(bebe);
                }
                return idParto;
            }
        }


        public bool Editar(entParto entidad)
        {
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_EditarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", entidad.IdParto);
                AddParameters(cmd, entidad);
                cn.Open();
                cmd.ExecuteNonQuery();
                // Eliminar intervenciones existentes y insertar nuevas
                var intervExistentes = DA_PartoIntervencion.Instancia.Listar().Where(i => i.IdParto == entidad.IdParto).ToList();
                foreach (var i in intervExistentes)
                {
                    DA_PartoIntervencion.Instancia.Eliminar(i.IdPartoIntervencion);
                }
                foreach (var interv in entidad.Intervenciones)
                {
                    DA_PartoIntervencion.Instancia.Insertar(new entPartoIntervencion { IdParto = entidad.IdParto, Intervencion = interv.Intervencion });
                }
                // Similar para bebes
                var bebesExistentes = DA_Bebe.Instancia.Listar().Where(b => b.IdParto == entidad.IdParto).ToList();
                foreach (var b in bebesExistentes)
                {
                    DA_Bebe.Instancia.Eliminar(b.IdBebe);
                }
                foreach (var bebe in entidad.Bebes)
                {
                    bebe.IdParto = entidad.IdParto;
                    DA_Bebe.Instancia.Insertar(bebe);
                }
                return true;
            }
        }

        public entParto? BuscarPorId(int idParto)
        {
            entParto? entidad = null;
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_BuscarParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    // Primer result set: Parto
                    if (dr.Read())
                    {
                        entidad = new entParto
                        {
                            IdParto = Convert.ToInt32(dr["IdParto"]),
                            IdEmbarazo = Convert.ToInt32(dr["IdEmbarazo"]),
                            IdEncuentro = dr["IdEncuentro"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdEncuentro"]) : null,
                            IdProfesional = dr["IdProfesional"] != DBNull.Value ? (int?)Convert.ToInt32(dr["IdProfesional"]) : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            HoraIngreso = dr["HoraIngreso"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HoraIngreso"]) : null,
                            HoraInicioTrabajo = dr["HoraInicioTrabajo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HoraInicioTrabajo"]) : null,
                            Membranas = dr["Membranas"].ToString(),
                            IdLiquido = dr["IdLiquido"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdLiquido"]) : null,
                            Analgesia = dr["Analgesia"].ToString(),
                            IdViaParto = dr["IdViaParto"] != DBNull.Value ? (short?)Convert.ToInt16(dr["IdViaParto"]) : null,
                            IndicacionCesarea = dr["IndicacionCesarea"].ToString(),
                            PerdidasML = dr["PerdidasML"] != DBNull.Value ? (int?)Convert.ToInt32(dr["PerdidasML"]) : null,
                            Desgarro = dr["Desgarro"].ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            HoraExpulsion = dr["HoraExpulsion"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HoraExpulsion"]) : null,
                            TipoParto = dr["TipoParto"].ToString(),
                            TiempoRoturaMembranasHoras = dr["TiempoRoturaMembranasHoras"] != DBNull.Value ? (int?)Convert.ToInt32(dr["TiempoRoturaMembranasHoras"]) : null,
                            AspectoLiquido = dr["AspectoLiquido"].ToString(),
                            PosicionMadre = dr["PosicionMadre"].ToString(),
                            Acompanante = dr["Acompanante"] != DBNull.Value ? Convert.ToBoolean(dr["Acompanante"]) : false,
                            LugarNacimiento = dr["LugarNacimiento"].ToString(),
                            DuracionSegundaEtapaMinutos = dr["DuracionSegundaEtapaMinutos"] != DBNull.Value ? (int?)Convert.ToInt32(dr["DuracionSegundaEtapaMinutos"]) : null,
                            Episiotomia = dr["Episiotomia"] != DBNull.Value ? Convert.ToBoolean(dr["Episiotomia"]) : false,
                            ComplicacionesMaternas = dr["ComplicacionesMaternas"].ToString(),
                            Derivacion = dr["Derivacion"] != DBNull.Value ? Convert.ToBoolean(dr["Derivacion"]) : false,
                            SeguroTipo = dr["SeguroTipo"].ToString(),
                            NumeroHijosPrevios = dr["NumeroHijosPrevios"] != DBNull.Value ? (int?)Convert.ToInt32(dr["NumeroHijosPrevios"]) : null,
                            NumeroCesareasPrevias = dr["NumeroCesareasPrevias"] != DBNull.Value ? (int?)Convert.ToInt32(dr["NumeroCesareasPrevias"]) : null,
                            EmbarazoMultiple = dr["EmbarazoMultiple"] != DBNull.Value ? Convert.ToBoolean(dr["EmbarazoMultiple"]) : false,
                            NumeroGemelos = dr["NumeroGemelos"] != DBNull.Value ? (int?)Convert.ToInt32(dr["NumeroGemelos"]) : null,
                            Observaciones = dr["Observaciones"].ToString(),
                            NombrePaciente = dr["NombrePaciente"].ToString(),
                            NombreProfesional = dr["NombreProfesional"].ToString(),
                            NombreViaParto = dr["NombreViaParto"].ToString(),
                            NombreLiquido = dr["NombreLiquido"].ToString(),
                            Intervenciones = new List<entPartoIntervencion>(),
                            Bebes = new List<entBebe>()
                        };
                    }
                    // Segundo result set: Intervenciones
                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            entidad.Intervenciones.Add(new entPartoIntervencion
                            {
                                IdPartoIntervencion = Convert.ToInt32(dr["IdPartoIntervencion"]),
                                Intervencion = dr["Intervencion"].ToString()
                            });
                        }
                    }
                    // Tercer result set: Bebes
                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            entidad.Bebes.Add(new entBebe
                            {
                                IdBebe = Convert.ToInt32(dr["IdBebe"]),
                                NumeroBebe = Convert.ToInt32(dr["NumeroBebe"]),
                                Sexo = dr["Sexo"].ToString(),
                                FechaHoraNacimiento = dr["FechaHoraNacimiento"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FechaHoraNacimiento"]) : null,
                                Apgar1 = dr["Apgar1"] != DBNull.Value ? (byte?)Convert.ToByte(dr["Apgar1"]) : null,
                                Apgar5 = dr["Apgar5"] != DBNull.Value ? (byte?)Convert.ToByte(dr["Apgar5"]) : null,
                                PesoGr = dr["PesoGr"] != DBNull.Value ? (int?)Convert.ToInt32(dr["PesoGr"]) : null,
                                TallaCm = dr["TallaCm"] != DBNull.Value ? Convert.ToDecimal(dr["TallaCm"]) : null,
                                PerimetroCefalico = dr["PerimetroCefalico"] != DBNull.Value ? Convert.ToDecimal(dr["PerimetroCefalico"]) : null,
                                EG_Semanas = dr["EG_Semanas"] != DBNull.Value ? Convert.ToDecimal(dr["EG_Semanas"]) : null,
                                Reanimacion = dr["Reanimacion"] != DBNull.Value ? (bool?)Convert.ToBoolean(dr["Reanimacion"]) : null,
                                Observaciones = dr["Observaciones"].ToString(),
                                Estado = Convert.ToBoolean(dr["Estado"])
                            });
                        }
                    }
                }
            }
            return entidad;
        }

        public bool Anular(int idParto)
        {
            using (var cn = Conexion.Instancia.Conectar())
            {
                var cmd = new SqlCommand("sp_AnularParto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParto", idParto);
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
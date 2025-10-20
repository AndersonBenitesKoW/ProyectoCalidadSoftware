﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaAccesoDatos
{
    public class DA_MetodoPF
    {
        #region Singleton
        private static readonly DA_MetodoPF _instancia = new DA_MetodoPF();
        public static DA_MetodoPF Instancia
        {
            get { return DA_MetodoPF._instancia; }
        }
        #endregion

        #region Métodos

        public List<entMetodoPF> Listar()
        {
            List<entMetodoPF> lista = new List<entMetodoPF>();

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            using (SqlCommand cmd = new SqlCommand("sp_ListarMetodoPF", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var metodo = new entMetodoPF
                        {
                            IdMetodoPF = Convert.ToInt16(dr["IdMetodoPF"]),
                            Nombre = dr["Nombre"].ToString()
                        };

                        lista.Add(metodo);
                    }
                }
            }

            return lista;
        }

        public bool Insertar(entMetodoPF entidad)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarMetodoPF", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Editar(int idMetodoPF, string nombre)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EditarMetodoPF", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdMetodoPF", idMetodoPF);
                cmd.Parameters.AddWithValue("@Nombre", (object)nombre ?? DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable BuscarPorId(int idMetodoPF)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_BuscarMetodoPF", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMetodoPF", idMetodoPF);

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public bool Eliminar(int idMetodoPF)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarMetodoPF", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMetodoPF", idMetodoPF);

                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }


}
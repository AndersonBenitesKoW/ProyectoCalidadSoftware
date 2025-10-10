﻿using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
namespace CapaLogica
{
    public class logPartoIntervencion
    {
        #region Singleton
        private static readonly logPartoIntervencion UnicaInstancia = new logPartoIntervencion();
        public static logPartoIntervencion Instancia
        {
            get { return logPartoIntervencion.UnicaInstancia; }
        }
        private logPartoIntervencion() { }
        #endregion

        // LISTAR
        public List<entPartoIntervencion> ListarPartoIntervencion()
        {
            return DA_PartoIntervencion.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarPartoIntervencion(entPartoIntervencion entidad)
        {
            return DA_PartoIntervencion.Instancia.Insertar(entidad);
        }


    }
}

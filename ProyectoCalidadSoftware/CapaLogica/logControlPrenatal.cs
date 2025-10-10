﻿using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaLogica
{
    public class logControlPrenatal
    {
        #region Singleton
        private static readonly logControlPrenatal UnicaInstancia = new logControlPrenatal();
        public static logControlPrenatal Instancia
        {
            get { return logControlPrenatal.UnicaInstancia; }
        }
        private logControlPrenatal() { }
        #endregion

        // LISTAR
        public List<entControlPrenatal> ListarControlPrenatal()
        {
            return DA_ControlPrenatal.Instancia.Listar();
        }

        // INSERTAR
        public bool  InsertarControlPrenatal(entControlPrenatal entidad)
        {
            return DA_ControlPrenatal.Instancia.Insertar(entidad);
        }

    }
}

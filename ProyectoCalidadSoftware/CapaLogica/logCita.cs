﻿using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
namespace CapaLogica
{
    public class logCita
    {
        #region Singleton
        private static readonly logCita UnicaInstancia = new logCita();
        public static logCita Instancia
        {
            get { return logCita.UnicaInstancia; }
        }
        private logCita() { }
        #endregion

        // LISTAR
        public List<entCita> ListarCita()
        {
            return DA_Cita.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarCita(entCita entidad)
        {
            return DA_Cita.Instancia.Insertar(entidad);
        }


    }
}

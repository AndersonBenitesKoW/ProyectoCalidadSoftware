using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
      public class logPaciente
    {

        #region Singleton
        private static readonly logPaciente UnicaInstancia = new logPaciente();
        public static logPaciente Instancia
        {
            get { return logPaciente.UnicaInstancia; }
        }
        private logPaciente() { }
        #endregion

        // LISTAR
        public List<entPaciente> ListarPaciente()
        {
            return DA_Paciente.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarPaciente(entPaciente entidad)
        {
            return DA_Paciente.Instancia.Insertar(entidad);
        }

    }
}

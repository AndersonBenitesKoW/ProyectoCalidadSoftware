using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class logPacienteFactorRiesgo
    {

        #region Singleton
        private static readonly logPacienteFactorRiesgo UnicaInstancia = new logPacienteFactorRiesgo();
        public static logPacienteFactorRiesgo Instancia
        {
            get { return logPacienteFactorRiesgo.UnicaInstancia; }
        }
        private logPacienteFactorRiesgo() { }
        #endregion

        #region Métodos
        // LISTAR
        public List<entPacienteFactorRiesgo> ListarPacienteFactorRiesgo()
        {
            return DA_PacienteFactorRiesgo.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarPacienteFactorRiesgo(entPacienteFactorRiesgo entidad)
        {
            return DA_PacienteFactorRiesgo.Instancia.Insertar(entidad);
        }
        #endregion


    }
}

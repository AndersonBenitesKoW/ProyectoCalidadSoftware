using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class logResultadoDiagnostico
    {


        #region Singleton
        private static readonly logResultadoDiagnostico UnicaInstancia = new logResultadoDiagnostico();
        public static logResultadoDiagnostico Instancia
        {
            get { return logResultadoDiagnostico.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        public List<entResultadoDiagnostico> ListarResultadoDiagnostico()
        {
            return DA_ResultadoDiagnostico.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarResultadoDiagnostico(entResultadoDiagnostico entidad)
        {
            return DA_ResultadoDiagnostico.Instancia.Insertar(entidad);
        }



    }
}

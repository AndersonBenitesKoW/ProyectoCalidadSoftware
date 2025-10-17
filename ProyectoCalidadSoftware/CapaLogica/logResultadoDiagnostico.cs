using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
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
        public bool InsertarResultadoDiagnostico(entResultadoDiagnostico entidad)
        {
            return DA_ResultadoDiagnostico.Instancia.Insertar(entidad);
        }

        public entResultadoDiagnostico BuscarResultadoDiagnostico(int id)
            => DA_ResultadoDiagnostico.Instancia.BuscarPorId(id);

        public bool ActualizarResultadoDiagnostico(entResultadoDiagnostico entidad)
            => DA_ResultadoDiagnostico.Instancia.Actualizar(entidad);

        // Si prefieres el nombre Modificar:
        public bool ModificarResultadoDiagnostico(entResultadoDiagnostico entidad)
            => DA_ResultadoDiagnostico.Instancia.Actualizar(entidad);

        public bool AnularResultadoDiagnostico(int id)
            => DA_ResultadoDiagnostico.Instancia.Anular(id);





    }
}

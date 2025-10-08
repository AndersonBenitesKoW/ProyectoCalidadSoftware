using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class logAyudaDiagnosticaOrden
    {
        #region Singleton
        private static readonly logAyudaDiagnosticaOrden UnicaInstancia = new logAyudaDiagnosticaOrden();
        public static logAyudaDiagnosticaOrden Instancia
        {
            get { return logAyudaDiagnosticaOrden.UnicaInstancia; }
        }
        private logAyudaDiagnosticaOrden() { }
        #endregion

        // LISTAR
        public List<entAyudaDiagnosticaOrden> ListarAyudaDiagnosticaOrden()
        {
            return DA_AyudaDiagnosticaOrden.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarAyudaDiagnosticaOrden(entAyudaDiagnosticaOrden entidad)
        {
            return DA_AyudaDiagnosticaOrden.Instancia.Insertar(entidad);
        }
    }
}

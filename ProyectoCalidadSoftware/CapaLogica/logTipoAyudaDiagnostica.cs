using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class logTipoAyudaDiagnostica
    {

        #region Singleton
        private static readonly logTipoAyudaDiagnostica UnicaInstancia = new logTipoAyudaDiagnostica();
        public static logTipoAyudaDiagnostica Instancia
        {
            get { return logTipoAyudaDiagnostica.UnicaInstancia; }
        }
        private logTipoAyudaDiagnostica() { }
        #endregion

        // LISTAR
        public List<entTipoAyudaDiagnostica> ListarTipoAyudaDiagnostica()
        {
            return DA_TipoAyudaDiagnostica.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarTipoAyudaDiagnostica(entTipoAyudaDiagnostica entidad)
        {
            return DA_TipoAyudaDiagnostica.Instancia.Insertar(entidad);
        }


    }
}

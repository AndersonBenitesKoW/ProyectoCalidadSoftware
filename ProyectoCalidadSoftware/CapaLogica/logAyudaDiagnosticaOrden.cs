using CapaAccesoDatos;
using CapaEntidad;

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
        public bool InsertarAyudaDiagnosticaOrden(entAyudaDiagnosticaOrden entidad)
        {
            return DA_AyudaDiagnosticaOrden.Instancia.Insertar(entidad);
        }



        public bool Inhabilitar(int idAyuda)
            => DA_AyudaDiagnosticaOrden.Instancia.Inhabilitar(idAyuda);

    }
}

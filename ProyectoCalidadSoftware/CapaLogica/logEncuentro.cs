using CapaAccesoDatos;
using CapaEntidad;

namespace CapaLogica
{
    public class logEncuentro
    {
        #region Singleton
        private static readonly logEncuentro UnicaInstancia = new logEncuentro();
        public static logEncuentro Instancia
        {
            get { return logEncuentro.UnicaInstancia; }
        }
        private logEncuentro() { }
        #endregion

        // LISTAR
        public List<entEncuentro> ListarPorEmbarazoYTipo(int idEmbarazo, string codigoTipo)
        {
            if (idEmbarazo <= 0 || string.IsNullOrWhiteSpace(codigoTipo))
            {
                return new List<entEncuentro>();
            }
            return DA_Encuentro.Instancia.ListarPorEmbarazoYTipo(idEmbarazo, codigoTipo);
        }

        // INSERTAR
        public int RegistrarEncuentro(entEncuentro encuentro)
        {
            if (encuentro.IdEmbarazo <= 0)
            {
                throw new ApplicationException("El IdEmbarazo es obligatorio.");
            }
            if (encuentro.IdTipoEncuentro <= 0)
            {
                throw new ApplicationException("El Tipo de Encuentro es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(encuentro.Estado))
            {
                encuentro.Estado = "Abierto";
            }

            return DA_Encuentro.Instancia.Insertar(encuentro);
        }




    }
}
using CapaAccesoDatos;
using CapaEntidad;

namespace CapaLogica
{
    public class logParto
    {


        #region Singleton
        private static readonly logParto _instancia = new logParto();
        public static logParto Instancia
        {
            get { return logParto._instancia; }
        }
        private logParto() { }
        #endregion

        /// <summary>
        /// Lógica de negocio para registrar un Parto.
        /// </summary>
        public int RegistrarParto(entParto parto)
        {
            if (parto.IdEmbarazo <= 0)
            {
                throw new ApplicationException("El IdEmbarazo es obligatorio.");
            }
            if (parto.Fecha > DateTime.Now)
            {
                throw new ApplicationException("La fecha del parto no puede ser en el futuro.");
            }


            return DA_Parto.Instancia.RegistrarParto(parto);
        }

        /// <summary>
        /// Lógica de negocio para anular un Parto.
        /// </summary>
        public bool AnularParto(int idParto)
        {
            if (idParto <= 0)
            {
                throw new ApplicationException("El IdParto no es válido.");
            }

            return DA_Parto.Instancia.AnularParto(idParto);
        }
        public List<entParto> ListarPartos(bool estado)
        {
            return DA_Parto.Instancia.Listar(estado);
        }
        public entParto BuscarPartoPorId(int idParto)
        {
            if (idParto <= 0)
            {
                return null;
            }
            return DA_Parto.Instancia.BuscarPorId(idParto);
        }
    }
}
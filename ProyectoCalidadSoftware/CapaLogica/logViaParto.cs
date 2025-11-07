using CapaAccesoDatos;
using CapaEntidad;
using System.Collections.Generic;

namespace CapaLogica
{
    public class logViaParto
    {
        #region Singleton
        private static readonly logViaParto _instancia = new logViaParto();
        public static logViaParto Instancia { get { return _instancia; } }
        #endregion

        public List<entViaParto> ListarViasParto()
        {
            return DA_ViaParto.Instancia.Listar();
        }
    }
}
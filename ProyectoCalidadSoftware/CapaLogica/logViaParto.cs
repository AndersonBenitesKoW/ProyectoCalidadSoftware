using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaLogica
{
    public class logViaParto
    {

        #region Singleton
        private static readonly logViaParto UnicaInstancia = new logViaParto();
        public static logViaParto Instancia
        {
            get { return logViaParto.UnicaInstancia; }
        }
        private logViaParto() { }
        #endregion

        // LISTAR
        public List<entViaParto> ListarViaParto()
        {
            return DA_ViaParto.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarViaParto(entViaParto entidad)
        {
            return DA_ViaParto.Instancia.Insertar(entidad);
        }

    }
}

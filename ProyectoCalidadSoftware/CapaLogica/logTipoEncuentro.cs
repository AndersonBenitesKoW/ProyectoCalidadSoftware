using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
namespace CapaLogica
{
    public class logTipoEncuentro
    {
        #region Singleton
        private static readonly logTipoEncuentro UnicaInstancia = new logTipoEncuentro();
        public static logTipoEncuentro Instancia
        {
            get { return logTipoEncuentro.UnicaInstancia; }
        }
        private logTipoEncuentro() { }
        #endregion

        // LISTAR
        public List<entTipoEncuentro> ListarTipoEncuentro()
        {
            return DA_TipoEncuentro.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarTipoEncuentro(entTipoEncuentro entidad)
        {
            return DA_TipoEncuentro.Instancia.Insertar(entidad);
        }

    }
}

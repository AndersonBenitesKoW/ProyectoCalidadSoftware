using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<entEncuentro> ListarEncuentro()
        {
            return DA_Encuentro.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarEncuentro(entEncuentro entidad)
        {
            return DA_Encuentro.Instancia.Insertar(entidad);
        }




    }
}
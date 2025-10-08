using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class logMetodoPF
    {

        #region Singleton
        private static readonly logMetodoPF UnicaInstancia = new logMetodoPF();
        public static logMetodoPF Instancia
        {
            get { return logMetodoPF.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        public List<entMetodoPF> ListarMetodoPF()
        {
            return DA_MetodoPF.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarMetodoPF(entMetodoPF entidad)
        {
            return DA_MetodoPF.Instancia.Insertar(entidad);
        }

    }
}

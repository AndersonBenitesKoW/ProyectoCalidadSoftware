using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
namespace CapaLogica
{
    public class logParto
    {


        #region Singleton
        private static readonly logParto UnicaInstancia = new logParto();
        public static logParto Instancia
        {
            get { return logParto.UnicaInstancia; }
        }
        private logParto() { }
        #endregion

        // LISTAR
        public List<entParto> ListarParto()
        {
            return DA_Parto.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarParto(entParto entidad)
        {
            return DA_Parto.Instancia.Insertar(entidad);
        }

    }
}

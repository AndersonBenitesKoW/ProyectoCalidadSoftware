using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaLogica
{
    public class logBebe
    {
        #region Singleton
        private static readonly logBebe UnicaInstancia = new logBebe();
        public static logBebe Instancia
        {
            get { return logBebe.UnicaInstancia; }
        }
        private logBebe() { }
        #endregion

        // LISTAR
        public List<entBebe> ListarBebe()
        {
            return DA_Bebe.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarBebe(entBebe entidad)
        {
            return DA_Bebe.Instancia.Insertar(entidad);
        }


        public entBebe BuscarBebe(int idBebe)
        {
            return DA_Bebe.Instancia.BuscarBebe(idBebe);
        }

    }
}

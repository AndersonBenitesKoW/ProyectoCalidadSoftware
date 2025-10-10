using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;  
namespace CapaLogica
{
    public class logEstadoCita
    {
        #region Singleton
        private static readonly logEstadoCita UnicaInstancia = new logEstadoCita();
        public static logEstadoCita Instancia
        {
            get { return logEstadoCita.UnicaInstancia; }
        }
        private logEstadoCita() { }
        #endregion

        // LISTAR
        public List<entEstadoCita> ListarEstadoCita()
        {
            return DA_EstadoCita.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarEstadoCita(entEstadoCita entidad)
        {
            return DA_EstadoCita.Instancia.Insertar(entidad);
        }

    }
}

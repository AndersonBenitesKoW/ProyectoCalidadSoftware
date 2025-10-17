using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaLogica
{
    public class logSeguimientoPuerperio
    {
        #region Singleton
        private static readonly logSeguimientoPuerperio UnicaInstancia = new logSeguimientoPuerperio();
        public static logSeguimientoPuerperio Instancia
        {
            get { return logSeguimientoPuerperio.UnicaInstancia; }
        }
        private logSeguimientoPuerperio() { }
        #endregion

        // LISTAR
        public List<entSeguimientoPuerperio> ListarSeguimientoPuerperio()
        {
            return DA_SeguimientoPuerperio.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarSeguimientoPuerperio(entSeguimientoPuerperio entidad)
        {
            return DA_SeguimientoPuerperio.Instancia.Insertar(entidad);
        }
        public bool Inhabilitar(int idPuerperio)
            => DA_SeguimientoPuerperio.Instancia.Inhabilitar(idPuerperio);


    }
}

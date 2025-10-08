using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class logProfesionalSalud
    {

        #region Singleton
        private static readonly logProfesionalSalud UnicaInstancia = new logProfesionalSalud();
        public static logProfesionalSalud Instancia
        {
            get { return logProfesionalSalud.UnicaInstancia; }
        }
        private logProfesionalSalud() { }
        #endregion

        // LISTAR
        public List<entProfesionalSalud> ListarProfesionalSalud()
        {
            return DA_ProfesionalSalud.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarProfesionalSalud(entProfesionalSalud entidad)
        {
            return DA_ProfesionalSalud.Instancia.Insertar(entidad);
        }


    }
}

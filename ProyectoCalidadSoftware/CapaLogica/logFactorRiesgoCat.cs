using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class logFactorRiesgoCat
    {
        #region Singleton
        private static readonly logFactorRiesgoCat UnicaInstancia = new logFactorRiesgoCat();
        public static logFactorRiesgoCat Instancia
        {
            get { return logFactorRiesgoCat.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        public List<entFactorRiesgoCat> ListarFactorRiesgoCat()
        {
            return DA_FactorRiesgoCat.Instancia.Listar();
        }

        // INSERTAR
        public int InsertarFactorRiesgoCat(entFactorRiesgoCat entidad)
        {
            return DA_FactorRiesgoCat.Instancia.Insertar(entidad);
        }

    }
}

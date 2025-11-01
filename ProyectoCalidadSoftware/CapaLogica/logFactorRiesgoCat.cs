using CapaAccesoDatos;
using CapaEntidad;

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
        public bool InsertarFactorRiesgoCat(entFactorRiesgoCat entidad)
        {
            return DA_FactorRiesgoCat.Instancia.Insertar(entidad);
        }

        public entFactorRiesgoCat BuscarFactorRiesgoCat(int id)
            => DA_FactorRiesgoCat.Instancia.BuscarPorId(id);

        public bool ActualizarFactorRiesgoCat(entFactorRiesgoCat entidad)
            => DA_FactorRiesgoCat.Instancia.Actualizar(entidad);

        public bool EliminarFactorRiesgoCat(int id)
            => DA_FactorRiesgoCat.Instancia.Eliminar(id);



    }
}

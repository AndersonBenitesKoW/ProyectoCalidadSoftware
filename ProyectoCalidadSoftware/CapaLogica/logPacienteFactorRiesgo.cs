using CapaAccesoDatos;
using CapaEntidad;
namespace CapaLogica
{
    public class logPacienteFactorRiesgo
    {

        #region Singleton
        private static readonly logPacienteFactorRiesgo UnicaInstancia = new logPacienteFactorRiesgo();
        public static logPacienteFactorRiesgo Instancia
        {
            get { return logPacienteFactorRiesgo.UnicaInstancia; }
        }
        private logPacienteFactorRiesgo() { }
        #endregion

        #region Métodos
        // LISTAR
        public List<entPacienteFactorRiesgo> ListarPacienteFactorRiesgo()
        {
            return DA_PacienteFactorRiesgo.Instancia.Listar();
        }

        // INSERTAR
        public bool InsertarPacienteFactorRiesgo(entPacienteFactorRiesgo entidad)
        {
            return DA_PacienteFactorRiesgo.Instancia.Insertar(entidad);
        }
        #endregion


        // NUEVO: Buscar devuelve entidad
        public entPacienteFactorRiesgo BuscarPacienteFactorRiesgo(int id)
            => DA_PacienteFactorRiesgo.Instancia.BuscarPorId(id);

        // NUEVO: Editar con objeto
        public bool EditarPacienteFactorRiesgo(entPacienteFactorRiesgo entidad)
            => DA_PacienteFactorRiesgo.Instancia.Editar(entidad);

        public bool EliminarPacienteFactorRiesgo(int id)
            => DA_PacienteFactorRiesgo.Instancia.Eliminar(id);




    }
}

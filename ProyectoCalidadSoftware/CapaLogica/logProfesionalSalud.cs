using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;  

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
        public List<entProfesionalSalud> ListarProfesionalSalud(bool estado)
        {
            return DA_ProfesionalSalud.Instancia.Listar(estado);
        }

        // INSERTAR
        public int InsertarProfesionalSalud(entProfesionalSalud entidad) 
        {
            try
            {
                return DA_ProfesionalSalud.Instancia.Insertar(entidad);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar insertar el profesional de salud. " + ex.Message);
            }
        }
        public entProfesionalSalud BuscarProfesionalSaludPorId(int id)
        {
            try
            {
                return DA_ProfesionalSalud.Instancia.BuscarPorId(id);
            }
            catch (Exception ex)
            {
                // Manejo de error
                throw new Exception("Error al buscar el profesional: " + ex.Message);
            }
        }
        public bool EditarProfesionalSalud(entProfesionalSalud entidad)
        {
            try
            {

                return DA_ProfesionalSalud.Instancia.Editar(entidad);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el profesional: " + ex.Message);
            }
        }


    }
}

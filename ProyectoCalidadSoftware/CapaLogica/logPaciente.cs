using CapaAccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
namespace CapaLogica
{
      public class logPaciente
    {

        #region Singleton
        private static readonly logPaciente UnicaInstancia = new logPaciente();
        public static logPaciente Instancia
        {
            get { return logPaciente.UnicaInstancia; }
        }
        private logPaciente() { }
        #endregion

        // LISTAR
        public List<entPaciente> ListarPaciente()
        {
            return DA_Paciente.Instancia.Listar();
        }

        // LISTAR PACIENTES ACTIVOS (alias para compatibilidad)
        public List<entPaciente> ListarPacientesActivos()
        {
            return ListarPaciente().Where(p => p.Estado).ToList();
        }
// INSERTAR
public bool InsertarPaciente(entPaciente entidad)
{
    return DA_Paciente.Instancia.Insertar(entidad);
}

// BUSCAR
public entPaciente BuscarPaciente(int id)
{
    var dt = DA_Paciente.Instancia.BuscarPorId(id);
    if (dt.Rows.Count > 0)
    {
        var row = dt.Rows[0];
        return new entPaciente
        {
            IdPaciente = Convert.ToInt32(row["IdPaciente"]),
            IdUsuario = row["IdUsuario"] != DBNull.Value ? (int?)Convert.ToInt32(row["IdUsuario"]) : null,
            Nombres = row["Nombres"].ToString(),
            Apellidos = row["Apellidos"].ToString(),
            DNI = row["DNI"].ToString(),
            FechaNacimiento = row["FechaNacimiento"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["FechaNacimiento"]) : null,
            Estado = Convert.ToBoolean(row["Estado"])
        };
    }
    return null;
}

// ACTUALIZAR
public bool ActualizarPaciente(entPaciente entidad)
{
    return DA_Paciente.Instancia.Editar(entidad.IdPaciente, entidad.Nombres, entidad.Apellidos,
                                       entidad.FechaNacimiento.Value, entidad.DNI, entidad.Estado);
}

// INHABILITAR
public bool InhabilitarPaciente(int id)
{
    return DA_Paciente.Instancia.Eliminar(id);
}


}
}

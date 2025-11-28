using System;

namespace CapaEntidad
{
    public class entEncuentro
    {
        public int IdEncuentro { get; set; }
        public int IdEmbarazo { get; set; }
        public int? IdProfesional { get; set; }
        public short IdTipoEncuentro { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime? FechaHoraFin { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Notas { get; set; }

        // Propiedades de JOINs (para vistas)
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreProfesional { get; set; } = string.Empty;
        public string TipoEncuentroDesc { get; set; } = string.Empty;
    }
}
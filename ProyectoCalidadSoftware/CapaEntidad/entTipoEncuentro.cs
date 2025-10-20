using System;

namespace CapaEntidad
{
    public class entTipoEncuentro
    {
        public short IdTipoEncuentro { get; set; }
        public string Codigo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}
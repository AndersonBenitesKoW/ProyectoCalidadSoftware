namespace CapaEntidad
{
    public class entAntecedenteObstetrico
    {
        public int IdAntecedente { get; set; }
        public int IdPaciente { get; set; }
        public short? Menarquia { get; set; }
        public short? CicloDias { get; set; }
        public short? Gestas { get; set; }
        public short? Partos { get; set; }
        public short? Abortos { get; set; }
        public string? Observacion { get; set; }
        public bool Estado { get; set; }
    }
}

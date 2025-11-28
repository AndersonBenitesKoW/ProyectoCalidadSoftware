namespace ProyectoCalidadSoftware.Models
{
    public class HomeDashboardViewModel
    {
        public int EmbarazosActivos { get; set; }
        public int ControlesDelMes { get; set; }   // 👈 este nombre exacto
        public int PartosDelMes { get; set; }
        public int PuerperiosActivos { get; set; }
        public string? MensajeBienvenida { get; set; }
        public bool EstaLogueado { get; set; }

        // Datos para gráficos
        public List<int> ControlesPorMes { get; set; } = new List<int>();
        public List<int> PartosPorMes { get; set; } = new List<int>();
        public List<int> DistribucionEstados { get; set; } = new List<int>(); // Activos, En Progreso, Completados

    }
}

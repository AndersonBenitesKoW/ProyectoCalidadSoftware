using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using ProyectoCalidadSoftware.Models;
using System.Diagnostics;

namespace ProyectoCalidadSoftware.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Ojo: estas llamadas dependen de tus nombres reales en CapaLogica.
            // Te los pongo como los que ya vi que usas: logEmbarazo, logControlPrenatal, etc.

            int embarazosActivos = 0;
            int controlesMes = 0;
            int partosMes = 0;
            int puerperiosActivos = 0;

            try
            {
                // 1. Embarazos activos
                var listaEmbarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);
                embarazosActivos = listaEmbarazos.Count;

                // 2. Controles prenatales del mes actual
                var listaControles = logControlPrenatal.Instancia.ListarControlPrenatal();
                controlesMes = listaControles
                    .Where(c => c.Fecha.Month == DateTime.Now.Month && c.Fecha.Year == DateTime.Now.Year)
                    .Count();

                // 3. Partos del mes actual
                var listaPartos = logParto.Instancia.ListarPartos(true); // ya tienes SP de partos activos
                partosMes = listaPartos
                    .Where(p => p.Fecha.Month == DateTime.Now.Month && p.Fecha.Year == DateTime.Now.Year)
                    .Count();

                // 4. Puerperios activos (si tu lógica se llama distinto, cámbialo)
                // si no tienes lógica de puerperio aún, déjalo en 0 o coméntalo
                 var listaPuerperios = logSeguimientoPuerperio.Instancia.ListarSeguimientoPuerperio();
                // puerperiosActivos = listaPuerperios.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo datos para el dashboard de inicio.");
            }

            var vm = new HomeDashboardViewModel
            {
                EmbarazosActivos = embarazosActivos,
                ControlesDelMes = controlesMes,
                PartosDelMes = partosMes,
                PuerperiosActivos = puerperiosActivos,
                MensajeBienvenida = "Sistema de Gestión Obstétrica - Control prenatal, parto y puerperio.",
                // aquí podrías leer de sesión si hay usuario
                EstaLogueado = HttpContext.User?.Identity?.IsAuthenticated ?? false
            };

            return View(vm);


        }
        // =============== LOGIN ===============
        // esto es solo para mostrar la vista de login en el home
        public IActionResult Login()
        {
            return View();
        }

        // =============== PRIVACIDAD (la dejo) ===============
        public IActionResult Privacy()
        {
            return View();
        }

        // =============== ERROR ===============
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}

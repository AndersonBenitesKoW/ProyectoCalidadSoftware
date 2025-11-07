using CapaLogica;
using Microsoft.AspNetCore.Authorization;
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


        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
        public IActionResult Index()
        {
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
                var listaPartos = logParto.Instancia.ListarPartos(true);
                partosMes = listaPartos
                    .Where(p => p.Fecha.Month == DateTime.Now.Month && p.Fecha.Year == DateTime.Now.Year)
                    .Count();

                // 4. Puerperios activos (TU lógica 👇)
                var listaPuerperios = logSeguimientoPuerperio.Instancia.ListarSeguimiento(true);

                // opción A: solo los que están en Estado = 1
                // puerperiosActivos = listaPuerperios.Where(p => p.Estado).Count();

                // opción B: activos y dentro de los últimos 6 meses
                var seisMesesAtras = DateTime.Now.AddMonths(-6);
                puerperiosActivos = listaPuerperios
                    .Where(p => p.Estado && p.Fecha >= seisMesesAtras)
                    .Count();
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
                EstaLogueado = HttpContext.User?.Identity?.IsAuthenticated ?? false
            };

            return View(vm);
        }
        // =============== LOGIN ===============
        

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

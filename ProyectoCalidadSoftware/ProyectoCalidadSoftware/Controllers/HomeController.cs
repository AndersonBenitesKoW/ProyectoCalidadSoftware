using CapaLogica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoCalidadSoftware.Models;
using System.Diagnostics;


namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "ADMIN,PERSONAL_SALUD,SECRETARIA")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

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

                // 4. Puerperios activos (últimos 6 meses)
                var listaPuerperios = logSeguimientoPuerperio.Instancia.ListarSeguimiento(true);
                var seisMesesAtras = DateTime.Now.AddMonths(-6);
                puerperiosActivos = listaPuerperios
                    .Where(p => p.Estado && p.Fecha >= seisMesesAtras)
                    .Count();

                // Datos para gráficos (primer cálculo)
                var controlesPorMes = new List<int>();
                var partosPorMes = new List<int>();
                for (int i = 5; i >= 0; i--)
                {
                    var fecha = DateTime.Now.AddMonths(-i);
                    var controlesCount = listaControles
                        .Where(c => c.Fecha.Month == fecha.Month && c.Fecha.Year == fecha.Year)
                        .Count();
                    var partosCount = listaPartos
                        .Where(p => p.Fecha.Month == fecha.Month && p.Fecha.Year == fecha.Year)
                        .Count();
                    controlesPorMes.Add(controlesCount);
                    partosPorMes.Add(partosCount);
                }
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

            // Datos para gráficos (segunda pasada, como ya lo tienes)
            try
            {
                var listaControles = logControlPrenatal.Instancia.ListarControlPrenatal();
                var listaPartos = logParto.Instancia.ListarPartos(true);
                var listaEmbarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);

                var controlesPorMes = new List<int>();
                var partosPorMes = new List<int>();
                for (int i = 5; i >= 0; i--)
                {
                    var fecha = DateTime.Now.AddMonths(-i);
                    var controlesCount = listaControles
                        .Where(c => c.Fecha.Month == fecha.Month && c.Fecha.Year == fecha.Year)
                        .Count();
                    var partosCount = listaPartos
                        .Where(p => p.Fecha.Month == fecha.Month && p.Fecha.Year == fecha.Year)
                        .Count();
                    controlesPorMes.Add(controlesCount);
                    partosPorMes.Add(partosCount);
                }

                var distribucionEstados = new List<int>();
                var primerTrimestre = listaEmbarazos
                    .Where(e => e.FUR.HasValue && (DateTime.Now - e.FUR.Value).TotalDays / 7 <= 12)
                    .Count();
                var segundoTrimestre = listaEmbarazos
                    .Where(e => e.FUR.HasValue && (DateTime.Now - e.FUR.Value).TotalDays / 7 > 12
                                && (DateTime.Now - e.FUR.Value).TotalDays / 7 <= 26)
                    .Count();
                var tercerTrimestre = listaEmbarazos
                    .Where(e => e.FUR.HasValue && (DateTime.Now - e.FUR.Value).TotalDays / 7 > 26)
                    .Count();

                distribucionEstados.Add(primerTrimestre);
                distribucionEstados.Add(segundoTrimestre);
                distribucionEstados.Add(tercerTrimestre);

                vm.ControlesPorMes = controlesPorMes;
                vm.PartosPorMes = partosPorMes;
                vm.DistribucionEstados = distribucionEstados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo datos para gráficos del dashboard.");
            }

            return View(vm);
        }

        // =============== PRIVACIDAD ===============
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // =============== ERROR ===============
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}

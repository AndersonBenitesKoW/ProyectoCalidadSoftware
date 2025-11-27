using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    //[Route("core/ayudas/resultados")]
    public class ResultadoDiagnosticoController : Controller
    {
        // GET: /core/ayudas/resultados
        [HttpGet]
        public IActionResult Listar(string? dni)
        {
            var lista = logResultadoDiagnostico.Instancia.ListarResultadoDiagnostico();

            if (!string.IsNullOrWhiteSpace(dni))
            {
                // Filtrar por DNI del paciente asociado a la ayuda
                var ayudasPaciente = logAyudaDiagnosticaOrden.Instancia.ListarAyudaDiagnosticaOrden()
                    .Where(a => logPaciente.Instancia.ListarPacientesActivos()
                        .Any(p => p.IdPaciente == a.IdPaciente && p.DNI == dni.Trim()))
                    .Select(a => a.IdAyuda)
                    .ToHashSet();

                lista = lista.Where(r => ayudasPaciente.Contains(r.IdAyuda)).ToList();
            }

            return View(lista);
        }

        // GET: /core/ayudas/resultados/registrar
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Registrar(int? idAyuda)
        {
            var modelo = new entResultadoDiagnostico
            {
                IdAyuda = idAyuda ?? 0,
                FechaResultado = DateTime.Now,
                Critico = false,
                Estado = "ACTIVO"
            };
            return View(modelo);
        }

        // POST: /core/ayudas/resultados/registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Registrar(entResultadoDiagnostico entidad)
        {
            try
            {
                if (entidad.IdAyuda <= 0)
                    ModelState.AddModelError(nameof(entidad.IdAyuda), "Seleccione una ayuda diagnóstica válida.");
                if (entidad.FechaResultado == default)
                    entidad.FechaResultado = DateTime.Now;
                if (string.IsNullOrWhiteSpace(entidad.Estado))
                    entidad.Estado = "ACTIVO";

                if (!ModelState.IsValid) return View(entidad);

                bool ok = logResultadoDiagnostico.Instancia.InsertarResultadoDiagnostico(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo registrar el resultado.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(entidad);
            }
        }

        // GET: /core/ayudas/resultados/modificar/5
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                var entidad = logResultadoDiagnostico.Instancia.BuscarResultadoDiagnostico(id);
                if (entidad == null)
                {
                    TempData["Error"] = "Resultado no encontrado.";
                    return RedirectToAction(nameof(Listar));
                }
                return View(entidad);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar edición: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // POST: /core/ayudas/resultados/modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(entResultadoDiagnostico entidad)
        {
            try
            {
                if (entidad.IdResultado <= 0)
                    ModelState.AddModelError(nameof(entidad.IdResultado), "Identificador inválido.");
                if (entidad.IdAyuda <= 0)
                    ModelState.AddModelError(nameof(entidad.IdAyuda), "Seleccione una ayuda diagnóstica válida.");

                if (entidad.FechaResultado == default)
                    entidad.FechaResultado = DateTime.Now;

                if (!ModelState.IsValid) return View(entidad);

                bool ok = logResultadoDiagnostico.Instancia.ActualizarResultadoDiagnostico(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo actualizar el resultado.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar: " + ex.Message;
                return View(entidad);
            }
        }

        // Alias opcional: /core/ayudas/resultados/actualizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(entResultadoDiagnostico entidad)
        {
            return Modificar(entidad);
        }

        // GET: /core/ayudas/resultados/{id}/anular  -> confirmación
        [HttpGet]
        public IActionResult Anular(int id)
        {
            var entidad = logResultadoDiagnostico.Instancia.BuscarResultadoDiagnostico(id);
            if (entidad == null)
            {
                TempData["Error"] = "Resultado no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            return View(entidad); // Views/ResultadoDiagnostico/Anular.cshtml (modelo: entResultadoDiagnostico)
        }

        // POST: /core/ayudas/resultados/{id}/anular
        [HttpPost]
         // permite usar asp-action="Anular" en el form
        [ValidateAntiForgeryToken]
        public IActionResult AnularConfirmado(int id)
        {
            try
            {
                bool ok = logResultadoDiagnostico.Instancia.AnularResultadoDiagnostico(id);
                TempData[ok ? "Ok" : "Error"] = ok ? "Resultado anulado." : "No se pudo anular el resultado.";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // GET: /ResultadoDiagnostico/BuscarAyudaPorPacienteDNI
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult BuscarAyudaPorPacienteDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return Json(new { success = false, message = "DNI requerido." });
            }

            try
            {
                // Buscar paciente por DNI
                var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.DNI == dni.Trim());
                if (paciente == null)
                {
                    return Json(new { success = false, message = "Paciente no encontrado con ese DNI." });
                }

                // Buscar ayudas diagnósticas para ese paciente
                var ayudas = logAyudaDiagnosticaOrden.Instancia.ListarAyudaDiagnosticaOrden()
                    .Where(a => a.IdPaciente == paciente.IdPaciente && a.Estado == "ACTIVO")
                    .ToList();

                if (!ayudas.Any())
                {
                    return Json(new { success = false, message = "No se encontraron ayudas diagnósticas activas para este paciente." });
                }

                // Crear la lista de ayudas con información básica por ahora
                // La información completa del control prenatal se cargará desde la vista de listado
                var ayudasData = ayudas.Select(a => new
                {
                    id = a.IdAyuda,
                    descripcion = a.Descripcion ?? "Sin descripción",
                    tipoAyuda = a.NombreTipoAyuda,
                    fechaOrden = a.FechaOrden.ToString("dd/MM/yyyy"),
                    urgente = a.Urgente,
                    nombrePaciente = paciente.Nombres + " " + paciente.Apellidos
                }).ToList();

                if (!ayudasData.Any())
                {
                    return Json(new { success = false, message = "No se encontraron ayudas diagnósticas activas para este paciente." });
                }

                return Json(new { success = true, ayudas = ayudasData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al buscar ayudas: " + ex.Message });
            }
        }
    }
}

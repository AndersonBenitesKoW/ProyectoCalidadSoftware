using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class ResultadoDiagnosticoController : Controller
    {

        // GET: /ResultadoDiagnostico/Listar
        public IActionResult Listar()
        {
            var lista = logResultadoDiagnostico.Instancia.ListarResultadoDiagnostico();
            return View(lista);
        }

        // GET: /ResultadoDiagnostico/Registrar
        [HttpGet]
        public IActionResult Registrar(int? idAyuda)
        {
            var modelo = new entResultadoDiagnostico
            {
                IdAyuda = idAyuda ?? 0,
                FechaResultado = DateTime.Now,
                Critico = false,
                Estado = "ACTIVO" // ajusta a tu catálogo
            };
            return View(modelo);
        }

        // POST: /ResultadoDiagnostico/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: /ResultadoDiagnostico/Modificar/5
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                var entidad = logResultadoDiagnostico.Instancia.BuscarResultadoDiagnostico(id);
                if (entidad == null) return NotFound();
                return View(entidad);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar edición: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // POST: /ResultadoDiagnostico/Modificar  (update con entidad)
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

        // Alias: /ResultadoDiagnostico/Actualizar (si quieres tener ambas rutas)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(entResultadoDiagnostico entidad)
        {
            return Modificar(entidad);
        }

        // POST: /ResultadoDiagnostico/Anular/5  (soft delete por estado)
        [HttpPost]
        public IActionResult Anular(int id)
        {
            try
            {
                bool ok = logResultadoDiagnostico.Instancia.AnularResultadoDiagnostico(id);
                if (ok) return RedirectToAction(nameof(Listar));

                TempData["Error"] = "No se pudo anular el resultado.";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }



    }
}

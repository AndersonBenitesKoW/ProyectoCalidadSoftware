using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    [Route("core/puerperio")]
    public class SeguimientoPuerperioController : Controller
    {
        // GET: /core/puerperio
        [HttpGet("")]
        public IActionResult Listar()
        {
            var lista = logSeguimientoPuerperio.Instancia.ListarSeguimientoPuerperio();
            ViewBag.Lista = lista;
            return View(lista); // Views/SeguimientoPuerperio/Listar.cshtml
        }

        // GET: /core/puerperio/registrar
        [HttpGet("registrar")]
        public IActionResult Registrar(int? idEmbarazo)
        {
            var modelo = new entSeguimientoPuerperio
            {
                IdEmbarazo = idEmbarazo ?? 0,
                Fecha = DateTime.Now,
                Estado = true
            };
            return View(modelo); // Views/SeguimientoPuerperio/Registrar.cshtml
        }

        // POST: /core/puerperio/registrar
        [HttpPost("registrar")]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(entSeguimientoPuerperio entidad)
        {
            try
            {
                if (entidad.IdEmbarazo <= 0)
                    ModelState.AddModelError(nameof(entidad.IdEmbarazo), "Debe seleccionar un embarazo válido.");
                if (entidad.Fecha == default)
                    ModelState.AddModelError(nameof(entidad.Fecha), "Debe ingresar una fecha válida.");

                if (!ModelState.IsValid) return View(entidad);

                entidad.Estado = true;

                bool ok = logSeguimientoPuerperio.Instancia.InsertarSeguimientoPuerperio(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo registrar el seguimiento de puerperio.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(entidad);
            }
        }

        // GET: /core/puerperio/{id}/inhabilitar  -> confirmación
        [HttpGet("{id:int}/inhabilitar")]
        public IActionResult Inhabilitar(int id)
        {
            var entidad = logSeguimientoPuerperio.Instancia
                .ListarSeguimientoPuerperio()
                .FirstOrDefault(x => x.IdSeguimientoPuerperio == id);

            if (entidad == null)
            {
                TempData["Error"] = "Seguimiento no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            return View(entidad); // Views/SeguimientoPuerperio/Inhabilitar.cshtml
        }

        // POST: /core/puerperio/{id}/inhabilitar
        [HttpPost("{id:int}/inhabilitar")]
        [ActionName("Inhabilitar")]
        [ValidateAntiForgeryToken]
        public IActionResult InhabilitarConfirmado(int id)
        {
            try
            {
                bool ok = logSeguimientoPuerperio.Instancia.Inhabilitar(id);
                TempData[ok ? "Ok" : "Error"] = ok
                    ? "Seguimiento inhabilitado correctamente."
                    : "No se pudo inhabilitar el seguimiento.";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al inhabilitar: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }
    }
}

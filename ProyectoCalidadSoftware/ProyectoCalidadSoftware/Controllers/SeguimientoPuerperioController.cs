using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class SeguimientoPuerperioController : Controller
    {
        public IActionResult Listar()
        {
            var lista = logSeguimientoPuerperio.Instancia.ListarSeguimientoPuerperio();
            ViewBag.Lista = lista;
            return View(lista);
        }

        // GET: /SeguimientoPuerperio/Registrar
        [HttpGet]
        public IActionResult Registrar(int? idEmbarazo)
        {
            var modelo = new entSeguimientoPuerperio
            {
                IdEmbarazo = idEmbarazo ?? 0,
                Fecha = DateTime.Now,
                Estado = true
            };
            return View(modelo);
        }

        // POST: /SeguimientoPuerperio/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(entSeguimientoPuerperio entidad)
        {
            try
            {
                if (entidad.IdEmbarazo <= 0)
                    ModelState.AddModelError(nameof(entidad.IdEmbarazo), "Debe seleccionar un embarazo válido.");
                if (entidad.Fecha == default)
                    ModelState.AddModelError(nameof(entidad.Fecha), "Debe ingresar una fecha válida.");

                if (!ModelState.IsValid)
                    return View(entidad);

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

        // POST: /SeguimientoPuerperio/Inhabilitar/5
        [HttpPost]
        public IActionResult Inhabilitar(int id)
        {
            try
            {
                bool ok = logSeguimientoPuerperio.Instancia.Inhabilitar(id);
                if (ok) return RedirectToAction(nameof(Listar));

                TempData["Error"] = "No se pudo inhabilitar el seguimiento.";
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

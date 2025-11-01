using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    //[Route("core/controles")]
    public class ControlPrenatalController : Controller
    {
        // GET: /core/controles
        [HttpGet]
        public IActionResult Listar()
        {
            var lista = logControlPrenatal.Instancia.ListarControlPrenatal();
            ViewBag.Lista = lista;
            return View(lista); // Views/ControlPrenatal/Listar.cshtml
        }

        // GET: /core/controles/registrar
        [HttpGet]
        public IActionResult Registrar(int? idEmbarazo)
        {
            var modelo = new entControlPrenatal
            {
                IdEmbarazo = idEmbarazo ?? 0,
                Fecha = DateTime.Now,
                Estado = true
            };
            return View(modelo); // Views/ControlPrenatal/Registrar.cshtml
        }

        // POST: /core/controles/registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(entControlPrenatal control)
        {
            try
            {
                if (control.IdEmbarazo <= 0)
                    ModelState.AddModelError(nameof(control.IdEmbarazo), "Debe seleccionar un embarazo válido.");
                if (control.Fecha == default)
                    ModelState.AddModelError(nameof(control.Fecha), "Debe ingresar una fecha válida.");

                if (!ModelState.IsValid)
                    return View(control);

                control.Estado = true;

                bool registrado = logControlPrenatal.Instancia.InsertarControlPrenatal(control);

                if (registrado)
                    return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo registrar el control prenatal.";
                return View(control);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(control);
            }
        }

        // GET: /core/controles/{id}/inhabilitar  -> página de confirmación
        [HttpGet]
        public IActionResult Inhabilitar(int id)
        {
            var control = logControlPrenatal.Instancia
                            .ListarControlPrenatal()
                            .FirstOrDefault(c => c.IdControlPrenatal == id);

            if (control == null)
            {
                TempData["Error"] = "Control prenatal no encontrado.";
                return RedirectToAction(nameof(Listar));
            }

            return View(control); // Views/ControlPrenatal/Inhabilitar.cshtml (modelo: entControlPrenatal)
        }

        // POST: /core/controles/{id}/inhabilitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InhabilitarConfirmado(int id)
        {
            try
            {
                bool resultado = logControlPrenatal.Instancia.Inhabilitar(id);

                TempData[resultado ? "Ok" : "Error"] = resultado
                    ? "Control prenatal inhabilitado correctamente."
                    : "No se pudo inhabilitar el control prenatal.";

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

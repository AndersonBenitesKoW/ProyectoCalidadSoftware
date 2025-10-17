using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class ControlPrenatalController : Controller
    {
        // GET: /ControlPrenatal/Listar
        public IActionResult Listar()
        {
            var lista = logControlPrenatal.Instancia.ListarControlPrenatal();
            ViewBag.Lista = lista;
            return View(lista);
        }

        // GET: /ControlPrenatal/Registrar
        [HttpGet]
        public IActionResult Registrar(int? idEmbarazo)
        {
            var modelo = new entControlPrenatal
            {
                IdEmbarazo = idEmbarazo ?? 0,
                Fecha = DateTime.Now,
                Estado = true
            };
            return View(modelo);
        }

        // POST: /ControlPrenatal/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(entControlPrenatal control)
        {
            try
            {
                // Validación básica
                if (control.IdEmbarazo <= 0)
                    ModelState.AddModelError(nameof(control.IdEmbarazo), "Debe seleccionar un embarazo válido.");

                if (control.Fecha == default)
                    ModelState.AddModelError(nameof(control.Fecha), "Debe ingresar una fecha válida.");

                if (!ModelState.IsValid)
                    return View(control);

                // Estado activo por defecto
                control.Estado = true;

                bool registrado = logControlPrenatal.Instancia.InsertarControlPrenatal(control);

                if (registrado)
                    return RedirectToAction(nameof(Listar));
                else
                {
                    ViewBag.Error = "No se pudo registrar el control prenatal.";
                    return View(control);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(control);
            }
        }

        // POST: /ControlPrenatal/Inhabilitar/5
        [HttpPost]
        public IActionResult Inhabilitar(int id)
        {
            try
            {
                bool resultado = logControlPrenatal.Instancia.Inhabilitar(id);

                if (resultado)
                    return RedirectToAction(nameof(Listar));
                else
                {
                    TempData["Error"] = "No se pudo inhabilitar el control prenatal.";
                    return RedirectToAction(nameof(Listar));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al inhabilitar: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }







    }
}

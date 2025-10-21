using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class CitaController : Controller
    {
        // GET: /Cita/Listar
        public IActionResult Listar()
        {
            var lista = logCita.Instancia.ListarCita();
            return View(lista);
        }

        // GET: /Cita/Insertar
        [HttpGet]
        public IActionResult Insertar()
        {
            return View(new entCita());
        }

        // POST: /Cita/Insertar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(entCita entidad)
        {
            try
            {
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");
                if (entidad.FechaCita == default(DateTime))
                    ModelState.AddModelError(nameof(entidad.FechaCita), "La fecha de la cita es obligatoria.");
                if (entidad.IdEstadoCita <= 0)
                    ModelState.AddModelError(nameof(entidad.IdEstadoCita), "Seleccione un estado válido.");

                if (!ModelState.IsValid) return View(entidad);

                bool ok = logCita.Instancia.InsertarCita(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo insertar la cita.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al insertar: " + ex.Message;
                return View(entidad);
            }
        }

        // POST: /Cita/Anular/5
        [HttpPost]
        public IActionResult Anular(int id)
        {
            try
            {
                bool ok = logCita.Instancia.AnularCita(id);
                if (ok) return RedirectToAction(nameof(Listar));

                TempData["Error"] = "No se pudo anular la cita.";
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
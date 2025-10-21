using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    [Route("core/citas")]
    public class CitaController : Controller
    {
        // GET: /core/citas
        [HttpGet("")]
        public IActionResult Listar()
        {
            var lista = logCita.Instancia.ListarCita();
            return View(lista);                       // Views/Cita/Listar.cshtml
        }

        // GET: /core/citas/insertar
        [HttpGet("insertar")]
        public IActionResult Insertar()
        {
            return View(new entCita());              // Views/Cita/Insertar.cshtml
        }

        // POST: /core/citas/insertar
        [HttpPost("insertar")]
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
                if (ok)
                {
                    TempData["Ok"] = "Cita registrada correctamente.";
                    return RedirectToAction(nameof(Listar));
                }

                ViewBag.Error = "No se pudo insertar la cita.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al insertar: " + ex.Message;
                return View(entidad);
            }
        }

        // GET: /core/citas/{id}/anular  -> página de confirmación
        [HttpGet("{id:int}/anular")]
        public IActionResult Anular(int id)
        {
            // Trae la cita para mostrar sus detalles en la vista (tu modelo es entCita)
            var cita = logCita.Instancia.ListarCita().FirstOrDefault(c => c.IdCita == id);
            if (cita == null)
            {
                TempData["Error"] = "Cita no encontrada.";
                return RedirectToAction(nameof(Listar));
            }
            return View(cita);                        // Views/Cita/Anular.cshtml (modelo: entCita)
        }

        // POST: /core/citas/{id}/anular
        [HttpPost("{id:int}/anular")]
        [ActionName("Anular")]                       // permite usar asp-action="Anular" en el form
        [ValidateAntiForgeryToken]
        public IActionResult AnularConfirmado(int id)
        {
            try
            {
                bool ok = logCita.Instancia.AnularCita(id);
                TempData[ok ? "Ok" : "Error"] = ok ? "Cita anulada." : "No se pudo anular la cita.";
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

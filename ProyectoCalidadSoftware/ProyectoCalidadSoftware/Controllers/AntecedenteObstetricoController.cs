using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class AntecedenteObstetricoController : Controller
    {

        // GET: /AntecedenteObstetrico/Listar
        public IActionResult Listar()
        {
            var lista = logAntecedenteObstetrico.Instancia.ListarAntecedenteObstetrico();
            return View(lista);
        }

        // GET: /AntecedenteObstetrico/Registrar
        [HttpGet]
        public IActionResult Registrar(int? idPaciente)
        {
            var modelo = new entAntecedenteObstetrico
            {
                IdPaciente = idPaciente ?? 0,
                Estado = true
            };
            return View(modelo);
        }

        // POST: /AntecedenteObstetrico/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(entAntecedenteObstetrico entidad)
        {
            try
            {
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");

                if (!ModelState.IsValid) return View(entidad);

                entidad.Estado = true;

                bool ok = logAntecedenteObstetrico.Instancia.InsertarAntecedenteObstetrico(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo registrar el antecedente obstétrico.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(entidad);
            }
        }

        // GET: /AntecedenteObstetrico/Modificar/5
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                var entidad = logAntecedenteObstetrico.Instancia.BuscarAntecedenteObstetrico(id);
                if (entidad == null) return NotFound();

                return View(entidad);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar edición: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // POST: /AntecedenteObstetrico/Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(entAntecedenteObstetrico entidad)
        {
            try
            {
                if (entidad.IdAntecedente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdAntecedente), "Identificador inválido.");
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");

                if (!ModelState.IsValid) return View(entidad);

                bool ok = logAntecedenteObstetrico.Instancia.ActualizarAntecedenteObstetrico(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo actualizar el antecedente obstétrico.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar: " + ex.Message;
                return View(entidad);
            }
        }

        // Alias: /AntecedenteObstetrico/Actualizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(entAntecedenteObstetrico entidad) => Modificar(entidad);

        // POST: /AntecedenteObstetrico/Anular/5
        [HttpPost]
        public IActionResult Anular(int id)
        {
            try
            {
                bool ok = logAntecedenteObstetrico.Instancia.AnularAntecedenteObstetrico(id);
                if (ok) return RedirectToAction(nameof(Listar));

                TempData["Error"] = "No se pudo anular el antecedente.";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // (Opcional) POST: /AntecedenteObstetrico/Eliminar/5 — si quieres borrado físico
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                bool ok = logAntecedenteObstetrico.Instancia.EliminarAntecedenteObstetrico(id);
                if (ok) return RedirectToAction(nameof(Listar));
                TempData["Error"] = "No se pudo eliminar el antecedente.";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }



    }
}

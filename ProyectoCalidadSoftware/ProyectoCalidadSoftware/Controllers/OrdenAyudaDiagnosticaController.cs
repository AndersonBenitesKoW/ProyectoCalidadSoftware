using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    //[Route("core/ayudas")]
    public class OrdenAyudaDiagnosticaController : Controller
    {
        // GET: /core/ayudas
        [HttpGet]
        public IActionResult Listar()
        {
            var lista = logAyudaDiagnosticaOrden.Instancia.ListarAyudaDiagnosticaOrden();
            return View(lista);
        }

        // GET: /core/ayudas/insertar
        [HttpGet]
        public IActionResult Insertar()
        {
            return View(new entAyudaDiagnosticaOrden());
        }

        // POST: /core/ayudas/insertar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(entAyudaDiagnosticaOrden entidad)
        {
            try
            {
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");
                if (entidad.IdTipoAyuda == null || entidad.IdTipoAyuda <= 0)
                    ModelState.AddModelError(nameof(entidad.IdTipoAyuda), "Seleccione un tipo de ayuda válido.");
                if (string.IsNullOrWhiteSpace(entidad.Descripcion))
                    ModelState.AddModelError(nameof(entidad.Descripcion), "La descripción es obligatoria.");
                if (entidad.FechaOrden == default(DateTime))
                    ModelState.AddModelError(nameof(entidad.FechaOrden), "La fecha de orden es obligatoria.");

                if (!ModelState.IsValid) return View(entidad);

                entidad.Estado = "Activo";

                bool ok = logAyudaDiagnosticaOrden.Instancia.InsertarAyudaDiagnosticaOrden(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo insertar la orden de ayuda diagnóstica.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al insertar: " + ex.Message;
                return View(entidad);
            }
        }
    }
}

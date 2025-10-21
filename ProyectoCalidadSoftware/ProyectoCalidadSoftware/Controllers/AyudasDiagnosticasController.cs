using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class AyudasDiagnosticasController : Controller
    {
        // GET: /AyudaDiagnosticaOrden/Listar
        public IActionResult Listar()
        {
            var lista = logAyudaDiagnosticaOrden.Instancia.ListarAyudaDiagnosticaOrden();
            ViewBag.Lista = lista;
            return View(lista);
        }

        // GET: /AyudaDiagnosticaOrden/Registrar
        [HttpGet]
        public IActionResult Registrar(int? idPaciente, int? idEmbarazo)
        {
            var modelo = new entAyudaDiagnosticaOrden
            {
                IdPaciente = idPaciente ?? 0,
                IdEmbarazo = idEmbarazo,
                FechaOrden = DateTime.Now,
                Urgente = false,
                Estado = "PENDIENTE"   // ajusta al catálogo que uses
            };
            return View(modelo);
        }

        // POST: /AyudaDiagnosticaOrden/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(entAyudaDiagnosticaOrden entidad)
        {
            try
            {
                // Validaciones mínimas
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");
                if (entidad.FechaOrden == default)
                    ModelState.AddModelError(nameof(entidad.FechaOrden), "Ingrese una fecha válida.");
                if (string.IsNullOrWhiteSpace(entidad.Estado))
                    ModelState.AddModelError(nameof(entidad.Estado), "El estado es obligatorio.");

                if (!ModelState.IsValid)
                    return View(entidad);

                bool ok = logAyudaDiagnosticaOrden.Instancia.InsertarAyudaDiagnosticaOrden(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo registrar la ayuda diagnóstica.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(entidad);
            }
        }

        // POST: /AyudaDiagnosticaOrden/Inhabilitar/5
        // Cambia Estado -> "INACTIVO" (soft-delete)
        [HttpPost]
        public IActionResult Inhabilitar(int id)
        {
            try
            {
                bool ok = logAyudaDiagnosticaOrden.Instancia.Inhabilitar(id);
                if (ok) return RedirectToAction(nameof(Listar));

                TempData["Error"] = "No se pudo inhabilitar la orden.";
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

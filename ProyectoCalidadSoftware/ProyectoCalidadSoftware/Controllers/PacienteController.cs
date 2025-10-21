using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class PacienteController : Controller
    {
        // GET: /Paciente/Listar
        public IActionResult Listar()
        {
            var lista = logPaciente.Instancia.ListarPaciente();
            return View(lista);
        }

        // GET: /Paciente/Insertar
        [HttpGet]
        public IActionResult Insertar()
        {
            return View(new entPaciente());
        }

        // POST: /Paciente/Insertar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(entPaciente entidad)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entidad.Nombres))
                    ModelState.AddModelError(nameof(entidad.Nombres), "Los nombres son obligatorios.");
                if (string.IsNullOrWhiteSpace(entidad.Apellidos))
                    ModelState.AddModelError(nameof(entidad.Apellidos), "Los apellidos son obligatorios.");
                if (string.IsNullOrWhiteSpace(entidad.DNI))
                    ModelState.AddModelError(nameof(entidad.DNI), "El DNI es obligatorio.");
                if (entidad.FechaNacimiento == null)
                    ModelState.AddModelError(nameof(entidad.FechaNacimiento), "La fecha de nacimiento es obligatoria.");

                if (!ModelState.IsValid) return View(entidad);

                entidad.Estado = true;

                bool ok = logPaciente.Instancia.InsertarPaciente(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo registrar el paciente.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(entidad);
            }
        }

        // GET: /Paciente/Modificar/5
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                var entidad = logPaciente.Instancia.BuscarPaciente(id);
                if (entidad == null) return NotFound();

                return View(entidad);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar edición: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // POST: /Paciente/Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(entPaciente entidad)
        {
            try
            {
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Identificador inválido.");
                if (string.IsNullOrWhiteSpace(entidad.Nombres))
                    ModelState.AddModelError(nameof(entidad.Nombres), "Los nombres son obligatorios.");
                if (string.IsNullOrWhiteSpace(entidad.Apellidos))
                    ModelState.AddModelError(nameof(entidad.Apellidos), "Los apellidos son obligatorios.");
                if (string.IsNullOrWhiteSpace(entidad.DNI))
                    ModelState.AddModelError(nameof(entidad.DNI), "El DNI es obligatorio.");
                if (entidad.FechaNacimiento == null)
                    ModelState.AddModelError(nameof(entidad.FechaNacimiento), "La fecha de nacimiento es obligatoria.");

                if (!ModelState.IsValid) return View(entidad);

                bool ok = logPaciente.Instancia.ActualizarPaciente(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo actualizar el paciente.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar: " + ex.Message;
                return View(entidad);
            }
        }
    }
}
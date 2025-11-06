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
        // POST: /Paciente/Insertar
        // POST: /Paciente/Insertar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(entPaciente entidad)
        {
            try
            {
                // --- Validaciones (Actualizadas) ---
                if (string.IsNullOrWhiteSpace(entidad.Nombres))
                    ModelState.AddModelError(nameof(entidad.Nombres), "Los nombres son obligatorios.");
                if (string.IsNullOrWhiteSpace(entidad.Apellidos))
                    ModelState.AddModelError(nameof(entidad.Apellidos), "Los apellidos son obligatorios.");

                // ==== MODIFICACIÓN AQUÍ ====
                // Ahora el DNI es obligatorio
                if (string.IsNullOrWhiteSpace(entidad.DNI))
                    ModelState.AddModelError(nameof(entidad.DNI), "El DNI es obligatorio.");
                // ===========================

                // (Opcional: Fecha de Nacimiento sigue siendo opcional)
                // if (entidad.FechaNacimiento == null)
                //    ModelState.AddModelError(nameof(entidad.FechaNacimiento), "La fecha de nacimiento es obligatoria.");

                // --- NUEVAS VALIDACIONES ---
                if (string.IsNullOrWhiteSpace(entidad.EmailPrincipal))
                    ModelState.AddModelError(nameof(entidad.EmailPrincipal), "El Email es obligatorio.");
                if (string.IsNullOrWhiteSpace(entidad.TelefonoPrincipal))
                    ModelState.AddModelError(nameof(entidad.TelefonoPrincipal), "El Teléfono es obligatorio.");

                if (!ModelState.IsValid) return View(entidad);

                // El SP ya asigna 'Estado = 1' por defecto, no necesitamos esto.
                // entidad.Estado = true; 

                // Asumimos que logPaciente.InsertarPaciente(entidad)
                // llama a DA_Paciente.Insertar(entidad)
                bool ok = logPaciente.Instancia.InsertarPaciente(entidad);

                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo registrar el paciente. Verifique los datos.";
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

        // Alias: /Paciente/Actualizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(entPaciente entidad) => Modificar(entidad);

        // POST: /Paciente/Inhabilitar/5
        [HttpGet]
        public IActionResult Inhabilitar(int id)
        {
            try
            {
                // Buscamos al paciente para mostrar sus detalles
                var entidad = logPaciente.Instancia.BuscarPaciente(id);
                if (entidad == null) return NotFound();

                // Devolvemos la vista "Inhabilitar.cshtml" con los datos
                return View("Inhabilitar", entidad);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la página: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }
        // ==== FIN DEL CÓDIGO NUEVO ====

        // POST: /Paciente/Inhabilitar/5
        // (Este método ya lo tenías, procesa el borrado)
        [HttpPost]
        [ValidateAntiForgeryToken] // <-- ¡Importante! Asegúrate de que tu form lo tenga
        public IActionResult Inhabilitar(entPaciente entidad) // Recibimos la entidad para obtener el ID
        {
            try
            {
                // Usamos el IdPaciente que viene del <input type="hidden">
                bool ok = logPaciente.Instancia.InhabilitarPaciente(entidad.IdPaciente);
                if (ok)
                {
                    TempData["Success"] = "Paciente inhabilitado correctamente.";
                    return RedirectToAction(nameof(Listar));
                }

                TempData["Error"] = "No se pudo inhabilitar el paciente.";
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
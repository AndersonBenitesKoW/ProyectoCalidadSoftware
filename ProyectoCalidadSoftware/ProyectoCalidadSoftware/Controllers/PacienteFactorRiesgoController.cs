using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class PacienteFactorRiesgoController : Controller
    {// GET: /PacienteFactorRiesgo/Listar
        public IActionResult Listar()
        {
            var lista = logPacienteFactorRiesgo.Instancia.ListarPacienteFactorRiesgo();
            ViewBag.Lista = lista;
            return View(lista);
        }

        // GET: /PacienteFactorRiesgo/Agregar
        [HttpGet]
        public IActionResult Agregar(int? idPaciente)
        {
            var modelo = new entPacienteFactorRiesgo
            {
                IdPaciente = idPaciente ?? 0,
                FechaRegistro = DateTime.Now,
                Estado = true
            };
            return View(modelo);
        }

        // POST: /PacienteFactorRiesgo/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Agregar(entPacienteFactorRiesgo entidad)
        {
            try
            {
                // Validaciones mínimas
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");

                if (entidad.IdFactorCat <= 0)
                    ModelState.AddModelError(nameof(entidad.IdFactorCat), "Seleccione una categoría de factor válida.");

                // Fecha por defecto si no viene seteada
                if (entidad.FechaRegistro == default)
                    entidad.FechaRegistro = DateTime.Now;

                if (!ModelState.IsValid)
                    return View(entidad);

                // Estado activo por defecto
                entidad.Estado = true;

                bool ok = logPacienteFactorRiesgo.Instancia.InsertarPacienteFactorRiesgo(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo agregar el factor de riesgo.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al agregar: " + ex.Message;
                return View(entidad);
            }
        }



        // GET: /PacienteFactorRiesgo/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                var entidad = logPacienteFactorRiesgo.Instancia.BuscarPacienteFactorRiesgo(id);
                if (entidad == null) return NotFound();

                return View(entidad); // Vista: Editar.cshtml
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar edición: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // POST: /PacienteFactorRiesgo/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(entPacienteFactorRiesgo entidad)
        {
            try
            {
                if (entidad.IdPacienteFactor <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPacienteFactor), "Identificador inválido.");
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");
                if (entidad.IdFactorCat <= 0)
                    ModelState.AddModelError(nameof(entidad.IdFactorCat), "Seleccione una categoría de factor válida.");

                // Si no te interesa que editen la fecha, puedes mantener la anterior
                if (entidad.FechaRegistro == default)
                    entidad.FechaRegistro = DateTime.Now;

                if (!ModelState.IsValid) return View(entidad);

                bool ok = logPacienteFactorRiesgo.Instancia.EditarPacienteFactorRiesgo(entidad);
                if (ok) return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo actualizar el factor de riesgo.";
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar: " + ex.Message;
                return View(entidad);
            }
        }

        // POST: /PacienteFactorRiesgo/Eliminar/5
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                bool ok = logPacienteFactorRiesgo.Instancia.EliminarPacienteFactorRiesgo(id);
                if (ok) return RedirectToAction(nameof(Listar));

                TempData["Error"] = "No se pudo eliminar el registro.";
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


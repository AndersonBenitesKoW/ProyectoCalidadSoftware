using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "ADMIN,PERSONAL_SALUD,SECRETARIA")]
    public class AntecedenteObstetricoController : Controller
    {

        public IActionResult Listar(string? dni = null)
        {
            var lista = logAntecedenteObstetrico.Instancia.ListarAntecedenteObstetrico();

            if (!string.IsNullOrEmpty(dni))
            {
                var pacientesFiltrados = logPaciente.Instancia
                    .ListarPacientesActivos()
                    .Where(p => p.DNI == dni)
                    .Select(p => p.IdPaciente)
                    .ToList();

                lista = lista.Where(a => pacientesFiltrados.Contains(a.IdPaciente)).ToList();
                ViewBag.DNIFiltro = dni;
            }

            return View(lista);
        }

        // GET: Registrar
        [HttpGet]
        public IActionResult Registrar(int? idPaciente)
        {
            var modelo = new entAntecedenteObstetrico
            {
                IdPaciente = idPaciente ?? 0,
                Estado = true
            };
            CargarViewBags(modelo);
            return View(modelo);
        }

        // POST: Registrar
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

        // GET: Detalles
        [HttpGet]
        public IActionResult Detalles(int id)
        {
            try
            {
                var entidad = logAntecedenteObstetrico.Instancia.BuscarAntecedenteObstetrico(id);
                if (entidad == null) return NotFound();

                var paciente = logPaciente.Instancia.BuscarPaciente(entidad.IdPaciente);
                ViewBag.NombrePaciente = paciente != null ? $"{paciente.Nombres} {paciente.Apellidos}" : "Paciente no encontrado";

                return View(entidad);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar detalles: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // GET: Modificar
        [HttpGet]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
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

        // POST: Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
        public IActionResult Actualizar(entAntecedenteObstetrico entidad) => Modificar(entidad);

        // POST: Anular — SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
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

        // POST: Eliminar — SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
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

        private void CargarViewBags(entAntecedenteObstetrico? entidad)
        {
            try
            {
                var pacientes = logPaciente.Instancia.ListarPacientesActivos();
                ViewBag.PacientesModal = pacientes;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
            }
        }
    }
}


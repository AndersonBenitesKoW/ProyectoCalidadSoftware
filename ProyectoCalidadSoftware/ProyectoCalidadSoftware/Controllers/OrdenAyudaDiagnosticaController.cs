using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var modelo = new entAyudaDiagnosticaOrden
            {
                FechaOrden = DateTime.Now,
                Urgente = false,
                Estado = "Activo"
            };

            CargarViewBags(modelo);
            return View(modelo);
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

                bool ok =Convert.ToBoolean(logAyudaDiagnosticaOrden.Instancia.InsertarAyudaDiagnosticaOrden(entidad));
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
        // --- Listas para combos y modals ---
        private void CargarViewBags(entAyudaDiagnosticaOrden? orden)
        {
            try
            {
                // PACIENTES
                var pacientes = logPaciente.Instancia.ListarPacientesActivos();
                ViewBag.PacientesModal = pacientes;

                ViewBag.ListaPacientes = new SelectList(
                    pacientes.Select(p => new
                    {
                        p.IdPaciente,
                        Nombre = $"{p.Nombres} {p.Apellidos} (DNI: {p.DNI})"
                    }),
                    "IdPaciente",
                    "Nombre",
                    orden?.IdPaciente
                );

                // PROFESIONALES
                var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);
                ViewBag.ProfesionalesModal = profesionales;

                ViewBag.ListaProfesionales = new SelectList(
                    profesionales.Select(p => new
                    {
                        p.IdProfesional,
                        Nombre = $"{p.Nombres} {p.Apellidos} (CMP: {p.CMP})"
                    }),
                    "IdProfesional",
                    "Nombre",
                    orden?.IdProfesional
                );

                // EMBARAZOS (opcional)
                var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);
                ViewBag.EmbarazosModal = embarazos;

                ViewBag.ListaEmbarazos = new SelectList(
                    embarazos.Select(e => new
                    {
                        e.IdEmbarazo,
                        Nombre = $"ID: {e.IdEmbarazo} - {e.NombrePaciente}"
                    }),
                    "IdEmbarazo",
                    "Nombre",
                    orden?.IdEmbarazo
                );

                // TIPOS DE AYUDA
                var tipos = logTipoAyudaDiagnostica.Instancia.ListarTiposAyuda(); // ajusta al nombre real
                ViewBag.TiposAyudaModal = tipos;

                ViewBag.ListaTiposAyuda = new SelectList(
                    tipos,
                    "IdTipoAyuda",    // nombre de la columna en tu entidad de tipo
                    "Descripcion",    // texto a mostrar
                    orden?.IdTipoAyuda
                );
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
            }
        }
    }
}

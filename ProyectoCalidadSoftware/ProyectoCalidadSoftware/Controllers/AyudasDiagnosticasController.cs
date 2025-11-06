using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // Para IdProfesional

namespace ProyectoCalidadSoftware.Controllers
{
    // [Authorize(Roles = "PERSONAL_SALUD,ADMIN")]
    public class AyudaDiagnosticaController : Controller
    {
        // GET: /AyudaDiagnostica/Listar
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                // ==== CORRECCIÓN ====
                // El método se llama "ListarAyudaDiagnosticaOrden" en tu capa lógica
                var lista = logAyudaDiagnosticaOrden.Instancia.ListarAyudaDiagnosticaOrden();
                return View(lista); // Views/AyudaDiagnostica/Listar.cshtml
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista: " + ex.Message;
                return View(new List<entAyudaDiagnosticaOrden>());
            }
        }

        // GET: /AyudaDiagnostica/Insertar
        [HttpGet]
        public IActionResult Insertar(int? idEmbarazo) // Opcional, si vienes desde un embarazo
        {
            CargarViewBags(null);
            var modelo = new entAyudaDiagnosticaOrden
            {
                IdEmbarazo = idEmbarazo,
                Urgente = false
            };

            if (idEmbarazo.HasValue)
            {
                var embarazo = logEmbarazo.Instancia.BuscarEmbarazoPorId(idEmbarazo.Value);
                if (embarazo != null)
                {
                    modelo.IdPaciente = embarazo.IdPaciente;
                }
            }

            return View(modelo); // Views/AyudaDiagnostica/Insertar.cshtml
        }

        // POST: /AyudaDiagnostica/Insertar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(entAyudaDiagnosticaOrden entidad)
        {
            try
            {
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Debe seleccionar un paciente.");

                // ==== CORRECCIÓN ====
                // Añadimos la validación para el nuevo campo
                if (entidad.IdProfesional <= 0)
                    ModelState.AddModelError(nameof(entidad.IdProfesional), "Debe seleccionar un profesional.");

                if (entidad.IdTipoAyuda <= 0 && string.IsNullOrWhiteSpace(entidad.Descripcion))
                    ModelState.AddModelError(nameof(entidad.Descripcion), "Debe seleccionar un tipo o escribir una descripción.");

                // ==== CORRECCIÓN ====
                // Se ELIMINA el bloque de código que intentaba
                // asignar el IdProfesional automáticamente basado en el usuario logueado.
                // (Ya no es necesario, el formulario lo envía).

                if (!ModelState.IsValid)
                {
                    CargarViewBags(entidad);
                    return View(entidad);
                }

                bool ok = logAyudaDiagnosticaOrden.Instancia.InsertarAyudaDiagnosticaOrden(entidad);
                if (ok)
                {
                    TempData["Ok"] = "Orden de ayuda diagnóstica registrada.";
                    return RedirectToAction(nameof(Listar));
                }

                ViewBag.Error = "No se pudo registrar la orden.";
                CargarViewBags(entidad);
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                CargarViewBags(entidad);
                return View(entidad);
            }
        }
        // GET: /AyudaDiagnostica/Detalles/5
        [HttpGet]
        public IActionResult Detalles(int id)
        {
            try
            {
                var orden = logAyudaDiagnosticaOrden.Instancia.BuscarOrden(id);
                if (orden == null)
                {
                    TempData["Error"] = "Orden no encontrada.";
                    return RedirectToAction(nameof(Listar));
                }
                return View(orden); // Devuelve Views/AyudaDiagnostica/Detalles.cshtml
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los detalles: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // GET: /AyudaDiagnostica/Anular/5
        [HttpGet]
        public IActionResult Anular(int id)
        {
            // (Esta llamada a "BuscarOrden" está bien,
            // ya que no la renombramos en la capa lógica)
            var orden = logAyudaDiagnosticaOrden.Instancia.BuscarOrden(id);
            if (orden == null)
            {
                TempData["Error"] = "Orden no encontrada.";
                return RedirectToAction(nameof(Listar));
            }
            return View(orden); // Views/AyudaDiagnostica/Anular.cshtml
        }

        // POST: /AyudaDiagnostica/Anular/5
        [HttpPost]
        [ActionName("Anular")]
        [ValidateAntiForgeryToken]
        public IActionResult AnularConfirmado(entAyudaDiagnosticaOrden entidad)
        {
            try
            {
                // (Esta llamada a "AnularOrden" está bien)
                bool ok = logAyudaDiagnosticaOrden.Instancia.AnularOrden(entidad.IdAyuda);
                TempData[ok ? "Ok" : "Error"] = ok ? "Orden anulada." : "No se pudo anular la orden.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular: " + ex.Message;
            }
            return RedirectToAction(nameof(Listar));
        }

        // (Puedes añadir Editar y Detalles de la misma forma)

        // --- Método privado para cargar DropDownLists ---
        // --- Método privado para cargar DropDownLists ---
        private void CargarViewBags(entAyudaDiagnosticaOrden? entidad)
        {
            // ==== INICIO DE LA CORRECCIÓN ====
            // Inicializamos todas las listas como vacías fuera del try.
            // Si una falla, las otras seguirán vacías pero no serán 'null'.
            ViewBag.ListaPacientes = new SelectList(new List<SelectListItem>());
            ViewBag.ListaEmbarazos = new SelectList(new List<SelectListItem>());
            ViewBag.ListaTiposAyuda = new SelectList(new List<SelectListItem>());
            ViewBag.ListaProfesionales = new SelectList(new List<SelectListItem>()); // <-- La lista problemática
                                                                                     // ==== FIN DE LA CORRECCIÓN ====

            try
            {
                // Cargar Pacientes Activos
                var pacientes = logPaciente.Instancia.ListarPacientesActivos();
                ViewBag.ListaPacientes = new SelectList(
                    pacientes.Select(p => new { p.IdPaciente, Nombre = $"{p.Nombres} {p.Apellidos} (DNI: {p.DNI})" }),
                    "IdPaciente", "Nombre", entidad?.IdPaciente
                );

                // Cargar Embarazos Activos
                var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);
                ViewBag.ListaEmbarazos = new SelectList(
                    embarazos.Select(e => new { e.IdEmbarazo, Nombre = $"ID: {e.IdEmbarazo} - {e.NombrePaciente}" }),
                    "IdEmbarazo", "Nombre", entidad?.IdEmbarazo
                );

                // Cargar Tipos de Ayuda
                var tipos = logTipoAyudaDiagnostica.Instancia.ListarTiposAyuda();
                ViewBag.ListaTiposAyuda = new SelectList(
                    tipos, "IdTipoAyuda", "Nombre", entidad?.IdTipoAyuda
                );

                // Cargar Profesionales Activos
                var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);
                ViewBag.ListaProfesionales = new SelectList(
                    profesionales.Select(p => new { p.IdProfesional, Nombre = $"{p.Nombres} {p.Apellidos} (CMP: {p.CMP})" }),
                    "IdProfesional", "Nombre", entidad?.IdProfesional
                );
            }
            catch (Exception ex)
            {
                // Ahora, si falla, el ViewBag.Error se mostrará,
                // pero las listas simplemente estarán vacías en lugar de causar un error.
                ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
            }
        }
    }
}
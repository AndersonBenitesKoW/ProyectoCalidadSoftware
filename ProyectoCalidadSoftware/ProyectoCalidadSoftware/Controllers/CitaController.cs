using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectList
using System;
using System.Collections.Generic; // Para List<>
using System.Linq; // Para .Select()
using System.Security.Claims; // Para obtener el ID del usuario

namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "PERSONAL_SALUD,SECRETARIA,ADMIN")] // Protege todo el controlador
    public class CitaController : Controller
    {
        // GET: /Cita/Listar (o /Cita/Index)
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var lista = logCita.Instancia.ListarCita();
                return View(lista); // Views/Cita/Listar.cshtml
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista de citas: " + ex.Message;
                return View(new List<entCita>());
            }
        }

        // GET: /Cita/Insertar
        [HttpGet]
        public IActionResult Insertar()
        {
            var modelo = new entCita
            {
                // Fecha por defecto: ahora mismo
                FechaCita = DateTime.Now
            };

            CargarViewBags(modelo); // pásale el modelo para que marque selecciones si hiciera falta
            return View(modelo);
        }

        // POST: /Cita/Insertar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(entCita entidad)
        {
            try
            {
                // --- Validaciones del Controlador ---
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");
                if (entidad.FechaCita < DateTime.Now.AddMinutes(-5)) // 5 min de tolerancia
                    ModelState.AddModelError(nameof(entidad.FechaCita), "La fecha de la cita no puede ser en el pasado.");

                // --- LÓGICA DE NEGOCIO ---
                // 1. Asignar el estado "Pendiente" (Asumo ID 1 = Pendiente)
                entidad.IdEstadoCita = 1;

                // 2. Asignar la recepcionista (el usuario logueado)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Busca el ID del usuario
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int recepcionistaId))
                {
                    entidad.IdRecepcionista = recepcionistaId;
                }
                else
                {
                    // Si no se encuentra el ID del usuario, es un error grave de sesión
                    ModelState.AddModelError("", "No se pudo identificar al usuario. Por favor, inicie sesión de nuevo.");
                }

                if (!ModelState.IsValid)
                {
                    CargarViewBags(entidad); // Recarga los dropdowns
                    return View(entidad);
                }
                // --- FIN LÓGICA DE NEGOCIO ---

                bool ok = logCita.Instancia.InsertarCita(entidad);
                if (ok)
                {
                    TempData["Ok"] = "Cita registrada correctamente.";
                    return RedirectToAction(nameof(Listar));
                }

                ViewBag.Error = "No se pudo insertar la cita. Revise los datos.";
                CargarViewBags(entidad);
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al insertar: " + ex.Message;
                CargarViewBags(entidad);
                return View(entidad);
            }
        }

        // GET: /Cita/Anular/5
        [HttpGet]
        public IActionResult Anular(int id)
        {
            var cita = logCita.Instancia.BuscarCita(id);
            if (cita == null)
            {
                TempData["Error"] = "Cita no encontrada.";
                return RedirectToAction(nameof(Listar));
            }
            // Asegúrate de tener una vista Views/Cita/Anular.cshtml
            return View(cita);
        }

        // POST: /Cita/AnularConfirmado/5
        // POST: /Cita/Anular
        [HttpPost]
        [ActionName("Anular")] // Mantiene la URL "Anular"
        [ValidateAntiForgeryToken]
        public IActionResult AnularConfirmado(entCita cita) // 1. Recibe la entidad 'cita' COMPLETA
        {
            try
            {
                // 2. Validamos que el motivo venga del formulario
                if (string.IsNullOrWhiteSpace(cita.MotivoAnulacion))
                {
                    ModelState.AddModelError(nameof(cita.MotivoAnulacion), "El motivo de anulación es obligatorio.");

                    // Si falla, recargamos la página de confirmación con el error
                    // (Necesitamos volver a buscar la cita para mostrar los nombres)
                    var citaActual = logCita.Instancia.BuscarCita(cita.IdCita);
                    return View("Anular", citaActual); // Devuelve la vista Anular.cshtml
                }

                // 3. Pasamos AMBOS datos a la capa lógica
                bool ok = logCita.Instancia.AnularCita(cita.IdCita, cita.MotivoAnulacion);

                TempData[ok ? "Ok" : "Error"] = ok ? "Cita anulada." : "No se pudo anular la cita.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular: " + ex.Message;
            }
            return RedirectToAction(nameof(Listar));
        }
        [HttpGet]
        public IActionResult DetallesCita(int id)
        {
            try
            {
                // Usamos el método 'BuscarCita' que ya existe en tu lógica
                var cita = logCita.Instancia.BuscarCita(id);
                if (cita == null)
                {
                    TempData["Error"] = "Cita no encontrada.";
                    return RedirectToAction(nameof(Listar));
                }
                return View(cita); // Devuelve Views/Cita/DetallesCita.cshtml
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los detalles: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        // --- Método privado para cargar DropDownLists ---
        private void CargarViewBags(entCita? cita)
        {
            try
            {
                // ---------- PACIENTES ----------
                var pacientes = logPaciente.Instancia.ListarPacientesActivos();

                // Para MODAL (tabla)
                ViewBag.PacientesModal = pacientes;

                // (Opcional) Para combobox clásico, por si lo usas en otras vistas
                ViewBag.ListaPacientes = new SelectList(
                    pacientes.Select(p => new
                    {
                        p.IdPaciente,
                        Nombre = $"{p.Nombres} {p.Apellidos} (DNI: {p.DNI})"
                    }),
                    "IdPaciente",
                    "Nombre",
                    cita?.IdPaciente
                );

                // ---------- PROFESIONALES ----------
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
                    cita?.IdProfesional
                );

                // ---------- EMBARAZOS (Opcional) ----------
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
                    cita?.IdEmbarazo
                );
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
            }
        }
    }
}
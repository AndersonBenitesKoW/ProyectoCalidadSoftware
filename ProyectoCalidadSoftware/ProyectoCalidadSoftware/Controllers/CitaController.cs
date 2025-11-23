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
    public class CitaController : Controller
    {
        private readonly ILogger<CitaController> _logger;

        public CitaController(ILogger<CitaController> logger)
        {
            _logger = logger;
        }
        // GET: /Cita/Listar (o /Cita/Index)
        [HttpGet]
        [Authorize(Roles = "PERSONAL_SALUD,SECRETARIA,ADMIN")]
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
        [Authorize(Roles = "PERSONAL_SALUD,SECRETARIA,ADMIN,PACIENTE")]
        public IActionResult Insertar()
        {
            _logger.LogWarning("Accediendo a Insertar cita. Usuario: {User}, Autenticado: {Auth}, Roles: {Roles}",
                User.Identity?.Name,
                User.Identity?.IsAuthenticated,
                string.Join(",", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));

            var modelo = new entCita
            {
                // Fecha por defecto: ahora mismo
                FechaCita = DateTime.Now
            };

            // Si es PACIENTE, pre-llenar el IdPaciente
            if (User.IsInRole("PACIENTE"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.IdUsuario == userId);
                    if (paciente != null)
                    {
                        modelo.IdPaciente = paciente.IdPaciente;
                    }
                }
            }

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

        // GET: /Cita/RegistrarCitaPublica
        [HttpGet]
        [Authorize(Roles = "PACIENTE")]
        public IActionResult RegistrarCitaPublica()
        {
            _logger.LogWarning("Accediendo a RegistrarCitaPublica. Usuario: {User}, Autenticado: {Auth}, Roles: {Roles}",
                User.Identity?.Name,
                User.Identity?.IsAuthenticated,
                string.Join(",", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));

            var modelo = new entCita
            {
                // Fecha por defecto: ahora mismo
                FechaCita = DateTime.Now
            };

            // Pre-llenar el IdPaciente para el paciente logueado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.IdUsuario == userId);
                if (paciente != null)
                {
                    modelo.IdPaciente = paciente.IdPaciente;
                    ViewBag.PacienteNombre = $"{paciente.Nombres} {paciente.Apellidos}";
                }
            }

            CargarViewBags(modelo);
            // No cargar pacientes para el modal, se buscará por DNI
            ViewBag.PacientesModal = null;
            return View(modelo);
        }

        // POST: /Cita/RegistrarCitaPublica
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarCitaPublica(entCita entidad)
        {
            try
            {
                // --- Validaciones del Controlador ---
                if (entidad.IdPaciente <= 0)
                    ModelState.AddModelError(nameof(entidad.IdPaciente), "Seleccione un paciente válido.");
                if (entidad.IdProfesional <= 0)
                    ModelState.AddModelError(nameof(entidad.IdProfesional), "Seleccione un profesional de salud válido.");
                if (entidad.FechaCita < DateTime.Now.AddMinutes(-5)) // 5 min de tolerancia
                    ModelState.AddModelError(nameof(entidad.FechaCita), "La fecha de la cita no puede ser en el pasado.");

                // Validar que el slot no esté ocupado por el profesional
                var citaProfesionalExistente = logCita.Instancia.ListarCita().Any(c => c.IdProfesional == entidad.IdProfesional && c.FechaCita == entidad.FechaCita);
                if (citaProfesionalExistente)
                    ModelState.AddModelError(nameof(entidad.FechaCita), "Este horario ya está ocupado por este profesional.");

                // Validar que el paciente no tenga cita en el mismo horario con otro profesional
                var citaPacienteMismoHorario = logCita.Instancia.ListarCita().Any(c => c.IdPaciente == entidad.IdPaciente && c.FechaCita == entidad.FechaCita);
                if (citaPacienteMismoHorario)
                    ModelState.AddModelError(nameof(entidad.FechaCita), "Ya tienes una cita programada en este horario con otro profesional.");

                // --- LÓGICA DE NEGOCIO ---
                // 1. Asignar el estado "Pendiente" (Asumo ID 1 = Pendiente)
                entidad.IdEstadoCita = 1;

                // 2. Asignar la recepcionista (el usuario logueado, si aplica)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int recepcionistaId))
                {
                    entidad.IdRecepcionista = recepcionistaId;
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo identificar al usuario. Por favor, inicie sesión de nuevo.");
                }

                if (!ModelState.IsValid)
                {
                    CargarViewBags(entidad);
                    return View(entidad);
                }

                bool ok = logCita.Instancia.InsertarCita(entidad);
                if (ok)
                {
                    TempData["Ok"] = "Cita registrada correctamente.";
                    return RedirectToAction("Index", "Portal"); // Redirigir al portal
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
        [Authorize(Roles = "PERSONAL_SALUD,SECRETARIA,ADMIN")]
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
        [Authorize(Roles = "PERSONAL_SALUD,SECRETARIA,ADMIN")]
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

        // GET: /Cita/BuscarPacientePorDNI
        [HttpGet]
        [Authorize(Roles = "PACIENTE")]
        public IActionResult BuscarPacientePorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return Json(new { success = false, message = "DNI requerido." });
            }

            // Obtener el ID del usuario logueado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new { success = false, message = "Usuario no identificado." });
            }

            // Buscar paciente que pertenezca al usuario logueado y tenga el DNI especificado
            var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.IdUsuario == userId && p.DNI == dni.Trim());
            if (paciente == null)
            {
                return Json(new { success = false, message = "Paciente no encontrado con ese DNI o no pertenece a su cuenta." });
            }

            return Json(new { success = true, paciente = new { id = paciente.IdPaciente, nombre = $"{paciente.Nombres} {paciente.Apellidos}", dni = paciente.DNI } });
        }

        // GET: /Cita/BuscarProfesionalPorCMP
        [HttpGet]
        [Authorize(Roles = "PACIENTE")]
        public IActionResult BuscarProfesionalPorCMP(string cmp)
        {
            if (string.IsNullOrWhiteSpace(cmp))
            {
                return Json(new { success = false, message = "CMP requerido." });
            }

            var profesional = logProfesionalSalud.Instancia.BuscarProfesionalPorCMP(cmp.Trim());
            if (profesional == null)
            {
                return Json(new { success = false, message = "Profesional no encontrado con ese CMP." });
            }

            return Json(new { success = true, profesional = new { id = profesional.IdProfesional, nombre = $"{profesional.Nombres} {profesional.Apellidos}", cmp = profesional.CMP, especialidad = profesional.Especialidad } });
        }

        // GET: /Cita/BuscarUltimoEmbarazoPorPacienteId
        [HttpGet]
        [Authorize(Roles = "PACIENTE")]
        public IActionResult BuscarUltimoEmbarazoPorPacienteId(int idPaciente)
        {
            if (idPaciente <= 0)
            {
                return Json(new { success = false, message = "ID de paciente inválido." });
            }

            var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true).Where(e => e.IdPaciente == idPaciente).OrderByDescending(e => e.FUR).FirstOrDefault();
            if (embarazos == null)
            {
                return Json(new { success = false, message = "No se encontró embarazo activo para este paciente." });
            }

            return Json(new { success = true, embarazo = new { id = embarazos.IdEmbarazo, nombre = embarazos.NombrePaciente } });
        }

        // GET: /Cita/ObtenerSlotsDisponibles
        [HttpGet]
        [Authorize(Roles = "PACIENTE")]
        public IActionResult ObtenerSlotsDisponibles(int idProfesional, string fecha, int idPaciente)
        {
            if (idProfesional <= 0 || string.IsNullOrWhiteSpace(fecha) || idPaciente <= 0)
            {
                return Json(new { success = false, message = "Parámetros inválidos." });
            }

            if (!DateTime.TryParse(fecha, out DateTime fechaSeleccionada))
            {
                return Json(new { success = false, message = "Fecha inválida." });
            }

            // Generar slots: 8:00, 8:30, ..., 11:30, 14:00, 14:30, ..., 17:30
            var slots = new List<string>();
            for (int hour = 8; hour <= 11; hour++)
            {
                slots.Add($"{hour:00}:00");
                slots.Add($"{hour:00}:30");
            }
            for (int hour = 14; hour <= 17; hour++)
            {
                slots.Add($"{hour:00}:00");
                slots.Add($"{hour:00}:30");
            }

            // Obtener citas existentes para ese profesional y fecha
            var citasProfesional = logCita.Instancia.ListarCita()
                .Where(c => c.IdProfesional == idProfesional && c.FechaCita.Date == fechaSeleccionada.Date)
                .Select(c => c.FechaCita.ToString("HH:mm"))
                .ToHashSet();

            // Obtener citas del paciente en esa fecha
            var citasPaciente = logCita.Instancia.ListarCita()
                .Where(c => c.IdPaciente == idPaciente && c.FechaCita.Date == fechaSeleccionada.Date)
                .Select(c => c.FechaCita.ToString("HH:mm"))
                .ToHashSet();

            var slotsDisponibles = slots.Select(slot => new
            {
                hora = slot,
                disponible = !citasProfesional.Contains(slot) && !citasPaciente.Contains(slot)
            }).ToList();

            return Json(new { success = true, slots = slotsDisponibles });
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
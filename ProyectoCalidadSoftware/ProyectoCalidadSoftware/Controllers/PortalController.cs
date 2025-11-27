using CapaLogica;
using CapaEntidad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoCalidadSoftware.Models;


namespace ProyectoCalidadSoftware.Controllers
{
    public class PortalController : Controller
    {
        // Página principal pública
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Si el usuario está autenticado, mantener la sesión activa
            if (User.Identity?.IsAuthenticated == true)
            {
                // El usuario puede navegar entre layouts sin cerrar sesión
                ViewData["IsAuthenticated"] = true;
                if (User.IsInRole("PACIENTE"))
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.IdUsuario == userId);
                        System.Diagnostics.Debug.WriteLine($"Index: userId={userId}, paciente encontrado: {paciente != null}, DNI: '{paciente?.DNI}', Estado: {paciente?.Estado}");
                        ViewData["PacienteRegistrado"] = paciente != null && !string.IsNullOrWhiteSpace(paciente.DNI);
                    }
                }
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Servicios() => View();

        // 🔐 Solo PACIENTE puede usar esta ruta de "Citas" del portal
        [Authorize(Roles = "PACIENTE")]
        public IActionResult Citas()
        {
            // Solo pacientes logueados pueden acceder al formulario de citas
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("PACIENTE"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.IdUsuario == userId);
                    System.Diagnostics.Debug.WriteLine($"Citas: userId={userId}, paciente encontrado: {paciente != null}, DNI: '{paciente?.DNI}', Estado: {paciente?.Estado}");
                    if (paciente == null || string.IsNullOrWhiteSpace(paciente.DNI))
                    {
                        System.Diagnostics.Debug.WriteLine("Citas: Redirigiendo a RegistrarPaciente");
                        return RedirectToAction("RegistrarPaciente");
                    }
                }
                System.Diagnostics.Debug.WriteLine("Citas: Redirigiendo a RegistrarCitaPublica");
                return RedirectToAction("RegistrarCitaPublica", "Cita");
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PACIENTE")]
        public IActionResult Citas(string nombreCompleto, string email, string telefono, string especialidad, DateTime fechaCita, string comentarios)
        {
            try
            {
                // Buscar paciente por email
                var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.EmailPrincipal == email);
                if (paciente == null)
                {
                    // Crear paciente
                    paciente = new entPaciente
                    {
                        Nombres = nombreCompleto.Split(' ')[0],
                        Apellidos = string.Join(" ", nombreCompleto.Split(' ').Skip(1)),
                        EmailPrincipal = email,
                        TelefonoPrincipal = telefono,
                        Estado = true
                    };
                    logPaciente.Instancia.InsertarPaciente(paciente);
                    // Asumir que InsertarPaciente asigna IdPaciente
                }

                // Crear cita
                var cita = new entCita
                {
                    IdPaciente = paciente.IdPaciente,
                    FechaCita = fechaCita,
                    Observacion = comentarios,
                    IdEstadoCita = 1, // Pendiente
                                      // IdProfesional = null, IdRecepcionista = null
                };

                logCita.Instancia.InsertarCita(cita);

                TempData["Success"] = "Cita agendada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al agendar cita: " + ex.Message;
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult Contacto() => View();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        // Login (form normal, no AJAX)
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string nombreUsuario, string clave)
        {
            var usuario = logUsuario.Instancia.ValidarUsuario(nombreUsuario, clave);
            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }

            await IniciarSesion(usuario);
            return RedirigirPorRol(usuario.NombreRol);
        }

        // Login AJAX (fetch)
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAjax(string NombreUsuario, string Clave)
        {
            try
            {
                var usuario = logUsuario.Instancia.ValidarUsuario(NombreUsuario, Clave);
                if (usuario == null)
                    return Json(new { success = false, message = "Usuario o contraseña incorrectos." });

                await IniciarSesion(usuario);
                return Json(new { success = true, message = "Inicio de sesión exitoso." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno: " + ex.Message });
            }
        }

        private async Task IniciarSesion(entUsuario usuario)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
        };

            if (!string.IsNullOrWhiteSpace(usuario.NombreRol))
                claims.Add(new Claim(ClaimTypes.Role, usuario.NombreRol));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private IActionResult RedirigirPorRol(string? nombreRol)
        {
            var rol = nombreRol?.ToUpper() ?? "";
            switch (rol)
            {
                case "ADMIN":
                case "PERSONAL_SALUD":
                    return RedirectToAction("Index", "Home");
                case "SECRETARIA":
                    return RedirectToAction("Insertar", "Cita");
                case "PACIENTE":
                    // Verificar si tiene paciente registrado con datos completos
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.IdUsuario == userId);
                        System.Diagnostics.Debug.WriteLine($"RedirigirPorRol: userId={userId}, paciente encontrado: {paciente != null}, DNI: '{paciente?.DNI}', Estado: {paciente?.Estado}");
                        if (paciente == null || string.IsNullOrWhiteSpace(paciente.DNI))
                        {
                            System.Diagnostics.Debug.WriteLine("RedirigirPorRol: Redirigiendo a RegistrarPaciente");
                            return RedirectToAction("RegistrarPaciente");
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("RedirigirPorRol: Redirigiendo a Index");
                    return RedirectToAction("Index", "Portal");
                default:
                    return RedirectToAction("Index", "Portal");
            }
        }

        // ===== Registro de USUARIO-PACIENTE (cuenta) =====
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            // Enviamos un modelo vacío a la vista
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new entUsuario
            {
                NombreUsuario = model.Username,
                Email = model.Email,
                ClaveHash = model.Password, // La lógica lo hashea
                Estado = true
            };

            try
            {
                var exito = logUsuario.Instancia.RegistrarPaciente(usuario);

                if (exito)
                {
                    TempData["Success"] = "Cuenta creada exitosamente. Ahora puedes iniciar sesión.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo crear la cuenta. Es posible que el usuario o email ya existan.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear la cuenta: " + ex.Message);
                return View(model);
            }
        }

        // ===== Registro de datos de PACIENTE (ficha clínica básica) =====
        [HttpGet]
        [Authorize(Roles = "PACIENTE")]
        public IActionResult RegistrarPaciente()
        {
            if (!User.Identity?.IsAuthenticated == true || !User.IsInRole("PACIENTE"))
            {
                return RedirectToAction("Login");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var paciente = logPaciente.Instancia.ListarPacientesActivos().FirstOrDefault(p => p.IdUsuario == userId);
                System.Diagnostics.Debug.WriteLine($"RegistrarPaciente GET: userId={userId}, paciente encontrado: {paciente != null}, DNI: '{paciente?.DNI}', Estado: {paciente?.Estado}");
                if (paciente != null && !string.IsNullOrWhiteSpace(paciente.DNI))
                {
                    System.Diagnostics.Debug.WriteLine("RegistrarPaciente GET: Ya registrado, redirigiendo a Index");
                    return RedirectToAction("Index");
                }
            }

            return View(new entPaciente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PACIENTE")]
        public IActionResult RegistrarPaciente(entPaciente entidad)
        {
            if (!User.Identity?.IsAuthenticated == true || !User.IsInRole("PACIENTE"))
            {
                return RedirectToAction("Login");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                entidad.IdUsuario = userId;
            }

            // --- Validaciones ---
            if (string.IsNullOrWhiteSpace(entidad.Nombres))
                ModelState.AddModelError(nameof(entidad.Nombres), "Los nombres son obligatorios.");
            if (string.IsNullOrWhiteSpace(entidad.Apellidos))
                ModelState.AddModelError(nameof(entidad.Apellidos), "Los apellidos son obligatorios.");
            if (string.IsNullOrWhiteSpace(entidad.DNI))
                ModelState.AddModelError(nameof(entidad.DNI), "El DNI es obligatorio.");
            if (string.IsNullOrWhiteSpace(entidad.EmailPrincipal))
                ModelState.AddModelError(nameof(entidad.EmailPrincipal), "El Email es obligatorio.");
            if (string.IsNullOrWhiteSpace(entidad.TelefonoPrincipal))
                ModelState.AddModelError(nameof(entidad.TelefonoPrincipal), "El Teléfono es obligatorio.");

            if (!ModelState.IsValid) return View(entidad);

            entidad.Estado = true;

            bool ok = logPaciente.Instancia.InsertarPaciente(entidad);
            System.Diagnostics.Debug.WriteLine($"RegistrarPaciente POST: insert ok={ok}, userId={entidad.IdUsuario}, DNI='{entidad.DNI}', Nombres='{entidad.Nombres}'");
            if (ok)
            {
                TempData["Success"] = "Datos de paciente registrados exitosamente.";
                return RedirectToAction("RegistrarCitaPublica", "Cita");
            }

            ViewBag.Error = "No se pudo registrar los datos del paciente. Verifique los datos.";
            return View(entidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // 🔐 Cualquier usuario autenticado puede cerrar sesión
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Portal");
        }
    }
}

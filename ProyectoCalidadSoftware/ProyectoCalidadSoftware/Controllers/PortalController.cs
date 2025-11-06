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
    public class PortalController: Controller
    {
        // Página principal pública
        public IActionResult Index()
        {
            // Si el usuario está autenticado, mantener la sesión activa
            if (User.Identity?.IsAuthenticated == true)
            {
                // El usuario puede navegar entre layouts sin cerrar sesión
                ViewData["IsAuthenticated"] = true;
            }
            return View();
        }

        public IActionResult Servicios() => View();

        public IActionResult Citas() => View();

        public IActionResult Contacto() => View();

        [HttpGet]
        public IActionResult Login() => View();

        // Login (form normal, no AJAX)
        [HttpPost]
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
            return rol switch
            {
                "ADMIN" => RedirectToAction("Index", "Home"),
                "PERSONAL_SALUD" => RedirectToAction("Index", "Home"),
                "SECRETARIA" => RedirectToAction("Insertar", "Cita"),
                "PACIENTE" => RedirectToAction("Index", "Portal"),
                _ => RedirectToAction("Index", "Portal")
            };
        }

        // ===== Registro de PACIENTE =====
        [HttpGet]
        public IActionResult Register()
        {
            // Enviamos un modelo vacío a la vista
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // <-- Buena práctica (requiere @Html.AntiForgeryToken() en la vista)
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // 1. La validación (campos vacíos, email, contraseñas coinciden)
            //    se hace automáticamente gracias al ViewModel.
            if (!ModelState.IsValid)
            {
                // Si algo falla, regresa a la vista y muestra los errores
                return View(model);
            }

            // 2. Si la validación pasa, creamos el usuario
            var usuario = new entUsuario
            {
                NombreUsuario = model.Username,
                Email = model.Email,
                ClaveHash = model.Password, // La lógica lo hashea
                Estado = true
            };

            try
            {
                // 3. Llamamos a tu lógica de negocio
                var exito = logUsuario.Instancia.RegistrarPaciente(usuario);

                if (exito)
                {
                    TempData["Success"] = "Cuenta creada exitosamente. Ahora puedes iniciar sesión.";
                    return RedirectToAction("Login");
                }
                else
                {
                    // Error de la base de datos (ej. usuario ya existe)
                    ModelState.AddModelError("", "No se pudo crear la cuenta. Es posible que el usuario o email ya existan.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Error inesperado
                ModelState.AddModelError("", "Error al crear la cuenta: " + ex.Message);
                return View(model);
            }
        }
        // En PortalController.cs - AGREGA ESTE MÉTODO:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Portal");
        }

    }
}

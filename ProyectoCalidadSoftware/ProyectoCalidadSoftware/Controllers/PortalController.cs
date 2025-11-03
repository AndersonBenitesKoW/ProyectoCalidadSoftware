using CapaLogica;
using CapaEntidad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Todos los campos son obligatorios.";
                return View();
            }

            if (!string.Equals(password, confirmPassword))
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                return View();
            }

            var usuario = new entUsuario
            {
                NombreUsuario = username,
                Email = email,
                ClaveHash = password, // texto plano; se hashea en la lógica
                Estado = true
            };

            // usa el flujo de registro de pacientes (rol PACIENTE)
            var exito = logUsuario.Instancia.RegistrarPaciente(usuario);

            if (exito)
            {
                TempData["Success"] = "Cuenta creada exitosamente. Ahora puedes iniciar sesión.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Error al crear la cuenta. Inténtalo nuevamente.";
            return View();
        }

    }
}

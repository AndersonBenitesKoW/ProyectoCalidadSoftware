using CapaLogica;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProyectoCalidadSoftware.Controllers
{
    public class UserValidationController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string clave)
        {
            // 1. Validar credenciales en la lógica
            var usuario = logUsuario.Instancia.ValidarUsuario(nombreUsuario, clave);

            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }

            // 2. Construir los claims básicos
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.NombreUsuario),
        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
    };

            // 3. Agregar el rol (SOLO UNO)
            // aseguramos no null
            if (!string.IsNullOrWhiteSpace(usuario.NombreRol))
            {
                claims.Add(new Claim(ClaimTypes.Role, usuario.NombreRol));
            }

            // 4. Crear identidad y principal
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            // 5. Firmar sesión
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            // 6. Redirección por rol
            // OJO: ahora es por un string, no por lista
            var rol = usuario.NombreRol?.ToUpper() ?? "";

            if (rol == "ADMIN")
                return RedirectToAction("Index", "Home");

            if (rol == "PERSONAL_SALUD")
                return RedirectToAction("Index", "Home");

            if (rol == "SECRETARIA")
                return RedirectToAction("Insertar", "Cita");

            if (rol == "PACIENTE")
                return RedirectToAction("Index", "Portal");

            // Por defecto
            return RedirectToAction("Index", "Portal");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Portal");
        }



    }
}

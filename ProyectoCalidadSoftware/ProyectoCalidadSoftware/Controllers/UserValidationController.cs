using CapaLogica;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoCalidadSoftware.Controllers
{
    public class UserValidationController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            var rol = usuario.NombreRol?.ToUpper() ?? "";

            switch (rol)
            {
                case "ADMIN":
                case "PERSONAL_SALUD":
                    return RedirectToAction("Index", "Home");

                case "SECRETARIA":
                    return RedirectToAction("Insertar", "Cita");

                case "PACIENTE":
                    return RedirectToAction("Index", "Portal");

                default:
                    return RedirectToAction("Index", "Portal");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Portal");
        }
    }

}

using Microsoft.AspNetCore.Mvc;
using CapaEntidad;
using CapaLogica;

namespace ProyectoCalidadSoftware.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(entLogin login)
        {
            if (ModelState.IsValid)
            {
                var usuario = logUsuario.Instancia.VerificarUsuario(login.NombreUsuario, login.Clave);
                if (usuario != null)
                {
                    // Login successful, perhaps set session or redirect
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                }
            }
            return View(login);
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class StaffLoginController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Index(entLogin login)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var profesional = logProfesionalSalud.Instancia.VerificarStaffLogin(login.NombreUsuario, login.Clave);
        //        if (profesional != null)
        //        {
        //            // Login successful, perhaps set session or redirect
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Usuario o contraseña incorrectos, o no es un profesional de salud");
        //        }
        //    }
        //    return View(login);
        //}
    }
}
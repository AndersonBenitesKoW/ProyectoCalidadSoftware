namespace ProyectoCalidadSoftware.Controllers
{
    //public class LoginController : Controller
    ////{
    ////    public IActionResult Index()
    ////    {
    ////        return View();
    ////    }

    ////    [HttpPost]
    ////    public IActionResult Index(entLogin login)
    ////    {
    ////        // Check if it's an AJAX request
    ////        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
    ////        {
    ////            if (ModelState.IsValid)
    ////            {
    ////                var usuario = logUsuario.Instancia.VerificarUsuario(login.NombreUsuario, login.Clave);
    ////                if (usuario != null)
    ////                {
    ////                    // Login successful
    ////                    return Json(new { success = true, message = "Login exitoso", redirectUrl = Url.Action("Index", "Home") });
    ////                }
    ////                else
    ////                {
    ////                    return Json(new { success = false, message = "Usuario o contraseña incorrectos" });
    ////                }
    ////            }
    ////            else
    ////            {
    ////                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
    ////                return Json(new { success = false, message = string.Join(", ", errors) });
    ////            }
    ////        }
    ////        else
    ////        {
    ////            // Original behavior for non-AJAX requests
    ////            if (ModelState.IsValid)
    ////            {
    ////                var usuario = logUsuario.Instancia.VerificarUsuario(login.NombreUsuario, login.Clave);
    ////                if (usuario != null)
    ////                {
    ////                    // Login successful, perhaps set session or redirect
    ////                    return RedirectToAction("Index", "Home");
    ////                }
    ////                else
    ////                {
    ////                    ModelState.AddModelError("", "Usuario o contraseña incorrectos");
    ////                }
    ////            }
    ////            return View(login);
    ////        }
    //    }
    //}
}
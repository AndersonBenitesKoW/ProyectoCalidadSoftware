using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "ADMIN")] // 👈 SOLO ADMIN
    public class FactorRiesgoCatController : Controller
    {
        // GET: /FactorRiesgoCat/Listar
        public IActionResult Listar()
        {
            var lista = logFactorRiesgoCat.Instancia.ListarFactorRiesgoCat();
            return View(lista);
        }

        // GET: /FactorRiesgoCat/Insertar
        [HttpGet]
        public IActionResult Insertar()
        {
            return View(new entFactorRiesgoCat());
        }

        // POST: /FactorRiesgoCat/Insertar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(entFactorRiesgoCat entidad)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entidad.Nombre))
                    ModelState.AddModelError(nameof(entidad.Nombre), "El nombre es obligatorio.");

                if (!ModelState.IsValid) return View(entidad);

                bool ok = logFactorRiesgoCat.Instancia.InsertarFactorRiesgoCat(entidad);
                if (ok)
                    return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo insertar la categoría de factor de riesgo.";
                return View(entidad);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = "Error al insertar: " + ex.Message;
                return View(entidad);
            }
        }

        // GET: /FactorRiesgoCat/Actualizar/5
        [HttpGet]
        public IActionResult Actualizar(int id)
        {
            var entidad = logFactorRiesgoCat.Instancia.BuscarFactorRiesgoCat(id);
            if (entidad == null) return NotFound();
            return View(entidad);
        }

        // POST: /FactorRiesgoCat/Actualizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(entFactorRiesgoCat entidad)
        {
            try
            {
                if (entidad.IdFactorCat <= 0)
                    ModelState.AddModelError(nameof(entidad.IdFactorCat), "Identificador inválido.");
                if (string.IsNullOrWhiteSpace(entidad.Nombre))
                    ModelState.AddModelError(nameof(entidad.Nombre), "El nombre es obligatorio.");

                if (!ModelState.IsValid) return View(entidad);

                bool ok = logFactorRiesgoCat.Instancia.ActualizarFactorRiesgoCat(entidad);
                if (ok)
                    return RedirectToAction(nameof(Listar));

                ViewBag.Error = "No se pudo actualizar la categoría.";
                return View(entidad);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = "Error al actualizar: " + ex.Message;
                return View(entidad);
            }
        }

        // POST: /FactorRiesgoCat/Eliminar/5
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                bool ok = logFactorRiesgoCat.Instancia.EliminarFactorRiesgoCat(id);
                if (ok)
                    return RedirectToAction(nameof(Listar));

                TempData["Error"] = "No se pudo eliminar la categoría.";
                return RedirectToAction(nameof(Listar));
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }
    }
}

using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
    public class bebeController : Controller
    {
        public IActionResult listar()
        {
            var listaBebes = logBebe.Instancia.ListarBebe();
            ViewBag.Lista = listaBebes;
            return View(listaBebes);
        }

        // GET: /bebe/Registrar
        [HttpGet]
        public IActionResult Registrar(int? idParto)
        {
            var modelo = new entBebe
            {
                IdParto = idParto ?? 0,
                Estado = true
            };
            return View(modelo);
        }

        // POST: /bebe/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(entBebe bebe)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bebe.EstadoBebe))
                    ModelState.AddModelError(nameof(bebe.EstadoBebe), "El estado del bebé es obligatorio.");

                if (bebe.Apgar1 is < 0 or > 10)
                    ModelState.AddModelError(nameof(bebe.Apgar1), "Apgar1 debe estar entre 0 y 10.");

                if (bebe.Apgar5 is < 0 or > 10)
                    ModelState.AddModelError(nameof(bebe.Apgar5), "Apgar5 debe estar entre 0 y 10.");

                if (!ModelState.IsValid)
                    return View(bebe);

                bebe.Estado = true;

                bool registrado = logBebe.Instancia.InsertarBebe(bebe);

                if (registrado)
                    return RedirectToAction(nameof(listar));
                else
                {
                    ViewBag.Error = "No se pudo registrar el bebé.";
                    return View(bebe);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View(bebe);
            }
        }

        // GET: /bebe/ConsultarEstado/5
        [HttpGet]
        public IActionResult ConsultarEstado(int id)
        {
            try
            {
                var bebe = logBebe.Instancia.BuscarBebe(id);
                if (bebe == null)
                    return NotFound();

                ViewBag.Estado = bebe.Estado ? "Activo" : "Inactivo";
                return View(bebe);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al consultar estado: " + ex.Message;
                return RedirectToAction(nameof(listar));
            }
        }
    }
}

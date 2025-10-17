using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
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
            // Si vienes desde la pantalla de Parto puedes precargar el IdParto
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
                // Validaciones mínimas (ajusta a tu gusto o usa DataAnnotations)
                if (string.IsNullOrWhiteSpace(bebe.EstadoBebe))
                    ModelState.AddModelError(nameof(bebe.EstadoBebe), "El estado del bebé es obligatorio.");

                if (bebe.Apgar1 is < 0 or > 10)
                    ModelState.AddModelError(nameof(bebe.Apgar1), "Apgar1 debe estar entre 0 y 10.");

                if (bebe.Apgar5 is < 0 or > 10)
                    ModelState.AddModelError(nameof(bebe.Apgar5), "Apgar5 debe estar entre 0 y 10.");

                // Si hay errores de validación, vuelve al formulario
                if (!ModelState.IsValid)
                    return View(bebe);

                // Estado activo por defecto al registrar
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

        // GET: /bebe/ConsultarEstado/5  (opcional, similar a tu ejemplo)
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

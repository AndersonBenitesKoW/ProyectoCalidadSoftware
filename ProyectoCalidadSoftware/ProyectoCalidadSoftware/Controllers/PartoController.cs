using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoCalidadSoftware.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
    public class PartoController : Controller
    {
        // GET: /Parto/Index
        [HttpGet]
        public IActionResult Index(bool mostrarActivos = true)
        {
            try
            {
                ViewBag.MostrandoActivos = mostrarActivos;
                var lista = logParto.Instancia.ListarPartos(mostrarActivos);
                // Para PERSONAL_SALUD, setear IdProfesionalActual para restringir eliminación
                if (User.IsInRole("PERSONAL_SALUD"))
                {
                    var idProfesional = GetIdProfesional();
                    if (idProfesional.HasValue)
                    {
                        ViewBag.IdProfesionalActual = idProfesional.Value;
                    }
                }
                return View(lista); // Views/Parto/Index.cshtml
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista: " + ex.Message;
                return View(new List<entParto>());
            }
        }

        // GET: /Parto/RegistrarParto
        [HttpGet]
        public IActionResult RegistrarParto(int? idEmbarazo)
        {
            var modelo = new entParto
            {
                IdEmbarazo = idEmbarazo ?? 0,
                Fecha = DateTime.Now.Date,   // solo fecha actual
                Estado = true
            };

            CargarViewBags(modelo);
            return View(modelo); // Views/Parto/RegistrarParto.cshtml
        }

        // POST: /Parto/RegistrarParto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarParto(entParto parto)
        {
            try
            {
                if (parto.IdEmbarazo <= 0)
                    ModelState.AddModelError(nameof(parto.IdEmbarazo), "Debe seleccionar un embarazo válido.");

                if (parto.IdViaParto == 3 && string.IsNullOrWhiteSpace(parto.IndicacionCesarea)) // 3 = CESAREA
                    ModelState.AddModelError(nameof(parto.IndicacionCesarea), "Debe indicar el motivo de la cesárea.");

                if (!ModelState.IsValid)
                {
                    CargarViewBags(parto);
                    return View(parto);
                }

                bool registrado = logParto.Instancia.RegistrarParto(parto);

                if (registrado)
                {
                    TempData["Ok"] = "Parto registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "No se pudo registrar el parto.";
                CargarViewBags(parto);
                return View(parto);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                CargarViewBags(parto);
                return View(parto);
            }
        }

        // GET: /Parto/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var parto = logParto.Instancia.BuscarParto(id);
            if (parto == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            CargarViewBags(parto);
            return View(parto); // Views/Parto/Editar.cshtml
        }

        // POST: /Parto/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(entParto parto)
        {
            try
            {
                if (parto.IdViaParto == 3 && string.IsNullOrWhiteSpace(parto.IndicacionCesarea))
                    ModelState.AddModelError(nameof(parto.IndicacionCesarea), "Debe indicar el motivo de la cesárea.");

                if (!ModelState.IsValid)
                {
                    CargarViewBags(parto);
                    return View(parto);
                }
                bool editado = logParto.Instancia.EditarParto(parto);
                if (editado)
                {
                    TempData["Ok"] = "Parto actualizado.";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = "No se pudo actualizar el registro.";
                CargarViewBags(parto);
                return View(parto);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar: " + ex.Message;
                CargarViewBags(parto);
                return View(parto);
            }
        }

        // GET: /Parto/Detalles/5
        [HttpGet]
        public IActionResult Detalles(int id)
        {
            var parto = logParto.Instancia.BuscarParto(id);
            if (parto == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(parto); // Views/Parto/Detalles.cshtml
        }

        // GET: /Parto/Anular/5
        [HttpGet]
        [Authorize(Roles = "ADMIN")] // 🔐 SOLO ADMIN VE LA CONFIRMACIÓN DE ANULAR
        public IActionResult Anular(int id)
        {
            var parto = logParto.Instancia.BuscarParto(id);
            if (parto == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(parto); // Views/Parto/Anular.cshtml
        }

        // POST: /Parto/Anular
        [HttpPost]
        [Authorize(Roles = "ADMIN")] // 🔐 SOLO ADMIN EJECUTA LA ANULACIÓN
        [ValidateAntiForgeryToken]
        public IActionResult Anular(entParto parto)
        {
            try
            {
                bool resultado = logParto.Instancia.AnularParto(parto.IdParto);
                TempData[resultado ? "Ok" : "Error"] = resultado
                    ? "Registro de parto anulado."
                    : "No se pudo anular.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetEncuentrosPorEmbarazo(int idEmbarazo)
        {
            try
            {
                var lista = logEncuentro.Instancia.ListarEncuentrosPorEmbarazo(idEmbarazo);
                return Json(lista); // Devuelve la lista como JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // --- Método privado para cargar DropDownLists / modals ---
        private void CargarViewBags(entParto? parto)
        {
            try
            {
                // ====== EMBARAZOS ======
                var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);

                ViewBag.ListaEmbarazos = new SelectList(
                    embarazos.Select(e => new { e.IdEmbarazo, Nombre = $"ID: {e.IdEmbarazo} - {e.NombrePaciente}" }),
                    "IdEmbarazo",
                    "Nombre",
                    parto?.IdEmbarazo
                );

                ViewBag.EmbarazosModal = embarazos;

                // ====== PROFESIONALES ======
                var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);

                ViewBag.ListaProfesionales = new SelectList(
                    profesionales.Select(p => new { p.IdProfesional, Nombre = $"{p.Nombres} {p.Apellidos} (CMP: {p.CMP})" }),
                    "IdProfesional",
                    "Nombre",
                    parto?.IdProfesional
                );

                ViewBag.ProfesionalesModal = profesionales;

                // ====== ENCUENTROS ======
                var encuentros = logEncuentro.Instancia.ListarEncuentros();
                ViewBag.EncuentrosModal = encuentros;

                // ====== VÍAS DE PARTO ======
                var vias = logViaParto.Instancia.ListarViasParto();
                ViewBag.ListaViasParto = new SelectList(
                    vias,
                    "IdViaParto",
                    "Descripcion",
                    parto?.IdViaParto
                );

                // ====== LÍQUIDO AMNIÓTICO ======
                var liquidos = logLiquidoAmniotico.Instancia.ListarLiquidos();
                ViewBag.ListaLiquidos = new SelectList(
                    liquidos,
                    "IdLiquido",
                    "Descripcion",
                    parto?.IdLiquido
                );
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
            }
        }
        [HttpGet]
        public IActionResult DescargarPdf(int id)
        {
            try
            {
                var parto = logParto.Instancia.BuscarParto(id);
                if (parto == null)
                {
                    TempData["Error"] = "Parto no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var pdfService = HttpContext.RequestServices.GetService<IPdfService>();
                var pdfBytes = pdfService.GeneratePartoPdf(parto);

                return File(pdfBytes, "application/pdf", $"Parto_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al generar PDF: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private int? GetIdProfesional()
        {
            if (User.IsInRole("PERSONAL_SALUD"))
            {
                var idUsuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    var profesional = logProfesionalSalud.Instancia
                        .ListarProfesionalSalud(true)
                        .FirstOrDefault(p => p.IdUsuario == idUsuario);
                    return profesional?.IdProfesional;
                }
            }
            return null;
        }
    }
}
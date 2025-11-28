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
    [Authorize(Roles = "ADMIN,PERSONAL_SALUD,SECRETARIA")]
    public class SeguimientoPuerperioController : Controller
    {
        // GET: /SeguimientoPuerperio/Listar
        [HttpGet]
        public IActionResult Listar(bool mostrarActivos = true)
        {
            try
            {
                ViewBag.MostrandoActivos = mostrarActivos;
                var lista = logSeguimientoPuerperio.Instancia.ListarSeguimiento(mostrarActivos);
                // Para PERSONAL_SALUD, setear IdProfesionalActual para restringir eliminación
                if (User.IsInRole("PERSONAL_SALUD"))
                {
                    var idProfesional = GetIdProfesional();
                    if (idProfesional.HasValue)
                    {
                        ViewBag.IdProfesionalActual = idProfesional.Value;
                    }
                }
                return View(lista); // Views/SeguimientoPuerperio/Listar.cshtml
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista: " + ex.Message;
                return View(new List<entSeguimientoPuerperio>());
            }
        }

        // GET: /SeguimientoPuerperio/Registrar
        [HttpGet]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
        public IActionResult Registrar(int? idEmbarazo)
        {
            var modelo = new entSeguimientoPuerperio
            {
                IdEmbarazo = idEmbarazo ?? 0,
                Fecha = DateTime.Now.Date,
                Estado = true
            };

            CargarViewBags(modelo);
            return View(modelo);   // Views/SeguimientoPuerperio/Registrar.cshtml
        }

        // POST: /SeguimientoPuerperio/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
        public IActionResult Registrar(entSeguimientoPuerperio control)
        {
            try
            {
                if (control.IdEmbarazo <= 0)
                    ModelState.AddModelError(nameof(control.IdEmbarazo), "Debe seleccionar un embarazo válido.");

                if (control.Fecha == default(DateTime))
                    ModelState.AddModelError(nameof(control.Fecha), "Debe ingresar una fecha válida.");

                if (!ModelState.IsValid)
                {
                    CargarViewBags(control);
                    return View(control);
                }

                bool registrado = logSeguimientoPuerperio.Instancia.InsertarSeguimiento(control);

                if (registrado)
                {
                    TempData["Ok"] = "Seguimiento de puerperio registrado.";
                    return RedirectToAction(nameof(Listar));
                }

                ViewBag.Error = "No se pudo registrar el seguimiento.";
                CargarViewBags(control);
                return View(control);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                CargarViewBags(control);
                return View(control);
            }
        }

        // GET: /SeguimientoPuerperio/Editar/5
        [HttpGet]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
        public IActionResult Editar(int id)
        {
            var control = logSeguimientoPuerperio.Instancia.BuscarSeguimiento(id);
            if (control == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Listar));
            }

            CargarViewBags(control);
            return View(control); // Views/SeguimientoPuerperio/Editar.cshtml
        }

        // POST: /SeguimientoPuerperio/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
        public IActionResult Editar(entSeguimientoPuerperio control)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    CargarViewBags(control);
                    return View(control);
                }

                bool editado = logSeguimientoPuerperio.Instancia.EditarSeguimiento(control);
                if (editado)
                {
                    TempData["Ok"] = "Seguimiento actualizado.";
                    return RedirectToAction(nameof(Listar));
                }

                ViewBag.Error = "No se pudo actualizar el seguimiento.";
                CargarViewBags(control);
                return View(control);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar: " + ex.Message;
                CargarViewBags(control);
                return View(control);
            }
        }

        // GET: /SeguimientoPuerperio/Detalles/5
        [HttpGet]
        public IActionResult Detalles(int id)
        {
            var control = logSeguimientoPuerperio.Instancia.BuscarSeguimiento(id);
            if (control == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            return View(control); // Views/SeguimientoPuerperio/Detalles.cshtml
        }

        // GET: /SeguimientoPuerperio/Inhabilitar/5
        [HttpGet]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
        public IActionResult Inhabilitar(int id)
        {
            var control = logSeguimientoPuerperio.Instancia.BuscarSeguimiento(id);
            if (control == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            return View(control); // Views/SeguimientoPuerperio/Inhabilitar.cshtml
        }

        // POST: /SeguimientoPuerperio/Inhabilitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
        public IActionResult Inhabilitar(entSeguimientoPuerperio control)
        {
            try
            {
                bool resultado = logSeguimientoPuerperio.Instancia.InhabilitarSeguimiento(control.IdPuerperio);
                TempData[resultado ? "Ok" : "Error"] = resultado
                    ? "Seguimiento inhabilitado."
                    : "No se pudo inhabilitar.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al inhabilitar: " + ex.Message;
            }
            return RedirectToAction(nameof(Listar));
        }

        // -------- Cargar combos + datos para modals --------
        private void CargarViewBags(entSeguimientoPuerperio? entidad)
        {
            try
            {
                // EMBARAZOS ACTIVOS
                var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);
                ViewBag.ListaEmbarazos = new SelectList(
                    embarazos.Select(e => new { e.IdEmbarazo, Nombre = $"ID: {e.IdEmbarazo} - {e.NombrePaciente}" }),
                    "IdEmbarazo", "Nombre", entidad?.IdEmbarazo
                );
                ViewBag.EmbarazosModal = embarazos;

                // PROFESIONALES ACTIVOS
                var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);
                ViewBag.ListaProfesionales = new SelectList(
                    profesionales.Select(p => new { p.IdProfesional, Nombre = $"{p.Nombres} {p.Apellidos} (CMP: {p.CMP})" }),
                    "IdProfesional", "Nombre", entidad?.IdProfesional
                );
                ViewBag.ProfesionalesModal = profesionales;

                // MÉTODOS PF
                var metodos = logMetodoPF.Instancia.ListarMetodosPF() ?? new List<entMetodoPF>();

                ViewBag.ListaMetodosPF = new SelectList(
                    metodos,          // lista de entMetodoPF
                    "IdMetodoPF",     // value
                    "Nombre",         // text
                    entidad?.IdMetodoPF
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
                var seguimiento = logSeguimientoPuerperio.Instancia.BuscarSeguimiento(id);
                if (seguimiento == null)
                {
                    TempData["Error"] = "Seguimiento no encontrado.";
                    return RedirectToAction(nameof(Listar));
                }

                var pdfService = HttpContext.RequestServices.GetService<IPdfService>();
                var pdfBytes = pdfService.GenerateSeguimientoPuerperioPdf(seguimiento);

                return File(pdfBytes, "application/pdf", $"Puerperio_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al generar PDF: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        private int? GetIdProfesional()
        {
            if (User.IsInRole("PERSONAL_SALUD"))
            {
                var idUsuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    var profesional = logProfesionalSalud.Instancia.ListarProfesionalSalud(true)
                        .FirstOrDefault(p => p.IdUsuario == idUsuario);
                    return profesional?.IdProfesional;
                }
            }
            return null;
        }
    }
}
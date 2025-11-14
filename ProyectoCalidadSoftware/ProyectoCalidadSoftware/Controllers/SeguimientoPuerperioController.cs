using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoCalidadSoftware.Controllers
{
    // [Authorize(Roles = "PERSONAL_SALUD,SECRETARIA,ADMIN")]
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

                // MÉTODOS PF  ✅ AQUÍ EL CAMBIO
                var metodos = logMetodoPF.Instancia.ListarMetodosPF() ?? new List<entMetodoPF>();

                ViewBag.ListaMetodosPF = new SelectList(
                    metodos,          // lista de entMetodoPF
                    "IdMetodoPF",     // value
                    "Nombre",         // text (antes pusiste "Descripcion")
                    entidad?.IdMetodoPF
                );
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
            }
        }
    }
}
using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class ProfesionalSaludController : Controller
    {
        // GET: /ProfesionalSalud/Index
        public IActionResult Index(bool mostrarActivos = true)
        {
            List<entProfesionalSalud> lista;
            ViewBag.MostrandoActivos = mostrarActivos;
            try
            {
                lista = logProfesionalSalud.Instancia.ListarProfesionalSalud(mostrarActivos);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista: " + ex.Message;
                lista = new List<entProfesionalSalud>();
            }
            return View(lista);
        }

        // GET: /ProfesionalSalud/InsertarProfesional
        public IActionResult InsertarProfesional()
        {
            // Necesitamos la entidad para los campos Email/Telefono
            return View(new entProfesionalSalud());
        }

        // POST: /ProfesionalSalud/InsertarProfesional
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertarProfesional(entProfesionalSalud profesional)
        {
            // --- VALIDACIONES ---
            if (string.IsNullOrWhiteSpace(profesional.CMP))
                ModelState.AddModelError(nameof(profesional.CMP), "El CMP es obligatorio.");
            if (string.IsNullOrWhiteSpace(profesional.Nombres))
                ModelState.AddModelError(nameof(profesional.Nombres), "El Nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(profesional.Apellidos))
                ModelState.AddModelError(nameof(profesional.Apellidos), "El Apellido es obligatorio.");
            if (string.IsNullOrWhiteSpace(profesional.EmailPrincipal))
                ModelState.AddModelError(nameof(profesional.EmailPrincipal), "El Email es obligatorio.");
            if (string.IsNullOrWhiteSpace(profesional.TelefonoPrincipal))
                ModelState.AddModelError(nameof(profesional.TelefonoPrincipal), "El Teléfono es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    // El SP se encarga del 'Estado = true' y de crear el Usuario
                    int idGenerado = logProfesionalSalud.Instancia.InsertarProfesionalSalud(profesional);

                    if (idGenerado > 0)
                    {
                        TempData["MensajeExito"] = $"Profesional {profesional.Nombres} registrado con ID: {idGenerado}";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.MensajeError = "No se pudo registrar el profesional.";
                        return View(profesional);
                    }
                }
                catch (System.Exception ex)
                {
                    ViewBag.MensajeError = "Ocurrió un error en el servidor: " + ex.Message;
                    return View(profesional);
                }
            }
            return View(profesional);
        }

        // GET: /ProfesionalSalud/EditarProfesional/5
        public IActionResult EditarProfesional(int id)
        {
            try
            {
                entProfesionalSalud profesional = logProfesionalSalud.Instancia.BuscarProfesionalSaludPorId(id);
                if (profesional == null)
                {
                    TempData["MensajeError"] = "No se encontró el profesional solicitado.";
                    return RedirectToAction("Index");
                }
                // La vista "EditarProfesional" usará el modelo con Email/Telefono
                return View(profesional);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cargar el profesional: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: /ProfesionalSalud/EditarProfesional/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarProfesional(entProfesionalSalud profesional)
        {
            // (Puedes añadir las mismas validaciones del Insertar aquí)
            if (ModelState.IsValid)
            {
                try
                {
                    bool exito = logProfesionalSalud.Instancia.EditarProfesionalSalud(profesional);
                    if (exito)
                    {
                        TempData["MensajeExito"] = $"Profesional {profesional.Nombres} actualizado.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.MensajeError = "No se pudo actualizar el profesional.";
                        return View(profesional);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.MensajeError = "Ocurrió un error en el servidor: " + ex.Message;
                    return View(profesional);
                }
            }
            return View(profesional);
        }

        // GET: /ProfesionalSalud/DetallesProfesional/5
        public IActionResult DetallesProfesional(int id)
        {
            try
            {
                entProfesionalSalud profesional = logProfesionalSalud.Instancia.BuscarProfesionalSaludPorId(id);
                if (profesional == null)
                {
                    TempData["MensajeError"] = "No se encontró el profesional solicitado.";
                    return RedirectToAction("Index");
                }
                return View(profesional);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cargar los detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // --- MÉTODOS AÑADIDOS PARA INHABILITAR ---

        // GET: /ProfesionalSalud/Inhabilitar/5
        public IActionResult Inhabilitar(int id)
        {
            try
            {
                entProfesionalSalud profesional = logProfesionalSalud.Instancia.BuscarProfesionalSaludPorId(id);
                if (profesional == null)
                {
                    TempData["MensajeError"] = "No se encontró el profesional.";
                    return RedirectToAction("Index");
                }
                return View(profesional); // Muestra la vista de confirmación
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cargar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: /ProfesionalSalud/Inhabilitar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Inhabilitar(entProfesionalSalud profesional)
        {
            try
            {
                bool exito = logProfesionalSalud.Instancia.InhabilitarProfesionalSalud(profesional.IdProfesional);
                if (exito)
                {
                    TempData["MensajeExito"] = "Profesional inhabilitado correctamente.";
                }
                else
                {
                    TempData["MensajeError"] = "No se pudo inhabilitar al profesional.";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al inhabilitar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
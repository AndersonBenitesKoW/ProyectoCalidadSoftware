using Microsoft.AspNetCore.Mvc;
using CapaLogica; // Asegúrate de tener la referencia a CapaLogica
using CapaEntidad; // Asegúrate de tener la referencia a CapaEntidad
using System.Collections.Generic;
using System.Linq;

namespace ProyectoCalidadSoftware.Controllers
{
    public class ProfesionalSaludController : Controller
    {
        // GET: /ProfesionalSalud/Index
        // Esta acción obtiene y muestra la lista de profesionales.
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
                ViewBag.Error = "Error al cargar la lista de profesionales: " + ex.Message;
                lista = new List<entProfesionalSalud>();
            }

            return View(lista);
        }

        // GET: /ProfesionalSalud/InsertarProfesional
        // Muestra la vista con el formulario para registrar un nuevo profesional.
        public IActionResult InsertarProfesional()
        {
            return View();
        }

        // POST: /ProfesionalSalud/InsertarProfesional
        // Procesa el formulario y registra un nuevo profesional.
        [HttpPost]
        [ValidateAntiForgeryToken] // Buena práctica de seguridad para formularios POST
        public IActionResult InsertarProfesional(entProfesionalSalud profesional)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    profesional.Estado = true;

                    if (profesional.IdUsuario == null)
                    {
                        
                    }

                    int idGenerado = logProfesionalSalud.Instancia.InsertarProfesionalSalud(profesional);

                    if (idGenerado > 0)
                    {
                        TempData["MensajeExito"] = $"Profesional {profesional.Nombres} registrado con ID: {idGenerado}";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.MensajeError = "No se pudo registrar el profesional. Intente de nuevo.";
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
                return View(profesional);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cargar el profesional: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarProfesional(entProfesionalSalud profesional)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool exito = logProfesionalSalud.Instancia.EditarProfesionalSalud(profesional);

                    if (exito)
                    {
                        TempData["MensajeExito"] = $"Profesional {profesional.Nombres} actualizado correctamente.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.MensajeError = "No se pudo actualizar el profesional (la base de datos no reportó filas afectadas).";
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
    }
}
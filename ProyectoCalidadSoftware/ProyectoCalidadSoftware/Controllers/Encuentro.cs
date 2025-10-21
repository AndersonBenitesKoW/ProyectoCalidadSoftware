using Microsoft.AspNetCore.Mvc;
using CapaLogica;
using CapaEntidad;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoCalidadSoftware.Controllers
{
    public class EncuentroController : Controller
    {
        /// <summary>
        /// GET: /Encuentro/RegistrarEncuentro
        /// Muestra el formulario para registrar un nuevo encuentro.
        /// </summary>
        public IActionResult RegistrarEncuentro()
        {
            CargarViewBags(null);

            var modelo = new entEncuentro
            {
                FechaHoraInicio = DateTime.Now, 
                Estado = "Abierto" 
            };
            return View(modelo);
        }

        /// <summary>
        /// POST: /Encuentro/RegistrarEncuentro
        /// Procesa el formulario.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarEncuentro(entEncuentro encuentro)
        {
            if (ModelState.IsValid && encuentro.IdEmbarazo > 0 && encuentro.IdTipoEncuentro > 0)
            {
                try
                {
                    int idGenerado = logEncuentro.Instancia.RegistrarEncuentro(encuentro);
                    if (idGenerado > 0)
                    {
                        TempData["MensajeExito"] = $"Encuentro ID {idGenerado} registrado exitosamente.";

                        return RedirectToAction("RegistrarEncuentro");
                    }
                    else
                    {
                        ViewBag.MensajeError = "No se pudo registrar el encuentro.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.MensajeError = "Error en el servidor: " + ex.Message;
                }
            }
            else
            {
                ViewBag.MensajeError = "Datos inválidos. Revise el formulario.";
            }

            CargarViewBags(encuentro);
            return View(encuentro);
        }

        /// <summary>
        /// Carga los ViewBags para los dropdowns del formulario de Encuentro.
        /// </summary>
        private void CargarViewBags(entEncuentro? encuentro)
        {
            try
            {
                var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);

                var listaEmbarazosFormateada = embarazos.Select(e => new SelectListItem
                {
                    Value = e.IdEmbarazo.ToString(),
                    Text = $"ID: {e.IdEmbarazo} - {e.NombrePaciente} (FPP: {e.FPP?.ToString("dd/MM/yyyy") ?? "N/A"})"
                });
                ViewBag.ListaEmbarazos = new SelectList(listaEmbarazosFormateada, "Value", "Text", encuentro?.IdEmbarazo);

                var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);
                ViewBag.ListaProfesionales = new SelectList(profesionales, "IdProfesional", "Nombres", encuentro?.IdProfesional);

                var tipos = logTipoEncuentro.Instancia.Listar();
                ViewBag.ListaTipos = new SelectList(tipos, "IdTipoEncuentro", "Descripcion", encuentro?.IdTipoEncuentro);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error cargando listas: " + ex.Message;
                ViewBag.ListaEmbarazos = new SelectList(new List<SelectListItem>());
                ViewBag.ListaProfesionales = new SelectList(new List<entProfesionalSalud>());
                ViewBag.ListaTipos = new SelectList(new List<entTipoEncuentro>());
            }
        }
    }
}
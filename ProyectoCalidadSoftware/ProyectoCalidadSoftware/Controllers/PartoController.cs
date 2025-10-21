﻿using Microsoft.AspNetCore.Mvc;
using CapaLogica;
using CapaEntidad;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace ProyectoCalidadSoftware.Controllers
{
    public class PartoController : Controller
    {
        /// <summary>
        /// 🚀 NUEVA ACCIÓN: Muestra el listado de partos.
        /// </summary>
        public IActionResult Index(bool mostrarActivos = true)
        {
            List<entParto> lista;
            try
            {
                ViewBag.MostrandoActivos = mostrarActivos;
                lista = logParto.Instancia.ListarPartos(mostrarActivos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista de partos: " + ex.Message;
                lista = new List<entParto>();
            }
            return View(lista);
        }

        /// <summary>
        /// GET: /Parto/RegistrarParto
        /// </summary>
        public IActionResult RegistrarParto()
        {
            CargarViewBags(null);
            return View(new entParto());
        }

        /// <summary>
        /// POST: /Parto/RegistrarParto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarParto(entParto parto, List<string> intervenciones)
        {
            if (intervenciones != null)
            {
                foreach (var intervencionTexto in intervenciones)
                {
                    if (!string.IsNullOrWhiteSpace(intervencionTexto))
                    {
                        parto.Intervenciones.Add(new entPartoIntervencion { Intervencion = intervencionTexto });
                    }
                }
            }
            parto.Estado = true;

            if (ModelState.IsValid && parto.IdEmbarazo > 0 && parto.IdEncuentro > 0) 
            {
                try
                {
                    int idGenerado = logParto.Instancia.RegistrarParto(parto);
                    if (idGenerado > 0)
                    {
                        TempData["MensajeExito"] = $"Parto registrado correctamente con ID: {idGenerado}.";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.MensajeError = "No se pudo registrar el parto. La base de datos no devolvió un ID.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.MensajeError = "Ocurrió un error en el servidor: " + ex.Message;
                }
            }
            else
            {
                if (parto.IdEmbarazo <= 0)
                {
                    ViewBag.MensajeError = "Error: Debe seleccionar un embarazo de la lista.";
                }
                else if (parto.IdEncuentro <= 0) 
                {
                    ViewBag.MensajeError = "Error: Debe seleccionar un encuentro de la lista.";
                }
                else
                {
                    ViewBag.MensajeError = "Datos inválidos. Revise el formulario.";
                }
            }

            CargarViewBags(parto);
            return View(parto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnularParto(int idParto) 
        {
            try
            {
                if (idParto <= 0)
                {
                    throw new ArgumentException("ID de Parto no válido.");
                }

                bool exito = logParto.Instancia.AnularParto(idParto);

                if (exito)
                {
                    TempData["MensajeExito"] = $"Parto ID {idParto} anulado correctamente.";
                }
                else
                {
                    TempData["MensajeError"] = $"No se pudo anular el Parto ID {idParto}.";
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al intentar anular el parto: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult DetallesParto(int id)
        {
            try
            {
                entParto? parto = logParto.Instancia.BuscarPartoPorId(id);

                if (parto == null)
                {
                    TempData["MensajeError"] = "No se encontró el registro de parto solicitado.";
                    return RedirectToAction("Index");
                }

                return View(parto); 
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cargar los detalles del parto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }



        [HttpGet]
        public JsonResult GetEncuentros(int idEmbarazo)
        {
            try
            {
                var encuentros = logEncuentro.Instancia.ListarPorEmbarazoYTipo(idEmbarazo, "INTRAPARTO");
                var listaFormateada = encuentros.Select(e => new SelectListItem
                {
                    Value = e.IdEncuentro.ToString(),
                    Text = $"ID: {e.IdEncuentro} (Inicio: {e.FechaHoraInicio:dd/MM/yy HH:mm} - Estado: {e.Estado})"
                });
                return Json(listaFormateada);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// MÉTODO PRIVADO para cargar todos los ViewBag de los Dropdowns.
        /// </summary>
        private void CargarViewBags(entParto? parto)
        {
            try
            {
                var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);
                var listaEmbarazosFormateada = embarazos.Select(e => new SelectListItem
                {
                    Value = e.IdEmbarazo.ToString(),
                    Text = $"ID: {e.IdEmbarazo} - {e.NombrePaciente} (FPP: {e.FPP?.ToString("dd/MM/yyyy") ?? "N/A"})"
                });
                ViewBag.ListaEmbarazos = new SelectList(listaEmbarazosFormateada, "Value", "Text", parto?.IdEmbarazo);

                var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);
                ViewBag.ListaProfesionales = new SelectList(profesionales, "IdProfesional", "Nombres", parto?.IdProfesional);

                var viasParto = logViaParto.Instancia.Listar();
                ViewBag.ListaViasParto = new SelectList(viasParto, "IdViaParto", "Descripcion", parto?.IdViaParto);

                var liquidos = logLiquidoAmniotico.Instancia.Listar();
                ViewBag.ListaLiquidos = new SelectList(liquidos, "IdLiquido", "Descripcion", parto?.IdLiquido);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error fatal al cargar listas desplegables: " + ex.Message;
                ViewBag.ListaEmbarazos = new SelectList(new List<SelectListItem>());
                ViewBag.ListaProfesionales = new SelectList(new List<entProfesionalSalud>());
                ViewBag.ListaViasParto = new SelectList(new List<entViaParto>());
                ViewBag.ListaLiquidos = new SelectList(new List<entLiquidoAmniotico>());
            }
        }
    }
}
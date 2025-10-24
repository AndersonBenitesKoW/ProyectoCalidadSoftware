﻿using Microsoft.AspNetCore.Mvc;
using CapaLogica; 
using CapaEntidad; 
using System;
using Microsoft.AspNetCore.Mvc.Rendering; 
using System.Collections.Generic;
using System.Linq; 

namespace ProyectoCalidadSoftware.Controllers
{
    public class EmbarazoController : Controller
    {
        /// <summary>
        /// 🚀 NUEVA ACCIÓN: Muestra el listado de embarazos.
        /// </summary>
        public IActionResult Index(bool mostrarActivos = true)
        {
            List<entEmbarazo> lista;
            try
            {
                ViewBag.MostrandoActivos = mostrarActivos;
                lista = logEmbarazo.Instancia.ListarEmbarazosPorEstado(mostrarActivos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista de embarazos: " + ex.Message;
                lista = new List<entEmbarazo>();
            }
            return View(lista);
        }

        public IActionResult DetallesEmbarazo(int id)
        {
            try
            {
                entEmbarazo? embarazo = logEmbarazo.Instancia.BuscarEmbarazoPorId(id);

                if (embarazo == null)
                {
                    TempData["MensajeError"] = "No se encontró el registro de embarazo solicitado.";
                    return RedirectToAction("Index");
                }

                return View(embarazo); 
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cargar los detalles del embarazo: " + ex.Message;
                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// GET: /Embarazo/RegistrarEmbarazo
        /// Carga la lista de pacientes y muestra el formulario.
        /// </summary>
        public IActionResult RegistrarEmbarazo()
        {
            CargarViewBags(null);
            return View(new entEmbarazo());
        }

        /// <summary>
        /// POST: /Embarazo/RegistrarEmbarazo
        /// Procesa el formulario, registra y redirige a sí mismo.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarEmbarazo(entEmbarazo embarazo)
        {
            if (ModelState.IsValid && embarazo.IdPaciente > 0)
            {
                try
                {
                    int idGenerado = logEmbarazo.Instancia.RegistrarEmbarazo(embarazo);

                    if (idGenerado > 0)
                    {
                        TempData["MensajeExito"] = $"Embarazo registrado con ID: {idGenerado} para la Paciente ID: {embarazo.IdPaciente}.";

                        return RedirectToAction("RegistrarEmbarazo");
                    }
                    else
                    {
                        ViewBag.MensajeError = "No se pudo registrar el embarazo (la base de datos no devolvió ID).";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.MensajeError = "Ocurrió un error en el servidor: " + ex.Message;
                }
            }
            else
            {
                if (embarazo.IdPaciente <= 0)
                {
                    ViewBag.MensajeError = "Error: Debe seleccionar una paciente de la lista.";
                }
                else
                {
                    ViewBag.MensajeError = "Datos inválidos. Revise el formulario.";
                }
            }

            CargarViewBags(embarazo);
            return View(embarazo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarEmbarazo(int id)
        {
            try
            {
                // 1. Llama al método que YA EXISTE en tu CapaLogica
                bool exito = logEmbarazo.Instancia.CerrarEmbarazo(id);

                if (exito)
                {
                    // 2. Envía un mensaje de éxito al _Layout
                    TempData["MensajeExito"] = "El embarazo se ha cerrado/anulado correctamente.";
                }
                else
                {
                    // 3. Envía un mensaje de error
                    TempData["MensajeError"] = "No se pudo cerrar el embarazo. Es posible que ya estuviera cerrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al intentar cerrar el embarazo: " + ex.Message;
            }

            // 4. Redirige de vuelta al listado
            return RedirectToAction("Index");
        }


        /// <summary>
        /// MÉTODO PRIVADO para cargar todos los ViewBags necesarios (actualmente solo Pacientes).
        /// </summary>
        private void CargarViewBags(entEmbarazo? embarazo)
        {
            try
            {
                var pacientes = logPaciente.Instancia.ListarPacientesActivos();
                var listaPacientesFormateada = pacientes.Select(p => new SelectListItem
                {
                    Value = p.IdPaciente.ToString(),
                    Text = $"{p.NombreCompleto} (DNI: {p.DNI ?? "N/A"})"
                });
                ViewBag.ListaPacientes = new SelectList(listaPacientesFormateada, "Value", "Text", embarazo?.IdPaciente);

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista de pacientes: " + ex.Message;
                ViewBag.ListaPacientes = new SelectList(new List<SelectListItem>());
            }
        }
    }
}
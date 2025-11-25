using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoCalidadSoftware.Controllers
{
    // Asumo que quieres la misma seguridad que Citas
    [Authorize(Roles = "PERSONAL_SALUD,SECRETARIA,ADMIN")]
    public class EncuentroController : Controller
    {
        // GET: /Encuentro/Listar
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var lista = logEncuentro.Instancia.ListarEncuentros();
                return View(lista); // Views/Encuentro/Listar.cshtml
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista: " + ex.Message;
                return View(new List<entEncuentro>());
            }
        }

        // GET: /Encuentro/Insertar
        [HttpGet]
        public IActionResult Insertar()
        {
            CargarViewBags(null); // Carga los dropdowns
            return View(new entEncuentro()); // Views/Encuentro/Insertar.cshtml
        }

        // POST: /Encuentro/Insertar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(entEncuentro entidad)
        {
            try
            {
                if (entidad.IdEmbarazo <= 0)
                    ModelState.AddModelError(nameof(entidad.IdEmbarazo), "Seleccione un embarazo.");
                if (entidad.IdTipoEncuentro <= 0)
                    ModelState.AddModelError(nameof(entidad.IdTipoEncuentro), "Seleccione el tipo de encuentro.");
                if (entidad.IdProfesional <= 0)
                    ModelState.AddModelError(nameof(entidad.IdProfesional), "Seleccione un profesional.");

                if (!ModelState.IsValid)
                {
                    CargarViewBags(entidad);
                    return View(entidad);
                }

                bool ok = Convert.ToBoolean(logEncuentro.Instancia.InsertarEncuentro(entidad));
                if (ok)
                {
                    TempData["Ok"] = "Encuentro registrado correctamente.";
                    return RedirectToAction(nameof(Listar));
                }

                ViewBag.Error = "No se pudo registrar el encuentro.";
                CargarViewBags(entidad);
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                CargarViewBags(entidad);
                return View(entidad);
            }
        }

        // GET: /Encuentro/Detalles/5
        [HttpGet]
        public IActionResult Detalles(int id)
        {
            var encuentro = logEncuentro.Instancia.BuscarEncuentro(id);
            if (encuentro == null)
            {
                TempData["Error"] = "Encuentro no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            return View(encuentro); // Views/Encuentro/Detalles.cshtml
        }

        // GET: /Encuentro/Anular/5
        [HttpGet]
        [Authorize(Roles = "ADMIN")] // 👈 solo ADMIN ve esta pantalla
        public IActionResult Anular(int id)
        {
            var encuentro = logEncuentro.Instancia.BuscarEncuentro(id);
            if (encuentro == null)
            {
                TempData["Error"] = "Encuentro no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            return View(encuentro); // Views/Encuentro/Anular.cshtml
        }

        // POST: /Encuentro/Anular/5
        [HttpPost]
        [ActionName("Anular")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")] // 👈 solo ADMIN confirma la anulación
        public IActionResult AnularConfirmado(entEncuentro entidad)
        {
            try
            {
                bool ok = logEncuentro.Instancia.AnularEncuentro(entidad.IdEncuentro);
                TempData[ok ? "Ok" : "Error"] = ok ? "Encuentro anulado." : "No se pudo anular el encuentro.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular: " + ex.Message;
            }
            return RedirectToAction(nameof(Listar));
        }

        // --- Método privado para cargar DropDownLists ---
        private void CargarViewBags(entEncuentro? entidad)
        {
            try
            {
                // 1) Embarazos activos
                var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);

                // Para modales (lista cruda)
                ViewBag.EmbarazosModal = embarazos;

                ViewBag.ListaEmbarazos = new SelectList(
                    embarazos.Select(e => new
                    {
                        e.IdEmbarazo,
                        Nombre = $"ID: {e.IdEmbarazo} - {e.NombrePaciente}"
                    }),
                    "IdEmbarazo",
                    "Nombre",
                    entidad?.IdEmbarazo
                );

                // 2) Profesionales activos
                var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);

                ViewBag.ProfesionalesModal = profesionales;

                ViewBag.ListaProfesionales = new SelectList(
                    profesionales.Select(p => new
                    {
                        p.IdProfesional,
                        Nombre = $"{p.Nombres} {p.Apellidos} (CMP: {p.CMP})"
                    }),
                    "IdProfesional",
                    "Nombre",
                    entidad?.IdProfesional
                );

                // 3) Tipos de encuentro
                var tipos = logTipoEncuentro.Instancia.ListarTiposEncuentro();
                ViewBag.ListaTiposEncuentro = new SelectList(
                    tipos,
                    "IdTipoEncuentro",
                    "Descripcion",
                    entidad?.IdTipoEncuentro
                );
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
            }
        }
    }
}
using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoCalidadSoftware.Controllers
{
    // Asumo que quieres la misma seguridad que Citas
    // [Authorize(Roles = "PERSONAL_SALUD,SECRETARIA,ADMIN")]
    public class ControlPrenatalController : Controller
    {
        // GET: /ControlPrenatal/Listar
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var lista = logControlPrenatal.Instancia.ListarControlPrenatal();
                return View(lista); // Views/ControlPrenatal/Listar.cshtml
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista: " + ex.Message;
                return View(new List<entControlPrenatal>());
            }
        }

        // GET: /ControlPrenatal/Registrar
        [HttpGet]
        public IActionResult Registrar(int? idEmbarazo)
        {
            var modelo = new entControlPrenatal
            {
                IdEmbarazo = idEmbarazo ?? 0,
                Fecha = DateTime.Now, // fecha actual
                Estado = true
            };

            CargarViewBags(modelo); // <--- pásale el modelo, no null
            return View(modelo);    // Views/ControlPrenatal/Registrar.cshtml
        }


        // POST: /ControlPrenatal/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(entControlPrenatal control)
        {
            Console.WriteLine("LOG: Iniciando registro de Control Prenatal");
            Console.WriteLine($"LOG: IdEmbarazo={control.IdEmbarazo}, Fecha={control.Fecha}, IdProfesional={control.IdProfesional}");
            try
            {
                if (control.IdEmbarazo <= 0)
                    ModelState.AddModelError(nameof(control.IdEmbarazo), "Debe seleccionar un embarazo válido.");
                if (control.Fecha == default)
                    ModelState.AddModelError(nameof(control.Fecha), "Debe ingresar una fecha válida.");
                // IdProfesional es opcional según la tabla

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("LOG: ModelState no válido");
                    CargarViewBags(control);
                    return View(control);
                }

                Console.WriteLine("LOG: ModelState válido, procediendo a registrar");

                // Obtener idProfesional del usuario logueado (asumiendo que está en sesión o claims)
                int idProfesional = control.IdProfesional ?? 1; // Usar el seleccionado o 1 por defecto
                Console.WriteLine($"LOG: Usando idProfesional={idProfesional}");
                int idControl = logControlPrenatal.Instancia.RegistrarControlPrenatalConEncuentro(control, idProfesional);
                Console.WriteLine($"LOG: idControl retornado={idControl}");

                if (idControl > 0)
                {
                    Console.WriteLine("LOG: Control registrado, procesando ayudas diagnósticas");
                    // Insertar ayudas diagnósticas
                    var embarazo = logEmbarazo.Instancia.BuscarEmbarazoPorId(control.IdEmbarazo);
                    Console.WriteLine($"LOG: Embarazo encontrado: {embarazo != null}, IdPaciente={embarazo?.IdPaciente}");
                    if (embarazo != null)
                    {
                        foreach (var ayuda in control.AyudasDiagnosticas.Where(a => a.IdTipoAyuda.HasValue && a.IdTipoAyuda > 0))
                        {
                            Console.WriteLine($"LOG: Procesando ayuda: IdTipoAyuda={ayuda.IdTipoAyuda}");
                            // Crear la orden de ayuda diagnóstica
                            var orden = new entAyudaDiagnosticaOrden
                            {
                                IdPaciente = embarazo.IdPaciente,
                                IdEmbarazo = control.IdEmbarazo,
                                IdProfesional = control.IdProfesional,
                                IdTipoAyuda = ayuda.IdTipoAyuda,
                                Descripcion = ayuda.DescripcionAyuda,
                                Urgente = ayuda.Urgente,
                                Estado = "Solicitada"
                            };
                            int idAyuda = logAyudaDiagnosticaOrden.Instancia.InsertarAyudaDiagnosticaOrden(orden);
                            Console.WriteLine($"LOG: idAyuda insertado={idAyuda}");

                            if (idAyuda > 0)
                            {
                                // Insertar en la tabla puente
                                var puente = new entControlPrenatal_AyudaDiagnostica
                                {
                                    IdControl = idControl,
                                    IdAyuda = idAyuda,
                                    FechaOrden = DateTime.Now,
                                    Comentario = ayuda.Comentario
                                };
                                logControlPrenatal_AyudaDiagnostica.Instancia.InsertarAyudaDiagnosticaAlControl(puente);
                                Console.WriteLine("LOG: Puente insertado");
                            }
                        }
                    }

                    Console.WriteLine("LOG: Registro completado exitosamente");
                    TempData["Ok"] = "Control prenatal registrado correctamente.";
                    return RedirectToAction(nameof(Listar));
                }

                Console.WriteLine("LOG: No se pudo registrar el control prenatal");
                ViewBag.Error = "No se pudo registrar el control prenatal.";
                CargarViewBags(control);
                return View(control);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG: Error en registro: {ex.Message}");
                ViewBag.Error = "Error al registrar: " + ex.Message;
                CargarViewBags(control);
                return View(control);
            }
        }

        // GET: /ControlPrenatal/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var control = logControlPrenatal.Instancia.BuscarControlPrenatal(id);
            if (control == null)
            {
                TempData["Error"] = "Control no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            // Cargar ayudas diagnósticas asociadas
            control.AyudasDiagnosticas = logControlPrenatal_AyudaDiagnostica.Instancia.ListarAyudasPorControl(id);
            CargarViewBags(control);
            return View(control); // Views/ControlPrenatal/Editar.cshtml
        }

        // POST: /ControlPrenatal/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(entControlPrenatal control)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    CargarViewBags(control);
                    return View(control);
                }

                bool editado = logControlPrenatal.Instancia.EditarControlPrenatal(control);

                if (editado)
                {
                    // Gestionar ayudas diagnósticas: eliminar existentes y reinsertar
                    var ayudasExistentes = logControlPrenatal_AyudaDiagnostica.Instancia.ListarAyudasPorControl(control.IdControlPrenatal);
                    foreach (var ayuda in ayudasExistentes)
                    {
                        logControlPrenatal_AyudaDiagnostica.Instancia.InhabilitarAyudaDiagnosticaDelControl(ayuda.IdCP_AD);
                    }

                    var embarazo = logEmbarazo.Instancia.BuscarEmbarazoPorId(control.IdEmbarazo);
                    if (embarazo != null)
                    {
                        foreach (var ayuda in control.AyudasDiagnosticas.Where(a => a.IdTipoAyuda.HasValue && a.IdTipoAyuda > 0))
                        {
                            // Crear la orden de ayuda diagnóstica
                            var orden = new entAyudaDiagnosticaOrden
                            {
                                IdPaciente = embarazo.IdPaciente,
                                IdEmbarazo = control.IdEmbarazo,
                                IdProfesional = control.IdProfesional,
                                IdTipoAyuda = ayuda.IdTipoAyuda,
                                Descripcion = ayuda.DescripcionAyuda,
                                Urgente = ayuda.Urgente,
                                Estado = "Solicitada"
                            };
                            int idAyuda = logAyudaDiagnosticaOrden.Instancia.InsertarAyudaDiagnosticaOrden(orden);

                            if (idAyuda > 0)
                            {
                                // Insertar en la tabla puente
                                var puente = new entControlPrenatal_AyudaDiagnostica
                                {
                                    IdControl = control.IdControlPrenatal,
                                    IdAyuda = idAyuda,
                                    FechaOrden = DateTime.Now,
                                    Comentario = ayuda.Comentario
                                };
                                logControlPrenatal_AyudaDiagnostica.Instancia.InsertarAyudaDiagnosticaAlControl(puente);
                            }
                        }
                    }

                    TempData["Ok"] = "Control prenatal actualizado.";
                    return RedirectToAction(nameof(Listar));
                }
                ViewBag.Error = "No se pudo actualizar el control.";
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

        // GET: /ControlPrenatal/Detalles/5
        [HttpGet]
        public IActionResult Detalles(int id)
        {
            var control = logControlPrenatal.Instancia.BuscarControlPrenatal(id);
            if (control == null)
            {
                TempData["Error"] = "Control no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            return View(control); // Views/ControlPrenatal/Detalles.cshtml
        }

        // GET: /ControlPrenatal/Inhabilitar/5
        [HttpGet]
        public IActionResult Inhabilitar(int id)
        {
            var control = logControlPrenatal.Instancia.BuscarControlPrenatal(id);
            if (control == null)
            {
                TempData["Error"] = "Control prenatal no encontrado.";
                return RedirectToAction(nameof(Listar));
            }
            return View(control); // Views/ControlPrenatal/Inhabilitar.cshtml
        }

        // POST: /ControlPrenatal/Inhabilitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Inhabilitar(entControlPrenatal control) // Recibe la entidad
        {
            try
            {
                bool resultado = logControlPrenatal.Instancia.InhabilitarControlPrenatal(control.IdControlPrenatal);
                TempData[resultado ? "Ok" : "Error"] = resultado
                    ? "Control prenatal inhabilitado."
                    : "No se pudo inhabilitar el control.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al inhabilitar: " + ex.Message;
            }
            return RedirectToAction(nameof(Listar));
        }

        // --- Método privado para cargar DropDownLists ---
        // --- Método privado para cargar DropDownLists y listas para modales ---
        private void CargarViewBags(entControlPrenatal? control)
        {
            try
            {
                // ---------- EMBARAZOS ACTIVOS ----------
                var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);

                // Para MODAL en Control Prenatal
                ViewBag.EmbarazosModalPrenatal = embarazos;

                // (Opcional) Para combobox clásico
                ViewBag.ListaEmbarazos = new SelectList(
                    embarazos.Select(e => new
                    {
                        e.IdEmbarazo,
                        Nombre = $"ID: {e.IdEmbarazo} - {e.NombrePaciente}"
                    }),
                    "IdEmbarazo",
                    "Nombre",
                    control?.IdEmbarazo
                );

                // ---------- PROFESIONALES ACTIVOS ----------
                var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);

                // Para MODAL
                ViewBag.ProfesionalesModalPrenatal = profesionales;

                // (Opcional) Para combobox
                ViewBag.ListaProfesionales = new SelectList(
                    profesionales.Select(p => new
                    {
                        p.IdProfesional,
                        Nombre = $"{p.Nombres} {p.Apellidos} (CMP: {p.CMP})"
                    }),
                    "IdProfesional",
                    "Nombre",
                    control?.IdProfesional
                );

                // ---------- TIPOS DE AYUDA DIAGNÓSTICA ----------
                var tiposAyuda = logTipoAyudaDiagnostica.Instancia.ListarTiposAyuda();
                ViewBag.TiposAyudaDiagnostica = new SelectList(tiposAyuda, "IdTipoAyuda", "Nombre");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
            }
        }

    }
}
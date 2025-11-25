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
[Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
public class ControlPrenatalController : Controller
{
    // GET: /ControlPrenatal/Listar
    [HttpGet]
    public IActionResult Listar()
    {
        try
        {
            var lista = logControlPrenatal.Instancia.ListarControlPrenatal();

            // Para PERSONAL_SALUD, setear IdProfesionalActual para restringir eliminación en la vista
            if (User.IsInRole("PERSONAL_SALUD"))
            {
                var idProfesional = GetIdProfesional();
                if (idProfesional.HasValue)
                {
                    ViewBag.IdProfesionalActual = idProfesional.Value;
                }
            }

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
            Fecha = DateTime.Now,
            Estado = true
        };

        CargarViewBags(modelo);
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

            if (!ModelState.IsValid)
            {
                Console.WriteLine("LOG: ModelState no válido");
                CargarViewBags(control);
                return View(control);
            }

            Console.WriteLine("LOG: ModelState válido, procediendo a registrar");

            int idProfesional = control.IdProfesional ?? 1; // Usar el seleccionado o 1 por defecto
            Console.WriteLine($"LOG: Usando idProfesional={idProfesional}");

            int idControl = logControlPrenatal.Instancia.RegistrarControlPrenatalConEncuentro(control, idProfesional);
            Console.WriteLine($"LOG: idControl retornado={idControl}");

            if (idControl > 0)
            {
                Console.WriteLine("LOG: Control registrado, procesando ayudas diagnósticas");

                var embarazo = logEmbarazo.Instancia.BuscarEmbarazoPorId(control.IdEmbarazo);
                Console.WriteLine($"LOG: Embarazo encontrado: {embarazo != null}, IdPaciente={embarazo?.IdPaciente}");

                if (embarazo != null && control.AyudasDiagnosticas != null)
                {
                    foreach (var ayuda in control.AyudasDiagnosticas.Where(a => a.IdTipoAyuda.HasValue && a.IdTipoAyuda > 0))
                    {
                        Console.WriteLine($"LOG: Procesando ayuda: IdTipoAyuda={ayuda.IdTipoAyuda}");

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
                var ayudasExistentes = logControlPrenatal_AyudaDiagnostica.Instancia.ListarAyudasPorControl(control.IdControlPrenatal);
                foreach (var ayuda in ayudasExistentes)
                {
                    logControlPrenatal_AyudaDiagnostica.Instancia.InhabilitarAyudaDiagnosticaDelControl(ayuda.IdCP_AD);
                }

                var embarazo = logEmbarazo.Instancia.BuscarEmbarazoPorId(control.IdEmbarazo);
                if (embarazo != null && control.AyudasDiagnosticas != null)
                {
                    foreach (var ayuda in control.AyudasDiagnosticas.Where(a => a.IdTipoAyuda.HasValue && a.IdTipoAyuda > 0))
                    {
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
    [Authorize(Roles = "ADMIN")] // 🔐 solo ADMIN puede ver pantalla de inhabilitar
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
    [Authorize(Roles = "ADMIN")] // 🔐 solo ADMIN confirma inhabilitación
    public IActionResult Inhabilitar(entControlPrenatal control)
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

    private void CargarViewBags(entControlPrenatal? control)
    {
        try
        {
            var embarazos = logEmbarazo.Instancia.ListarEmbarazosPorEstado(true);

            ViewBag.EmbarazosModalPrenatal = embarazos;

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

            var profesionales = logProfesionalSalud.Instancia.ListarProfesionalSalud(true);

            ViewBag.ProfesionalesModalPrenatal = profesionales;

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

            var tiposAyuda = logTipoAyudaDiagnostica.Instancia.ListarTiposAyuda();
            ViewBag.TiposAyudaDiagnostica = new SelectList(tiposAyuda, "IdTipoAyuda", "Nombre");
        }
        catch (Exception ex)
        {
            ViewBag.Error = "Error al cargar listas desplegables: " + ex.Message;
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
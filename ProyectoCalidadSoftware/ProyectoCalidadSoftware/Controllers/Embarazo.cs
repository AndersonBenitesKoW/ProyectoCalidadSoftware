using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "ADMIN,PERSONAL_SALUD")]
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
            System.Diagnostics.Debug.WriteLine($"[LOG] IdPaciente recibido: {embarazo.IdPaciente}");
            System.Diagnostics.Debug.WriteLine($"[LOG] ModelState.IsValid: {ModelState.IsValid}");
            System.Diagnostics.Debug.WriteLine($"[LOG] FUR: {embarazo.FUR}, FPP: {embarazo.FPP}, Riesgo: {embarazo.Riesgo}");

            if (ModelState.IsValid && embarazo.IdPaciente > 0)
            {
                System.Diagnostics.Debug.WriteLine($"[LOG] Entrando al try, IdPaciente: {embarazo.IdPaciente}");
                try
                {
                    // Verificar que el paciente existe y está activo
                    entPaciente? paciente = logPaciente.Instancia.BuscarPaciente(embarazo.IdPaciente);
                    System.Diagnostics.Debug.WriteLine($"[LOG] Paciente encontrado: {paciente != null}, Estado: {paciente?.Estado}");
                    if (paciente == null || !paciente.Estado)
                    {
                        ViewBag.MensajeError = "El paciente seleccionado no existe o no está activo.";
                        CargarViewBags(embarazo);
                        return View(embarazo);
                    }

                    int idGenerado = logEmbarazo.Instancia.RegistrarEmbarazo(embarazo);
                    System.Diagnostics.Debug.WriteLine($"[LOG] ID generado: {idGenerado}");

                    if (idGenerado > 0)
                    {
                        TempData["MensajeExito"] = $"Embarazo registrado con ID: {idGenerado} para la Paciente ID: {embarazo.IdPaciente}.";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.MensajeError = "No se pudo registrar el embarazo (la base de datos no devolvió ID).";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[LOG] Error en try: {ex.Message}");
                    ViewBag.MensajeError = "Ocurrió un error en el servidor: " + ex.Message;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[LOG] No entró al try, IdPaciente: {embarazo.IdPaciente}, ModelState errors: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
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

        // GET: /Embarazo/CerrarEmbarazo/5
        [HttpGet]
        public IActionResult CerrarEmbarazo(int id)
        {
            try
            {
                entEmbarazo? embarazo = logEmbarazo.Instancia.BuscarEmbarazoPorId(id);
                if (embarazo == null)
                {
                    TempData["MensajeError"] = "No se encontró el registro de embarazo solicitado.";
                    return RedirectToAction("Index");
                }

                return View("CerrarEmbarazo", embarazo);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cargar la confirmación: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: /Embarazo/CerrarEmbarazo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarEmbarazo(entEmbarazo embarazo)
        {
            try
            {
                bool exito = logEmbarazo.Instancia.CerrarEmbarazo(embarazo.IdEmbarazo);

                if (exito)
                {
                    TempData["MensajeExito"] = "El embarazo se ha cerrado/anulado correctamente.";
                }
                else
                {
                    TempData["MensajeError"] = "No se pudo cerrar el embarazo. Es posible que ya estuviera cerrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al intentar cerrar el embarazo: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

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

                ViewBag.PacientesModal = pacientes;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista de pacientes: " + ex.Message;
                ViewBag.ListaPacientes = new SelectList(new List<SelectListItem>());
                ViewBag.PacientesModal = new List<entPaciente>();
            }
        }
    }
}
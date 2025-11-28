using CapaEntidad;
using CapaLogica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class UsuarioController : Controller
    {
        private readonly logUsuario _logUsuario = logUsuario.Instancia;
        private readonly logRol _logRol = logRol.Instancia;


        public IActionResult Listar()
        {
            try
            {
                var lista = _logUsuario.ListarUsuario();
                return View(lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar la lista de usuarios: " + ex.Message;
                return View(new List<entUsuario>());
            }
        }

        public IActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine("UsuarioController.Index() llamado");
            var usuarios = _logUsuario.ListarUsuario();
            System.Diagnostics.Debug.WriteLine($"Usuarios enviados a vista: {usuarios.Count}");
            return View(usuarios);
        }

        // GET: /Usuario/Create
        public IActionResult Create()
        {
            ViewBag.Roles = _logRol.ListarRol();
            System.Diagnostics.Debug.WriteLine($"Roles cargados: {ViewBag.Roles?.Count ?? 0}");
            return View();
        }

        // POST: /Usuario/Create  (ADMIN crea internos)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(entUsuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Roles = _logRol.ListarRol();
                    return View(usuario);
                }

                // Buscar el IdRol por el nombre del rol seleccionado
                var rolSeleccionado = _logRol.ListarRol().FirstOrDefault(r => r.NombreRol == usuario.NombreRol);
                if (rolSeleccionado == null)
                {
                    ModelState.AddModelError("", "Rol seleccionado no válido.");
                    ViewBag.Roles = _logRol.ListarRol();
                    return View(usuario);
                }

                // Agregar logging para debug
                System.Diagnostics.Debug.WriteLine($"Intentando crear usuario: {usuario.NombreUsuario}, Rol: {rolSeleccionado.NombreRol} (Id: {rolSeleccionado.IdRol}), Estado: {usuario.Estado}");

                // No hagas hash aquí; la lógica lo hace.
                // usuario.ClaveHash llega en texto plano desde el formulario.
                var ok = _logUsuario.InsertarPorAdmin(usuario, rolSeleccionado.IdRol);

                System.Diagnostics.Debug.WriteLine($"Resultado de InsertarPorAdmin: {ok}");

                if (ok)
                {
                    TempData["Success"] = "Usuario creado exitosamente.";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Error al crear el usuario.");
                ViewBag.Roles = _logRol.ListarRol();
                return View(usuario);
            }
            catch (ArgumentException argEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error de validación en controlador: {argEx.Message}");
                ModelState.AddModelError("", $"Datos inválidos: {argEx.Message}");
                ViewBag.Roles = _logRol.ListarRol();
                return View(usuario);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general en UsuarioController.Create: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Error interno del servidor: {ex.Message}");
                ViewBag.Roles = _logRol.ListarRol();
                return View(usuario);
            }
        }

        // GET: /Usuario/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = _logUsuario.ListarUsuario().FirstOrDefault(u => u.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _logRol.ListarRol();
            // Para la vista, necesitamos el nombre del rol
            usuario.NombreRol = _logRol.ListarRol().FirstOrDefault(r => r.IdRol == usuario.IdRol)?.NombreRol ?? "";
            return View(usuario);
        }

        // POST: /Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, entUsuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            try
            {
                // Validación personalizada: permitir contraseña vacía (mantener actual)
                if (string.IsNullOrWhiteSpace(usuario.ClaveHash))
                {
                    ModelState.Remove("ClaveHash");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Roles = _logRol.ListarRol();
                    return View(usuario);
                }

                // Buscar el IdRol por el nombre del rol seleccionado
                var rolSeleccionado = _logRol.ListarRol().FirstOrDefault(r => r.NombreRol == usuario.NombreRol);
                if (rolSeleccionado == null)
                {
                    ModelState.AddModelError("", "Rol seleccionado no válido.");
                    ViewBag.Roles = _logRol.ListarRol();
                    return View(usuario);
                }

                System.Diagnostics.Debug.WriteLine($"Editando usuario: {usuario.IdUsuario}, Rol: {rolSeleccionado.NombreRol} (Id: {rolSeleccionado.IdRol})");

                var ok = _logUsuario.EditarUsuario(usuario, rolSeleccionado.IdRol);

                if (ok)
                {
                    TempData["Success"] = "Usuario editado exitosamente.";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Error al editar el usuario.");
                ViewBag.Roles = _logRol.ListarRol();
                return View(usuario);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en UsuarioController.Edit: {ex.Message}");
                ModelState.AddModelError("", $"Error interno del servidor: {ex.Message}");
                ViewBag.Roles = _logRol.ListarRol();
                return View(usuario);
            }
        }

        // GET: /Usuario/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = _logUsuario.ListarUsuario().FirstOrDefault(u => u.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: /Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Eliminando usuario con id: {id}");

                var ok = _logUsuario.EliminarUsuario(id);

                if (ok)
                {
                    TempData["Success"] = "Usuario eliminado exitosamente.";
                }
                else
                {
                    TempData["Error"] = "Error al eliminar el usuario.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en UsuarioController.DeleteConfirmed: {ex.Message}");
                TempData["Error"] = $"Error interno del servidor: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
using CapaAccesoDatos;
using CapaEntidad;
using System.Text;
using System.Security.Cryptography;


namespace CapaLogica
{
    public class logUsuario
    {
        #region Singleton
        private static readonly logUsuario UnicaInstancia = new logUsuario();
        public static logUsuario Instancia
        {
            get { return logUsuario.UnicaInstancia; }
        }
        #endregion

        // LISTAR
        // EN: logUsuario.cs
        // Reemplaza tu método ListarUsuario() por este:
        public List<entUsuario> ListarUsuario()
        {
            System.Diagnostics.Debug.WriteLine("logUsuario.ListarUsuario() llamado");

            // El DAO (corregido arriba) ahora trae IdRol y NombreRol
            var usuarios = DA_Usuario.Instancia.Listar();

            System.Diagnostics.Debug.WriteLine($"Usuarios obtenidos del DAO: {usuarios.Count}");

            // Ya no necesitamos el bucle 'foreach' que fallaba.
            // Los roles ya vienen incluidos.

            return usuarios;
        }




        // Hasheo centralizado
        public string CalcularSha256(string texto)
            {
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(texto);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }

            // ==========  A) ADMIN CREA USUARIOS  ==========
            public bool InsertarPorAdmin(entUsuario u, int idRol)
            {
                System.Diagnostics.Debug.WriteLine($"InsertarPorAdmin llamado con usuario: {u.NombreUsuario}, rol: {idRol}");

                try
                {
                    if (string.IsNullOrWhiteSpace(u.NombreUsuario) ||
                        string.IsNullOrWhiteSpace(u.ClaveHash) ||
                        idRol <= 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Validación fallida: nombre='{u.NombreUsuario}', clave='{u.ClaveHash}', rol={idRol}");
                        throw new ArgumentException("Datos de usuario inválidos: nombre, clave o rol faltantes/incorrectos");
                    }

                    // la vista manda contraseña en texto plano en u.ClaveHash
                    u.ClaveHash = CalcularSha256(u.ClaveHash);
                    u.Estado = true;
                    u.IdRol = idRol;

                    System.Diagnostics.Debug.WriteLine($"Hash calculado, llamando a DA_Usuario.Insertar");

                    // (opcional) prefijo por rol
                    var rol = DA_Rol.Instancia.BuscarPorId(idRol);
                    if (rol != null)
                    {
                        var pref = PrefijoPorRol(rol.NombreRol);
                        if (!string.IsNullOrWhiteSpace(pref) &&
                            !u.NombreUsuario.StartsWith(pref, StringComparison.OrdinalIgnoreCase))
                        {
                            u.NombreUsuario = pref + u.NombreUsuario;
                            System.Diagnostics.Debug.WriteLine($"Prefijo aplicado: {u.NombreUsuario}");
                        }
                    }

                    var result = DA_Usuario.Instancia.Insertar(u, idRol);
                    System.Diagnostics.Debug.WriteLine($"Resultado de DA_Usuario.Insertar: {result}");

                    if (!result)
                    {
                        throw new Exception("La inserción en base de datos falló sin excepciones específicas");
                    }

                    return result;
                }
                catch (ArgumentException argEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error de validación en InsertarPorAdmin: {argEx.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en logUsuario.InsertarPorAdmin: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                    throw new Exception($"Error al crear usuario: {ex.Message}", ex);
                }
            }

            // ==========  B) PACIENTE SE REGISTRA  ==========
            public bool RegistrarPaciente(entUsuario u)
            {
                if (string.IsNullOrWhiteSpace(u.NombreUsuario) ||
                    string.IsNullOrWhiteSpace(u.ClaveHash))
                    return false;

                u.ClaveHash = CalcularSha256(u.ClaveHash);
                u.Estado = true;

                // busca Id del rol PACIENTE
                var rolPaciente = DA_Rol.Instancia.ObtenerPorNombre("PACIENTE");
                if (rolPaciente == null) return false;

                u.IdRol = rolPaciente.IdRol;

                // (opcional) prefijo para pacientes
                // u.NombreUsuario = "PAC" + u.NombreUsuario;

                return DA_Usuario.Instancia.Insertar(u, rolPaciente.IdRol);
            }

            private string PrefijoPorRol(string nombreRol)
                => nombreRol.ToUpper() switch
                {
                    "ADMIN" => "ADMIN_",
                    "PERSONAL_SALUD" => "MED_",
                    "SECRETARIA" => "SECRE_",
                    _ => ""
                };

        // Listar / Validar (sin cambios)


        // ========== EDITAR USUARIO ==========
        public bool EditarUsuario(entUsuario usuario, int idRol) // idRol es el NUEVO rol
        {
            System.Diagnostics.Debug.WriteLine($"EditarUsuario llamado con usuario: {usuario.IdUsuario}, rol: {idRol}");

            try
            {
                if (usuario.IdUsuario <= 0 || idRol <= 0)
                {
                    throw new ArgumentException("Datos de usuario inválidos para edición");
                }

                // ... (Código de buscar usuario actual - Opcional pero recomendado) ...

                // CORRECCIÓN: Solo hashear si se proporcionó una nueva contraseña
                if (!string.IsNullOrWhiteSpace(usuario.ClaveHash))
                {
                    // (Tu lógica de hasheo está bien)
                    if (usuario.ClaveHash.Length != 64)
                    {
                        usuario.ClaveHash = CalcularSha256(usuario.ClaveHash);
                    }
                }
                else
                {
                    // Mantener contraseña actual estableciendo como null
                    // El SP (y tu DA_Usuario) ya manejan el DBNull.Value
                    usuario.ClaveHash = null;
                }

                // ... (Tu lógica de prefijos está bien) ...

                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // !!               AQUÍ ESTÁ EL ARREGLO             !!
                // Asignamos el nuevo IdRol al objeto 'usuario'
                // para que DA_Usuario.Editar lo reciba correctamente.
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                usuario.IdRol = idRol;


                // Ahora 'usuario' tiene el IdUsuario, Nombre, Email, Estado
                // Y el 'IdRol' correcto que debe guardarse.
                var result = DA_Usuario.Instancia.Editar(usuario);

                System.Diagnostics.Debug.WriteLine($"Resultado de DA_Usuario.Editar: {result}");

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en logUsuario.EditarUsuario: {ex.Message}");
                throw new Exception($"Error al editar usuario: {ex.Message}", ex);
            }
        }

        // ========== ELIMINAR USUARIO ==========
        public bool EliminarUsuario(int idUsuario)
            {
                System.Diagnostics.Debug.WriteLine($"EliminarUsuario llamado con id: {idUsuario}");

                try
                {
                    if (idUsuario <= 0)
                    {
                        throw new ArgumentException("ID de usuario inválido");
                    }

                    return DA_Usuario.Instancia.Eliminar(idUsuario);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en logUsuario.EliminarUsuario: {ex.Message}");
                    throw new Exception($"Error al eliminar usuario: {ex.Message}", ex);
                }
            }

            public entUsuario? ValidarUsuario(string nombreUsuario, string clave)
            {
                var usuario = DA_Usuario.Instancia.ObtenerPorNombre(nombreUsuario);
                if (usuario == null) return null;

                var hashEntrada = CalcularSha256(clave);
                if (!string.Equals(usuario.ClaveHash, hashEntrada, StringComparison.OrdinalIgnoreCase))
                    return null;

                return usuario;
            }
        public entUsuario? BuscarPorId(int idUsuario)
        {
            try
            {
                // Llama al método de la capa de datos que ya existe
                return DA_Usuario.Instancia.BuscarPorId(idUsuario);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en logUsuario.BuscarPorId: {ex.Message}");
                // No relanzamos la excepción completa para un 'buscar'
                return null;
            }
        }






    }
}

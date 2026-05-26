using PostaCitasWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    /// <summary>
    /// Interfaz para operaciones de Usuario en la base de datos.
    /// </summary>
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        /// <summary>
        /// Obtiene un usuario por su nombre de usuario.
        /// </summary>
        Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);

        /// <summary>
        /// Obtiene un usuario por su DNI.
        /// </summary>
        Task<Usuario?> GetByDniAsync(string dni);

        /// <summary>
        /// Obtiene todos los usuarios activos.
        /// </summary>
        Task<IEnumerable<Usuario>> GetActiveUsersAsync();

        /// <summary>
        /// Obtiene usuarios por rol.
        /// </summary>
        Task<IEnumerable<Usuario>> GetByRolAsync(Rol rol);

        /// <summary>
        /// Verifica si un nombre de usuario ya existe.
        /// </summary>
        Task<bool> NombreUsuarioExistsAsync(string nombreUsuario);

        /// <summary>
        /// Verifica si un DNI ya existe.
        /// </summary>
        Task<bool> DniExistsAsync(string dni);
    }
}

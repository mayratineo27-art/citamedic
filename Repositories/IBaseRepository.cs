using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    /// <summary>
    /// Interfaz genérica para operaciones CRUD base en cualquier entidad.
    /// Proporciona métodos estándar para crear, leer, actualizar y eliminar registros.
    /// </summary>
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene una entidad por su ID.
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene todas las entidades.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtiene entidades que cumplen con una condición.
        /// </summary>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Obtiene la primera entidad que cumple con una condición, o nula.
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Verifica si existe una entidad que cumple con una condición.
        /// </summary>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Agrega una entidad.
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Agrega múltiples entidades.
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Actualiza una entidad.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Elimina una entidad.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Elimina múltiples entidades.
        /// </summary>
        void DeleteRange(IEnumerable<T> entities);

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}

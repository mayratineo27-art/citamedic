using PostaCitasWeb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    /// <summary>
    /// Interfaz para operaciones de Paciente en la base de datos.
    /// </summary>
    public interface IPacienteRepository : IBaseRepository<Paciente>
    {
        /// <summary>
        /// Obtiene un paciente por su ID de usuario.
        /// </summary>
        Task<Paciente?> GetByUsuarioIdAsync(int usuarioId);

        /// <summary>
        /// Obtiene un paciente por su DNI.
        /// </summary>
        Task<Paciente?> GetByDniAsync(string dni);

        /// <summary>
        /// Obtiene todos los pacientes menores de edad.
        /// </summary>
        Task<IEnumerable<Paciente>> GetMenoresAsync();

        /// <summary>
        /// Obtiene los menores dependientes de un responsable.
        /// </summary>
        Task<IEnumerable<Paciente>> GetDependientesByResponsableAsync(int responsableId);

        /// <summary>
        /// Obtiene un paciente con todas sus propiedades de navegación cargadas.
        /// </summary>
        Task<Paciente?> GetWithDetailsAsync(int id);

        /// <summary>
        /// Verifica si un paciente existe por DNI.
        /// </summary>
        Task<bool> DniExistsAsync(string dni);

        /// <summary>
        /// Obtiene pacientes por rango de edad.
        /// </summary>
        Task<IEnumerable<Paciente>> GetByAgeRangeAsync(int edadMin, int edadMax);
    }
}

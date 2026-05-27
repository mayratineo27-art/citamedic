using PostaCitasWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    /// <summary>
    /// Interfaz para operaciones específicas de AvisoAtencionInmediata.
    /// </summary>
    public interface IAvisoRepository : IBaseRepository<AvisoAtencionInmediata>
    {
        /// <summary>
        /// Obtiene todos los avisos cargando la propiedad de navegación Paciente.
        /// </summary>
        Task<IEnumerable<AvisoAtencionInmediata>> GetAllWithPacienteAsync();
    }
}

using PostaCitasWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Interfaz para el servicio de gestión de avisos de atención inmediata.
    /// </summary>
    public interface IAvisoService
    {
        /// <summary>
        /// Actualiza el estado de un aviso de atención inmediata (Pendiente, Visualizado, Cerrado).
        /// </summary>
        Task<bool> ActualizarEstadoAvisoAsync(int avisoId, EstadoAviso nuevoEstado);
        
        /// <summary>
        /// Registra un nuevo aviso de atención inmediata.
        /// </summary>
        Task<bool> RegistrarAvisoAsync(int pacienteId, string motivo);
        
        /// <summary>
        /// Obtiene todos los avisos (solo Enfermería). RN26.
        /// </summary>
        Task<IEnumerable<AvisoAtencionInmediata>> ObtenerTodosAsync(int solicitanteId);
    }
}

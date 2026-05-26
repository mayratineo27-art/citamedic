using PostaCitasWeb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    /// <summary>
    /// Interfaz para operaciones de Cita en la base de datos.
    /// </summary>
    public interface ICitaRepository : IBaseRepository<Cita>
    {
        /// <summary>
        /// Obtiene las citas de un paciente.
        /// </summary>
        Task<IEnumerable<Cita>> GetByPacienteIdAsync(int pacienteId);

        /// <summary>
        /// Obtiene las citas de un slot.
        /// </summary>
        Task<IEnumerable<Cita>> GetBySlotIdAsync(int slotId);

        /// <summary>
        /// Obtiene las citas por estado.
        /// </summary>
        Task<IEnumerable<Cita>> GetByEstadoAsync(EstadoCita estado);

        /// <summary>
        /// Obtiene las citas pendientes o en triaje de un paciente.
        /// </summary>
        Task<IEnumerable<Cita>> GetActiveCitasByPacienteAsync(int pacienteId);

        /// <summary>
        /// Verifica si un paciente ya tiene una cita activa en un slot (RN31).
        /// </summary>
        Task<bool> HasActiveCitaInSlotAsync(int pacienteId, int slotId);

        /// <summary>
        /// Obtiene todas las citas con su información relacionada (Paciente, Slot, etc).
        /// </summary>
        Task<IEnumerable<Cita>> GetAllWithDetailsAsync();

        /// <summary>
        /// Obtiene citas por rango de fechas.
        /// </summary>
        Task<IEnumerable<Cita>> GetByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}

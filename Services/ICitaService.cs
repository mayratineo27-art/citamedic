using PostaCitasWeb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Interfaz para servicios de gestión de citas.
    /// Define operaciones centrales del dominio para reservas y triaje.
    /// </summary>
    public interface ICitaService
    {
        /// <summary>
        /// Crea una cita aplicando RN37.
        /// </summary>
        Task<bool> CreateCitaAsync(Cita cita);

        /// <summary>
        /// Reserva una cita para un paciente en un slot.
        /// RN04: Decrementa cupos disponibles.
        /// RN12: Genera automáticamente un ticket.
        /// RN31: Valida que no exista otra cita activa para el mismo paciente y slot.
        /// </summary>
        Task<CitaResult> ReserveCitaAsync(int pacienteId, int slotId, OrigenReserva origen, int? registradoPorUsuarioId = null);

        /// <summary>
        /// Cancela una cita si es posible.
        /// RN04: Incrementa cupos disponibles.
        /// RN13: Solo se puede cancelar en horarios permitidos (antes de triaje).
        /// RN36: Límites de cancellación por turno.
        /// </summary>
        Task<CitaResult> CancelCitaAsync(int citaId, string motivo = "");

        /// <summary>
        /// Registra un triaje para una cita.
        /// RN19: Una cita tiene a lo sumo un triaje.
        /// RN20: Solo Enfermería puede registrar triajes.
        /// RN22: Actualiza automáticamente estado a EnTriaje.
        /// RN30: Genera histórico de cambio de estado.
        /// </summary>
        Task<CitaResult> RegistrarTriajeAsync(int citaId, int enfermeriaUsuarioId, decimal peso, decimal talla,
            decimal temperatura, int presionSistolica, int presionDiastolica, string? observacion = null);

        /// <summary>
        /// Obtiene todas las citas de un paciente.
        /// </summary>
        Task<IEnumerable<Cita>> GetCitasByPacienteAsync(int pacienteId);

        /// <summary>
        /// Obtiene el estado actual de una cita.
        /// </summary>
        Task<Cita?> GetCitaAsync(int citaId);

        /// <summary>
        /// Actualiza el estado de una cita (ListoAtencion, NoAsistio, etc.) y registra el historial (RN21, RN30).
        /// </summary>
        Task<CitaResult> ActualizarEstadoCitaAsync(int citaId, EstadoCita nuevoEstado, int usuarioActuanteId);

        /// <summary>
        /// RN37: Genera sobrecupos automáticamente cuando la demanda lo requiere.
        /// Este método será implementado según la demanda del sistema.
        /// </summary>
        Task<bool> GenerarSobrecuposAsync(int slotId);
    }

    /// <summary>
    /// Resultado de una operación de cita.
    /// </summary>
    public class CitaResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CitaId { get; set; }
        public string? TicketCodigo { get; set; }
        public EstadoCita? NuevoEstado { get; set; }

        public static CitaResult CreateSuccess(int citaId, string message, string? ticketCodigo = null, EstadoCita? estado = null)
        {
            return new CitaResult
            {
                Success = true,
                Message = message,
                CitaId = citaId,
                TicketCodigo = ticketCodigo,
                NuevoEstado = estado
            };
        }

        public static CitaResult CreateFailure(string message)
        {
            return new CitaResult
            {
                Success = false,
                Message = message
            };
        }
    }
}

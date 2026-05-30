using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Implementación del servicio de gestión de citas.
    /// Coordina las operaciones de reserva, cancelación y triaje.
    /// </summary>
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly ISlotRepository _slotRepository;
        private readonly IBaseRepository<Ticket> _ticketRepository;
        private readonly IBaseRepository<HistorialEstadoCita> _historialRepository;
        private readonly IBaseRepository<Triaje> _triajeRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IEspecialidadService _especialidadService;

        public CitaService(
            ICitaRepository citaRepository,
            IPacienteRepository pacienteRepository,
            ISlotRepository slotRepository,
            IBaseRepository<Ticket> ticketRepository,
            IBaseRepository<HistorialEstadoCita> historialRepository,
            IBaseRepository<Triaje> triajeRepository,
            IDateTimeProvider dateTimeProvider,
            IEspecialidadService especialidadService)
        {
            _citaRepository = citaRepository ?? throw new ArgumentNullException(nameof(citaRepository));
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
            _slotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            _historialRepository = historialRepository ?? throw new ArgumentNullException(nameof(historialRepository));
            _triajeRepository = triajeRepository ?? throw new ArgumentNullException(nameof(triajeRepository));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _especialidadService = especialidadService ?? throw new ArgumentNullException(nameof(especialidadService));
        }

        public async Task<bool> CreateCitaAsync(Cita cita)
        {
            if (cita == null)
            {
                return false;
            }

            if (DateOnly.FromDateTime(cita.FechaReserva) == _dateTimeProvider.Today)
            {
                return false;
            }

            if (_dateTimeProvider.Today.DayOfWeek == DayOfWeek.Saturday && DateOnly.FromDateTime(cita.FechaReserva) == _dateTimeProvider.Today.AddDays(2))
            {
                return false;
            }

            if (!await _slotRepository.IsSlotAvailableAsync(cita.SlotId))
            {
                return false;
            }

            await _citaRepository.AddAsync(cita);
            await _citaRepository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Reserva una cita para un paciente en un slot disponible.
        /// Implementa RN04, RN12, RN31.
        /// </summary>
        public async Task<CitaResult> ReserveCitaAsync(int pacienteId, int slotId, OrigenReserva origen, int? registradoPorUsuarioId = null)
        {
            try
            {
                // Validar paciente existe
                var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
                if (paciente == null)
                    return CitaResult.CreateFailure("El paciente no existe.");

                // Validar slot existe y tiene cupos disponibles
                var slot = await _slotRepository.GetByIdWithProgramacionAsync(slotId);
                if (slot == null || slot.Programacion == null)
                    return CitaResult.CreateFailure("El slot o la programación asociada no existe.");

                // Validar ventana de reserva para citas del mismo día (RN37 / Regla de Horario de Reserva)
                var ahora = _dateTimeProvider.Now;
                var fechaHoy = _dateTimeProvider.Today;

                if (slot.Programacion.Fecha == fechaHoy)
                {
                    if (slot.Programacion.Turno == Turno.Manana)
                    {
                        var horaInicioReserva = new TimeSpan(6, 0, 0);
                        var horaFinReserva = new TimeSpan(8, 0, 0);
                        var horaActual = ahora.TimeOfDay;

                        if (horaActual < horaInicioReserva || horaActual > horaFinReserva)
                        {
                            return CitaResult.CreateFailure("El plazo de reserva para el turno mañana de hoy ha finalizado o aún no ha comenzado (horario permitido: 06:00 a.m. a 08:00 a.m.).");
                        }
                    }
                    else if (slot.Programacion.Turno == Turno.Tarde)
                    {
                        var horaInicioReserva = new TimeSpan(13, 0, 0);
                        var horaFinReserva = new TimeSpan(15, 0, 0);
                        var horaActual = ahora.TimeOfDay;

                        if (horaActual < horaInicioReserva || horaActual > horaFinReserva)
                        {
                            return CitaResult.CreateFailure("El plazo de reserva para el turno tarde de hoy ha finalizado o aún no ha comenzado (horario permitido: 01:00 p.m. a 03:00 p.m.).");
                        }
                    }
                }

                if (slot.CuposDisponibles <= 0)
                    return CitaResult.CreateFailure("No hay cupos disponibles en este horario.");

                // RN31 y FA02: Verificar que no exista otra cita activa para el mismo paciente en la MISMA FECHA
                // Un paciente solo puede tener una cita por día.
                bool tieneCitaEnFecha = await _citaRepository.HasActiveCitaOnDateAsync(pacienteId, slot.Programacion.Fecha);
                if (tieneCitaEnFecha)
                    return CitaResult.CreateFailure("Solo puedes reservar una cita por día. Ya tienes una reserva activa para esta fecha.");

                // Crear cita
                var cita = new Cita
                {
                    PacienteId = pacienteId,
                    SlotId = slotId,
                    EspecialidadId = slot.Programacion.EspecialidadId, // ASIGNAR FK REQUERIDA
                    EstadoCita = EstadoCita.Pendiente,
                    OrigenReserva = origen,
                    FechaReserva = _dateTimeProvider.UtcNow,
                    RegistradaPorUsuarioId = registradoPorUsuarioId,
                    FechaUltimaActualizacion = _dateTimeProvider.UtcNow
                };

                await _citaRepository.AddAsync(cita);
                await _citaRepository.SaveChangesAsync();

                // RN04: Decrementar cupos disponibles
                slot.CuposDisponibles--;
                _slotRepository.Update(slot);
                await _slotRepository.SaveChangesAsync();

                // RN12: Generar ticket automáticamente
                string codigoTicket = GenerarCodigoTicket();
                var ticket = new Ticket
                {
                    CitaId = cita.CitaId,
                    Codigo = codigoTicket,
                    FechaEmision = _dateTimeProvider.UtcNow
                };

                await _ticketRepository.AddAsync(ticket);
                await _ticketRepository.SaveChangesAsync();

                // Registrar cambio de estado en historial (RN30)
                var historial = new HistorialEstadoCita
                {
                    CitaId = cita.CitaId,
                    EstadoAnterior = EstadoCita.Pendiente, // Estado inicial
                    EstadoNuevo = EstadoCita.Pendiente,
                    FechaCambio = _dateTimeProvider.UtcNow,
                    UsuarioId = registradoPorUsuarioId ?? pacienteId, // Simplificado para demo
                    Observacion = origen == OrigenReserva.Web ? "Reserva web" : "Reserva presencial"
                };

                await _historialRepository.AddAsync(historial);
                await _historialRepository.SaveChangesAsync();

                return CitaResult.CreateSuccess(cita.CitaId, "Cita reservada exitosamente", codigoTicket, EstadoCita.Pendiente);
            }
            catch (Exception ex)
            {
                return CitaResult.CreateFailure($"Error al reservar cita: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancela una cita si es posible según RN13 y RN36.
        /// </summary>
        public async Task<CitaResult> CancelCitaAsync(int citaId, string motivo = "")
        {
            try
            {
                var cita = await _citaRepository.GetByIdAsync(citaId);
                if (cita == null)
                    return CitaResult.CreateFailure("La cita no existe.");

                // RN13: Solo se puede cancelar si está en estado Pendiente
                if (cita.EstadoCita != EstadoCita.Pendiente)
                    return CitaResult.CreateFailure("Solo se pueden cancelar citas en estado Pendiente.");

                // RN36: Validar horario de cancelación según turno
                var slot = await _slotRepository.GetByIdWithProgramacionAsync(cita.SlotId);
                if (slot == null)
                    return CitaResult.CreateFailure("Slot no encontrado.");

                // Obtener horario de triaje según turno
                TimeOnly horaLimite = ObtenerHoraLimiteCancel(slot);
                var ahora = _dateTimeProvider.Now;
                var fechaHoy = _dateTimeProvider.Today;

                if (slot.Programacion.Fecha < fechaHoy)
                {
                    return CitaResult.CreateFailure("Ha pasado el horario de cancelación para este turno.");
                }

                if (slot.Programacion.Fecha == fechaHoy && ahora.TimeOfDay > horaLimite.ToTimeSpan())
                {
                    return CitaResult.CreateFailure("Ha pasado el horario de cancelación para este turno.");
                }

                // Cambiar estado a Cancelada
                var estadoAnterior = cita.EstadoCita;
                cita.EstadoCita = EstadoCita.Cancelada;
                cita.FechaUltimaActualizacion = _dateTimeProvider.UtcNow;

                _citaRepository.Update(cita);
                await _citaRepository.SaveChangesAsync();

                // RN04: Incrementar cupos disponibles
                slot.CuposDisponibles++;
                _slotRepository.Update(slot);
                await _slotRepository.SaveChangesAsync();

                // RN30: Registrar en historial
                var historial = new HistorialEstadoCita
                {
                    CitaId = citaId,
                    EstadoAnterior = estadoAnterior,
                    EstadoNuevo = EstadoCita.Cancelada,
                    FechaCambio = _dateTimeProvider.UtcNow,
                    UsuarioId = cita.PacienteId,
                    Observacion = motivo ?? "Cancelación por paciente"
                };

                await _historialRepository.AddAsync(historial);
                await _historialRepository.SaveChangesAsync();

                return CitaResult.CreateSuccess(citaId, "Cita cancelada exitosamente", null, EstadoCita.Cancelada);
            }
            catch (Exception ex)
            {
                return CitaResult.CreateFailure($"Error al cancelar cita: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra un triaje para una cita (RN19, RN20, RN22, RN30).
        /// </summary>
        public async Task<CitaResult> RegistrarTriajeAsync(int citaId, int enfermeriaUsuarioId, decimal peso, decimal talla,
            decimal temperatura, int presionSistolica, int presionDiastolica, string? observacion = null)
        {
            try
            {
                var cita = await _citaRepository.GetByIdAsync(citaId);
                if (cita == null)
                    return CitaResult.CreateFailure("La cita no existe.");

                // RN19: Verificar que no exista otro triaje
                if (cita.Triaje != null)
                    return CitaResult.CreateFailure("Esta cita ya tiene un triaje registrado.");

                // Crear triaje
                var triaje = new Triaje
                {
                    CitaId = citaId,
                    EnfermeriaUsuarioId = enfermeriaUsuarioId,
                    Peso = peso,
                    Talla = talla,
                    Temperatura = temperatura,
                    PresionSistolica = presionSistolica,
                    PresionDiastolica = presionDiastolica,
                    Observacion = observacion,
                    FechaRegistro = _dateTimeProvider.UtcNow
                };

                await _triajeRepository.AddAsync(triaje);
                await _triajeRepository.SaveChangesAsync();

                // RN22: Cambiar estado a EnTriaje
                var estadoAnterior = cita.EstadoCita;
                cita.EstadoCita = EstadoCita.EnTriaje;
                cita.FechaUltimaActualizacion = _dateTimeProvider.UtcNow;

                _citaRepository.Update(cita);
                await _citaRepository.SaveChangesAsync();

                // RN30: Registrar en historial
                var historial = new HistorialEstadoCita
                {
                    CitaId = citaId,
                    EstadoAnterior = estadoAnterior,
                    EstadoNuevo = EstadoCita.EnTriaje,
                    FechaCambio = _dateTimeProvider.UtcNow,
                    UsuarioId = enfermeriaUsuarioId,
                    Observacion = "Triaje registrado"
                };

                await _historialRepository.AddAsync(historial);
                await _historialRepository.SaveChangesAsync();

                return CitaResult.CreateSuccess(citaId, "Triaje registrado exitosamente", null, EstadoCita.EnTriaje);
            }
            catch (Exception ex)
            {
                return CitaResult.CreateFailure($"Error al registrar triaje: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todas las citas de un paciente.
        /// </summary>
        public async Task<IEnumerable<Cita>> GetCitasByPacienteAsync(int pacienteId)
        {
            return await _citaRepository.GetByPacienteIdAsync(pacienteId);
        }

        /// <summary>
        /// Obtiene una cita específica.
        /// </summary>
        public async Task<Cita?> GetCitaAsync(int citaId)
        {
            return await _citaRepository.GetByIdAsync(citaId);
        }

        /// <summary>
        /// Actualiza el estado de una cita (ListoAtencion, NoAsistio, etc.) y registra el historial (RN21, RN30).
        /// </summary>
        public async Task<CitaResult> ActualizarEstadoCitaAsync(int citaId, EstadoCita nuevoEstado, int usuarioActuanteId)
        {
            try
             {
                 var cita = await _citaRepository.GetByIdAsync(citaId);
                 if (cita == null)
                     return CitaResult.CreateFailure("La cita no existe.");

                 var estadoAnterior = cita.EstadoCita;

                 // RN21: Validar transiciones de estado
                 if (estadoAnterior == EstadoCita.Cancelada)
                     return CitaResult.CreateFailure("No se puede cambiar el estado de una cita cancelada.");

                 if (nuevoEstado == EstadoCita.ListoAtencion && estadoAnterior != EstadoCita.EnTriaje)
                     return CitaResult.CreateFailure("Solo se puede marcar como Listo para Atención si la cita está En Triaje.");

                 cita.EstadoCita = nuevoEstado;
                 cita.FechaUltimaActualizacion = _dateTimeProvider.UtcNow;

                 _citaRepository.Update(cita);
                 await _citaRepository.SaveChangesAsync();

                 // RN30: Registrar en historial
                 var historial = new HistorialEstadoCita
                 {
                     CitaId = citaId,
                     EstadoAnterior = estadoAnterior,
                     EstadoNuevo = nuevoEstado,
                     FechaCambio = _dateTimeProvider.UtcNow,
                     UsuarioId = usuarioActuanteId,
                     Observacion = $"Estado actualizado a {nuevoEstado}"
                 };

                 await _historialRepository.AddAsync(historial);
                 await _historialRepository.SaveChangesAsync();

                 return CitaResult.CreateSuccess(citaId, "Estado de la cita actualizado exitosamente", null, nuevoEstado);
             }
             catch (Exception ex)
             {
                 return CitaResult.CreateFailure($"Error al actualizar estado de la cita: {ex.Message}");
             }
        }

        /// <summary>
        /// RN37: Genera sobrecupos automáticamente cuando sea necesario.
        /// PENDIENTE: Implementar lógica de generación de sobrecupos según demanda.
        /// </summary>
        public async Task<bool> GenerarSobrecuposAsync(int slotId)
        {
            // TODO: Implementar lógica de generación de sobrecupos (RN37)
            // - Analizar demanda del slot
            // - Crear nuevos SlotDisponible con EsSobrecupo = true
            // - Retornar true si se generaron, false si no fue necesario
            await Task.CompletedTask;
            return false;
        }

        public async Task<IEnumerable<SlotDisponible>> GetSlotsDisponiblesAsync(int especialidadId, Turno turno)
        {
            // Calcular fechas según RN37 y FA03
            var localToday = _dateTimeProvider.Today;
            var dayOfWeek = localToday.DayOfWeek;
            var targetDates = new List<DateOnly>();

            if (dayOfWeek >= DayOfWeek.Monday && dayOfWeek <= DayOfWeek.Friday)
            {
                targetDates.Add(localToday); // Lunes a Viernes: Mismo día
            }
            else if (dayOfWeek == DayOfWeek.Saturday)
            {
                targetDates.Add(localToday); // Sábado: Mismo día (según FA03)
                targetDates.Add(localToday.AddDays(2)); // Sábado: Próximo lunes
            }
            else // Domingo
            {
                // Las reservas web no están disponibles los domingos (RN37)
                return Array.Empty<SlotDisponible>();
            }

            var ahora = _dateTimeProvider.Now;
            var horaActual = ahora.TimeOfDay;

            var allSlots = new List<SlotDisponible>();
            foreach (var date in targetDates)
            {
                if (date == localToday)
                {
                    if (turno == Turno.Manana)
                    {
                        var horaInicioReserva = new TimeSpan(6, 0, 0);
                        var horaFinReserva = new TimeSpan(8, 0, 0);
                        if (horaActual < horaInicioReserva || horaActual > horaFinReserva)
                        {
                            continue; // Fuera del horario de reserva
                        }
                    }
                    else if (turno == Turno.Tarde)
                    {
                        var horaInicioReserva = new TimeSpan(13, 0, 0);
                        var horaFinReserva = new TimeSpan(15, 0, 0);
                        if (horaActual < horaInicioReserva || horaActual > horaFinReserva)
                        {
                            continue; // Fuera del horario de reserva
                        }
                    }
                }

                var slotsForDate = await _slotRepository.GetSlotsByEspecialidadAndTurnoAndDateAsync(especialidadId, turno, date);
                allSlots.AddRange(slotsForDate);
            }

            return allSlots;
        }

        public async Task<bool> UpdateCitaAsync(int citaId, int especialidadId, DateTime fecha)
        {
            var cita = await _citaRepository.GetByIdAsync(citaId);
            if (cita == null)
            {
                return false;
            }

            cita.EspecialidadId = especialidadId;
            cita.FechaReserva = fecha;
            cita.FechaUltimaActualizacion = _dateTimeProvider.UtcNow;

            _citaRepository.Update(cita);
            await _citaRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Especialidad>> GetEspecialidadesActivasAsync()
        {
            var list = await _especialidadService.GetAllAsync();
            // RN07: El paciente no visualiza la UPS interna, solo Especialidades activas.
            var activas = new List<Especialidad>();
            foreach (var e in list)
            {
                if (e.Activa)
                {
                    activas.Add(e);
                }
            }
            return activas;
        }

        // ==================== MÉTODOS PRIVADOS ====================

        private string GenerarCodigoTicket()
        {
            // Generar código único para ticket
            // Formato: TC-YYYYMMDD-NNNNN
            var fecha = _dateTimeProvider.Now.ToString("yyyyMMdd");
            var random = new Random().Next(10000, 99999);
            return $"TC-{fecha}-{random}";
        }

        private TimeOnly ObtenerHoraLimiteCancel(SlotDisponible slot)
        {
            // RN36: Horario de cancelación según turno
            // Mañana: cancelación posible antes de 07:40
            // Tarde: cancelación posible antes de 14:40
            return slot.Programacion.Turno == Turno.Manana
                ? new TimeOnly(7, 40)
                : new TimeOnly(14, 40);
        }

        private async Task<bool> IsSlotAvailableAsync(int slotId)
        {
            var slot = await _slotRepository.GetByIdAsync(slotId);
            return slot != null && slot.CuposDisponibles > 0;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Data;
using PostaCitasWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    /// <summary>
    /// Implementación del repositorio para Cita.
    /// </summary>
    public class CitaRepository : BaseRepository<Cita>, ICitaRepository
    {
        public CitaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Cita>> GetByPacienteIdAsync(int pacienteId)
        {
            return await _dbSet
                .Where(c => c.PacienteId == pacienteId)
                .Include(c => c.Especialidad)
                .Include(c => c.Slot)
                    .ThenInclude(s => s.Programacion)
                    .ThenInclude(p => p.Medico)
                .Include(c => c.Ticket)
                .Include(c => c.Triaje)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> GetBySlotIdAsync(int slotId)
        {
            return await _dbSet
                .Where(c => c.SlotId == slotId)
                .Include(c => c.Paciente)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> GetByEstadoAsync(EstadoCita estado)
        {
            return await _dbSet
                .Where(c => c.EstadoCita == estado)
                .Include(c => c.Paciente)
                .Include(c => c.Slot)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> GetActiveCitasByPacienteAsync(int pacienteId)
        {
            // Estados activos: Pendiente, EnTriaje, ListoAtencion
            var estadosActivos = new[] { EstadoCita.Pendiente, EstadoCita.EnTriaje, EstadoCita.ListoAtencion };

            return await _dbSet
                .Where(c => c.PacienteId == pacienteId && estadosActivos.Contains(c.EstadoCita))
                .Include(c => c.Slot)
                .ThenInclude(s => s.Programacion)
                .ToListAsync();
        }

        public async Task<bool> HasActiveCitaInSlotAsync(int pacienteId, int slotId)
        {
            return await _dbSet.AnyAsync(c =>
                c.PacienteId == pacienteId &&
                c.SlotId == slotId &&
                c.EstadoCita != EstadoCita.Cancelada);
        }

        public async Task<bool> HasActiveCitaOnDateAsync(int pacienteId, DateOnly date)
        {
            return await _dbSet.AnyAsync(c =>
                c.PacienteId == pacienteId &&
                c.Slot.Programacion.Fecha == date &&
                c.EstadoCita != EstadoCita.Cancelada);
        }

        public async Task<IEnumerable<Cita>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.Paciente)
                .ThenInclude(p => p.Usuario)
                .Include(c => c.Slot)
                .ThenInclude(s => s.Programacion)
                .ThenInclude(p => p.Especialidad)
                .Include(c => c.Ticket)
                .Include(c => c.Triaje)
                .Include(c => c.Historial)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> GetByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _dbSet
                .Where(c => c.FechaReserva >= fechaInicio && c.FechaReserva <= fechaFin)
                .Include(c => c.Paciente)
                .Include(c => c.Slot)
                .ToListAsync();
        }
    }
}

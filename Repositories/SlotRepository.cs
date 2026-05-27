using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Data;
using PostaCitasWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    public class SlotRepository : BaseRepository<SlotDisponible>, ISlotRepository
    {
        public SlotRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> IsSlotAvailableAsync(int slotId)
        {
            var slot = await _dbSet.FirstOrDefaultAsync(s => s.SlotId == slotId);
            return slot != null && slot.CuposDisponibles > 0;
        }

        public async Task<SlotDisponible?> GetByIdWithProgramacionAsync(int slotId)
        {
            return await _dbSet
                .Include(s => s.Programacion)
                .FirstOrDefaultAsync(s => s.SlotId == slotId);
        }

        public async Task<IEnumerable<SlotDisponible>> GetSlotsByEspecialidadAndTurnoAndDateAsync(int especialidadId, Turno turno, DateOnly fecha)
        {
            return await _context.SlotsDisponibles
                .Include(s => s.Programacion)
                .ThenInclude(p => p.Medico)
                .Where(s => s.Programacion.EspecialidadId == especialidadId &&
                            s.Programacion.Turno == turno &&
                            s.Programacion.Fecha == fecha &&
                            s.Programacion.Habilitada &&
                            !s.EsSobrecupo)
                .ToListAsync();
        }
    }
}
using PostaCitasWeb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    public interface ISlotRepository : IBaseRepository<SlotDisponible>
    {
        Task<bool> IsSlotAvailableAsync(int slotId);
        Task<IEnumerable<SlotDisponible>> GetSlotsByEspecialidadAndTurnoAndDateAsync(int especialidadId, Turno turno, DateOnly fecha);
        Task<SlotDisponible?> GetByIdWithProgramacionAsync(int slotId);
    }
}
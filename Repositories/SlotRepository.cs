using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Data;
using PostaCitasWeb.Entities;
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
    }
}
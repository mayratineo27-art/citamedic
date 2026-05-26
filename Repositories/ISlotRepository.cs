using PostaCitasWeb.Entities;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    public interface ISlotRepository : IBaseRepository<SlotDisponible>
    {
        Task<bool> IsSlotAvailableAsync(int slotId);
    }
}
using PostaCitasWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    public interface IEspecialidadService
    {
        Task<IEnumerable<Especialidad>> GetAllAsync();
    }
}

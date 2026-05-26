using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly IBaseRepository<Especialidad> _especialidadRepository;

        public EspecialidadService(IBaseRepository<Especialidad> especialidadRepository)
        {
            _especialidadRepository = especialidadRepository;
        }

        public async Task<IEnumerable<Especialidad>> GetAllAsync()
        {
            return await _especialidadRepository.GetAllAsync();
        }
    }
}

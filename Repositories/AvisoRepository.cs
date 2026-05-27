using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Data;
using PostaCitasWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Repositories
{
    /// <summary>
    /// Implementación del repositorio para AvisoAtencionInmediata.
    /// </summary>
    public class AvisoRepository : BaseRepository<AvisoAtencionInmediata>, IAvisoRepository
    {
        public AvisoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AvisoAtencionInmediata>> GetAllWithPacienteAsync()
        {
            return await _dbSet
                .Include(a => a.Paciente)
                .ToListAsync();
        }
    }
}

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
    /// Implementación del repositorio para Paciente.
    /// </summary>
    public class PacienteRepository : BaseRepository<Paciente>, IPacienteRepository
    {
        public PacienteRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Paciente?> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }

        public async Task<Paciente?> GetByDniAsync(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El DNI no puede estar vacío.", nameof(dni));

            return await _dbSet
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.DNI == dni);
        }

        public async Task<IEnumerable<Paciente>> GetMenoresAsync()
        {
            return await _dbSet
                .Where(p => p.EsMenor)
                .Include(p => p.Usuario)
                .Include(p => p.Responsable)
                .ToListAsync();
        }

        public async Task<IEnumerable<Paciente>> GetDependientesByResponsableAsync(int responsableId)
        {
            return await _dbSet
                .Where(p => p.ResponsableId == responsableId)
                .Include(p => p.Usuario)
                .ToListAsync();
        }

        public async Task<Paciente?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Responsable)
                .Include(p => p.Dependientes)
                .Include(p => p.Citas)
                .ThenInclude(c => c.Slot)
                .FirstOrDefaultAsync(p => p.PacienteId == id);
        }

        public async Task<bool> DniExistsAsync(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return false;

            return await _dbSet.AnyAsync(p => p.DNI == dni);
        }

        public async Task<IEnumerable<Paciente>> GetByAgeRangeAsync(int edadMin, int edadMax)
        {
            var ahora = DateTime.Now;
            var fechaNacimientoMax = DateOnly.FromDateTime(ahora.AddYears(-edadMin));
            var fechaNacimientoMin = DateOnly.FromDateTime(ahora.AddYears(-edadMax));

            return await _dbSet
                .Where(p => p.FechaNacimiento <= fechaNacimientoMax && p.FechaNacimiento >= fechaNacimientoMin)
                .Include(p => p.Usuario)
                .ToListAsync();
        }
    }
}

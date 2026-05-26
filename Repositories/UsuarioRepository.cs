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
    /// Implementación del repositorio para Usuario.
    /// </summary>
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(nombreUsuario));

            return await _dbSet.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
        }

        public async Task<Usuario?> GetByDniAsync(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El DNI no puede estar vacío.", nameof(dni));

            return await _dbSet.FirstOrDefaultAsync(u => u.DNI == dni);
        }

        public async Task<IEnumerable<Usuario>> GetActiveUsersAsync()
        {
            return await _dbSet.Where(u => u.Activo).ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetByRolAsync(Rol rol)
        {
            return await _dbSet.Where(u => u.Rol == rol).ToListAsync();
        }

        public async Task<bool> NombreUsuarioExistsAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return false;

            return await _dbSet.AnyAsync(u => u.NombreUsuario == nombreUsuario);
        }

        public async Task<bool> DniExistsAsync(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return false;

            return await _dbSet.AnyAsync(u => u.DNI == dni);
        }
    }
}

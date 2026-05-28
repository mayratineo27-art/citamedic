using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Servicio que implementa la lógica de avisos de atención inmediata.
    /// </summary>
    public class AvisoService : IAvisoService
    {
        private readonly IAvisoRepository _avisoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public AvisoService(IAvisoRepository avisoRepository, IUsuarioRepository usuarioRepository)
        {
            _avisoRepository = avisoRepository ?? throw new ArgumentNullException(nameof(avisoRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<bool> ActualizarEstadoAvisoAsync(int avisoId, EstadoAviso nuevoEstado)
        {
            var aviso = await _avisoRepository.GetByIdAsync(avisoId);
            if (aviso == null)
            {
                return false;
            }

            aviso.EstadoAviso = nuevoEstado;
            aviso.FechaActualizacion = DateTime.UtcNow;

            _avisoRepository.Update(aviso);
            await _avisoRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegistrarAvisoAsync(int pacienteId, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo) || motivo.Length > 300)
            {
                return false;
            }

            var aviso = new AvisoAtencionInmediata
            {
                PacienteId = pacienteId,
                Motivo = motivo,
                EstadoAviso = EstadoAviso.Pendiente,
                FechaEnvio = DateTime.UtcNow
            };

            await _avisoRepository.AddAsync(aviso);
            await _avisoRepository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtiene todos los avisos (solo Enfermería). RN26.
        /// </summary>
        public async Task<IEnumerable<AvisoAtencionInmediata>> ObtenerTodosAsync(int solicitanteId)
        {
            // Verificar que el Usuario con solicitanteId tenga Rol = Enfermeria
            var solicitante = await _usuarioRepository.GetByIdAsync(solicitanteId);
            if (solicitante == null)
                throw new KeyNotFoundException("Solicitante no encontrado.");

            if (solicitante.Rol != Rol.Enfermeria)
                throw new UnauthorizedAccessException("Solo Enfermería puede obtener todos los avisos.");

            // Retornar todos los avisos ordenados por FechaEnvio descendente
            var avisos = await _avisoRepository.GetAllWithPacienteAsync();
            return avisos.OrderByDescending(a => a.FechaEnvio);
        }
    }
}

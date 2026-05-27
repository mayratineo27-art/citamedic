using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using System;
using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Servicio que implementa la lógica de avisos de atención inmediata.
    /// </summary>
    public class AvisoService : IAvisoService
    {
        private readonly IAvisoRepository _avisoRepository;

        public AvisoService(IAvisoRepository avisoRepository)
        {
            _avisoRepository = avisoRepository ?? throw new ArgumentNullException(nameof(avisoRepository));
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
    }
}

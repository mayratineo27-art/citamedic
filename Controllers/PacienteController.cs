using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Data;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Models.ViewModels;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostaCitasWeb.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class PacienteController : Controller
    {
        private readonly IEspecialidadService _especialidadService;
        private readonly ICitaService _citaService;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly AppDbContext _context;
        private readonly IAvisoService _avisoService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PacienteController(
            IEspecialidadService especialidadService, 
            ICitaService citaService, 
            IPacienteRepository pacienteRepository,
            IUsuarioRepository usuarioRepository,
            AppDbContext context,
            IAvisoService avisoService,
            IDateTimeProvider dateTimeProvider)
        {
            _especialidadService = especialidadService ?? throw new ArgumentNullException(nameof(especialidadService));
            _citaService = citaService ?? throw new ArgumentNullException(nameof(citaService));
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _avisoService = avisoService ?? throw new ArgumentNullException(nameof(avisoService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Challenge();
            }

            var paciente = await _pacienteRepository.GetByUsuarioIdAsync(userId);
            if (paciente == null)
            {
                return NotFound("No se encontró la información del paciente.");
            }

            var list = await _citaService.GetCitasByPacienteAsync(paciente.PacienteId);

            // CU03: Consultar Programación Referencial de Atención
            // Obtener programaciones operativas habilitadas futuras (o de hoy en adelante)
            var programaciones = await _context.ProgramacionesOperativas
                .Where(p => p.Habilitada && p.Fecha >= _dateTimeProvider.Today)
                .Include(p => p.Especialidad)
                .Include(p => p.Medico)
                .OrderBy(p => p.Fecha)
                .ThenBy(p => p.Turno)
                .ToListAsync();

            // Filtrar programaciones de hoy que ya expiraron (después de las 8 a.m. para mañana y de las 3 p.m. para tarde)
            var ahora = _dateTimeProvider.Now;
            var horaActual = ahora.TimeOfDay;
            programaciones = programaciones.Where(p => 
                p.Fecha > _dateTimeProvider.Today || 
                (p.Fecha == _dateTimeProvider.Today && 
                 ((p.Turno == Turno.Manana && horaActual <= new TimeSpan(8, 0, 0)) ||
                  (p.Turno == Turno.Tarde && horaActual <= new TimeSpan(15, 0, 0))))
            ).ToList();

            var model = new PacienteDashboardViewModel
            {
                Paciente = paciente,
                Especialidades = await _especialidadService.GetAllAsync(),
                MisCitas = list,
                Programaciones = programaciones
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPerfil(string celular, string? password, string? confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(celular) || celular.Length < 9 || celular.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(celular, @"^\d+$"))
            {
                return Json(new { success = false, message = "El celular debe contener entre 9 y 15 dígitos numéricos." });
            }

            if (!string.IsNullOrEmpty(password))
            {
                if (password.Length < 8)
                {
                    return Json(new { success = false, message = "La contraseña debe tener al menos 8 caracteres." });
                }
                if (password != confirmPassword)
                {
                    return Json(new { success = false, message = "Las contraseñas no coinciden." });
                }
            }

            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Sesión inválida." });
            }

            var usuario = await _usuarioRepository.GetByIdAsync(userId);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado." });
            }

            usuario.Celular = celular;
            if (!string.IsNullOrEmpty(password))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return Json(new { success = true, message = "Perfil actualizado exitosamente." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarCita(int citaId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            var paciente = await _pacienteRepository.GetByUsuarioIdAsync(userId);
            if (paciente == null)
            {
                return Json(new { success = false, message = "Perfil de paciente no encontrado." });
            }

            var cita = await _citaService.GetCitaAsync(citaId);
            if (cita == null)
            {
                return Json(new { success = false, message = "La cita no existe." });
            }

            if (cita.PacienteId != paciente.PacienteId)
            {
                return Json(new { success = false, message = "No tiene permisos para cancelar esta cita." });
            }

            var result = await _citaService.CancelCitaAsync(citaId, "Cancelada por el paciente.");
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = "Cita cancelada con éxito." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarAviso(string motivo)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            var paciente = await _pacienteRepository.GetByUsuarioIdAsync(userId);
            if (paciente == null)
            {
                return Json(new { success = false, message = "Perfil de paciente no encontrado." });
            }

            if (string.IsNullOrWhiteSpace(motivo) || motivo.Length > 300)
            {
                return Json(new { success = false, message = "El motivo es obligatorio y no debe exceder los 300 caracteres." });
            }

            var success = await _avisoService.RegistrarAvisoAsync(paciente.PacienteId, motivo);
            if (!success)
            {
                return Json(new { success = false, message = "No se pudo registrar el aviso de atención inmediata." });
            }

            return Json(new { success = true, message = "Aviso de atención inmediata enviado con éxito. Enfermería ha sido notificada." });
        }
    }
}

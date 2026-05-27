using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
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

            // Load dependientes (children)
            paciente.Dependientes = await _context.Pacientes
                .Where(p => p.ResponsableId == paciente.PacienteId)
                .Include(p => p.Usuario)
                .ToListAsync();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccederComoDependiente(int dependienteId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Challenge();
            }

            var tutor = await _pacienteRepository.GetByUsuarioIdAsync(userId);
            if (tutor == null)
            {
                return NotFound("Tutor no encontrado.");
            }

            var dependiente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.PacienteId == dependienteId);

            if (dependiente == null || dependiente.ResponsableId != tutor.PacienteId)
            {
                return Forbid("No tiene autorización para acceder a la cuenta de este dependiente.");
            }

            // Sign out current tutor user identity
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in minor child identity with tutor claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, dependiente.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, dependiente.Usuario.NombreUsuario ?? dependiente.DNI),
                new Claim(ClaimTypes.Role, "Paciente"),
                new Claim("TutorOriginalUsuarioId", tutor.UsuarioId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VolverACuentaTutor()
        {
            var tutorUsuarioIdStr = User.FindFirst("TutorOriginalUsuarioId")?.Value;
            if (string.IsNullOrEmpty(tutorUsuarioIdStr) || !int.TryParse(tutorUsuarioIdStr, out int tutorUsuarioId))
            {
                return BadRequest("No se encontró sesión original de tutor activa.");
            }

            var tutorUsuario = await _usuarioRepository.GetByIdAsync(tutorUsuarioId);
            if (tutorUsuario == null)
            {
                return RedirectToAction("Logout", "Auth");
            }

            // Sign out minor child identity
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in back as tutor original identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, tutorUsuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, tutorUsuario.NombreUsuario ?? tutorUsuario.DNI),
                new Claim(ClaimTypes.Role, tutorUsuario.Rol.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToAction("Index");
        }
    }
}

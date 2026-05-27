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
    [Authorize(Roles = "Admision,Administrador")]
    public class AdmisionController : Controller
    {
        private readonly IBaseRepository<PostaCitasWeb.Entities.ProgramacionOperativa> _programacionRepository;
        private readonly ICitaRepository _citaRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly ISlotRepository _slotRepository;
        private readonly ICitaService _citaService;
        private readonly AppDbContext _context;

        public AdmisionController(
            IBaseRepository<PostaCitasWeb.Entities.ProgramacionOperativa> programacionRepository,
            ICitaRepository citaRepository,
            IDateTimeProvider dateTimeProvider,
            IPacienteRepository pacienteRepository,
            ISlotRepository slotRepository,
            ICitaService citaService,
            AppDbContext context)
        {
            _programacionRepository = programacionRepository ?? throw new ArgumentNullException(nameof(programacionRepository));
            _citaRepository = citaRepository ?? throw new ArgumentNullException(nameof(citaRepository));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
            _slotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
            _citaService = citaService ?? throw new ArgumentNullException(nameof(citaService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.AdmisionNombre = User.Identity?.Name ?? "Admisión";
            
            // Get all programaciones (real data from DB)
            var programaciones = await _context.ProgramacionesOperativas
                .Include(p => p.Especialidad)
                .Include(p => p.Medico)
                .ToListAsync();

            // Get all appointments (both Web and Presencial) with details
            var citas = await _citaRepository.GetAllWithDetailsAsync();

            // Get all active specialties with UPS
            var especialidades = await _context.Especialidades
                .Include(e => e.UPS)
                .Where(e => e.Activa)
                .ToListAsync();

            // Prepare KPIs
            var today = _dateTimeProvider.Today;
            ViewBag.TotalCitasHoy = citas.Count(c => DateOnly.FromDateTime(c.FechaReserva) == today);
            ViewBag.TotalProgramaciones = programaciones.Count();
            ViewBag.TotalProgramacionesHabilitadas = programaciones.Count(p => p.Habilitada);
            ViewBag.TotalCitasPresenciales = citas.Count(c => c.OrigenReserva == PostaCitasWeb.Entities.OrigenReserva.Presencial);
            ViewBag.TotalCitasWeb = citas.Count(c => c.OrigenReserva == PostaCitasWeb.Entities.OrigenReserva.Web);

            var model = new AdmisionDashboardViewModel
            {
                Programaciones = programaciones,
                CitasPresenciales = citas, // Conceptually holding all citas for tracing
                Especialidades = especialidades
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPacientePorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni) || dni.Length != 8)
            {
                return Json(new { success = false, message = "El DNI debe tener 8 dígitos." });
            }

            var paciente = await _context.Pacientes
                .Include(p => p.Responsable)
                .FirstOrDefaultAsync(p => p.DNI == dni);

            if (paciente == null)
            {
                return Json(new { success = false, message = "Paciente no registrado en el sistema." });
            }

            string responsableNombre = "";
            string responsableDni = "";
            if (paciente.ResponsableId.HasValue)
            {
                var resp = await _pacienteRepository.GetByIdAsync(paciente.ResponsableId.Value);
                if (resp != null)
                {
                    responsableNombre = $"{resp.Nombres} {resp.ApellidoPaterno} {resp.ApellidoMaterno}";
                    responsableDni = resp.DNI;
                }
            }

            return Json(new
            {
                success = true,
                pacienteId = paciente.PacienteId,
                dni = paciente.DNI,
                nombres = paciente.Nombres,
                apellidoPaterno = paciente.ApellidoPaterno,
                apellidoMaterno = paciente.ApellidoMaterno,
                tieneSIS = paciente.TieneSIS,
                esMenor = paciente.EsMenor,
                historiaClinicaCode = $"HC-{paciente.PacienteId:D6}",
                responsableId = paciente.ResponsableId,
                responsableDni = responsableDni,
                responsableNombre = responsableNombre
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetSlotsDisponibles(int especialidadId, Turno turno, string? fecha = null)
        {
            DateOnly targetDate = _dateTimeProvider.Today;
            if (!string.IsNullOrEmpty(fecha) && DateOnly.TryParse(fecha, out var parsedDate))
            {
                targetDate = parsedDate;
            }

            var slots = await _slotRepository.GetSlotsByEspecialidadAndTurnoAndDateAsync(especialidadId, turno, targetDate);

            return Json(slots.Select(s => new
            {
                slotId = s.SlotId,
                horaInicio = s.HoraInicio.ToString(@"hh\:mm"),
                horaFin = s.HoraFin.ToString(@"hh\:mm"),
                cuposDisponibles = s.CuposDisponibles,
                medicoNombre = s.Programacion?.Medico != null
                    ? $"Dr. {s.Programacion.Medico.Nombres} {s.Programacion.Medico.ApellidoPaterno}"
                    : "Médico no asignado"
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservarCitaPresencial(int pacienteId, int slotId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int? registradoPorUsuarioId = null;
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                registradoPorUsuarioId = userId;
            }

            var result = await _citaService.ReserveCitaAsync(pacienteId, slotId, OrigenReserva.Presencial, registradoPorUsuarioId);
            if (result.Success)
            {
                return Json(new { success = true, message = result.Message, ticket = result.TicketCodigo });
            }
            return Json(new { success = false, message = result.Message });
        }

        // ============= CU11: HABILITAR DISPONIBILIDAD =============

        [HttpPost]
        public async Task<IActionResult> HabilitarProgramacion(int id)
        {
            try
            {
                var programacion = await _programacionRepository.GetByIdAsync(id);
                if (programacion == null)
                {
                    return Json(new { success = false, message = "Programación no encontrada." });
                }

                if (programacion.Fecha < _dateTimeProvider.Today)
                {
                    return Json(new { success = false, message = "No se puede habilitar programaciones pasadas." });
                }

                programacion.Habilitada = true;
                _programacionRepository.Update(programacion);
                await _programacionRepository.SaveChangesAsync();

                return Json(new { success = true, message = "Disponibilidad habilitada exitosamente. Los pacientes ya pueden reservar." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al habilitar: {ex.Message}" });
            }
        }

        // ============= CU12: AJUSTAR DISPONIBILIDAD (Deshabilitar) =============

        [HttpPost]
        public async Task<IActionResult> DeshabilitarProgramacion(int id)
        {
            try
            {
                var programacion = await _programacionRepository.GetByIdAsync(id);
                if (programacion == null)
                {
                    return Json(new { success = false, message = "Programación no encontrada." });
                }

                programacion.Habilitada = false;
                _programacionRepository.Update(programacion);
                await _programacionRepository.SaveChangesAsync();

                return Json(new { success = true, message = "Disponibilidad deshabilitada. Los pacientes ya no verán esta programación." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al deshabilitar: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AjustarProgramacion(int programacionId, int nuevosCupos)
        {
            try
            {
                var programacion = await _programacionRepository.GetByIdAsync(programacionId);
                if (programacion == null)
                {
                    return Json(new { success = false, message = "Programación no encontrada." });
                }

                if (programacion.Fecha < _dateTimeProvider.Today.AddDays(-1))
                {
                    return Json(new { success = false, message = "Solo se pueden ajustar programaciones futuras." });
                }

                if (nuevosCupos <= 0)
                {
                    return Json(new { success = false, message = "Los cupos deben ser mayores a 0." });
                }

                programacion.CuposTotal = nuevosCupos;
                _programacionRepository.Update(programacion);
                await _programacionRepository.SaveChangesAsync();

                return Json(new { success = true, message = $"Cupos ajustados a {nuevosCupos} exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al ajustar: {ex.Message}" });
            }
        }
    }
}

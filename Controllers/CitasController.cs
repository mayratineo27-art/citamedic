using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    [Authorize]
    public class CitasController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly IEspecialidadService _especialidadService;
        private readonly ICitaRepository _citaRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly ISlotRepository _slotRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CitasController(
            ICitaService citaService,
            IEspecialidadService especialidadService,
            ICitaRepository citaRepository,
            IPacienteRepository pacienteRepository,
            ISlotRepository slotRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _citaService = citaService ?? throw new ArgumentNullException(nameof(citaService));
            _especialidadService = especialidadService ?? throw new ArgumentNullException(nameof(especialidadService));
            _citaRepository = citaRepository ?? throw new ArgumentNullException(nameof(citaRepository));
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
            _slotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Challenge();
            }

            IEnumerable<Cita> list;
            if (User.IsInRole("Paciente"))
            {
                var paciente = await _pacienteRepository.GetByUsuarioIdAsync(userId);
                if (paciente == null)
                {
                    list = Array.Empty<Cita>();
                }
                else
                {
                    list = await _citaService.GetCitasByPacienteAsync(paciente.PacienteId);
                }
            }
            else
            {
                list = await _citaRepository.GetAllWithDetailsAsync();
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var allCitas = await _citaRepository.GetAllWithDetailsAsync();
            var cita = allCitas.FirstOrDefault(c => c.CitaId == id);
            if (cita == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(cita);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            await _citaService.CancelCitaAsync(id, "Cancelación por el usuario");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var result = await _citaService.CancelCitaAsync(id, "Cancelación por el usuario");
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Cita cancelada con éxito.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var especialidades = await _especialidadService.GetAllAsync();
            var cita = await _citaRepository.GetByIdAsync(id);
            if (cita == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new CitaEditViewModel
            {
                CitaId = cita.CitaId,
                EspecialidadId = cita.EspecialidadId,
                Fecha = cita.FechaReserva.Date,
                EstadoCita = cita.EstadoCita,
                PacienteNombre = cita.Paciente?.Nombres ?? string.Empty,
                EspecialidadNombre = cita.Especialidad?.Nombre ?? string.Empty,
                Especialidades = especialidades.Select(e => new SelectListItem
                {
                    Value = e.EspecialidadId.ToString(),
                    Text = e.Nombre
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CitaEditViewModel model)
        {
            var especialidades = await _especialidadService.GetAllAsync();

            if (!ModelState.IsValid)
            {
                model.Especialidades = especialidades.Select(e => new SelectListItem
                {
                    Value = e.EspecialidadId.ToString(),
                    Text = e.Nombre
                }).ToList();
                return View(model);
            }

            var cita = await _citaRepository.GetByIdAsync(model.CitaId);
            if (cita != null)
            {
                cita.EspecialidadId = model.EspecialidadId;
                cita.FechaReserva = model.Fecha;
                cita.FechaUltimaActualizacion = _dateTimeProvider.UtcNow;

                var especialidad = especialidades.FirstOrDefault(e => e.EspecialidadId == model.EspecialidadId);
                if (especialidad != null)
                {
                    cita.Especialidad = especialidad;
                }

                _citaRepository.Update(cita);
                await _citaRepository.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var especialidades = await _especialidadService.GetAllAsync();
            var model = new CitaViewModel
            {
                Especialidades = especialidades.Select(e => new SelectListItem
                {
                    Value = e.EspecialidadId.ToString(),
                    Text = e.Nombre
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var especialidades = await _especialidadService.GetAllAsync();
                model.Especialidades = especialidades.Select(e => new SelectListItem
                {
                    Value = e.EspecialidadId.ToString(),
                    Text = e.Nombre
                }).ToList();
                return View(model);
            }

            var cita = new Cita
            {
                EspecialidadId = model.EspecialidadId,
                FechaReserva = model.Fecha,
                EstadoCita = EstadoCita.Pendiente,
                OrigenReserva = OrigenReserva.Web,
                FechaUltimaActualizacion = _dateTimeProvider.UtcNow,
                Historial = new HashSet<HistorialEstadoCita>()
            };

            var creada = await _citaService.CreateCitaAsync(cita);
            if (!creada)
            {
                ModelState.AddModelError(string.Empty, "No fue posible crear la cita. Verifique la fecha y disponibilidad.");
                var especialidades = await _especialidadService.GetAllAsync();
                model.Especialidades = especialidades.Select(e => new SelectListItem
                {
                    Value = e.EspecialidadId.ToString(),
                    Text = e.Nombre
                }).ToList();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Reservar()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEspecialidades()
        {
            var list = await _especialidadService.GetAllAsync();
            // RN07: El paciente no visualiza la UPS interna, solo Especialidades activas.
            var result = list.Where(e => e.Activa).Select(e => new
            {
                especialidadId = e.EspecialidadId,
                nombre = e.Nombre
            }).ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSlots(int especialidadId, string turno)
        {
            Turno targetTurno;
            if (string.Equals(turno, "manana", StringComparison.OrdinalIgnoreCase))
            {
                targetTurno = Turno.Manana;
            }
            else if (string.Equals(turno, "tarde", StringComparison.OrdinalIgnoreCase))
            {
                targetTurno = Turno.Tarde;
            }
            else
            {
                return BadRequest("Turno inválido.");
            }

            // Calcular fechas según RN37 y FA03
            var localToday = _dateTimeProvider.Today;
            var dayOfWeek = localToday.DayOfWeek;
            var targetDates = new List<DateOnly>();

            if (dayOfWeek >= DayOfWeek.Monday && dayOfWeek <= DayOfWeek.Friday)
            {
                targetDates.Add(localToday); // Lunes a Viernes: Mismo día
            }
            else if (dayOfWeek == DayOfWeek.Saturday)
            {
                targetDates.Add(localToday); // Sábado: Mismo día (según FA03)
                targetDates.Add(localToday.AddDays(2)); // Sábado: Próximo lunes
            }
            else // Domingo
            {
                return Json(new { success = false, message = "Las reservas web no están disponibles los domingos (RN37)." });
            }

            var allSlots = new List<SlotDisponible>();
            foreach (var date in targetDates)
            {
                var slotsForDate = await _slotRepository.GetSlotsByEspecialidadAndTurnoAndDateAsync(especialidadId, targetTurno, date);
                allSlots.AddRange(slotsForDate);
            }
            
            var result = allSlots.OrderBy(s => s.Programacion?.Fecha).ThenBy(s => s.HoraInicio).Select(s => new
            {
                slotId = s.SlotId,
                horaInicio = s.HoraInicio.ToString(@"hh\:mm"),
                horaFin = s.HoraFin.ToString(@"hh\:mm"),
                cuposDisponibles = s.CuposDisponibles,
                nombreMedico = s.Programacion?.Medico != null 
                    ? $"{s.Programacion.Medico.Nombres} {s.Programacion.Medico.ApellidoPaterno}"
                    : "Médico no asignado",
                fecha = s.Programacion?.Fecha.ToString("dd/MM/yyyy") ?? string.Empty,
                fechaLarga = s.Programacion?.Fecha.ToString("dd 'de' MMMM, yyyy") ?? string.Empty
            }).ToList();

            return Json(new { success = true, slots = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar([FromForm] ReservaCitaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos del formulario inválidos." });
            }

            int pacienteId = 0;
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            if (User.IsInRole("Paciente"))
            {
                var paciente = await _pacienteRepository.GetByUsuarioIdAsync(userId);
                if (paciente == null)
                {
                    return Json(new { success = false, message = "No se encontró el perfil de paciente." });
                }
                pacienteId = paciente.PacienteId;
            }
            else if (model.PacienteDependienteId.HasValue)
            {
                pacienteId = model.PacienteDependienteId.Value;
            }
            else
            {
                return Json(new { success = false, message = "ID de paciente no especificado." });
            }

            var origen = User.IsInRole("Paciente") ? OrigenReserva.Web : OrigenReserva.Presencial;

            var result = await _citaService.ReserveCitaAsync(pacienteId, model.SlotId, origen, userId);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new
            {
                success = true,
                message = result.Message,
                citaId = result.CitaId,
                ticketCodigo = result.TicketCodigo
            });
        }

        [HttpGet]
        public async Task<IActionResult> Ticket(int id)
        {
            var allCitas = await _citaRepository.GetAllWithDetailsAsync();
            var cita = allCitas.FirstOrDefault(c => c.CitaId == id);
            if (cita == null || cita.Ticket == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new TicketViewModel
            {
                Codigo = cita.Ticket.Codigo,
                EspecialidadNombre = cita.Especialidad?.Nombre ?? string.Empty,
                MedicoNombre = cita.Slot?.Programacion?.Medico != null 
                    ? $"{cita.Slot.Programacion.Medico.Nombres} {cita.Slot.Programacion.Medico.ApellidoPaterno}"
                    : "Médico no asignado",
                FechaCita = cita.Slot?.Programacion != null ? cita.Slot.Programacion.Fecha : DateOnly.FromDateTime(cita.FechaReserva),
                HoraCita = cita.Slot != null ? cita.Slot.HoraInicio : TimeOnly.FromDateTime(cita.FechaReserva),
                Turno = cita.Slot?.Programacion?.Turno.ToString() ?? string.Empty,
                FechaEmision = cita.Ticket.FechaEmision,
                NombrePaciente = cita.Paciente != null 
                    ? $"{cita.Paciente.Nombres} {cita.Paciente.ApellidoPaterno} {cita.Paciente.ApellidoMaterno}"
                    : "Paciente no identificado"
            };

            return View(model);
        }
    }
}

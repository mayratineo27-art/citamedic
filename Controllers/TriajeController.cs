using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Models.ViewModels;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PostaCitasWeb.Controllers
{
    [Authorize(Roles = "Enfermeria")]
    public class TriajeController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly ICitaRepository _citaRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TriajeController(ICitaService citaService, ICitaRepository citaRepository, IDateTimeProvider dateTimeProvider)
        {
            _citaService = citaService ?? throw new ArgumentNullException(nameof(citaService));
            _citaRepository = citaRepository ?? throw new ArgumentNullException(nameof(citaRepository));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarCitaPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
             {
                 TempData["ErrorMessage"] = "El DNI ingresado no es válido.";
                 return RedirectToAction("Index", "Enfermeria");
             }

             var allCitas = await _citaRepository.GetAllWithDetailsAsync();
             var today = _dateTimeProvider.Today;

             // Buscar cita activa para hoy (Pendiente o EnTriaje)
             var cita = allCitas.FirstOrDefault(c =>
                 c.Paciente != null &&
                 c.Paciente.DNI == dni &&
                 c.Slot != null &&
                 c.Slot.Programacion != null &&
                 c.Slot.Programacion.Fecha == today &&
                 (c.EstadoCita == EstadoCita.Pendiente || c.EstadoCita == EstadoCita.EnTriaje)
             );

             if (cita == null)
             {
                 TempData["ErrorMessage"] = "El paciente no tiene reserva de cita registrada.";
                 return RedirectToAction("Index", "Enfermeria");
             }

             return RedirectToAction("Create", new { id = cita.CitaId });
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var allCitas = await _citaRepository.GetAllWithDetailsAsync();
            var cita = allCitas.FirstOrDefault(c => c.CitaId == id);
            if (cita == null)
            {
                return NotFound();
            }

            var model = new TriajeCreateViewModel
            {
                CitaId = id,
                PacienteNombre = cita.Paciente != null 
                    ? $"{cita.Paciente.Nombres} {cita.Paciente.ApellidoPaterno} {cita.Paciente.ApellidoMaterno}"
                    : $"Paciente #{cita.PacienteId}",
                EspecialidadNombre = cita.Especialidad?.Nombre ?? $"Especialidad #{cita.EspecialidadId}"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TriajeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int enfermeriaUserId))
            {
                return Challenge();
            }

            var result = await _citaService.RegistrarTriajeAsync(
                model.CitaId,
                enfermeriaUserId,
                model.Peso,
                model.Talla,
                model.Temperatura,
                model.PresionSistolica,
                model.PresionDiastolica,
                model.Observacion
            );

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction("Index", "Enfermeria");
        }
    }
}

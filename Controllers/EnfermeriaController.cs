using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Models.ViewModels;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PostaCitasWeb.Controllers
{
    [Authorize(Roles = "Enfermeria")]
    public class EnfermeriaController : Controller
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IAvisoRepository _avisoRepository;
        private readonly ICitaService _citaService;
        private readonly IAvisoService _avisoService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public EnfermeriaController(
            ICitaRepository citaRepository, 
            IAvisoRepository avisoRepository,
            ICitaService citaService,
            IAvisoService avisoService,
            IDateTimeProvider dateTimeProvider)
        {
            _citaRepository = citaRepository ?? throw new ArgumentNullException(nameof(citaRepository));
            _avisoRepository = avisoRepository ?? throw new ArgumentNullException(nameof(avisoRepository));
            _citaService = citaService ?? throw new ArgumentNullException(nameof(citaService));
            _avisoService = avisoService ?? throw new ArgumentNullException(nameof(avisoService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.EnfermeraNombre = User.Identity?.Name ?? "Enfermera";
            var allCitas = await _citaRepository.GetAllWithDetailsAsync();
            var today = _dateTimeProvider.Today;

            // RN21 & RN36: Obtener todas las citas del día
            var todayCitas = allCitas.Where(c =>
                c.Slot != null &&
                c.Slot.Programacion != null &&
                c.Slot.Programacion.Fecha == today
            ).ToList();

            // Cargar los avisos de atención inmediata incluyendo datos del Paciente
            var avisos = await _avisoRepository.GetAllWithPacienteAsync();

            var model = new EnfermeriaDashboardViewModel
            {
                CitasEnEspera = todayCitas,
                AvisosPendientes = avisos.Where(a => a.EstadoAviso == EstadoAviso.Pendiente).ToList(),
                AvisosVisualizados = avisos.Where(a => a.EstadoAviso == EstadoAviso.Visualizado).ToList(),
                AvisosCerrados = avisos.Where(a => a.EstadoAviso == EstadoAviso.Cerrado).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoCita(int citaId, EstadoCita estado)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int enfermeriaUserId))
            {
                return Challenge();
            }

            var result = await _citaService.ActualizarEstadoCitaAsync(citaId, estado, enfermeriaUserId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoAviso(int avisoId, EstadoAviso estado)
        {
            var success = await _avisoService.ActualizarEstadoAvisoAsync(avisoId, estado);
            if (success)
            {
                TempData["SuccessMessage"] = $"Se actualizó el estado del aviso a {estado}.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo actualizar el estado del aviso.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Models.ViewModels;
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
        private static readonly object _sync = new();
        private static readonly List<Cita> _citas = new()
        {
            new Cita
            {
                CitaId = 1,
                PacienteId = 1,
                Paciente = new Paciente { PacienteId = 1, DNI = "44444444", Nombres = "Juan", ApellidoPaterno = "Pérez", ApellidoMaterno = "García", FechaNacimiento = new DateOnly(1990, 1, 1), TieneSIS = true },
                EspecialidadId = 1,
                Especialidad = new Especialidad { EspecialidadId = 1, Nombre = "Medicina General", UPSId = 1, DuracionMinutos = 20, Activa = true },
                SlotId = 1,
                Slot = new SlotDisponible
                {
                    SlotId = 1,
                    ProgramacionId = 1,
                    HoraInicio = new TimeOnly(8, 0),
                    HoraFin = new TimeOnly(8, 20),
                    CuposDisponibles = 1,
                    CuposTotal = 10,
                    Programacion = new ProgramacionOperativa
                    {
                        ProgramacionId = 1,
                        EspecialidadId = 1,
                        MedicoId = 1,
                        Turno = Turno.Manana,
                        Fecha = DateOnly.FromDateTime(DateTime.Today),
                        CuposTotal = 10,
                        DuracionMinutos = 20,
                        Habilitada = true,
                        CreadaPorUsuarioId = 1,
                        Medico = new Medico { MedicoId = 1, Nombres = "Carlos", ApellidoPaterno = "Ramírez", ApellidoMaterno = string.Empty, CMP = "CMP001", Activo = true },
                        Especialidad = new Especialidad { EspecialidadId = 1, Nombre = "Medicina General", UPSId = 1, DuracionMinutos = 20, Activa = true }
                    }
                },
                EstadoCita = EstadoCita.Pendiente,
                OrigenReserva = OrigenReserva.Web,
                FechaReserva = DateTime.Today,
                FechaUltimaActualizacion = DateTime.UtcNow
            },
            new Cita
            {
                CitaId = 2,
                PacienteId = 1,
                Paciente = new Paciente { PacienteId = 1, DNI = "44444444", Nombres = "Juan", ApellidoPaterno = "Pérez", ApellidoMaterno = "García", FechaNacimiento = new DateOnly(1990, 1, 1), TieneSIS = true },
                EspecialidadId = 2,
                Especialidad = new Especialidad { EspecialidadId = 2, Nombre = "Odontología", UPSId = 2, DuracionMinutos = 25, Activa = true },
                SlotId = 2,
                Slot = new SlotDisponible
                {
                    SlotId = 2,
                    ProgramacionId = 1,
                    HoraInicio = new TimeOnly(8, 20),
                    HoraFin = new TimeOnly(8, 45),
                    CuposDisponibles = 2,
                    CuposTotal = 10,
                    Programacion = new ProgramacionOperativa
                    {
                        ProgramacionId = 1,
                        EspecialidadId = 2,
                        MedicoId = 1,
                        Turno = Turno.Manana,
                        Fecha = DateOnly.FromDateTime(DateTime.Today),
                        CuposTotal = 10,
                        DuracionMinutos = 25,
                        Habilitada = true,
                        CreadaPorUsuarioId = 1,
                        Medico = new Medico { MedicoId = 1, Nombres = "Carlos", ApellidoPaterno = "Ramírez", ApellidoMaterno = string.Empty, CMP = "CMP001", Activo = true },
                        Especialidad = new Especialidad { EspecialidadId = 2, Nombre = "Odontología", UPSId = 2, DuracionMinutos = 25, Activa = true }
                    }
                },
                EstadoCita = EstadoCita.EnTriaje,
                OrigenReserva = OrigenReserva.Presencial,
                FechaReserva = DateTime.Today.AddHours(8).AddMinutes(20),
                FechaUltimaActualizacion = DateTime.UtcNow
            }
        };

        private readonly ICitaService _citaService;
        private readonly IEspecialidadService _especialidadService;

        public CitasController(ICitaService citaService, IEspecialidadService especialidadService)
        {
            _citaService = citaService ?? throw new ArgumentNullException(nameof(citaService));
            _especialidadService = especialidadService ?? throw new ArgumentNullException(nameof(especialidadService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            lock (_sync)
            {
                return View(_citas.ToList());
            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            lock (_sync)
            {
                var cita = _citas.FirstOrDefault(c => c.CitaId == id);
                if (cita == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View(cita);
            }
        }

        [HttpGet]
        public IActionResult Cancel(int id)
        {
            lock (_sync)
            {
                var cita = _citas.FirstOrDefault(c => c.CitaId == id);
                if (cita != null)
                {
                    cita.EstadoCita = EstadoCita.Cancelada;
                    cita.FechaUltimaActualizacion = DateTime.UtcNow;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelConfirmed(int id)
        {
            lock (_sync)
            {
                var cita = _citas.FirstOrDefault(c => c.CitaId == id);
                if (cita != null)
                {
                    cita.EstadoCita = EstadoCita.Cancelada;
                    cita.FechaUltimaActualizacion = DateTime.UtcNow;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var especialidades = await _especialidadService.GetAllAsync();

            lock (_sync)
            {
                var cita = _citas.FirstOrDefault(c => c.CitaId == id);
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

            lock (_sync)
            {
                var cita = _citas.FirstOrDefault(c => c.CitaId == model.CitaId);
                if (cita != null)
                {
                    cita.EspecialidadId = model.EspecialidadId;
                    cita.FechaReserva = model.Fecha;
                    cita.FechaUltimaActualizacion = DateTime.UtcNow;

                    var especialidad = especialidades.FirstOrDefault(e => e.EspecialidadId == model.EspecialidadId);
                    if (especialidad != null)
                    {
                        cita.Especialidad = new Especialidad
                        {
                            EspecialidadId = especialidad.EspecialidadId,
                            Nombre = especialidad.Nombre,
                            UPSId = especialidad.UPSId,
                            DuracionMinutos = especialidad.DuracionMinutos,
                            Activa = especialidad.Activa
                        };
                    }
                }
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
                FechaUltimaActualizacion = DateTime.UtcNow,
                Historial = new System.Collections.Generic.HashSet<HistorialEstadoCita>()
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
    }
}

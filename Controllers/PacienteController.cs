using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Models.ViewModels;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostaCitasWeb.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class PacienteController : Controller
    {
        private readonly IEspecialidadService _especialidadService;
        private readonly ICitaService _citaService;

        public PacienteController(IEspecialidadService especialidadService, ICitaService citaService)
        {
            _especialidadService = especialidadService ?? throw new ArgumentNullException(nameof(especialidadService));
            _citaService = citaService ?? throw new ArgumentNullException(nameof(citaService));
        }

        public async Task<IActionResult> Index()
        {
            var citas = new List<Cita>
            {
                new Cita
                {
                    CitaId = 101,
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
                            Medico = new Medico { MedicoId = 1, Nombres = "Carlos", ApellidoPaterno = "Ramírez", ApellidoMaterno = string.Empty, CMP = "CMP001", Activo = true },
                            Especialidad = new Especialidad { EspecialidadId = 1, Nombre = "Medicina General", UPSId = 1, DuracionMinutos = 20, Activa = true },
                            Turno = Turno.Manana,
                            Fecha = DateOnly.FromDateTime(DateTime.Today),
                            CuposTotal = 10,
                            DuracionMinutos = 20,
                            Habilitada = true,
                            CreadaPorUsuarioId = 1
                        }
                    },
                    EstadoCita = EstadoCita.Pendiente,
                    OrigenReserva = OrigenReserva.Web,
                    FechaReserva = DateTime.Today.AddHours(8),
                    FechaUltimaActualizacion = DateTime.UtcNow
                }
            };

            var model = new PacienteDashboardViewModel
            {
                Especialidades = await _especialidadService.GetAllAsync(),
                MisCitas = citas
            };

            return View(model);
        }
    }
}

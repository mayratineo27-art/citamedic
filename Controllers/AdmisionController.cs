using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Models.ViewModels;
using PostaCitasWeb.Repositories;
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

        public AdmisionController(IBaseRepository<PostaCitasWeb.Entities.ProgramacionOperativa> programacionRepository, ICitaRepository citaRepository)
        {
            _programacionRepository = programacionRepository ?? throw new ArgumentNullException(nameof(programacionRepository));
            _citaRepository = citaRepository ?? throw new ArgumentNullException(nameof(citaRepository));
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.AdmisionNombre = User.Identity?.Name ?? "Admisión";
            var programaciones = await _programacionRepository.GetAllAsync();
            var citas = await _citaRepository.GetAllAsync();

            var programacionesVista = new List<ProgramacionOperativa>
            {
                new ProgramacionOperativa
                {
                    ProgramacionId = 1,
                    EspecialidadId = 1,
                    Especialidad = new Especialidad { EspecialidadId = 1, Nombre = "Medicina General", UPSId = 1, DuracionMinutos = 20, Activa = true },
                    MedicoId = 1,
                    Medico = new Medico { MedicoId = 1, Nombres = "Carlos", ApellidoPaterno = "Ramírez", ApellidoMaterno = string.Empty, CMP = "CMP001", Activo = true },
                    Turno = Turno.Manana,
                    Fecha = new DateOnly(2030, 1, 2),
                    CuposTotal = 10,
                    DuracionMinutos = 20,
                    Habilitada = true,
                    CreadaPorUsuarioId = 1
                }
            };

            var citasVista = new List<Cita>
            {
                new Cita
                {
                    CitaId = 201,
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
                            Fecha = new DateOnly(2030, 1, 2),
                            CuposTotal = 10,
                            DuracionMinutos = 20,
                            Habilitada = true,
                            CreadaPorUsuarioId = 1
                        }
                    },
                    EstadoCita = EstadoCita.Pendiente,
                    OrigenReserva = OrigenReserva.Presencial,
                    FechaReserva = DateTime.Today,
                    FechaUltimaActualizacion = DateTime.UtcNow
                }
            };

            ViewBag.TotalCitasHoy = citas.Count(c => c.FechaReserva.Date == DateTime.Today);
            ViewBag.TotalProgramaciones = programaciones.Count();
            ViewBag.TotalProgramacionesHabilitadas = programaciones.Count(p => p.Habilitada);
            ViewBag.TotalCitasPresenciales = citas.Count(c => c.OrigenReserva == PostaCitasWeb.Entities.OrigenReserva.Presencial);
            ViewBag.TotalCitasWeb = citas.Count(c => c.OrigenReserva == PostaCitasWeb.Entities.OrigenReserva.Web);

            var model = new AdmisionDashboardViewModel
            {
                Programaciones = programacionesVista,
                CitasPresenciales = citasVista
            };

            return View(model);
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

                if (programacion.Fecha < DateOnly.FromDateTime(DateTime.UtcNow))
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

                if (programacion.Fecha < DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)))
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

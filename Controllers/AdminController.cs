using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Models.ViewModels;
using PostaCitasWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Data;

namespace PostaCitasWeb.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IBaseRepository<UPS> _upsRepository;
        private readonly IBaseRepository<Especialidad> _especialidadRepository;
        private readonly IBaseRepository<Usuario> _usuarioRepository;
        private readonly IBaseRepository<Medico> _medicoRepository;
        private readonly IBaseRepository<ProgramacionOperativa> _programacionRepository;
        private readonly IBaseRepository<Paciente> _pacienteRepository;
        private readonly AppDbContext _context;

        public AdminController(
            IBaseRepository<UPS> upsRepository,
            IBaseRepository<Especialidad> especialidadRepository,
            IBaseRepository<Usuario> usuarioRepository,
            IBaseRepository<Medico> medicoRepository,
            IBaseRepository<ProgramacionOperativa> programacionRepository,
            IBaseRepository<Paciente> pacienteRepository,
            AppDbContext context)
        {
            _upsRepository = upsRepository;
            _especialidadRepository = especialidadRepository;
            _usuarioRepository = usuarioRepository;
            _medicoRepository = medicoRepository;
            _programacionRepository = programacionRepository;
            _pacienteRepository = pacienteRepository;
            _context = context;
        }

        // ==========================================
        // 1. DASHBOARD HUB
        // ==========================================
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.AdminNombre = User.Identity?.Name ?? "Administrador";
            return View();
        }

        // ==========================================
        // 2. MÓDULOS (Vistas separadas con mapeo)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var model = new AdminDashboardViewModel
            {
                Usuarios = usuarios.Select(u => new UsuarioViewModel
                {
                    UsuarioId = u.UsuarioId,
                    DNI = u.DNI,
                    NombreUsuario = u.NombreUsuario,
                    Rol = u.Rol.ToString(),
                    Celular = u.Celular,
                    Activo = u.Activo
                }).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UPS()
        {
            var ups = await _upsRepository.GetAllAsync();
            var model = new AdminDashboardViewModel
            {
                UPS = ups.Select(u => new UPSViewModel { UPSId = u.UPSId, Nombre = u.Nombre, Activa = u.Activa }).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Especialidades()
        {
            var especialidades = await _especialidadRepository.GetAllAsync();
            var upsList = await _upsRepository.GetAllAsync();

            var model = new AdminDashboardViewModel
            {
                Especialidades = especialidades.Select(e => new EspecialidadViewModel
                {
                    EspecialidadId = e.EspecialidadId,
                    Nombre = e.Nombre,
                    DuracionMinutos = e.DuracionMinutos,
                    UPSId = e.UPSId,
                    Activa = e.Activa,
                    UPSNombre = e.UPS?.Nombre ?? "N/A"
                }).ToList(),
                UPS = upsList.Select(u => new UPSViewModel { UPSId = u.UPSId, Nombre = u.Nombre, Activa = u.Activa }).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Medicos()
        {
            var medicos = await _medicoRepository.GetAllAsync();
            var model = new AdminDashboardViewModel
            {
                Medicos = medicos.Select(m => new MedicoViewModel
                {
                    MedicoId = m.MedicoId,
                    Nombres = m.Nombres,
                    ApellidoPaterno = m.ApellidoPaterno,
                    ApellidoMaterno = m.ApellidoMaterno,
                    CMP = m.CMP,
                    Activo = m.Activo
                }).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Programacion()
        {
            var programaciones = await _context.ProgramacionesOperativas
                .Include(p => p.Especialidad)
                .Include(p => p.Medico)
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                ProgramacionesOperativas = programaciones.Select(p => new ProgramacionViewModel
                {
                    ProgramacionId = p.ProgramacionId,
                    EspecialidadNombre = p.Especialidad?.Nombre ?? "N/A",
                    MedicoNombre = p.Medico != null ? $"{p.Medico.Nombres} {p.Medico.ApellidoPaterno}" : "N/A",
                    Turno = p.Turno.ToString(),
                    Fecha = p.Fecha.ToString("yyyy-MM-dd"),
                    CuposTotal = p.CuposTotal,
                    DuracionMinutos = p.DuracionMinutos,
                    Habilitada = p.Habilitada,
                    ProgramacionId_Key = p.ProgramacionId
                }).ToList(),
                Especialidades = (await _especialidadRepository.GetAllAsync())
                    .Select(e => new EspecialidadViewModel 
                    { 
                        EspecialidadId = e.EspecialidadId, 
                        Nombre = e.Nombre,
                        Activa = e.Activa 
                    }).ToList(),
                Medicos = (await _medicoRepository.GetAllAsync())
                    .Select(m => new MedicoViewModel 
                    { 
                        MedicoId = m.MedicoId, 
                        Nombres = m.Nombres, 
                        ApellidoPaterno = m.ApellidoPaterno, 
                        ApellidoMaterno = m.ApellidoMaterno,
                        Activo = m.Activo 
                    }).ToList()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Pacientes()
        {
            var pacientes = await _pacienteRepository.GetAllAsync();
            var usuarios = await _usuarioRepository.GetAllAsync();

            var usuariosDict = usuarios.ToDictionary(u => u.UsuarioId);

            var pacientesVm = pacientes.Select(p => {
                var usuario = usuariosDict.TryGetValue(p.UsuarioId, out var u) ? u : null;
                var responsable = p.ResponsableId.HasValue 
                    ? pacientes.FirstOrDefault(r => r.PacienteId == p.ResponsableId) 
                    : null;
                
                return new PacienteViewModel
                {
                    PacienteId = p.PacienteId,
                    UsuarioId = p.UsuarioId,
                    DNI = p.DNI,
                    Nombres = p.Nombres,
                    ApellidoPaterno = p.ApellidoPaterno,
                    ApellidoMaterno = p.ApellidoMaterno,
                    FechaNacimiento = p.FechaNacimiento.ToString("yyyy-MM-dd"),
                    TieneSIS = p.TieneSIS,
                    EsMenor = p.EsMenor,
                    ResponsableNombre = responsable != null ? $"{responsable.Nombres} {responsable.ApellidoPaterno}" : "Ninguno",
                    Celular = usuario?.Celular ?? "N/A"
                };
            }).ToList();

            var model = new AdminDashboardViewModel
            {
                Pacientes = pacientesVm
            };

            ViewBag.TotalPacientes = pacientesVm.Count;
            ViewBag.TotalSIS = pacientesVm.Count(p => p.TieneSIS);
            ViewBag.TotalMenores = pacientesVm.Count(p => p.EsMenor);

            return View(model);
        }

        // ==========================================
        // 3. MÉTODOS DE ACCIÓN (POST)
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> CreateUPS(CreateUPSViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(UPS));
            await _upsRepository.AddAsync(new UPS { Nombre = model.Nombre, Activa = model.Activa });
            await _upsRepository.SaveChangesAsync();
            return RedirectToAction(nameof(UPS));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEspecialidad(CreateEspecialidadViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Especialidades));
            await _especialidadRepository.AddAsync(new Especialidad { Nombre = model.Nombre, UPSId = model.UPSId, DuracionMinutos = model.DuracionMinutos, Activa = model.Activa });
            await _especialidadRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Especialidades));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedico(CreateMedicoViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Medicos));

            // Validar CMP único
            var medicosExistentes = await _medicoRepository.GetAllAsync();
            if (medicosExistentes.Any(m => m.CMP == model.CMP))
            {
                TempData["ErrorMessage"] = $"El número de colegiatura (CMP) '{model.CMP}' ya está registrado.";
                return RedirectToAction(nameof(Medicos));
            }

            await _medicoRepository.AddAsync(new Medico 
            { 
                Nombres = model.Nombres, 
                ApellidoPaterno = model.ApellidoPaterno, 
                ApellidoMaterno = model.ApellidoMaterno ?? "", 
                CMP = model.CMP, 
                Activo = true 
            });
            await _medicoRepository.SaveChangesAsync();
            TempData["SuccessMessage"] = "Médico registrado exitosamente.";
            return RedirectToAction(nameof(Medicos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUPS(int id)
        {
            var ups = await _upsRepository.GetByIdAsync(id);
            if (ups != null)
            {
                ups.Activa = false;
                _upsRepository.Update(ups);
                await _upsRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "UPS desactivada exitosamente";
            }

            return RedirectToAction(nameof(UPS));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUPS(int id)
        {
            var ups = await _upsRepository.GetByIdAsync(id);
            if (ups != null)
            {
                ups.Activa = true;
                _upsRepository.Update(ups);
                await _upsRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "UPS activada exitosamente";
            }

            return RedirectToAction(nameof(UPS));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEspecialidad(int id)
        {
            var especialidad = await _especialidadRepository.GetByIdAsync(id);
            if (especialidad != null)
            {
                especialidad.Activa = false;
                _especialidadRepository.Update(especialidad);
                await _especialidadRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Especialidad desactivada exitosamente";
            }

            return RedirectToAction(nameof(Especialidades));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateEspecialidad(int id)
        {
            var especialidad = await _especialidadRepository.GetByIdAsync(id);
            if (especialidad != null)
            {
                especialidad.Activa = true;
                _especialidadRepository.Update(especialidad);
                await _especialidadRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Especialidad activada exitosamente";
            }

            return RedirectToAction(nameof(Especialidades));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUPS(int upsId, string nombre, bool activa)
        {
            var ups = await _upsRepository.GetByIdAsync(upsId);
            if (ups != null)
            {
                ups.Nombre = nombre;
                ups.Activa = activa;
                _upsRepository.Update(ups);
                await _upsRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "UPS actualizada exitosamente";
            }

            return RedirectToAction(nameof(UPS));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEspecialidad(int especialidadId, string nombre, int upsId, int duracionMinutos, bool activa)
        {
            var especialidad = await _especialidadRepository.GetByIdAsync(especialidadId);
            if (especialidad != null)
            {
                especialidad.Nombre = nombre;
                especialidad.UPSId = upsId;
                especialidad.DuracionMinutos = duracionMinutos;
                especialidad.Activa = activa;
                _especialidadRepository.Update(especialidad);
                await _especialidadRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Especialidad actualizada exitosamente";
            }

            return RedirectToAction(nameof(Especialidades));
        }

        // MÉTODOS PARA USUARIOS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUsuario(CreateUsuarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nuevoUsuario = new Usuario
                {
                    DNI = model.DNI,
                    NombreUsuario = model.NombreUsuario,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Rol = (Rol)model.Rol,
                    Activo = model.Activo,
                    Celular = model.Celular,
                    FechaCreacion = DateTime.UtcNow
                };

                await _usuarioRepository.AddAsync(nuevoUsuario);
                await _usuarioRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Usuarios));
            }

            return RedirectToAction(nameof(Usuarios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.Activo = false;
                _usuarioRepository.Update(usuario);
                await _usuarioRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario desactivado exitosamente";
            }

            return RedirectToAction(nameof(Usuarios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUsuario(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.Activo = true;
                _usuarioRepository.Update(usuario);
                await _usuarioRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario activado exitosamente";
            }

            return RedirectToAction(nameof(Usuarios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedico(int id)
        {
            var medico = await _medicoRepository.GetByIdAsync(id);
            if (medico != null)
            {
                medico.Activo = false;
                _medicoRepository.Update(medico);
                await _medicoRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Médico desactivado exitosamente";
            }

            return RedirectToAction(nameof(Medicos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateMedico(int id)
        {
            var medico = await _medicoRepository.GetByIdAsync(id);
            if (medico != null)
            {
                medico.Activo = true;
                _medicoRepository.Update(medico);
                await _medicoRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Médico activado exitosamente";
            }

            return RedirectToAction(nameof(Medicos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMedico(int medicoId, string nombres, string apellidoPaterno, string apellidoMaterno, string cmp, bool activo)
        {
            var medico = await _medicoRepository.GetByIdAsync(medicoId);
            if (medico != null)
            {
                // Validar CMP único (excluyendo al propio médico)
                var medicosExistentes = await _medicoRepository.GetAllAsync();
                if (medicosExistentes.Any(m => m.CMP == cmp && m.MedicoId != medicoId))
                {
                    TempData["ErrorMessage"] = $"El número de colegiatura (CMP) '{cmp}' ya está registrado por otro médico.";
                    return RedirectToAction(nameof(Medicos));
                }

                medico.Nombres = nombres;
                medico.ApellidoPaterno = apellidoPaterno;
                medico.ApellidoMaterno = apellidoMaterno ?? "";
                medico.CMP = cmp;
                medico.Activo = activo;

                _medicoRepository.Update(medico);
                await _medicoRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Médico actualizado exitosamente.";
            }

            return RedirectToAction(nameof(Medicos));
        }

        // MÉTODOS PARA PROGRAMACIÓN OPERATIVA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProgramacion(CreateProgramacionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!DateTime.TryParse(model.Fecha, out DateTime fecha))
                {
                    TempData["ErrorMessage"] = "Fecha inválida";
                    return RedirectToAction(nameof(Programacion));
                }

                var nuevaProgramacion = new ProgramacionOperativa
                {
                    EspecialidadId = model.EspecialidadId,
                    MedicoId = model.MedicoId,
                    Turno = (Turno)model.Turno,
                    Fecha = DateOnly.FromDateTime(fecha),
                    CuposTotal = model.CuposTotal,
                    DuracionMinutos = model.DuracionMinutos,
                    Habilitada = model.Habilitada,
                    FechaCreacion = DateTime.UtcNow,
                    CreadaPorUsuarioId = 1 // TODO: Obtener usuario actual
                };

                var slots = new List<SlotDisponible>();
                var horaInicio = model.Turno == (int)Turno.Manana ? new TimeOnly(8, 0) : new TimeOnly(14, 0);

                for (int i = 0; i < model.CuposTotal; i++)
                {
                    slots.Add(new SlotDisponible
                    {
                        Programacion = nuevaProgramacion,
                        HoraInicio = horaInicio,
                        HoraFin = horaInicio.AddMinutes(model.DuracionMinutos),
                        CuposDisponibles = 1,
                        CuposTotal = 1,
                        EsSobrecupo = false
                    });
                    horaInicio = horaInicio.AddMinutes(model.DuracionMinutos);
                }

                nuevaProgramacion.Slots = slots;

                await _programacionRepository.AddAsync(nuevaProgramacion);
                await _programacionRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Programación creada exitosamente";
                return RedirectToAction(nameof(Programacion));
            }

            return RedirectToAction(nameof(Programacion));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProgramacion(int id)
        {
            var programacion = await _programacionRepository.GetByIdAsync(id);
            if (programacion != null)
            {
                programacion.Habilitada = false;
                _programacionRepository.Update(programacion);
                await _programacionRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Programación deshabilitada exitosamente";
            }

            return RedirectToAction(nameof(Programacion));
        }

        // ============= CU18: GESTIONAR USUARIOS =============

        [HttpGet]
        public async Task<IActionResult> BuscarDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni) || dni.Length != 8 || !dni.All(char.IsDigit))
            {
                return Json(new { success = false, message = "DNI inválido. Debe ser 8 dígitos numéricos." });
            }

            var pacientes = await _pacienteRepository.GetAllAsync();
            var paciente = pacientes.FirstOrDefault(p => p.DNI == dni);

            if (paciente != null)
            {
                return Json(new
                {
                    success = true,
                    existe = true,
                    nombres = paciente.Nombres,
                    apellidos = $"{paciente.ApellidoPaterno} {paciente.ApellidoMaterno}".Trim()
                });
            }

            return Json(new
            {
                success = true,
                existe = false,
                message = "DNI no encontrado. Por favor, registre los datos del paciente manualmente."
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado." });
            }

            return Json(new
            {
                success = true,
                usuarioId = usuario.UsuarioId,
                dni = usuario.DNI,
                nombreUsuario = usuario.NombreUsuario,
                rol = (int)usuario.Rol,
                celular = usuario.Celular,
                activo = usuario.Activo
            });
        }

        [HttpPost]
        public async Task<IActionResult> GuardarUsuario([FromBody] CreateUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Validación fallida en los datos ingresados.", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            if (string.IsNullOrWhiteSpace(model.DNI) || model.DNI.Length != 8 || !model.DNI.All(char.IsDigit))
            {
                return Json(new { success = false, message = "DNI inválido. Debe ser 8 dígitos numéricos." });
            }

            try
            {
                var usuarioExistente = (await _usuarioRepository.GetAllAsync())
                    .FirstOrDefault(u => u.DNI == model.DNI);

                if (usuarioExistente != null)
                {
                    return Json(new { success = false, message = "DNI ya registrado en el sistema." });
                }

                var nuevoUsuario = new Usuario
                {
                    DNI = model.DNI,
                    NombreUsuario = model.NombreUsuario,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Rol = (Rol)model.Rol,
                    Activo = model.Activo,
                    Celular = model.Celular,
                    FechaCreacion = DateTime.UtcNow
                };

                await _usuarioRepository.AddAsync(nuevoUsuario);
                await _usuarioRepository.SaveChangesAsync();

                return Json(new { success = true, message = "Usuario creado exitosamente.", usuarioId = nuevoUsuario.UsuarioId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al guardar usuario: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarUsuario(int usuarioId, [FromBody] CreateUsuarioViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password) && string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ModelState.Remove(nameof(model.Password));
                ModelState.Remove(nameof(model.ConfirmPassword));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, message = "Validación fallida: " + string.Join(", ", errors) });
            }

            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }

                usuario.NombreUsuario = model.NombreUsuario;
                usuario.Rol = (Rol)model.Rol;
                usuario.Celular = model.Celular;
                usuario.Activo = model.Activo;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                _usuarioRepository.Update(usuario);
                await _usuarioRepository.SaveChangesAsync();

                return Json(new { success = true, message = "Usuario actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al actualizar usuario: {ex.Message}" });
            }
        }

        private RENIECMockData ObtenerDatosRENIECMock(string dni)
        {
            var mockData = new Dictionary<string, RENIECMockData>
            {
                { "12345678", new RENIECMockData { Nombres = "Juan", Apellidos = "Pérez García" } },
                { "87654321", new RENIECMockData { Nombres = "María", Apellidos = "López Martínez" } },
                { "11223344", new RENIECMockData { Nombres = "Carlos", Apellidos = "Rodríguez Sánchez" } },
                { "99887766", new RENIECMockData { Nombres = "Ana", Apellidos = "Fernández Díaz" } }
            };

            if (mockData.TryGetValue(dni, out var data))
            {
                return data;
            }

            return new RENIECMockData
            {
                Nombres = $"Usuario_{dni.Substring(0, 4)}",
                Apellidos = $"Apellido_{dni.Substring(4, 4)}"
            };
        }

    }

    public class RENIECMockData
    {
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
    }
}
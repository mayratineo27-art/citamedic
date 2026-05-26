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

        public AdminController(
            IBaseRepository<UPS> upsRepository,
            IBaseRepository<Especialidad> especialidadRepository,
            IBaseRepository<Usuario> usuarioRepository,
            IBaseRepository<Medico> medicoRepository,
            IBaseRepository<ProgramacionOperativa> programacionRepository)
        {
            _upsRepository = upsRepository ?? throw new ArgumentNullException(nameof(upsRepository));
            _especialidadRepository = especialidadRepository ?? throw new ArgumentNullException(nameof(especialidadRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _medicoRepository = medicoRepository ?? throw new ArgumentNullException(nameof(medicoRepository));
            _programacionRepository = programacionRepository ?? throw new ArgumentNullException(nameof(programacionRepository));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ups = await _upsRepository.GetAllAsync();
            var especialidades = await _especialidadRepository.GetAllAsync();
            var usuarios = await _usuarioRepository.GetAllAsync();
            var medicos = await _medicoRepository.GetAllAsync();
            var programaciones = await _programacionRepository.GetAllAsync();

            var model = new AdminDashboardViewModel
            {
                Programaciones = new List<AdminProgramacionViewModel>(),
                UPS = ups.Select(u => new UPSViewModel
                {
                    UPSId = u.UPSId,
                    Nombre = u.Nombre,
                    Activa = u.Activa
                }),
                Especialidades = especialidades.Select(e => new EspecialidadViewModel
                {
                    EspecialidadId = e.EspecialidadId,
                    Nombre = e.Nombre,
                    DuracionMinutos = e.DuracionMinutos,
                    UPSNombre = e.UPS?.Nombre ?? "N/A",
                    Activa = e.Activa
                }),
                Usuarios = usuarios.Select(u => new UsuarioViewModel
                {
                    UsuarioId = u.UsuarioId,
                    DNI = u.DNI,
                    NombreUsuario = u.NombreUsuario,
                    Rol = u.Rol.ToString(),
                    Celular = u.Celular,
                    Activo = u.Activo,
                    FechaCreacion = u.FechaCreacion.ToString("dd/MM/yyyy")
                }),
                Medicos = medicos.Select(m => new MedicoViewModel
                {
                    MedicoId = m.MedicoId,
                    Nombres = m.Nombres,
                    ApellidoPaterno = m.ApellidoPaterno,
                    ApellidoMaterno = m.ApellidoMaterno,
                    CMP = m.CMP,
                    Activo = m.Activo
                }),
                ProgramacionesOperativas = programaciones.Select(p => new ProgramacionViewModel
                {
                    ProgramacionId = p.ProgramacionId,
                    EspecialidadNombre = p.Especialidad?.Nombre ?? "N/A",
                    MedicoNombre = p.Medico != null ? $"{p.Medico.Nombres} {p.Medico.ApellidoPaterno}" : "N/A",
                    Turno = p.Turno.ToString(),
                    Fecha = p.Fecha.ToString("dd/MM/yyyy"),
                    CuposTotal = p.CuposTotal,
                    DuracionMinutos = p.DuracionMinutos,
                    Habilitada = p.Habilitada,
                    ProgramacionId_Key = p.ProgramacionId
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUPS(CreateUPSViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nuevoUPS = new UPS
                {
                    Nombre = model.Nombre,
                    Activa = model.Activa
                };

                await _upsRepository.AddAsync(nuevoUPS);
                await _upsRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "UPS creada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEspecialidad(CreateEspecialidadViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nuevaEspecialidad = new Especialidad
                {
                    Nombre = model.Nombre,
                    UPSId = model.UPSId,
                    DuracionMinutos = model.DuracionMinutos,
                    Activa = model.Activa
                };

                await _especialidadRepository.AddAsync(nuevaEspecialidad);
                await _especialidadRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Especialidad creada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUPS(int upsId, string nombre)
        {
            var ups = await _upsRepository.GetByIdAsync(upsId);
            if (ups != null)
            {
                ups.Nombre = nombre;
                _upsRepository.Update(ups);
                await _upsRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "UPS actualizada exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEspecialidad(int especialidadId, string nombre, int upsId, int duracionMinutos)
        {
            var especialidad = await _especialidadRepository.GetByIdAsync(especialidadId);
            if (especialidad != null)
            {
                especialidad.Nombre = nombre;
                especialidad.UPSId = upsId;
                especialidad.DuracionMinutos = duracionMinutos;
                _especialidadRepository.Update(especialidad);
                await _especialidadRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Especialidad actualizada exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        // MÉTODOS PARA USUARIOS
        [HttpPost]
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
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
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

            return RedirectToAction(nameof(Index));
        }

        // MÉTODOS PARA MÉDICOS
        [HttpPost]
        public async Task<IActionResult> CreateMedico(CreateMedicoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nuevoMedico = new Medico
                {
                    Nombres = model.Nombres,
                    ApellidoPaterno = model.ApellidoPaterno,
                    ApellidoMaterno = model.ApellidoMaterno,
                    CMP = model.CMP,
                    Activo = model.Activo
                };

                await _medicoRepository.AddAsync(nuevoMedico);
                await _medicoRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Médico creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
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

            return RedirectToAction(nameof(Index));
        }

        // MÉTODOS PARA PROGRAMACIÓN OPERATIVA
        [HttpPost]
        public async Task<IActionResult> CreateProgramacion(CreateProgramacionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!DateTime.TryParse(model.Fecha, out DateTime fecha))
                {
                    TempData["ErrorMessage"] = "Fecha inválida";
                    return RedirectToAction(nameof(Index));
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

                await _programacionRepository.AddAsync(nuevaProgramacion);
                await _programacionRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Programación creada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
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

            return RedirectToAction(nameof(Index));
        }

        // ============= CU18: GESTIONAR USUARIOS =============

        [HttpGet]
        public async Task<IActionResult> GestionarUsuarios()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var usuariosViewModel = usuarios.Select(u => new UsuarioViewModel
            {
                UsuarioId = u.UsuarioId,
                DNI = u.DNI,
                NombreUsuario = u.NombreUsuario,
                Rol = u.Rol.ToString(),
                Celular = u.Celular,
                Activo = u.Activo,
                FechaCreacion = u.FechaCreacion.ToString("dd/MM/yyyy")
            }).ToList();

            return View(usuariosViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarDni(string dni)
        {
            // Validar formato: exactamente 8 dígitos numéricos
            if (string.IsNullOrWhiteSpace(dni) || dni.Length != 8 || !dni.All(char.IsDigit))
            {
                return Json(new { success = false, message = "DNI inválido. Debe ser 8 dígitos numéricos." });
            }

            // Verificar que el DNI no exista en la base de datos
            var usuarioExistente = (await _usuarioRepository.GetAllAsync())
                .FirstOrDefault(u => u.DNI == dni);

            if (usuarioExistente != null)
            {
                return Json(new { success = false, message = "DNI ya registrado en el sistema." });
            }

            // Mock RENIEC: retorna datos simulados
            var datosRENIEC = ObtenerDatosRENIECMock(dni);

            return Json(new
            {
                success = true,
                nombres = datosRENIEC.Nombres,
                apellidos = datosRENIEC.Apellidos
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
        public async Task<IActionResult> GuardarUsuario(CreateUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Validación fallida en los datos ingresados.", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validar DNI: exactamente 8 dígitos
            if (string.IsNullOrWhiteSpace(model.DNI) || model.DNI.Length != 8 || !model.DNI.All(char.IsDigit))
            {
                return Json(new { success = false, message = "DNI inválido. Debe ser 8 dígitos numéricos." });
            }

            try
            {
                // Verificar si es creación o edición (por ahora solo creación)
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
        public async Task<IActionResult> EditarUsuario(int usuarioId, CreateUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Validación fallida en los datos ingresados." });
            }

            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuario == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }

                // No permitir cambiar DNI
                // Actualizar campos permitidos
                usuario.NombreUsuario = model.NombreUsuario;
                usuario.Rol = (Rol)model.Rol;
                usuario.Celular = model.Celular;
                usuario.Activo = model.Activo;

                // Si proporciona nueva contraseña, actualizarla
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
            // Mock simple: datos de prueba basados en DNI
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

            // Generar datos aleatorios para DNI no predefinido
            return new RENIECMockData
            {
                Nombres = $"Usuario_{dni.Substring(0, 4)}",
                Apellidos = $"Apellido_{dni.Substring(4, 4)}"
            };
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

                // Validar que no sea fecha pasada
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

        // ============= CU12: AJUSTAR DISPONIBILIDAD (Desabilitar) =============

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
                return Json(new { success = false, message = $"Error al desabilitar: {ex.Message}" });
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

                // Validar que sea fecha futura
                if (programacion.Fecha < DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)))
                {
                    return Json(new { success = false, message = "Solo se pueden ajustar programaciones futuras." });
                }

                // Validar cupos mínimos
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

    public class RENIECMockData
    {
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
    }
}
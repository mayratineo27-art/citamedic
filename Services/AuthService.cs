using BC = BCrypt.Net.BCrypt;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using System;
using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Implementación del servicio de autenticación.
    /// Maneja login, registro y validaciones de usuarios.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPacienteRepository _pacienteRepository;

        public AuthService(IUsuarioRepository usuarioRepository, IPacienteRepository pacienteRepository)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        /// <summary>
        /// Autentica un usuario validando sus credenciales.
        /// RN01: Solo usuarios con Activo=true pueden acceder.
        /// </summary>
        public Task<AuthResult> ValidarCredenciales(string dni, string password)
        {
            return LoginAsync(dni, password);
        }

        /// <summary>
        /// Autentica un usuario validando sus credenciales.
        /// RN01: Solo usuarios con Activo=true pueden acceder.
        /// </summary>
        public async Task<AuthResult> LoginAsync(string nombreUsuario, string password)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return AuthResult.CreateFailure("El nombre de usuario es requerido.");

            if (string.IsNullOrWhiteSpace(password))
                return AuthResult.CreateFailure("La contraseña es requerida.");

            // Buscar usuario por DNI o por el nombre semilla usado en desarrollo
            var usuario = await _usuarioRepository.GetByDniAsync(nombreUsuario);
            if (usuario == null)
            {
                usuario = await _usuarioRepository.GetByNombreUsuarioAsync(nombreUsuario);
            }
            if (usuario == null)
                return AuthResult.CreateFailure("Nombre de usuario o contraseña incorrectos.");

            // Verificar que esté activo (RN01)
            if (!usuario.Activo)
                return AuthResult.CreateFailure("Esta cuenta no está habilitada. Contacte al administrador.");

            // Verificar contraseña
            if (!EsPasswordValida(password, usuario.PasswordHash))
                return AuthResult.CreateFailure("Nombre de usuario o contraseña incorrectos.");

            // Obtener información adicional si es paciente
            int? pacienteId = null;
            if (usuario.Rol == Rol.Paciente)
            {
                var paciente = await _pacienteRepository.GetByUsuarioIdAsync(usuario.UsuarioId);
                pacienteId = paciente?.PacienteId;
            }

            // Retornar resultado exitoso
            var rol = usuario.Rol.ToString();
            return AuthResult.CreateSuccess(usuario.UsuarioId, usuario.NombreUsuario, pacienteId, rol);
        }

        /// <summary>
        /// Registra un nuevo usuario con rol Paciente.
        /// RN01: Se crea con Activo=false, debe ser habilitado por Admisión.
        /// RN02: Los datos del paciente son inmutables después de la creación.
        /// </summary>
        public async Task<AuthResult> RegisterPacienteAsync(string dni, string nombres, string apellidoPaterno,
            string apellidoMaterno, string nombreUsuario, string password, string celular)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dni) || dni.Length != 8)
                return AuthResult.CreateFailure("El DNI debe tener 8 dígitos.");

            if (string.IsNullOrWhiteSpace(nombres))
                return AuthResult.CreateFailure("Los nombres son requeridos.");

            if (string.IsNullOrWhiteSpace(apellidoPaterno))
                return AuthResult.CreateFailure("El apellido paterno es requerido.");

            if (string.IsNullOrWhiteSpace(nombreUsuario) || nombreUsuario.Length < 4)
                return AuthResult.CreateFailure("El nombre de usuario debe tener al menos 4 caracteres.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return AuthResult.CreateFailure("La contraseña debe tener al menos 8 caracteres.");

            if (string.IsNullOrWhiteSpace(celular) || celular.Length < 9)
                return AuthResult.CreateFailure("El celular debe tener al menos 9 dígitos.");

            // Verificar duplicados
            if (await _usuarioRepository.NombreUsuarioExistsAsync(nombreUsuario))
                return AuthResult.CreateFailure("El nombre de usuario ya está registrado.");

            if (await _usuarioRepository.DniExistsAsync(dni))
                return AuthResult.CreateFailure("El DNI ya está registrado.");

            if (await _pacienteRepository.DniExistsAsync(dni))
                return AuthResult.CreateFailure("El DNI ya está registrado como paciente.");

            try
            {
                // Crear usuario (inactivo por defecto - RN01)
                var usuario = new Usuario
                {
                    DNI = dni,
                    NombreUsuario = nombreUsuario,
                    PasswordHash = BC.HashPassword(password),
                    Rol = Rol.Paciente,
                    Activo = false, // RN01: Debe ser habilitado por Admisión
                    Celular = celular,
                    FechaCreacion = DateTime.UtcNow
                };

                await _usuarioRepository.AddAsync(usuario);
                await _usuarioRepository.SaveChangesAsync();

                // Crear paciente con datos inmutables (RN02)
                var paciente = new Paciente
                {
                    UsuarioId = usuario.UsuarioId,
                    DNI = dni,
                    Nombres = nombres,
                    ApellidoPaterno = apellidoPaterno,
                    ApellidoMaterno = apellidoMaterno ?? string.Empty,
                    FechaNacimiento = DateOnly.FromDateTime(DateTime.Now) // Será actualizado después
                };

                await _pacienteRepository.AddAsync(paciente);
                await _pacienteRepository.SaveChangesAsync();

                return AuthResult.CreateSuccess(usuario.UsuarioId, usuario.NombreUsuario, paciente.PacienteId, "Paciente registrado");
            }
            catch (Exception ex)
            {
                return AuthResult.CreateFailure($"Error al registrar: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si un nombre de usuario está disponible para registro.
        /// </summary>
        public async Task<bool> IsNombreUsuarioAvailableAsync(string nombreUsuario)
        {
            return !await _usuarioRepository.NombreUsuarioExistsAsync(nombreUsuario);
        }

        /// <summary>
        /// Verifica si un DNI ya está registrado en el sistema.
        /// </summary>
        public async Task<bool> IsDniRegisteredAsync(string dni)
        {
            return await _usuarioRepository.DniExistsAsync(dni);
        }

        private static bool EsPasswordValida(string password, string storedPassword)
        {
            if (string.IsNullOrWhiteSpace(storedPassword))
            {
                return false;
            }

            if (storedPassword.StartsWith("$2"))
            {
                return BC.Verify(password, storedPassword);
            }

            return string.Equals(password, storedPassword, StringComparison.Ordinal);
        }
    }
}

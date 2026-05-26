using System.Threading.Tasks;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Interfaz para servicios de autenticación.
    /// Define métodos para login y registro de usuarios.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Valida credenciales contra la base de datos usando el DNI como identificador.
        /// </summary>
        Task<AuthResult> ValidarCredenciales(string dni, string password);

        /// <summary>
        /// Autentica un usuario con nombre de usuario y contraseña.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Resultado de autenticación con información del usuario</returns>
        Task<AuthResult> LoginAsync(string nombreUsuario, string password);

        /// <summary>
        /// Registra un nuevo usuario paciente.
        /// </summary>
        Task<AuthResult> RegisterPacienteAsync(string dni, string nombres, string apellidoPaterno, 
            string apellidoMaterno, string nombreUsuario, string password, string celular);

        /// <summary>
        /// Verifica si un nombre de usuario está disponible.
        /// </summary>
        Task<bool> IsNombreUsuarioAvailableAsync(string nombreUsuario);

        /// <summary>
        /// Verifica si un DNI ya está registrado.
        /// </summary>
        Task<bool> IsDniRegisteredAsync(string dni);
    }

    /// <summary>
    /// Resultado de una operación de autenticación.
    /// </summary>
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public int? PacienteId { get; set; }
        public string? Rol { get; set; }

        public static AuthResult CreateSuccess(int usuarioId, string nombreUsuario, int? pacienteId, string rol)
        {
            return new AuthResult
            {
                Success = true,
                Message = "Autenticación exitosa",
                UsuarioId = usuarioId,
                NombreUsuario = nombreUsuario,
                PacienteId = pacienteId,
                Rol = rol
            };
        }

        public static AuthResult CreateFailure(string message)
        {
            return new AuthResult
            {
                Success = false,
                Message = message
            };
        }
    }
}

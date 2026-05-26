using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Models.ViewModels
{
    public class CreateUsuarioViewModel
    {
        [Required(ErrorMessage = "El DNI es requerido")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 dígitos")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es requerido")]
        public int Rol { get; set; }

        [Required(ErrorMessage = "El celular es requerido")]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "El celular debe tener entre 9 y 15 dígitos")]
        public string Celular { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe indicar si el usuario está activo")]
        public bool Activo { get; set; } = true;
    }

    public class UsuarioViewModel
    {
        public int UsuarioId { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string FechaCreacion { get; set; } = string.Empty;
    }
}

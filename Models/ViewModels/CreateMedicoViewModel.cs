using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Models.ViewModels
{
    public class CreateMedicoViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido paterno es requerido")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres")]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El apellido debe tener máximo 50 caracteres")]
        public string ApellidoMaterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CMP es requerido")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "El CMP debe tener entre 5 y 20 caracteres")]
        public string CMP { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }

    public class MedicoViewModel
    {
        public int MedicoId { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string CMP { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string NombreCompleto => $"{Nombres} {ApellidoPaterno} {ApellidoMaterno}".Trim();
    }
}

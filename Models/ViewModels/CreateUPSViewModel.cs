using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Models.ViewModels
{
    public class CreateUPSViewModel
    {
        [Required(ErrorMessage = "El nombre de la UPS es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        public bool Activa { get; set; } = true;
    }
}

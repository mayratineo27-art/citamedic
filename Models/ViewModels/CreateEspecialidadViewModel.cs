using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Models.ViewModels
{
    public class CreateEspecialidadViewModel
    {
        [Required(ErrorMessage = "El nombre de la especialidad es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar una UPS")]
        public int UPSId { get; set; }

        [Required(ErrorMessage = "La duración en minutos es requerida")]
        [Range(1, 480, ErrorMessage = "La duración debe estar entre 1 y 480 minutos")]
        public int DuracionMinutos { get; set; }

        public bool Activa { get; set; } = true;

        // Para llenar el dropdown de UPS
        public IEnumerable<UPSViewModel> UPS { get; set; } = new List<UPSViewModel>();
    }

    public class UPSViewModel
    {
        public int UPSId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activa { get; set; }
    }
}

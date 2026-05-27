using System;
using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Models.ViewModels
{
    public class ReservaCitaViewModel
    {
        [Required(ErrorMessage = "El slot es obligatorio.")]
        public int SlotId { get; set; }

        public int? PacienteDependienteId { get; set; }

        // Campos informativos para mostrar resumen antes de confirmar
        public string EspecialidadNombre { get; set; } = string.Empty;
        public string MedicoNombre { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
    }
}

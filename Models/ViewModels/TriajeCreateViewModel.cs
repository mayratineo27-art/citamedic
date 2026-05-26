using System;
using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Models.ViewModels
{
    public class TriajeCreateViewModel
    {
        public int CitaId { get; set; }

        public string PacienteNombre { get; set; } = string.Empty;

        public string EspecialidadNombre { get; set; } = string.Empty;

        [Required]
        public decimal Peso { get; set; }

        [Required]
        public decimal Talla { get; set; }

        [Required]
        public decimal Temperatura { get; set; }

        [Required]
        public int PresionSistolica { get; set; }

        [Required]
        public int PresionDiastolica { get; set; }

        public string? Observacion { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}

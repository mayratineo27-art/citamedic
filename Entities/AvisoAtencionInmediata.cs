using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public enum EstadoAviso
    {
        Pendiente = 0,
        Visualizado = 1,
        Cerrado = 2
    }

    public class AvisoAtencionInmediata
    {
        [Key]
        public int AvisoId { get; set; }

        [Required]
        [ForeignKey("Paciente")]
        public int PacienteId { get; set; }

        [Required]
        [StringLength(300)]
        public string Motivo { get; set; } = string.Empty;

        [Required]
        public EstadoAviso EstadoAviso { get; set; } = EstadoAviso.Pendiente;

        [Required]
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        // Propiedad de navegación
        public Paciente Paciente { get; set; } = null!;

        public AvisoAtencionInmediata()
        {
            Motivo = string.Empty;
        }
    }
}

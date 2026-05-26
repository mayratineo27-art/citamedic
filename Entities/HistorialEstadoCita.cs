using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public class HistorialEstadoCita
    {
        [Key]
        public int HistorialId { get; set; }

        [Required]
        [ForeignKey("Cita")]
        public int CitaId { get; set; }

        [Required]
        public EstadoCita EstadoAnterior { get; set; }

        [Required]
        public EstadoCita EstadoNuevo { get; set; }

        [Required]
        public DateTime FechaCambio { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [StringLength(300)]
        public string? Observacion { get; set; }

        // Propiedades de navegación
        public Cita Cita { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;

        public HistorialEstadoCita()
        {
        }
    }
}

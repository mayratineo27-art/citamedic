using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public class Triaje
    {
        [Key]
        public int TriajeId { get; set; }

        [Required]
        [ForeignKey("Cita")]
        public int CitaId { get; set; }

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

        [StringLength(500)]
        public string? Observacion { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("EnfermeriaUsuario")]
        public int EnfermeriaUsuarioId { get; set; }

        // Propiedades de navegación
        public Cita Cita { get; set; } = null!;
        public Usuario EnfermeriaUsuario { get; set; } = null!;

        public Triaje()
        {
        }
    }
}

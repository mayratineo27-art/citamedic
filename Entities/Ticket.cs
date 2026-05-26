using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        [ForeignKey("Cita")]
        public int CitaId { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public DateTime FechaEmision { get; set; } = DateTime.UtcNow;

        // Propiedad de navegación
        public Cita Cita { get; set; } = null!;

        public Ticket()
        {
            Codigo = string.Empty;
        }
    }
}

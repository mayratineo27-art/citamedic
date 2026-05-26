using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public class SlotDisponible
    {
        [Key]
        public int SlotId { get; set; }

        [Required]
        [ForeignKey("Programacion")]
        public int ProgramacionId { get; set; }

        [Required]
        public TimeOnly HoraInicio { get; set; }

        [Required]
        public TimeOnly HoraFin { get; set; }

        [Required]
        public int CuposDisponibles { get; set; }

        [Required]
        public int CuposTotal { get; set; }

        [Required]
        public bool EsSobrecupo { get; set; } = false;

        // Propiedades de navegación
        public ProgramacionOperativa Programacion { get; set; } = null!;
        public ICollection<Cita> Citas { get; set; } = new HashSet<Cita>();

        public SlotDisponible()
        {
        }
    }
}

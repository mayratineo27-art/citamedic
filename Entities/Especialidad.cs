using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public class Especialidad
    {
        [Key]
        public int EspecialidadId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [ForeignKey("UPS")]
        public int UPSId { get; set; }

        [Required]
        public int DuracionMinutos { get; set; }

        [Required]
        public bool Activa { get; set; } = true;

        // Propiedades de navegación
        public UPS UPS { get; set; } = null!;
        public ICollection<ProgramacionOperativa> Programaciones { get; set; } = new HashSet<ProgramacionOperativa>();

        public Especialidad()
        {
            Nombre = string.Empty;
        }
    }
}

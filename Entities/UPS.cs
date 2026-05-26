using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Entities
{
    public class UPS
    {
        [Key]
        public int UPSId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool Activa { get; set; } = true;

        // Propiedades de navegación
        public ICollection<Especialidad> Especialidades { get; set; } = new HashSet<Especialidad>();

        public UPS()
        {
            Nombre = string.Empty;
        }
    }
}

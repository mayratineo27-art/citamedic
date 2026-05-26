using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Entities
{
    public class Medico
    {
        [Key]
        public int MedicoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [StringLength(50)]
        public string ApellidoMaterno { get; set; } = string.Empty;

        [StringLength(20)]
        public string CMP { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        // Propiedades de navegación
        public ICollection<ProgramacionOperativa> Programaciones { get; set; } = new HashSet<ProgramacionOperativa>();

        public Medico()
        {
            Nombres = string.Empty;
            ApellidoPaterno = string.Empty;
            ApellidoMaterno = string.Empty;
            CMP = string.Empty;
        }
    }
}

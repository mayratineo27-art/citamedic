using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public class Paciente
    {
        [Key]
        public int PacienteId { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(8)]
        public string DNI { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [StringLength(50)]
        public string ApellidoMaterno { get; set; } = string.Empty;

        [Required]
        public DateOnly FechaNacimiento { get; set; }

        [Required]
        public bool TieneSIS { get; set; } = false;

        [ForeignKey("Posta")]
        public int? PostaAsociadaId { get; set; }

        public bool EsMenor => FechaNacimiento.AddYears(18) > DateOnly.FromDateTime(DateTime.Now);

        [ForeignKey("Responsable")]
        public int? ResponsableId { get; set; }

        // Propiedades de navegación
        public Usuario Usuario { get; set; } = null!;
        public Paciente? Responsable { get; set; }
        public ICollection<Paciente> Dependientes { get; set; } = new HashSet<Paciente>();
        public ICollection<Cita> Citas { get; set; } = new HashSet<Cita>();

        public Paciente()
        {
            DNI = string.Empty;
            Nombres = string.Empty;
            ApellidoPaterno = string.Empty;
            ApellidoMaterno = string.Empty;
        }
    }
}

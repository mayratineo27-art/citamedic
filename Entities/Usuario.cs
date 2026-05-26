using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Entities
{
    public enum Rol
    {
        Paciente = 0,
        Admision = 1,
        Enfermeria = 2,
        Administrador = 3,
        Medico = 4
    }

    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(8)]
        public string DNI { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public Rol Rol { get; set; }

        [Required]
        public bool Activo { get; set; } = false;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(15)]
        public string Celular { get; set; } = string.Empty;

        // Propiedad de navegación 1:1
        public Paciente? Paciente { get; set; }

        public Usuario()
        {
            DNI = string.Empty;
            NombreUsuario = string.Empty;
            PasswordHash = string.Empty;
            Celular = string.Empty;
        }
    }
}

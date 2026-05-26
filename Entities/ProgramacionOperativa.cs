using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public enum Turno
    {
        Manana = 0,
        Tarde = 1
    }

    public class ProgramacionOperativa
    {
        [Key]
        public int ProgramacionId { get; set; }

        [Required]
        [ForeignKey("Especialidad")]
        public int EspecialidadId { get; set; }

        [Required]
        [ForeignKey("Medico")]
        public int MedicoId { get; set; }

        [Required]
        public Turno Turno { get; set; }

        [Required]
        public DateOnly Fecha { get; set; }

        [Required]
        public int CuposTotal { get; set; }

        [Required]
        public int DuracionMinutos { get; set; }

        [Required]
        public bool Habilitada { get; set; } = false;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("CreadaPorUsuario")]
        public int CreadaPorUsuarioId { get; set; }

        // Propiedades de navegación
        public Especialidad Especialidad { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public Usuario CreadaPorUsuario { get; set; } = null!;
        public ICollection<SlotDisponible> Slots { get; set; } = new HashSet<SlotDisponible>();

        public ProgramacionOperativa()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostaCitasWeb.Entities
{
    public enum EstadoCita
    {
        Pendiente = 0,
        EnTriaje = 1,
        ListoAtencion = 2,
        NoAsistio = 3,
        Cancelada = 4
    }

    public enum OrigenReserva
    {
        Web = 0,
        Presencial = 1
    }

    public class Cita
    {
        [Key]
        public int CitaId { get; set; }

        [Required]
        [ForeignKey("Paciente")]
        public int PacienteId { get; set; }

        [Required]
        [ForeignKey("Especialidad")]
        public int EspecialidadId { get; set; }

        [Required]
        [ForeignKey("Slot")]
        public int SlotId { get; set; }

        [Required]
        public EstadoCita EstadoCita { get; set; }

        [Required]
        public OrigenReserva OrigenReserva { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; } = DateTime.UtcNow;

        [Required]
        public bool EsSobrecupo { get; set; } = false;

        [ForeignKey("RegistradaPorUsuario")]
        public int? RegistradaPorUsuarioId { get; set; }

        [Required]
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        public Paciente Paciente { get; set; } = null!;
        public Especialidad Especialidad { get; set; } = null!;
        public SlotDisponible Slot { get; set; } = null!;
        public Usuario? RegistradaPorUsuario { get; set; }
        public Ticket? Ticket { get; set; }
        public Triaje? Triaje { get; set; }
        public ICollection<HistorialEstadoCita> Historial { get; set; } = new HashSet<HistorialEstadoCita>();

        public Cita()
        {
        }
    }
}

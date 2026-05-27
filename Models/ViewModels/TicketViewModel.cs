using System;

namespace PostaCitasWeb.Models.ViewModels
{
    public class TicketViewModel
    {
        public string Codigo { get; set; } = string.Empty;
        public string EspecialidadNombre { get; set; } = string.Empty;
        public string MedicoNombre { get; set; } = string.Empty;
        public DateOnly FechaCita { get; set; }
        public TimeOnly HoraCita { get; set; }
        public string Turno { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
    }
}

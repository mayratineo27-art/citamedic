using PostaCitasWeb.Entities;
using System.Collections.Generic;

namespace PostaCitasWeb.Models.ViewModels
{
    public class PacienteDashboardViewModel
    {
        public Paciente Paciente { get; set; } = null!;
        public IEnumerable<Especialidad> Especialidades { get; set; } = new HashSet<Especialidad>();
        public IEnumerable<Cita> MisCitas { get; set; } = new HashSet<Cita>();
        public IEnumerable<ProgramacionOperativa> Programaciones { get; set; } = new HashSet<ProgramacionOperativa>();
    }
}

using PostaCitasWeb.Entities;
using System.Collections.Generic;

namespace PostaCitasWeb.Models.ViewModels
{
    public class PacienteDashboardViewModel
    {
        public IEnumerable<Especialidad> Especialidades { get; set; } = new HashSet<Especialidad>();
        public IEnumerable<Cita> MisCitas { get; set; } = new HashSet<Cita>();
    }
}

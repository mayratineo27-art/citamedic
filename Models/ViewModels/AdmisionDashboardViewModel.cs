using PostaCitasWeb.Entities;
using System.Collections.Generic;

namespace PostaCitasWeb.Models.ViewModels
{
    public class AdmisionDashboardViewModel
    {
        public IEnumerable<ProgramacionOperativa> Programaciones { get; set; } = new HashSet<ProgramacionOperativa>();
        public IEnumerable<Cita> CitasPresenciales { get; set; } = new HashSet<Cita>();
    }
}

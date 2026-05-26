using PostaCitasWeb.Entities;
using System.Collections.Generic;

namespace PostaCitasWeb.Models.ViewModels
{
    public class EnfermeriaDashboardViewModel
    {
        public IEnumerable<Cita> CitasEnEspera { get; set; } = new HashSet<Cita>();
        public IEnumerable<AvisoAtencionInmediata> AvisosPendientes { get; set; } = new HashSet<AvisoAtencionInmediata>();
    }
}

using System.Collections.Generic;

namespace PostaCitasWeb.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<AdminProgramacionViewModel> Programaciones { get; set; } = new HashSet<AdminProgramacionViewModel>();
        public IEnumerable<UPSViewModel> UPS { get; set; } = new List<UPSViewModel>();
        public IEnumerable<EspecialidadViewModel> Especialidades { get; set; } = new List<EspecialidadViewModel>();
        public IEnumerable<UsuarioViewModel> Usuarios { get; set; } = new List<UsuarioViewModel>();
        public IEnumerable<MedicoViewModel> Medicos { get; set; } = new List<MedicoViewModel>();
        public IEnumerable<ProgramacionViewModel> ProgramacionesOperativas { get; set; } = new List<ProgramacionViewModel>();
        public IEnumerable<PacienteViewModel> Pacientes { get; set; } = new List<PacienteViewModel>();
    }

    public class EspecialidadViewModel
    {
        public int EspecialidadId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int DuracionMinutos { get; set; }
        public int UPSId { get; set; }
        public string UPSNombre { get; set; } = string.Empty;
        public bool Activa { get; set; }
    }

    public class AdminProgramacionViewModel
    {
        public int ProgramacionId { get; set; }
        public string EspecialidadNombre { get; set; } = string.Empty;
        public string MedicoNombre { get; set; } = string.Empty;
        public string Turno { get; set; } = string.Empty;
        public int DuracionMinutos { get; set; }
        public int CuposTotal { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public bool Habilitada { get; set; }
    }

    public class PacienteViewModel
    {
        public int PacienteId { get; set; }
        public int UsuarioId { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string FechaNacimiento { get; set; } = string.Empty;
        public bool TieneSIS { get; set; }
        public bool EsMenor { get; set; }
        public string ResponsableNombre { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    /// <summary>
    /// ViewModel para registrar un menor de edad vinculado a un padre/tutor (CU-NUEVO).
    /// </summary>
    public class RegistrarMenorViewModel
    {
        [Required(ErrorMessage = "El DNI del menor es obligatorio.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener solo dígitos numéricos.")]
        public string DniMenor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [StringLength(50)]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ApellidoMaterno { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public string FechaNacimiento { get; set; } = string.Empty;

        public bool TieneSIS { get; set; } = false;

        /// <summary>DNI del padre/tutor responsable (debe estar registrado previamente).</summary>
        [Required(ErrorMessage = "El DNI del responsable es obligatorio.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener solo dígitos numéricos.")]
        public string DniResponsable { get; set; } = string.Empty;

        /// <summary>Contraseña inicial para las credenciales del menor.</summary>
        [Required(ErrorMessage = "La contraseña inicial es obligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PostaCitasWeb.Models.ViewModels
{
    public class CreateProgramacionViewModel
    {
        [Required(ErrorMessage = "La especialidad es requerida")]
        public int EspecialidadId { get; set; }

        [Required(ErrorMessage = "El médico es requerido")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "El turno es requerido")]
        public int Turno { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        public string Fecha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los cupos totales son requeridos")]
        [Range(1, 100, ErrorMessage = "Los cupos deben estar entre 1 y 100")]
        public int CuposTotal { get; set; }

        [Required(ErrorMessage = "La duración es requerida")]
        [Range(1, 480, ErrorMessage = "La duración debe estar entre 1 y 480 minutos")]
        public int DuracionMinutos { get; set; }

        public bool Habilitada { get; set; } = false;

        // Para llenar los dropdowns
        public IEnumerable<EspecialidadViewModel> Especialidades { get; set; } = new List<EspecialidadViewModel>();
        public IEnumerable<MedicoViewModel> Medicos { get; set; } = new List<MedicoViewModel>();
    }

    public class ProgramacionViewModel
    {
        public int ProgramacionId { get; set; }
        public string EspecialidadNombre { get; set; } = string.Empty;
        public string MedicoNombre { get; set; } = string.Empty;
        public string Turno { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public int CuposTotal { get; set; }
        public int DuracionMinutos { get; set; }
        public bool Habilitada { get; set; }
        public int ProgramacionId_Key { get; set; }
    }
}

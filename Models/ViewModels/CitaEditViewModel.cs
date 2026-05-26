using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Models.ViewModels
{
    public class CitaEditViewModel
    {
        public int CitaId { get; set; }

        [Required]
        [Display(Name = "Especialidad")]
        public int EspecialidadId { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        public string PacienteNombre { get; set; } = string.Empty;

        public string EspecialidadNombre { get; set; } = string.Empty;

        public EstadoCita EstadoCita { get; set; }

        public IEnumerable<SelectListItem> Especialidades { get; set; } = new HashSet<SelectListItem>();
    }
}

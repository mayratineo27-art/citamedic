using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PostaCitasWeb.Models.ViewModels
{
    public class CitaViewModel
    {
        [Required]
        [Display(Name = "Especialidad")]
        public int EspecialidadId { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        public IEnumerable<SelectListItem> Especialidades { get; set; } = new HashSet<SelectListItem>();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostaCitasWeb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PostaCitasWeb.Controllers
{
    [Authorize(Roles = "Enfermeria")]
    public class TriajeController : Controller
    {
        private static readonly List<TriajeCreateViewModel> _triajes = new();

        [HttpGet]
        public IActionResult Create(int id)
        {
            var model = new TriajeCreateViewModel
            {
                CitaId = id,
                PacienteNombre = "Juan Pérez García",
                EspecialidadNombre = "Medicina General"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TriajeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.FechaRegistro = DateTime.UtcNow;
            _triajes.Add(model);

            return RedirectToAction("Index", "Enfermeria");
        }
    }
}

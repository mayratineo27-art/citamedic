using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostaCitasWeb.Models.ViewModels;
using PostaCitasWeb.Repositories;
using System;
using System.Threading.Tasks;

namespace PostaCitasWeb.Controllers
{
    [Authorize(Roles = "Enfermeria")]
    public class EnfermeriaController : Controller
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IBaseRepository<PostaCitasWeb.Entities.AvisoAtencionInmediata> _avisoRepository;

        public EnfermeriaController(ICitaRepository citaRepository, IBaseRepository<PostaCitasWeb.Entities.AvisoAtencionInmediata> avisoRepository)
        {
            _citaRepository = citaRepository ?? throw new ArgumentNullException(nameof(citaRepository));
            _avisoRepository = avisoRepository ?? throw new ArgumentNullException(nameof(avisoRepository));
        }

        public async Task<IActionResult> Index()
        {
            var model = new EnfermeriaDashboardViewModel
            {
                CitasEnEspera = await _citaRepository.GetAllAsync(),
                AvisosPendientes = await _avisoRepository.GetAllAsync()
            };

            return View(model);
        }
    }
}

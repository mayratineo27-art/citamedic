using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostaCitasWeb.Models.ViewModels;
using PostaCitasWeb.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PostaCitasWeb.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            var dni = (model.Usuario ?? string.Empty).Trim();
            var password = model.Password ?? string.Empty;

            var result = await _authService.ValidarCredenciales(dni, password);
            if (!result.Success || string.IsNullOrWhiteSpace(result.Rol))
            {
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas o DNI inexistente.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UsuarioId?.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Name, result.NombreUsuario ?? dni),
                new Claim(ClaimTypes.Role, result.Rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });

            var controllerDestino = result.Rol switch
            {
                "Administrador" => "Admin",
                "Paciente" => "Paciente",
                "Admision" => "Admision",
                "Enfermeria" => "Enfermeria",
                "Medico" => "Home",
                _ => "Home"
            };
            return RedirectToAction("Index", controllerDestino);
        }
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}

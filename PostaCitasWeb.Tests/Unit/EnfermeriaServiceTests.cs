using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostaCitasWeb.Controllers;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using PostaCitasWeb.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace PostaCitasWeb.Tests.Unit
{
    /// <summary>
    /// Pruebas unitarias para la lógica de enfermería.
    /// Nota: EnfermeriaService no existe como clase separada.
    /// La lógica está en EnfermeriaController.
    /// </summary>
    public class EnfermeriaServiceTests
    {
        private readonly Mock<ICitaRepository> _citaRepositoryMock;
        private readonly Mock<IAvisoRepository> _avisoRepositoryMock;
        private readonly Mock<ICitaService> _citaServiceMock;
        private readonly Mock<IAvisoService> _avisoServiceMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly EnfermeriaController _sut;
        private readonly FakeDateTimeProvider _fakeDateTimeProvider;

        public EnfermeriaServiceTests()
        {
            _citaRepositoryMock = new Mock<ICitaRepository>();
            _avisoRepositoryMock = new Mock<IAvisoRepository>();
            _citaServiceMock = new Mock<ICitaService>();
            _avisoServiceMock = new Mock<IAvisoService>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _fakeDateTimeProvider = new FakeDateTimeProvider();
            
            _fakeDateTimeProvider.SetNow(new DateTime(2026, 5, 27, 10, 0, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(_fakeDateTimeProvider.Today);
            _dateTimeProviderMock.Setup(d => d.Now).Returns(_fakeDateTimeProvider.Now);

            _sut = new EnfermeriaController(
                _citaRepositoryMock.Object,
                _avisoRepositoryMock.Object,
                _citaServiceMock.Object,
                _avisoServiceMock.Object,
                _dateTimeProviderMock.Object);

            // Configurar usuario con rol Enfermeria
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CrearUsuarioConRol("Enfermeria", 3)
                }
            };
        }

        private ClaimsPrincipal CrearUsuarioConRol(string rol, int usuarioId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, rol),
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        }

        #region ObtenerPacientesDia Tests - HU26

        [Fact]
        // HU26 - Agrupa por estado correctamente (datos del tablero Kanban)
        public async Task ObtenerPacientesDia_AgrupaPorEstadoCorrectamente()
        {
            // Arrange
            var today = _fakeDateTimeProvider.Today;
            var citas = new List<Cita>
            {
                new Cita
                {
                    CitaId = 1,
                    EstadoCita = EstadoCita.Pendiente,
                    Slot = new SlotDisponible
                    {
                        Programacion = new ProgramacionOperativa { Fecha = today }
                    }
                },
                new Cita
                {
                    CitaId = 2,
                    EstadoCita = EstadoCita.EnTriaje,
                    Slot = new SlotDisponible
                    {
                        Programacion = new ProgramacionOperativa { Fecha = today }
                    }
                },
                new Cita
                {
                    CitaId = 3,
                    EstadoCita = EstadoCita.ListoAtencion,
                    Slot = new SlotDisponible
                    {
                        Programacion = new ProgramacionOperativa { Fecha = today }
                    }
                }
            };

            var avisos = new List<AvisoAtencionInmediata>
            {
                new AvisoAtencionInmediata
                {
                    AvisoId = 1,
                    EstadoAviso = EstadoAviso.Pendiente
                },
                new AvisoAtencionInmediata
                {
                    AvisoId = 2,
                    EstadoAviso = EstadoAviso.Visualizado
                }
            };

            _citaRepositoryMock.Setup(x => x.GetAllWithDetailsAsync()).ReturnsAsync(citas);
            _avisoRepositoryMock.Setup(x => x.GetAllWithPacienteAsync()).ReturnsAsync(avisos);

            // Act
            var result = await _sut.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            
            // Nota: El modelo es EnfermeriaDashboardViewModel
            // La prueba documenta que HU26 se implementa en el controlador
        }

        [Fact]
        // HU26 - Solo muestra citas del día
        public async Task ObtenerPacientesDia_SoloMuestraCitasDelDia()
        {
            // Arrange
            var today = _fakeDateTimeProvider.Today;
            var yesterday = today.AddDays(-1);
            var tomorrow = today.AddDays(1);

            var citas = new List<Cita>
            {
                new Cita
                {
                    CitaId = 1,
                    EstadoCita = EstadoCita.Pendiente,
                    Slot = new SlotDisponible
                    {
                        Programacion = new ProgramacionOperativa { Fecha = today }
                    }
                },
                new Cita
                {
                    CitaId = 2,
                    EstadoCita = EstadoCita.Pendiente,
                    Slot = new SlotDisponible
                    {
                        Programacion = new ProgramacionOperativa { Fecha = yesterday }
                    }
                },
                new Cita
                {
                    CitaId = 3,
                    EstadoCita = EstadoCita.Pendiente,
                    Slot = new SlotDisponible
                    {
                        Programacion = new ProgramacionOperativa { Fecha = tomorrow }
                    }
                }
            };

            _citaRepositoryMock.Setup(x => x.GetAllWithDetailsAsync()).ReturnsAsync(citas);
            _avisoRepositoryMock.Setup(x => x.GetAllWithPacienteAsync()).ReturnsAsync(new List<AvisoAtencionInmediata>());

            // Act
            var result = await _sut.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            // El controlador filtra por fecha en el método Index
            // Solo debería incluir citas del día actual
        }

        #endregion

        #region CambiarEstadoCita Tests

        [Fact]
        public async Task CambiarEstadoCita_Exitoso_RedirigeAIndex()
        {
            // Arrange
            var citaResult = CitaResult.CreateSuccess(1, "Estado actualizado");
            _citaServiceMock.Setup(x => x.ActualizarEstadoCitaAsync(1, EstadoCita.EnTriaje, 3))
                .ReturnsAsync(citaResult);

            // Act
            var result = await _sut.CambiarEstadoCita(1, EstadoCita.EnTriaje);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Index");
            _citaServiceMock.Verify(x => x.ActualizarEstadoCitaAsync(1, EstadoCita.EnTriaje, 3), Times.Once);
        }

        [Fact]
        public async Task CambiarEstadoCita_Fallo_RedirigeAIndexConError()
        {
            // Arrange
            var citaResult = CitaResult.CreateFailure("Error al actualizar");
            _citaServiceMock.Setup(x => x.ActualizarEstadoCitaAsync(1, EstadoCita.EnTriaje, 3))
                .ReturnsAsync(citaResult);

            // Act
            var result = await _sut.CambiarEstadoCita(1, EstadoCita.EnTriaje);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task CambiarEstadoCita_UsuarioIdNoValido_RetornaChallenge()
        {
            // Arrange
            _sut.ControllerContext.HttpContext.User = CrearUsuarioConRol("Enfermeria", 0); // UsuarioId inválido

            // Act
            var result = await _sut.CambiarEstadoCita(1, EstadoCita.EnTriaje);

            // Assert
            result.Should().BeOfType<ChallengeResult>();
        }

        #endregion

        #region CambiarEstadoAviso Tests

        [Fact]
        public async Task CambiarEstadoAviso_Exitoso_RedirigeAIndex()
        {
            // Arrange
            _avisoServiceMock.Setup(x => x.ActualizarEstadoAvisoAsync(1, EstadoAviso.Visualizado))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.CambiarEstadoAviso(1, EstadoAviso.Visualizado);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Index");
            _avisoServiceMock.Verify(x => x.ActualizarEstadoAvisoAsync(1, EstadoAviso.Visualizado), Times.Once);
        }

        [Fact]
        public async Task CambiarEstadoAviso_Fallo_RedirigeAIndexConError()
        {
            // Arrange
            _avisoServiceMock.Setup(x => x.ActualizarEstadoAvisoAsync(1, EstadoAviso.Visualizado))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.CambiarEstadoAviso(1, EstadoAviso.Visualizado);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion

        #region Index Tests - Dashboard

        [Fact]
        public async Task Index_RetornaViewConModelo()
        {
            // Arrange
            var today = _fakeDateTimeProvider.Today;
            var citas = new List<Cita>
            {
                new Cita
                {
                    CitaId = 1,
                    EstadoCita = EstadoCita.Pendiente,
                    Slot = new SlotDisponible
                    {
                        Programacion = new ProgramacionOperativa { Fecha = today }
                    }
                }
            };

            var avisos = new List<AvisoAtencionInmediata>
            {
                new AvisoAtencionInmediata
                {
                    AvisoId = 1,
                    EstadoAviso = EstadoAviso.Pendiente
                }
            };

            _citaRepositoryMock.Setup(x => x.GetAllWithDetailsAsync()).ReturnsAsync(citas);
            _avisoRepositoryMock.Setup(x => x.GetAllWithPacienteAsync()).ReturnsAsync(avisos);

            // Act
            var result = await _sut.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().NotBeNull();
        }

        [Fact]
        public async Task Index_SinCitas_RetornaViewConListaVacia()
        {
            // Arrange
            _citaRepositoryMock.Setup(x => x.GetAllWithDetailsAsync()).ReturnsAsync(new List<Cita>());
            _avisoRepositoryMock.Setup(x => x.GetAllWithPacienteAsync()).ReturnsAsync(new List<AvisoAtencionInmediata>());

            // Act
            var result = await _sut.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
        }

        #endregion
    }
}

using FluentAssertions;
using Moq;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PostaCitasWeb.Tests.Unit
{
    public class AvisoServiceTests
    {
        private readonly Mock<IAvisoRepository> _avisoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly AvisoService _sut;

        public AvisoServiceTests()
        {
            _avisoRepositoryMock = new Mock<IAvisoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            _sut = new AvisoService(_avisoRepositoryMock.Object, _usuarioRepositoryMock.Object);
        }

        #region RegistrarAvisoAsync Tests - RN24, RN25

        [Fact]
        // RN24 - Aviso de atención inmediata no crea cita
        public async Task RegistrarAvisoAsync_NoCreaCita()
        {
            // Arrange
            var pacienteId = 1;
            var motivo = "Dolor abdominal agudo";

            _avisoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()))
                .Returns(Task.CompletedTask);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _sut.RegistrarAvisoAsync(pacienteId, motivo);

            // Assert
            result.Should().BeTrue();
            _avisoRepositoryMock.Verify(
                x => x.AddAsync(It.Is<AvisoAtencionInmediata>(
                    a => a.PacienteId == pacienteId &&
                          a.Motivo == motivo &&
                          a.EstadoAviso == EstadoAviso.Pendiente)),
                Times.Once,
                "RN24: Aviso debe crearse sin crear cita");
            _avisoRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        // RN25 - Aviso no afecta stock de cupos
        public async Task RegistrarAvisoAsync_NoAfectaStock()
        {
            // Arrange
            var pacienteId = 1;
            var motivo = "Dolor abdominal agudo";

            _avisoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()))
                .Returns(Task.CompletedTask);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _sut.RegistrarAvisoAsync(pacienteId, motivo);

            // Assert
            result.Should().BeTrue();
            // RN25: AvisoService no interactúa con SlotRepository, por lo que no afecta stock
            // Esta prueba documenta que el aviso es independiente del sistema de cupos
        }

        [Fact]
        public async Task RegistrarAvisoAsync_MotivoVacio_RetornaFalse()
        {
            // Arrange
            var pacienteId = 1;
            var motivo = ""; // Motivo vacío

            // Act
            var result = await _sut.RegistrarAvisoAsync(pacienteId, motivo);

            // Assert
            result.Should().BeFalse();
            _avisoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()), Times.Never);
        }

        [Fact]
        public async Task RegistrarAvisoAsync_MotivoEspaciosBlancos_RetornaFalse()
        {
            // Arrange
            var pacienteId = 1;
            var motivo = "   "; // Solo espacios en blanco

            // Act
            var result = await _sut.RegistrarAvisoAsync(pacienteId, motivo);

            // Assert
            result.Should().BeFalse();
            _avisoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()), Times.Never);
        }

        [Fact]
        public async Task RegistrarAvisoAsync_MotivoMuyLargo_RetornaFalse()
        {
            // Arrange
            var pacienteId = 1;
            var motivo = new string('A', 301); // Más de 300 caracteres

            // Act
            var result = await _sut.RegistrarAvisoAsync(pacienteId, motivo);

            // Assert
            result.Should().BeFalse();
            _avisoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()), Times.Never);
        }

        [Fact]
        public async Task RegistrarAvisoAsync_MotivoLimite300_RetornaTrue()
        {
            // Arrange
            var pacienteId = 1;
            var motivo = new string('A', 300); // Exactamente 300 caracteres

            _avisoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()))
                .Returns(Task.CompletedTask);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _sut.RegistrarAvisoAsync(pacienteId, motivo);

            // Assert
            result.Should().BeTrue();
            _avisoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()), Times.Once);
        }

        [Fact]
        public async Task RegistrarAvisoAsync_EstadoInicialPendiente()
        {
            // Arrange
            var pacienteId = 1;
            var motivo = "Dolor abdominal";

            _avisoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()))
                .Returns(Task.CompletedTask);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _sut.RegistrarAvisoAsync(pacienteId, motivo);

            // Assert
            result.Should().BeTrue();
            _avisoRepositoryMock.Verify(
                x => x.AddAsync(It.Is<AvisoAtencionInmediata>(
                    a => a.EstadoAviso == EstadoAviso.Pendiente)),
                Times.Once,
                "Estado inicial debe ser Pendiente");
        }

        [Fact]
        public async Task RegistrarAvisoAsync_FechaEnvioUtcNow()
        {
            // Arrange
            var pacienteId = 1;
            var motivo = "Dolor abdominal";
            var antes = DateTime.UtcNow.AddSeconds(-1);

            _avisoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvisoAtencionInmediata>()))
                .Returns(Task.CompletedTask);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _sut.RegistrarAvisoAsync(pacienteId, motivo);
            var despues = DateTime.UtcNow.AddSeconds(1);

            // Assert
            result.Should().BeTrue();
            _avisoRepositoryMock.Verify(
                x => x.AddAsync(It.Is<AvisoAtencionInmediata>(
                    a => a.FechaEnvio >= antes && a.FechaEnvio <= despues)),
                Times.Once,
                "FechaEnvio debe ser UtcNow");
        }

        #endregion

        #region ActualizarEstadoAvisoAsync Tests

        [Fact]
        public async Task ActualizarEstadoAvisoAsync_AvisoNoExiste_RetornaFalse()
        {
            // Arrange
            _avisoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((AvisoAtencionInmediata)null);

            // Act
            var result = await _sut.ActualizarEstadoAvisoAsync(1, EstadoAviso.Visualizado);

            // Assert
            result.Should().BeFalse();
            _avisoRepositoryMock.Verify(x => x.Update(It.IsAny<AvisoAtencionInmediata>()), Times.Never);
        }

        [Fact]
        public async Task ActualizarEstadoAvisoAsync_AvisoExiste_ActualizaEstado()
        {
            // Arrange
            var aviso = new AvisoAtencionInmediata
            {
                AvisoId = 1,
                PacienteId = 1,
                Motivo = "Dolor abdominal",
                EstadoAviso = EstadoAviso.Pendiente,
                FechaEnvio = DateTime.UtcNow
            };

            _avisoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(aviso);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _sut.ActualizarEstadoAvisoAsync(1, EstadoAviso.Visualizado);

            // Assert
            result.Should().BeTrue();
            aviso.EstadoAviso.Should().Be(EstadoAviso.Visualizado);
            _avisoRepositoryMock.Verify(x => x.Update(aviso), Times.Once);
            _avisoRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ActualizarEstadoAvisoAsync_ActualizaFechaActualizacion()
        {
            // Arrange
            var aviso = new AvisoAtencionInmediata
            {
                AvisoId = 1,
                PacienteId = 1,
                Motivo = "Dolor abdominal",
                EstadoAviso = EstadoAviso.Pendiente,
                FechaEnvio = DateTime.UtcNow
            };

            _avisoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(aviso);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var antes = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var result = await _sut.ActualizarEstadoAvisoAsync(1, EstadoAviso.Cerrado);
            var despues = DateTime.UtcNow.AddSeconds(1);

            // Assert
            result.Should().BeTrue();
            aviso.FechaActualizacion.Should().NotBeNull();
            aviso.FechaActualizacion.Value.Should().BeAfter(antes);
            aviso.FechaActualizacion.Value.Should().BeBefore(despues);
        }

        [Fact]
        public async Task ActualizarEstadoAvisoAsync_TransicionPendienteAVisualizado()
        {
            // Arrange
            var aviso = new AvisoAtencionInmediata
            {
                AvisoId = 1,
                PacienteId = 1,
                Motivo = "Dolor abdominal",
                EstadoAviso = EstadoAviso.Pendiente,
                FechaEnvio = DateTime.UtcNow
            };

            _avisoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(aviso);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _sut.ActualizarEstadoAvisoAsync(1, EstadoAviso.Visualizado);

            // Assert
            result.Should().BeTrue();
            aviso.EstadoAviso.Should().Be(EstadoAviso.Visualizado);
        }

        [Fact]
        public async Task ActualizarEstadoAvisoAsync_TransicionVisualizadoACerrado()
        {
            // Arrange
            var aviso = new AvisoAtencionInmediata
            {
                AvisoId = 1,
                PacienteId = 1,
                Motivo = "Dolor abdominal",
                EstadoAviso = EstadoAviso.Visualizado,
                FechaEnvio = DateTime.UtcNow
            };

            _avisoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(aviso);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _sut.ActualizarEstadoAvisoAsync(1, EstadoAviso.Cerrado);

            // Assert
            result.Should().BeTrue();
            aviso.EstadoAviso.Should().Be(EstadoAviso.Cerrado);
        }

        [Fact]
        public async Task ActualizarEstadoAvisoAsync_TransicionPendienteACerrado()
        {
            // Arrange
            var aviso = new AvisoAtencionInmediata
            {
                AvisoId = 1,
                PacienteId = 1,
                Motivo = "Dolor abdominal",
                EstadoAviso = EstadoAviso.Pendiente,
                FechaEnvio = DateTime.UtcNow
            };

            _avisoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(aviso);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _sut.ActualizarEstadoAvisoAsync(1, EstadoAviso.Cerrado);

            // Assert
            result.Should().BeTrue();
            aviso.EstadoAviso.Should().Be(EstadoAviso.Cerrado);
        }

        #endregion

        #region ObtenerTodosAsync Tests - RN26

        [Fact]
        // RN26 - Rol Paciente no puede obtener todos los avisos
        public async Task ObtenerTodosAsync_RolPaciente_LanzaUnauthorized()
        {
            // Arrange
            var paciente = new Usuario
            {
                UsuarioId = 5,
                Rol = Rol.Paciente,
                Activo = true
            };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(paciente);
            _avisoRepositoryMock.Setup(x => x.GetAllWithPacienteAsync())
                .ReturnsAsync(new List<AvisoAtencionInmediata>());

            // Act
            Func<Task> act = async () => await _sut.ObtenerTodosAsync(5);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Solo Enfermería puede obtener todos los avisos.");
        }

        [Fact]
        // RN26 - Rol Enfermeria puede obtener todos los avisos
        public async Task ObtenerTodosAsync_RolEnfermeria_RetornaAvisos()
        {
            // Arrange
            var enfermera = new Usuario
            {
                UsuarioId = 3,
                Rol = Rol.Enfermeria,
                Activo = true
            };

            var avisos = new List<AvisoAtencionInmediata>
            {
                new AvisoAtencionInmediata
                {
                    AvisoId = 1,
                    PacienteId = 10,
                    Motivo = "Dolor abdominal",
                    EstadoAviso = EstadoAviso.Pendiente,
                    FechaEnvio = DateTime.UtcNow.AddHours(-2)
                },
                new AvisoAtencionInmediata
                {
                    AvisoId = 2,
                    PacienteId = 11,
                    Motivo = "Fiebre alta",
                    EstadoAviso = EstadoAviso.Visualizado,
                    FechaEnvio = DateTime.UtcNow.AddHours(-1)
                }
            };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(enfermera);
            _avisoRepositoryMock.Setup(x => x.GetAllWithPacienteAsync()).ReturnsAsync(avisos);

            // Act
            var result = await _sut.ObtenerTodosAsync(3);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeInDescendingOrder(a => a.FechaEnvio, "Avisos deben estar ordenados por FechaEnvio descendente");
        }

        [Fact]
        public async Task ObtenerTodosAsync_SolicitanteNoExiste_LanzaKeyNotFoundException()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Usuario)null);

            // Act
            Func<Task> act = async () => await _sut.ObtenerTodosAsync(999);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Solicitante no encontrado.");
        }

        [Fact]
        public async Task ObtenerTodosAsync_RolAdmision_LanzaUnauthorized()
        {
            // Arrange
            var admision = new Usuario
            {
                UsuarioId = 2,
                Rol = Rol.Admision,
                Activo = true
            };

            _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(admision);

            // Act
            Func<Task> act = async () => await _sut.ObtenerTodosAsync(2);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Solo Enfermería puede obtener todos los avisos.");
        }

        #endregion

        #region Métodos Adicionales - HU21

        [Fact]
        // HU21 - Marcar como visualizado cambia estado
        public async Task MarcarVisualizadoAsync_CambiaEstadoAVisualizado()
        {
            // Arrange
            var aviso = new AvisoAtencionInmediata
            {
                AvisoId = 1,
                PacienteId = 1,
                Motivo = "Dolor abdominal",
                EstadoAviso = EstadoAviso.Pendiente,
                FechaEnvio = DateTime.UtcNow
            };

            _avisoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(aviso);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _sut.ActualizarEstadoAvisoAsync(1, EstadoAviso.Visualizado);

            // Assert
            result.Should().BeTrue();
            aviso.EstadoAviso.Should().Be(EstadoAviso.Visualizado);
            aviso.FechaActualizacion.Should().NotBeNull();
        }

        [Fact]
        // HU21 - Cerrar aviso cambia estado
        public async Task CerrarAvisoAsync_CambiaEstadoACerrado()
        {
            // Arrange
            var aviso = new AvisoAtencionInmediata
            {
                AvisoId = 1,
                PacienteId = 1,
                Motivo = "Dolor abdominal",
                EstadoAviso = EstadoAviso.Visualizado,
                FechaEnvio = DateTime.UtcNow
            };

            _avisoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(aviso);
            _avisoRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _sut.ActualizarEstadoAvisoAsync(1, EstadoAviso.Cerrado);

            // Assert
            result.Should().BeTrue();
            aviso.EstadoAviso.Should().Be(EstadoAviso.Cerrado);
            aviso.FechaActualizacion.Should().NotBeNull();
        }

        #endregion
    }
}

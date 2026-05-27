using FluentAssertions;
using Moq;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PostaCitasWeb.Tests.Services
{
    public class CitaServiceTests
    {
        private readonly Mock<ICitaRepository> _citaRepositoryMock;
        private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
        private readonly Mock<ISlotRepository> _slotRepositoryMock;
        private readonly Mock<IBaseRepository<Ticket>> _ticketRepositoryMock;
        private readonly Mock<IBaseRepository<HistorialEstadoCita>> _historialRepositoryMock;
        private readonly Mock<IBaseRepository<Triaje>> _triajeRepositoryMock;
        private readonly CitaService _sut;

        public CitaServiceTests()
        {
            _citaRepositoryMock = new Mock<ICitaRepository>();
            _pacienteRepositoryMock = new Mock<IPacienteRepository>();
            _slotRepositoryMock = new Mock<ISlotRepository>();
            _ticketRepositoryMock = new Mock<IBaseRepository<Ticket>>();
            _historialRepositoryMock = new Mock<IBaseRepository<HistorialEstadoCita>>();
            _triajeRepositoryMock = new Mock<IBaseRepository<Triaje>>();

            _sut = new CitaService(
                _citaRepositoryMock.Object,
                _pacienteRepositoryMock.Object,
                _slotRepositoryMock.Object,
                _ticketRepositoryMock.Object,
                _historialRepositoryMock.Object,
                _triajeRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateCitaAsync_WhenFechaEsHoy_ReturnsFalse()
        {
            // Arrange
            var cita = new Cita
            {
                PacienteId = 1,
                SlotId = 10,
                FechaReserva = DateTime.Today,
                EstadoCita = EstadoCita.Pendiente,
                OrigenReserva = OrigenReserva.Web,
                FechaUltimaActualizacion = DateTime.UtcNow,
                Historial = new HashSet<HistorialEstadoCita>()
            };

            // Act
            var result = await _sut.CreateCitaAsync(cita);

            // Assert
            result.Should().BeFalse();
            _citaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cita>()), Times.Never);
        }

        [Fact]
        public async Task CreateCitaAsync_WhenSlotNoDisponible_ReturnsFalse()
        {
            // Arrange
            var cita = new Cita
            {
                PacienteId = 1,
                SlotId = 10,
                FechaReserva = DateTime.Today.AddDays(1),
                EstadoCita = EstadoCita.Pendiente,
                OrigenReserva = OrigenReserva.Web,
                FechaUltimaActualizacion = DateTime.UtcNow,
                Historial = new HashSet<HistorialEstadoCita>()
            };

            _slotRepositoryMock
                .Setup(x => x.IsSlotAvailableAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.CreateCitaAsync(cita);

            // Assert
            result.Should().BeFalse();
            _citaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cita>()), Times.Never);
        }

        [Fact]
        public async Task CancelCitaAsync_WhenValidAppointmentAndWithinTimeframe_ReturnsSuccess()
        {
            // Arrange
            var citaId = 1;
            var slotId = 10;
            var cita = new Cita
            {
                CitaId = citaId,
                SlotId = slotId,
                EstadoCita = EstadoCita.Pendiente,
                PacienteId = 5
            };

            var slot = new SlotDisponible
            {
                SlotId = slotId,
                CuposDisponibles = 2,
                Programacion = new ProgramacionOperativa
                {
                    Fecha = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), // Tomorrow
                    Turno = Turno.Manana
                }
            };

            _citaRepositoryMock.Setup(x => x.GetByIdAsync(citaId)).ReturnsAsync(cita);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(slotId)).ReturnsAsync(slot);

            // Act
            var result = await _sut.CancelCitaAsync(citaId, "Test");

            // Assert
            result.Success.Should().BeTrue();
            cita.EstadoCita.Should().Be(EstadoCita.Cancelada);
            slot.CuposDisponibles.Should().Be(3);
            _citaRepositoryMock.Verify(x => x.Update(cita), Times.Once);
            _slotRepositoryMock.Verify(x => x.Update(slot), Times.Once);
        }

        [Fact]
        public async Task CancelCitaAsync_WhenPastTimeframe_ReturnsFailure()
        {
            // Arrange
            var citaId = 1;
            var slotId = 10;
            var cita = new Cita
            {
                CitaId = citaId,
                SlotId = slotId,
                EstadoCita = EstadoCita.Pendiente,
                PacienteId = 5
            };

            var slot = new SlotDisponible
            {
                SlotId = slotId,
                CuposDisponibles = 2,
                Programacion = new ProgramacionOperativa
                {
                    Fecha = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), // Yesterday
                    Turno = Turno.Manana
                }
            };

            _citaRepositoryMock.Setup(x => x.GetByIdAsync(citaId)).ReturnsAsync(cita);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(slotId)).ReturnsAsync(slot);

            // Act
            var result = await _sut.CancelCitaAsync(citaId, "Test");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Ha pasado el horario de cancelación");
            cita.EstadoCita.Should().Be(EstadoCita.Pendiente); // unmodified
        }
    }
}

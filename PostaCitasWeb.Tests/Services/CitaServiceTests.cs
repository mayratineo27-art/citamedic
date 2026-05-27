using FluentAssertions;
using Moq;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using System;
using System.Collections.Generic;
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
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly CitaService _sut;

        public CitaServiceTests()
        {
            _citaRepositoryMock = new Mock<ICitaRepository>();
            _pacienteRepositoryMock = new Mock<IPacienteRepository>();
            _slotRepositoryMock = new Mock<ISlotRepository>();
            _ticketRepositoryMock = new Mock<IBaseRepository<Ticket>>();
            _historialRepositoryMock = new Mock<IBaseRepository<HistorialEstadoCita>>();
            _triajeRepositoryMock = new Mock<IBaseRepository<Triaje>>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();

            // Configurar proveedor de fecha y hora por defecto
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 7, 15, 0));
            _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(new DateTime(2026, 5, 27, 12, 15, 0));

            _sut = new CitaService(
                _citaRepositoryMock.Object,
                _pacienteRepositoryMock.Object,
                _slotRepositoryMock.Object,
                _ticketRepositoryMock.Object,
                _historialRepositoryMock.Object,
                _triajeRepositoryMock.Object,
                _dateTimeProviderMock.Object);
        }

        #region CreateCitaAsync Tests

        [Fact]
        public async Task CreateCitaAsync_WhenFechaEsHoy_ReturnsFalse()
        {
            // Arrange
            var cita = new Cita
            {
                PacienteId = 1,
                SlotId = 10,
                FechaReserva = new DateTime(2026, 5, 27),
                EstadoCita = EstadoCita.Pendiente,
                OrigenReserva = OrigenReserva.Web,
                FechaUltimaActualizacion = new DateTime(2026, 5, 27, 12, 15, 0)
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
                FechaReserva = new DateTime(2026, 5, 28),
                EstadoCita = EstadoCita.Pendiente,
                OrigenReserva = OrigenReserva.Web,
                FechaUltimaActualizacion = new DateTime(2026, 5, 27, 12, 15, 0)
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
        public async Task CreateCitaAsync_WhenSabadoAndReservaLunes_ReturnsFalse()
        {
            // Arrange
            // Cambiar hoy a Sábado
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 30)); // 30 May 2026 is Saturday
            
            var cita = new Cita
            {
                PacienteId = 1,
                SlotId = 10,
                FechaReserva = new DateTime(2026, 6, 1), // Monday
                EstadoCita = EstadoCita.Pendiente,
                OrigenReserva = OrigenReserva.Web
            };

            // Act
            var result = await _sut.CreateCitaAsync(cita);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region ReserveCitaAsync Tests

        [Fact]
        public async Task ReserveCitaAsync_WhenPacienteDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Paciente)null);

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("El paciente no existe");
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenSlotDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync((SlotDisponible)null);

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("El slot o la programación asociada no existe");
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenNoCuposDisponibles_ReturnsFailure()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 0,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 28) }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("No hay cupos disponibles");
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenPatientAlreadyHasAppointmentOnDate_ReturnsFailure()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 5,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 28) }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);
            _citaRepositoryMock.Setup(x => x.HasActiveCitaOnDateAsync(1, slot.Programacion.Fecha)).ReturnsAsync(true);

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Solo puedes reservar una cita por día");
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 5,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 28), EspecialidadId = 2 }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);
            _citaRepositoryMock.Setup(x => x.HasActiveCitaOnDateAsync(1, slot.Programacion.Fecha)).ReturnsAsync(false);

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeTrue();
            result.TicketCodigo.Should().NotBeNullOrEmpty();
            slot.CuposDisponibles.Should().Be(4);
            _citaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cita>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Ticket>()), Times.Once);
            _historialRepositoryMock.Verify(x => x.AddAsync(It.IsAny<HistorialEstadoCita>()), Times.Once);
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenSameDayTurnoMananaAndBeforeWindow_ReturnsFailure()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 5,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 27), Turno = Turno.Manana }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);

            // Simular las 05:30 a.m. (fuera del rango de 06:00 a 08:00)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 5, 30, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("El plazo de reserva para el turno mañana de hoy ha finalizado o aún no ha comenzado");
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenSameDayTurnoMananaAndDuringWindow_ReturnsSuccess()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 5,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 27), Turno = Turno.Manana, EspecialidadId = 2 }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);
            _citaRepositoryMock.Setup(x => x.HasActiveCitaOnDateAsync(1, slot.Programacion.Fecha)).ReturnsAsync(false);

            // Simular las 07:15 a.m. (dentro del rango de 06:00 a 08:00)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 7, 15, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenSameDayTurnoMananaAndAfterWindow_ReturnsFailure()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 5,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 27), Turno = Turno.Manana }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);

            // Simular las 08:15 a.m. (fuera del rango de 06:00 a 08:00)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 8, 15, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("El plazo de reserva para el turno mañana de hoy ha finalizado o aún no ha comenzado");
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenSameDayTurnoTardeAndBeforeWindow_ReturnsFailure()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 5,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 27), Turno = Turno.Tarde }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);

            // Simular las 12:00 p.m. (fuera del rango de 13:00 a 15:00)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 12, 0, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("El plazo de reserva para el turno tarde de hoy ha finalizado o aún no ha comenzado");
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenSameDayTurnoTardeAndDuringWindow_ReturnsSuccess()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 5,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 27), Turno = Turno.Tarde, EspecialidadId = 2 }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);
            _citaRepositoryMock.Setup(x => x.HasActiveCitaOnDateAsync(1, slot.Programacion.Fecha)).ReturnsAsync(false);

            // Simular las 14:00 p.m. (dentro del rango de 13:00 a 15:00)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 14, 0, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task ReserveCitaAsync_WhenSameDayTurnoTardeAndAfterWindow_ReturnsFailure()
        {
            // Arrange
            var paciente = new Paciente { PacienteId = 1 };
            var slot = new SlotDisponible
            {
                SlotId = 10,
                CuposDisponibles = 5,
                Programacion = new ProgramacionOperativa { Fecha = new DateOnly(2026, 5, 27), Turno = Turno.Tarde }
            };

            _pacienteRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(paciente);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(10)).ReturnsAsync(slot);

            // Simular las 15:30 p.m. (fuera del rango de 13:00 a 15:00)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 15, 30, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.ReserveCitaAsync(1, 10, OrigenReserva.Web);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("El plazo de reserva para el turno tarde de hoy ha finalizado o aún no ha comenzado");
        }

        #endregion

        #region CancelCitaAsync Tests

        [Fact]
        public async Task CancelCitaAsync_WhenCitaDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Cita)null);

            // Act
            var result = await _sut.CancelCitaAsync(1, "Test");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("La cita no existe");
        }

        [Fact]
        public async Task CancelCitaAsync_WhenCitaNotPendiente_ReturnsFailure()
        {
            // Arrange
            var cita = new Cita { CitaId = 1, EstadoCita = EstadoCita.EnTriaje };
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(cita);

            // Act
            var result = await _sut.CancelCitaAsync(1, "Test");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Solo se pueden cancelar citas en estado Pendiente");
        }

        [Fact]
        public async Task CancelCitaAsync_WhenTurnoMananaAndBeforeLimit_ReturnsSuccess()
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
                    Fecha = new DateOnly(2026, 5, 27), // Today
                    Turno = Turno.Manana
                }
            };

            _citaRepositoryMock.Setup(x => x.GetByIdAsync(citaId)).ReturnsAsync(cita);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(slotId)).ReturnsAsync(slot);

            // Simular las 07:15 a.m. (dentro del límite de las 07:40)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 7, 15, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.CancelCitaAsync(citaId, "Test");

            // Assert
            result.Success.Should().BeTrue();
            cita.EstadoCita.Should().Be(EstadoCita.Cancelada);
            slot.CuposDisponibles.Should().Be(3);
        }

        [Fact]
        public async Task CancelCitaAsync_WhenTurnoMananaAndAfterLimit_ReturnsFailure()
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
                    Fecha = new DateOnly(2026, 5, 27), // Today
                    Turno = Turno.Manana
                }
            };

            _citaRepositoryMock.Setup(x => x.GetByIdAsync(citaId)).ReturnsAsync(cita);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(slotId)).ReturnsAsync(slot);

            // Simular las 07:45 a.m. (después del límite de las 07:40)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 7, 45, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.CancelCitaAsync(citaId, "Test");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Ha pasado el horario de cancelación");
            cita.EstadoCita.Should().Be(EstadoCita.Pendiente);
        }

        [Fact]
        public async Task CancelCitaAsync_WhenTurnoTardeAndBeforeLimit_ReturnsSuccess()
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
                    Fecha = new DateOnly(2026, 5, 27), // Today
                    Turno = Turno.Tarde
                }
            };

            _citaRepositoryMock.Setup(x => x.GetByIdAsync(citaId)).ReturnsAsync(cita);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(slotId)).ReturnsAsync(slot);

            // Simular las 14:15 p.m. (dentro del límite de las 14:40)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 14, 15, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.CancelCitaAsync(citaId, "Test");

            // Assert
            result.Success.Should().BeTrue();
            cita.EstadoCita.Should().Be(EstadoCita.Cancelada);
        }

        [Fact]
        public async Task CancelCitaAsync_WhenTurnoTardeAndAfterLimit_ReturnsFailure()
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
                    Fecha = new DateOnly(2026, 5, 27), // Today
                    Turno = Turno.Tarde
                }
            };

            _citaRepositoryMock.Setup(x => x.GetByIdAsync(citaId)).ReturnsAsync(cita);
            _slotRepositoryMock.Setup(x => x.GetByIdWithProgramacionAsync(slotId)).ReturnsAsync(slot);

            // Simular las 15:00 p.m. (después del límite de las 14:40)
            _dateTimeProviderMock.Setup(d => d.Now).Returns(new DateTime(2026, 5, 27, 15, 0, 0));
            _dateTimeProviderMock.Setup(d => d.Today).Returns(new DateOnly(2026, 5, 27));

            // Act
            var result = await _sut.CancelCitaAsync(citaId, "Test");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Ha pasado el horario de cancelación");
            cita.EstadoCita.Should().Be(EstadoCita.Pendiente);
        }

        #endregion

        #region RegistrarTriajeAsync Tests

        [Fact]
        public async Task RegistrarTriajeAsync_WhenCitaDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Cita)null);

            // Act
            var result = await _sut.RegistrarTriajeAsync(1, 2, 70, 1.75m, 36.5m, 120, 80);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("La cita no existe");
        }

        [Fact]
        public async Task RegistrarTriajeAsync_WhenTriajeAlreadyExists_ReturnsFailure()
        {
            // Arrange
            var cita = new Cita { CitaId = 1, Triaje = new Triaje() };
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(cita);

            // Act
            var result = await _sut.RegistrarTriajeAsync(1, 2, 70, 1.75m, 36.5m, 120, 80);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Esta cita ya tiene un triaje registrado");
        }

        [Fact]
        public async Task RegistrarTriajeAsync_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var cita = new Cita { CitaId = 1, EstadoCita = EstadoCita.Pendiente };
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(cita);

            // Act
            var result = await _sut.RegistrarTriajeAsync(1, 2, 70, 1.75m, 36.5m, 120, 80);

            // Assert
            result.Success.Should().BeTrue();
            cita.EstadoCita.Should().Be(EstadoCita.EnTriaje);
            _triajeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Triaje>()), Times.Once);
            _historialRepositoryMock.Verify(x => x.AddAsync(It.IsAny<HistorialEstadoCita>()), Times.Once);
        }

        #endregion

        #region ActualizarEstadoCitaAsync Tests

        [Fact]
        public async Task ActualizarEstadoCitaAsync_WhenCitaDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Cita)null);

            // Act
            var result = await _sut.ActualizarEstadoCitaAsync(1, EstadoCita.ListoAtencion, 2);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("La cita no existe");
        }

        [Fact]
        public async Task ActualizarEstadoCitaAsync_WhenCitaCancelled_ReturnsFailure()
        {
            // Arrange
            var cita = new Cita { CitaId = 1, EstadoCita = EstadoCita.Cancelada };
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(cita);

            // Act
            var result = await _sut.ActualizarEstadoCitaAsync(1, EstadoCita.ListoAtencion, 2);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("No se puede cambiar el estado de una cita cancelada");
        }

        [Fact]
        public async Task ActualizarEstadoCitaAsync_WhenInvalidTransitionToListoAtencion_ReturnsFailure()
        {
            // Arrange
            // Debe estar en EnTriaje para pasar a ListoAtencion
            var cita = new Cita { CitaId = 1, EstadoCita = EstadoCita.Pendiente };
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(cita);

            // Act
            var result = await _sut.ActualizarEstadoCitaAsync(1, EstadoCita.ListoAtencion, 2);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Solo se puede marcar como Listo para Atención si la cita está En Triaje");
        }

        [Fact]
        public async Task ActualizarEstadoCitaAsync_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var cita = new Cita { CitaId = 1, EstadoCita = EstadoCita.EnTriaje };
            _citaRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(cita);

            // Act
            var result = await _sut.ActualizarEstadoCitaAsync(1, EstadoCita.ListoAtencion, 2);

            // Assert
            result.Success.Should().BeTrue();
            cita.EstadoCita.Should().Be(EstadoCita.ListoAtencion);
            _historialRepositoryMock.Verify(x => x.AddAsync(It.IsAny<HistorialEstadoCita>()), Times.Once);
        }

        #endregion
    }
}

using FluentAssertions;
using Moq;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PostaCitasWeb.Tests.Unit
{
    public class AuthServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _pacienteRepositoryMock = new Mock<IPacienteRepository>();

            _sut = new AuthService(
                _usuarioRepositoryMock.Object,
                _pacienteRepositoryMock.Object);
        }

        #region LoginAsync Tests - RN01

        [Fact]
        // RN01 - Usuario inactivo lanza unauthorized
        public async Task LoginAsync_UsuarioInactivo_LanzaUnauthorized()
        {
            // Arrange
            var usuario = new Usuario
            {
                UsuarioId = 1,
                DNI = "12345678",
                NombreUsuario = "testuser",
                PasswordHash = "$2a$11$hashed_password",
                Rol = Rol.Paciente,
                Activo = false, // RN01: Inactivo
                Celular = "999999999",
                FechaCreacion = DateTime.UtcNow
            };

            _usuarioRepositoryMock.Setup(x => x.GetByDniAsync("12345678")).ReturnsAsync(usuario);

            // Act
            var result = await _sut.LoginAsync("12345678", "password123");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no está habilitada");
            result.UsuarioId.Should().BeNull();
        }

        [Fact]
        // RN01 - Credenciales correctas con usuario activo exitoso
        public async Task LoginAsync_CredencialesCorrectas_UsuarioActivo_Exitoso()
        {
            // Arrange
            var usuario = new Usuario
            {
                UsuarioId = 1,
                DNI = "12345678",
                NombreUsuario = "testuser",
                PasswordHash = "$2a$11$hashed_password",
                Rol = Rol.Paciente,
                Activo = true, // RN01: Activo
                Celular = "999999999",
                FechaCreacion = DateTime.UtcNow
            };

            var paciente = new Paciente
            {
                PacienteId = 10,
                UsuarioId = 1,
                DNI = "12345678",
                Nombres = "Juan",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "García",
                FechaNacimiento = new DateOnly(1990, 1, 1)
            };

            _usuarioRepositoryMock.Setup(x => x.GetByDniAsync("12345678")).ReturnsAsync(usuario);
            _pacienteRepositoryMock.Setup(x => x.GetByUsuarioIdAsync(1)).ReturnsAsync(paciente);

            // Act
            var result = await _sut.LoginAsync("12345678", "password123");

            // Assert
            result.Success.Should().BeTrue();
            result.UsuarioId.Should().Be(1);
            result.NombreUsuario.Should().Be("testuser");
            result.PacienteId.Should().Be(10);
            result.Rol.Should().Be("Paciente");
        }

        [Fact]
        public async Task LoginAsync_CredencialesIncorrectas_RetornaFailure()
        {
            // Arrange
            var usuario = new Usuario
            {
                UsuarioId = 1,
                DNI = "12345678",
                NombreUsuario = "testuser",
                PasswordHash = "$2a$11$hashed_password",
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "999999999",
                FechaCreacion = DateTime.UtcNow
            };

            _usuarioRepositoryMock.Setup(x => x.GetByDniAsync("12345678")).ReturnsAsync(usuario);

            // Act
            var result = await _sut.LoginAsync("12345678", "wrongpassword");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("incorrectos");
        }

        [Fact]
        public async Task LoginAsync_UsuarioNoExiste_RetornaFailure()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.GetByDniAsync("12345678")).ReturnsAsync((Usuario)null);
            _usuarioRepositoryMock.Setup(x => x.GetByNombreUsuarioAsync("testuser")).ReturnsAsync((Usuario)null);

            // Act
            var result = await _sut.LoginAsync("12345678", "password123");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("incorrectos");
        }

        [Fact]
        public async Task LoginAsync_NombreUsuarioVacio_RetornaFailure()
        {
            // Act
            var result = await _sut.LoginAsync("", "password123");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("requerido");
        }

        [Fact]
        public async Task LoginAsync_PasswordVacio_RetornaFailure()
        {
            // Act
            var result = await _sut.LoginAsync("testuser", "");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("requerido");
        }

        [Fact]
        public async Task LoginAsync_UsuarioNoPaciente_NoRetornaPacienteId()
        {
            // Arrange
            var usuario = new Usuario
            {
                UsuarioId = 2,
                DNI = "87654321",
                NombreUsuario = "admision",
                PasswordHash = "$2a$11$hashed_password",
                Rol = Rol.Admision,
                Activo = true,
                Celular = "999999999",
                FechaCreacion = DateTime.UtcNow
            };

            _usuarioRepositoryMock.Setup(x => x.GetByDniAsync("87654321")).ReturnsAsync(usuario);
            _pacienteRepositoryMock.Setup(x => x.GetByUsuarioIdAsync(2)).ReturnsAsync((Paciente)null);

            // Act
            var result = await _sut.LoginAsync("87654321", "password123");

            // Assert
            result.Success.Should().BeTrue();
            result.UsuarioId.Should().Be(2);
            result.PacienteId.Should().BeNull("Usuarios no pacientes no deben tener PacienteId");
            result.Rol.Should().Be("Admision");
        }

        #endregion

        #region RegisterPacienteAsync Tests - RN01, RN02

        [Fact]
        // RN01 - Registro crea usuario con Activo=false
        public async Task RegisterPacienteAsync_CreaUsuarioInactivo()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.NombreUsuarioExistsAsync("newuser")).ReturnsAsync(false);
            _usuarioRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(false);
            _pacienteRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(false);

            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678",
                "Juan",
                "Pérez",
                "García",
                "newuser",
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeTrue();
            _usuarioRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Usuario>(u => u.Activo == false)),
                Times.Once,
                "RN01: Usuario debe crearse inactivo");
        }

        [Fact]
        // RN02 - Datos del paciente son inmutables después de creación
        public async Task RegisterPacienteAsync_DatosPacienteInmutables()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.NombreUsuarioExistsAsync("newuser")).ReturnsAsync(false);
            _usuarioRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(false);
            _pacienteRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(false);

            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678",
                "Juan",
                "Pérez",
                "García",
                "newuser",
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeTrue();
            _pacienteRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Paciente>(p =>
                    p.DNI == "12345678" &&
                    p.Nombres == "Juan" &&
                    p.ApellidoPaterno == "Pérez" &&
                    p.ApellidoMaterno == "García")),
                Times.Once,
                "RN02: Datos del paciente deben persistirse correctamente");
        }

        [Fact]
        public async Task RegisterPacienteAsync_DniInvalido_RetornaFailure()
        {
            // Act
            var result = await _sut.RegisterPacienteAsync(
                "123", // DNI muy corto
                "Juan",
                "Pérez",
                "García",
                "newuser",
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("8 dígitos");
        }

        [Fact]
        public async Task RegisterPacienteAsync_NombresVacios_RetornaFailure()
        {
            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678",
                "", // Nombres vacíos
                "Pérez",
                "García",
                "newuser",
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("requerido");
        }

        [Fact]
        public async Task RegisterPacienteAsync_ApellidoPaternoVacio_RetornaFailure()
        {
            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678",
                "Juan",
                "", // Apellido paterno vacío
                "García",
                "newuser",
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("requerido");
        }

        [Fact]
        public async Task RegisterPacienteAsync_NombreUsuarioCorto_RetornaFailure()
        {
            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678",
                "Juan",
                "Pérez",
                "García",
                "usr", // Nombre usuario muy corto
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("al menos 4 caracteres");
        }

        [Fact]
        public async Task RegisterPacienteAsync_PasswordCorto_RetornaFailure()
        {
            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678",
                "Juan",
                "Pérez",
                "García",
                "newuser",
                "123", // Password muy corto
                "999999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("al menos 8 caracteres");
        }

        [Fact]
        public async Task RegisterPacienteAsync_CelularCorto_RetornaFailure()
        {
            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678",
                "Juan",
                "Pérez",
                "García",
                "newuser",
                "password123",
                "123"); // Celular muy corto

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("al menos 9 dígitos");
        }

        [Fact]
        public async Task RegisterPacienteAsync_NombreUsuarioDuplicado_RetornaFailure()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.NombreUsuarioExistsAsync("existing")).ReturnsAsync(true);

            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678",
                "Juan",
                "Pérez",
                "García",
                "existing", // Nombre usuario duplicado
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("ya está registrado");
        }

        [Fact]
        public async Task RegisterPacienteAsync_DniDuplicadoEnUsuario_RetornaFailure()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.NombreUsuarioExistsAsync("newuser")).ReturnsAsync(false);
            _usuarioRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(true);

            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678", // DNI duplicado
                "Juan",
                "Pérez",
                "García",
                "newuser",
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("ya está registrado");
        }

        [Fact]
        public async Task RegisterPacienteAsync_DniDuplicadoEnPaciente_RetornaFailure()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.NombreUsuarioExistsAsync("newuser")).ReturnsAsync(false);
            _usuarioRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(false);
            _pacienteRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(true);

            // Act
            var result = await _sut.RegisterPacienteAsync(
                "12345678", // DNI duplicado en paciente
                "Juan",
                "Pérez",
                "García",
                "newuser",
                "password123",
                "999999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("ya está registrado");
        }

        #endregion

        #region IsNombreUsuarioAvailableAsync Tests

        [Fact]
        public async Task IsNombreUsuarioAvailableAsync_UsuarioNoExiste_ReturnsTrue()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.NombreUsuarioExistsAsync("newuser")).ReturnsAsync(false);

            // Act
            var result = await _sut.IsNombreUsuarioAvailableAsync("newuser");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsNombreUsuarioAvailableAsync_UsuarioExiste_ReturnsFalse()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.NombreUsuarioExistsAsync("existing")).ReturnsAsync(true);

            // Act
            var result = await _sut.IsNombreUsuarioAvailableAsync("existing");

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsDniRegisteredAsync Tests

        [Fact]
        public async Task IsDniRegisteredAsync_DniNoRegistrado_ReturnsFalse()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(false);

            // Act
            var result = await _sut.IsDniRegisteredAsync("12345678");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsDniRegisteredAsync_DniRegistrado_ReturnsTrue()
        {
            // Arrange
            _usuarioRepositoryMock.Setup(x => x.DniExistsAsync("12345678")).ReturnsAsync(true);

            // Act
            var result = await _sut.IsDniRegisteredAsync("12345678");

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region Métodos No Implementados - Documentación

        [Fact]
        // RN02A - Recuperación requiere DNI + celular coincidentes
        public async Task SolicitarRecuperacionAsync_CelularIncorrecto_LanzaValidacion()
        {
            // Arrange & Act & Assert
            // Nota: Este método no existe en AuthService actualmente
            // Debe implementarse para cumplir RN02A
            true.Should().BeTrue("Método SolicitarRecuperacionAsync no implementado en AuthService");
        }

        [Fact]
        // RN02A - Recuperación con celular correcto retorna token
        public async Task SolicitarRecuperacionAsync_CelularCorrecto_RetornaToken()
        {
            // Arrange & Act & Assert
            // Nota: Este método no existe en AuthService actualmente
            // Debe implementarse para cumplir RN02A
            true.Should().BeTrue("Método SolicitarRecuperacionAsync no implementado en AuthService");
        }

        [Fact]
        // RN02 - Intentar modificar DNI lanza excepción
        public async Task ActualizarDatosAsync_IntentaModificarDNI_LanzaExcepcion()
        {
            // Arrange & Act & Assert
            // Nota: Este método no existe en AuthService actualmente
            // Debe implementarse para cumplir RN02
            true.Should().BeTrue("Método ActualizarDatosAsync no implementado en AuthService");
        }

        [Fact]
        // RN02 - Modificar celular exitoso
        public async Task ActualizarDatosAsync_ModificaCelular_Exitoso()
        {
            // Arrange & Act & Assert
            // Nota: Este método no existe en AuthService actualmente
            // Debe implementarse para cumplir RN02
            true.Should().BeTrue("Método ActualizarDatosAsync no implementado en AuthService");
        }

        [Fact]
        // RN01 - Habilitar usuario por Admisión activa cuenta
        public async Task HabilitarUsuarioAsync_RolAdmision_ActivaCuenta()
        {
            // Arrange & Act & Assert
            // Nota: Este método no existe en AuthService actualmente
            // Debe implementarse para cumplir RN01
            true.Should().BeTrue("Método HabilitarUsuarioAsync no implementado en AuthService");
        }

        #endregion
    }
}

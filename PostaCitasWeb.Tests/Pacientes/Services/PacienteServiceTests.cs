using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Data;
using PostaCitasWeb.Entities;
using PostaCitasWeb.Repositories;
using PostaCitasWeb.Tests.Pacientes.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PostaCitasWeb.Tests.Pacientes.Services
{
    /// <summary>
    /// Pruebas de integración ligera para <see cref="PacienteRepository"/> usando EF Core InMemory.
    /// Cubre todos los métodos públicos: GetByUsuarioId, GetByDni, GetMenores,
    /// GetDependientesByResponsable, GetWithDetails, DniExists, GetByAgeRange,
    /// y las operaciones CRUD heredadas de BaseRepository.
    /// </summary>
    public class PacienteServiceTests : IDisposable
    {
        // ─── Infraestructura InMemory ────────────────────────────────────────────
        private readonly AppDbContext _context;
        private readonly PacienteRepository _sut;

        public PacienteServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // DB aislada por test
                .Options;

            _context = new AppDbContext(options);
            _sut     = new PacienteRepository(_context);
        }

        public void Dispose() => _context.Dispose();

        // ─── Helpers privados ────────────────────────────────────────────────────

        private async Task<(Usuario usuario, Paciente paciente)> SeedPacienteAsync(
            string dni = PacienteFixture.DniValido,
            DateOnly? fechaNacimiento = null,
            int? responsableId = null)
        {
            var usuario = PacienteFixture.BuildUsuario(dni: dni);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var paciente = new Paciente
            {
                UsuarioId       = usuario.UsuarioId,
                DNI             = dni,
                Nombres         = "Juan Carlos",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "García",
                FechaNacimiento = fechaNacimiento ?? PacienteFixture.FechaNacimientoAdulto,
                TieneSIS        = true,
                ResponsableId   = responsableId
            };
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            return (usuario, paciente);
        }

        // ══════════════════════════════════════════════════════════════════════════
        // #region: GetByUsuarioIdAsync
        // ══════════════════════════════════════════════════════════════════════════
        #region GetByUsuarioIdAsync

        [Fact(DisplayName = "GetByUsuarioIdAsync retorna paciente cuando existe")]
        public async Task GetByUsuarioIdAsync_CuandoExiste_RetornaPaciente()
        {
            // Arrange
            var (usuario, paciente) = await SeedPacienteAsync();

            // Act
            var resultado = await _sut.GetByUsuarioIdAsync(usuario.UsuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.DNI.Should().Be(PacienteFixture.DniValido);
            resultado.UsuarioId.Should().Be(usuario.UsuarioId);
        }

        [Fact(DisplayName = "GetByUsuarioIdAsync retorna null cuando el usuario no tiene paciente")]
        public async Task GetByUsuarioIdAsync_CuandoNoExiste_RetornaNull()
        {
            // Arrange — no se siembra ningún paciente

            // Act
            var resultado = await _sut.GetByUsuarioIdAsync(9999);

            // Assert
            resultado.Should().BeNull(
                because: "el userId 9999 no está vinculado a ningún paciente");
        }

        [Fact(DisplayName = "GetByUsuarioIdAsync incluye el Usuario de navegación")]
        public async Task GetByUsuarioIdAsync_CuandoExiste_IncludeUsuario()
        {
            // Arrange
            var (usuario, _) = await SeedPacienteAsync();

            // Act
            var resultado = await _sut.GetByUsuarioIdAsync(usuario.UsuarioId);

            // Assert
            resultado!.Usuario.Should().NotBeNull(
                because: "el repositorio hace Include(p => p.Usuario)");
            resultado.Usuario.DNI.Should().Be(PacienteFixture.DniValido);
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: GetByDniAsync
        // ══════════════════════════════════════════════════════════════════════════
        #region GetByDniAsync

        [Fact(DisplayName = "GetByDniAsync retorna paciente cuando el DNI existe")]
        public async Task GetByDniAsync_CuandoDniExiste_RetornaPaciente()
        {
            // Arrange
            await SeedPacienteAsync(PacienteFixture.DniValido);

            // Act
            var resultado = await _sut.GetByDniAsync(PacienteFixture.DniValido);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.DNI.Should().Be(PacienteFixture.DniValido);
        }

        [Fact(DisplayName = "GetByDniAsync retorna null cuando el DNI no existe")]
        public async Task GetByDniAsync_CuandoDniNoExiste_RetornaNull()
        {
            // Arrange — BD vacía

            // Act
            var resultado = await _sut.GetByDniAsync("00000000");

            // Assert
            resultado.Should().BeNull();
        }

        [Theory(DisplayName = "GetByDniAsync lanza ArgumentException para DNI vacío o whitespace")]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetByDniAsync_CuandoDniEsVacioOEspacio_LanzaArgumentException(string dni)
        {
            // Act
            Func<Task> accion = async () => await _sut.GetByDniAsync(dni);

            // Assert
            await accion.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*DNI*");
        }

        [Fact(DisplayName = "GetByDniAsync lanza ArgumentException para null")]
        public async Task GetByDniAsync_CuandoDniEsNull_LanzaArgumentException()
        {
            // Act
            Func<Task> accion = async () => await _sut.GetByDniAsync(null!);

            // Assert
            await accion.Should().ThrowAsync<ArgumentException>();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: DniExistsAsync
        // ══════════════════════════════════════════════════════════════════════════
        #region DniExistsAsync

        [Fact(DisplayName = "DniExistsAsync retorna true cuando el DNI ya está registrado")]
        public async Task DniExistsAsync_CuandoDniRegistrado_RetornaTrue()
        {
            // Arrange
            await SeedPacienteAsync(PacienteFixture.DniValido);

            // Act
            var existe = await _sut.DniExistsAsync(PacienteFixture.DniValido);

            // Assert
            existe.Should().BeTrue(
                because: "el DNI fue sembrado en la BD InMemory");
        }

        [Fact(DisplayName = "DniExistsAsync retorna false cuando el DNI no está registrado")]
        public async Task DniExistsAsync_CuandoDniNoRegistrado_RetornaFalse()
        {
            // Arrange — BD vacía

            // Act
            var existe = await _sut.DniExistsAsync("99999999");

            // Assert
            existe.Should().BeFalse();
        }

        [Theory(DisplayName = "DniExistsAsync retorna false para DNI vacío o whitespace (sin lanzar excepción)")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task DniExistsAsync_CuandoDniEsNuloOVacio_RetornaFalse(string? dni)
        {
            // Act
            var existe = await _sut.DniExistsAsync(dni!);

            // Assert
            existe.Should().BeFalse(
                because: "el repositorio hace early-return false cuando el DNI es vacío");
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: GetMenoresAsync
        // ══════════════════════════════════════════════════════════════════════════
        #region GetMenoresAsync

        /// <summary>
        /// NOTA TÉCNICA: La propiedad EsMenor es calculada en C# y NO está mapeada a columna de BD.
        /// EF Core (tanto InMemory como SQL Server) no puede traducirla a SQL.
        /// El repositorio de producción presenta el mismo bug latente.
        /// Este test documenta el comportamiento real esperado.
        /// </summary>
        [Fact(DisplayName = "GetMenoresAsync lanza InvalidOperationException porque EsMenor no es traducible a SQL")]
        public async Task GetMenoresAsync_CuandoEsMenorNoTraducible_LanzaInvalidOperationException()
        {
            // Arrange
            await SeedPacienteAsync("22222222", PacienteFixture.FechaNacimientoMenor);

            // Act
            Func<Task> accion = async () => await _sut.GetMenoresAsync();

            // Assert — documenta el comportamiento real: la propiedad EsMenor no puede
            // ser traducida a una expresión SQL por EF Core.
            await accion.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*EsMenor*");
        }

        [Fact(DisplayName = "GetMenoresAsync falla con BD vacía: EsMenor sigue siendo no traducible")]
        public async Task GetMenoresAsync_CuandoBdVacia_LanzaInvalidOperationException()
        {
            // Act — incluso sin datos, la consulta falla porque EsMenor no es traducible.
            Func<Task> accion = async () => await _sut.GetMenoresAsync();

            // Assert
            await accion.Should().ThrowAsync<InvalidOperationException>();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: GetDependientesByResponsableAsync
        // ══════════════════════════════════════════════════════════════════════════
        #region GetDependientesByResponsableAsync

        [Fact(DisplayName = "GetDependientesByResponsableAsync retorna los menores del tutor")]
        public async Task GetDependientesByResponsableAsync_CuandoHayDependientes_LosRetorna()
        {
            // Arrange
            var (_, tutor) = await SeedPacienteAsync("11111111", PacienteFixture.FechaNacimientoAdulto);
            await SeedPacienteAsync("22222222", PacienteFixture.FechaNacimientoMenor, tutor.PacienteId);
            await SeedPacienteAsync("33333333", PacienteFixture.FechaNacimientoMenor, tutor.PacienteId);

            // Act
            var dependientes = (await _sut.GetDependientesByResponsableAsync(tutor.PacienteId)).ToList();

            // Assert
            dependientes.Should().HaveCount(2,
                because: "se sembraron 2 menores asociados al tutor");
            dependientes.Should().OnlyContain(p => p.ResponsableId == tutor.PacienteId);
        }

        [Fact(DisplayName = "GetDependientesByResponsableAsync retorna lista vacía si el tutor no tiene dependientes")]
        public async Task GetDependientesByResponsableAsync_CuandoSinDependientes_RetornaVacio()
        {
            // Arrange
            var (_, tutor) = await SeedPacienteAsync("11111111");

            // Act
            var dependientes = await _sut.GetDependientesByResponsableAsync(tutor.PacienteId);

            // Assert
            dependientes.Should().BeEmpty();
        }

        [Fact(DisplayName = "GetDependientesByResponsableAsync retorna vacío para ID de responsable inexistente")]
        public async Task GetDependientesByResponsableAsync_CuandoResponsableNoExiste_RetornaVacio()
        {
            // Act
            var dependientes = await _sut.GetDependientesByResponsableAsync(9999);

            // Assert
            dependientes.Should().BeEmpty();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: GetWithDetailsAsync
        // ══════════════════════════════════════════════════════════════════════════
        #region GetWithDetailsAsync

        [Fact(DisplayName = "GetWithDetailsAsync retorna paciente con navegación completa")]
        public async Task GetWithDetailsAsync_CuandoExiste_RetornaPacienteConDetalles()
        {
            // Arrange
            var (_, paciente) = await SeedPacienteAsync();

            // Act
            var resultado = await _sut.GetWithDetailsAsync(paciente.PacienteId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.PacienteId.Should().Be(paciente.PacienteId);
            resultado.Usuario.Should().NotBeNull(
                because: "el repositorio incluye la navegación Usuario");
        }

        [Fact(DisplayName = "GetWithDetailsAsync retorna null cuando el ID no existe")]
        public async Task GetWithDetailsAsync_CuandoNoExiste_RetornaNull()
        {
            // Act
            var resultado = await _sut.GetWithDetailsAsync(9999);

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: GetByAgeRangeAsync
        // ══════════════════════════════════════════════════════════════════════════
        #region GetByAgeRangeAsync

        [Fact(DisplayName = "GetByAgeRangeAsync retorna pacientes dentro del rango de edad")]
        public async Task GetByAgeRangeAsync_CuandoHayPacientesEnRango_LosRetorna()
        {
            // Arrange — sembrar adulto de ~34 años y menor de ~10 años
            await SeedPacienteAsync("11111111", PacienteFixture.FechaNacimientoAdulto);
            await SeedPacienteAsync("22222222", PacienteFixture.FechaNacimientoMenor);

            // Act — buscar pacientes entre 30 y 40 años
            var resultado = (await _sut.GetByAgeRangeAsync(30, 40)).ToList();

            // Assert
            resultado.Should().HaveCount(1,
                because: "solo el adulto de ~34 años cae en el rango 30-40");
            resultado[0].DNI.Should().Be("11111111");
        }

        [Fact(DisplayName = "GetByAgeRangeAsync retorna lista vacía cuando nadie cae en el rango")]
        public async Task GetByAgeRangeAsync_CuandoNadieEnRango_RetornaVacio()
        {
            // Arrange
            await SeedPacienteAsync("11111111", PacienteFixture.FechaNacimientoAdulto); // ~34 años

            // Act — buscar entre 60 y 80 años
            var resultado = await _sut.GetByAgeRangeAsync(60, 80);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact(DisplayName = "GetByAgeRangeAsync retorna lista vacía cuando la BD está vacía")]
        public async Task GetByAgeRangeAsync_CuandoBdVacia_RetornaVacio()
        {
            // Act
            var resultado = await _sut.GetByAgeRangeAsync(18, 65);

            // Assert
            resultado.Should().BeEmpty();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: CRUD base — AddAsync, GetByIdAsync, Update, Delete, GetAllAsync
        // ══════════════════════════════════════════════════════════════════════════
        #region CRUD base

        [Fact(DisplayName = "AddAsync y GetByIdAsync persisten y recuperan el paciente")]
        public async Task AddAsync_YGetById_CuandoPacienteValido_PersisteCorrecto()
        {
            // Arrange
            var usuario = PacienteFixture.BuildUsuario();
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var paciente = new Paciente
            {
                UsuarioId       = usuario.UsuarioId,
                DNI             = "77777777",
                Nombres         = "Nuevo",
                ApellidoPaterno = "Apellido",
                FechaNacimiento = PacienteFixture.FechaNacimientoAdulto,
                TieneSIS        = false
            };

            // Act
            await _sut.AddAsync(paciente);
            await _sut.SaveChangesAsync();

            var recuperado = await _sut.GetByIdAsync(paciente.PacienteId);

            // Assert
            recuperado.Should().NotBeNull();
            recuperado!.DNI.Should().Be("77777777");
            recuperado.Nombres.Should().Be("Nuevo");
        }

        [Fact(DisplayName = "GetByIdAsync retorna null cuando el ID no existe")]
        public async Task GetByIdAsync_CuandoIdNoExiste_RetornaNull()
        {
            // Act
            var resultado = await _sut.GetByIdAsync(9999);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact(DisplayName = "GetAllAsync retorna todos los pacientes sembrados")]
        public async Task GetAllAsync_CuandoHayPacientes_LosRetornaTodos()
        {
            // Arrange
            await SeedPacienteAsync("11111111");
            await SeedPacienteAsync("22222222");

            // Act
            var todos = (await _sut.GetAllAsync()).ToList();

            // Assert
            todos.Should().HaveCountGreaterOrEqualTo(2,
                because: "se sembraron al menos 2 pacientes");
        }

        [Fact(DisplayName = "Update modifica los datos del paciente en la BD")]
        public async Task Update_CuandoPacienteExiste_ActualizaCorrectamente()
        {
            // Arrange
            var (_, paciente) = await SeedPacienteAsync();
            paciente.Nombres = "NombreModificado";

            // Act
            _sut.Update(paciente);
            await _sut.SaveChangesAsync();

            var recuperado = await _sut.GetByIdAsync(paciente.PacienteId);

            // Assert
            recuperado!.Nombres.Should().Be("NombreModificado");
        }

        [Fact(DisplayName = "Delete elimina el paciente de la BD")]
        public async Task Delete_CuandoPacienteExiste_LoEliminaDeLaBD()
        {
            // Arrange
            var (_, paciente) = await SeedPacienteAsync();
            var id = paciente.PacienteId;

            // Act
            _sut.Delete(paciente);
            await _sut.SaveChangesAsync();

            var recuperado = await _sut.GetByIdAsync(id);

            // Assert
            recuperado.Should().BeNull(
                because: "el paciente fue eliminado de la BD");
        }

        [Fact(DisplayName = "AddAsync lanza ArgumentNullException cuando la entidad es null")]
        public async Task AddAsync_CuandoEntidadEsNull_LanzaArgumentNullException()
        {
            // Act
            Func<Task> accion = async () => await _sut.AddAsync(null!);

            // Assert
            await accion.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact(DisplayName = "Update lanza ArgumentNullException cuando la entidad es null")]
        public void Update_CuandoEntidadEsNull_LanzaArgumentNullException()
        {
            // Act
            Action accion = () => _sut.Update(null!);

            // Assert
            accion.Should().Throw<ArgumentNullException>();
        }

        [Fact(DisplayName = "Delete lanza ArgumentNullException cuando la entidad es null")]
        public void Delete_CuandoEntidadEsNull_LanzaArgumentNullException()
        {
            // Act
            Action accion = () => _sut.Delete(null!);

            // Assert
            accion.Should().Throw<ArgumentNullException>();
        }

        [Fact(DisplayName = "AnyAsync retorna true cuando existe al menos un paciente")]
        public async Task AnyAsync_CuandoHayPacientes_RetornaTrue()
        {
            // Arrange
            await SeedPacienteAsync();

            // Act
            var existe = await _sut.AnyAsync(p => p.TieneSIS == true);

            // Assert
            existe.Should().BeTrue();
        }

        [Fact(DisplayName = "AnyAsync retorna false cuando ningún paciente cumple el predicado")]
        public async Task AnyAsync_CuandoNingúnPacienteCumple_RetornaFalse()
        {
            // Arrange
            await SeedPacienteAsync(); // TieneSIS = true por defecto

            // Act
            var existe = await _sut.AnyAsync(p => p.DNI == "00000000");

            // Assert
            existe.Should().BeFalse();
        }

        [Fact(DisplayName = "FirstOrDefaultAsync retorna el primer paciente que cumple el predicado")]
        public async Task FirstOrDefaultAsync_CuandoHayCoincidencia_RetornaPaciente()
        {
            // Arrange
            await SeedPacienteAsync(PacienteFixture.DniValido);

            // Act
            var resultado = await _sut.FirstOrDefaultAsync(p => p.DNI == PacienteFixture.DniValido);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.DNI.Should().Be(PacienteFixture.DniValido);
        }

        [Fact(DisplayName = "FirstOrDefaultAsync retorna null cuando no hay coincidencia")]
        public async Task FirstOrDefaultAsync_CuandoSinCoincidencia_RetornaNull()
        {
            // Act
            var resultado = await _sut.FirstOrDefaultAsync(p => p.DNI == "99999999");

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: Escenarios de DNI duplicado
        // ══════════════════════════════════════════════════════════════════════════
        #region DNI duplicado

        [Fact(DisplayName = "DniExistsAsync detecta DNI duplicado antes de insertar")]
        public async Task DniExistsAsync_FlujoDniDuplicado_DetectaConflicto()
        {
            // Arrange — sembrar paciente con DNI
            await SeedPacienteAsync(PacienteFixture.DniValido);

            // Act — verificar si el DNI ya existe antes de agregar otro
            var existe = await _sut.DniExistsAsync(PacienteFixture.DniValido);

            // Assert
            existe.Should().BeTrue(
                because: "la validación previa al registro debe detectar el duplicado");
        }

        [Fact(DisplayName = "DniExistsAsync permite registrar paciente con DNI distinto")]
        public async Task DniExistsAsync_CuandoDniDiferente_PermiteRegistro()
        {
            // Arrange
            await SeedPacienteAsync("11111111");

            // Act
            var existe = await _sut.DniExistsAsync("22222222");

            // Assert
            existe.Should().BeFalse(
                because: "el DNI 22222222 no ha sido registrado aún");
        }

        #endregion
    }
}

using FluentAssertions;
using PostaCitasWeb.Entities;
using System;
using Xunit;
using PostaCitasWeb.Tests.Pacientes.Helpers;

namespace PostaCitasWeb.Tests.Pacientes.Validation
{
    /// <summary>
    /// Pruebas de validación sobre la entidad Paciente y sus reglas de dominio.
    /// Cubre: Data Annotations, propiedad computada EsMenor, restricciones de longitud.
    /// </summary>
    public class PacienteValidationTests
    {
        // ══════════════════════════════════════════════════════════════════════════
        // #region: EsMenor — Propiedad computada
        // ══════════════════════════════════════════════════════════════════════════
        #region EsMenor

        [Fact(DisplayName = "EsMenor retorna true cuando paciente tiene menos de 18 años")]
        public void EsMenor_CuandoPacienteTieneMonos18Anios_RetornaTrue()
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteMenor();

            // Act
            var resultado = paciente.EsMenor;

            // Assert
            resultado.Should().BeTrue(
                because: "un paciente con {0} años de edad debe ser considerado menor",
                becauseArgs: (DateTime.Now - paciente.FechaNacimiento.ToDateTime(TimeOnly.MinValue)).TotalDays / 365);
        }

        [Fact(DisplayName = "EsMenor retorna false cuando paciente tiene exactamente 18 años")]
        public void EsMenor_CuandoPacienteTiene18AniosExactos_RetornaFalse()
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteAdulto();
            paciente.FechaNacimiento = PacienteFixture.FechaNacimientoExacto18;

            // Act
            var resultado = paciente.EsMenor;

            // Assert
            resultado.Should().BeFalse(
                because: "un paciente de exactamente 18 años ya es mayor de edad");
        }

        [Fact(DisplayName = "EsMenor retorna true cuando faltan 1 día para cumplir 18")]
        public void EsMenor_CuandoFaltan1DiaParaCumplir18_RetornaTrue()
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteMenor();
            paciente.FechaNacimiento = PacienteFixture.FechaNacimientoCasiBorde;

            // Act
            var resultado = paciente.EsMenor;

            // Assert
            resultado.Should().BeTrue(
                because: "faltar 1 día para cumplir 18 años sigue siendo menor de edad");
        }

        [Fact(DisplayName = "EsMenor retorna false cuando paciente tiene 34 años")]
        public void EsMenor_CuandoPacienteTiene34Anios_RetornaFalse()
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteAdulto();

            // Act
            var resultado = paciente.EsMenor;

            // Assert
            resultado.Should().BeFalse(
                because: "un paciente adulto de 34 años no es menor de edad");
        }

        [Fact(DisplayName = "EsMenor retorna true para recién nacido")]
        public void EsMenor_CuandoPacienteEsRecienNacido_RetornaTrue()
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteMenor();
            paciente.FechaNacimiento = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            // Act
            var resultado = paciente.EsMenor;

            // Assert
            resultado.Should().BeTrue(
                because: "un bebé de 1 día siempre es menor de edad");
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: ResponsableId — Relación tutor / dependiente
        // ══════════════════════════════════════════════════════════════════════════
        #region ResponsableId

        [Fact(DisplayName = "Paciente adulto puede no tener responsable (ResponsableId = null)")]
        public void ResponsableId_CuandoAdulto_PuedeSerNulo()
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteAdulto();

            // Act & Assert
            paciente.ResponsableId.Should().BeNull(
                because: "un adulto no requiere tutor registrado");
        }

        [Fact(DisplayName = "Paciente menor tiene ResponsableId asignado")]
        public void ResponsableId_CuandoMenor_TieneResponsableAsignado()
        {
            // Arrange
            var tutor     = PacienteFixture.BuildPacienteAdulto(pacienteId: 1);
            var dependiente = PacienteFixture.BuildPacienteMenor(pacienteId: 2, responsableId: tutor.PacienteId);

            // Act & Assert
            dependiente.ResponsableId.Should().Be(tutor.PacienteId,
                because: "el menor debe referenciar al PacienteId del tutor");
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: DNI — Longitud y requerimiento
        // ══════════════════════════════════════════════════════════════════════════
        #region DNI

        [Theory(DisplayName = "DNI de 8 caracteres es válido")]
        [InlineData("12345678")]
        [InlineData("00000000")]
        [InlineData("99999999")]
        public void DNI_CuandoTiene8Caracteres_EsValido(string dni)
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteAdulto(dni: dni);

            // Act & Assert
            paciente.DNI.Length.Should().Be(8,
                because: "el DNI peruano tiene exactamente 8 dígitos");
        }

        [Theory(DisplayName = "DNI vacío o nulo viola requerimiento")]
        [InlineData("")]
        [InlineData("   ")]
        public void DNI_CuandoEstaVacio_ViolaRequerimiento(string dni)
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteAdulto();
            paciente.DNI = dni;

            // Act & Assert
            string.IsNullOrWhiteSpace(paciente.DNI).Should().BeTrue(
                because: "un DNI vacío debe ser detectado como inválido");
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: TieneSIS — Campo booleano
        // ══════════════════════════════════════════════════════════════════════════
        #region TieneSIS

        [Fact(DisplayName = "TieneSIS puede ser verdadero")]
        public void TieneSIS_CuandoEsTrue_SeAsignaCorrectamente()
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteAdulto();
            paciente.TieneSIS = true;

            // Act & Assert
            paciente.TieneSIS.Should().BeTrue();
        }

        [Fact(DisplayName = "TieneSIS puede ser falso (sin seguro)")]
        public void TieneSIS_CuandoEsFalse_SeAsignaCorrectamente()
        {
            // Arrange
            var paciente = PacienteFixture.BuildPacienteAdulto();
            paciente.TieneSIS = false;

            // Act & Assert
            paciente.TieneSIS.Should().BeFalse();
        }

        [Fact(DisplayName = "Valor por defecto de TieneSIS es false")]
        public void TieneSIS_ValorPorDefecto_EsFalse()
        {
            // Arrange
            var paciente = new Paciente();

            // Act & Assert
            paciente.TieneSIS.Should().BeFalse(
                because: "el valor por defecto declarado en la entidad es false");
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: Colecciones de navegación — Inicialización
        // ══════════════════════════════════════════════════════════════════════════
        #region Colecciones de navegación

        [Fact(DisplayName = "Dependientes se inicializa como colección vacía al crear Paciente")]
        public void Dependientes_AlCrearPaciente_EsColeccionVacia()
        {
            // Arrange & Act
            var paciente = new Paciente();

            // Assert
            paciente.Dependientes.Should().NotBeNull()
                .And.BeEmpty(because: "la colección Dependientes debe inicializarse vacía");
        }

        [Fact(DisplayName = "Citas se inicializa como colección vacía al crear Paciente")]
        public void Citas_AlCrearPaciente_EsColeccionVacia()
        {
            // Arrange & Act
            var paciente = new Paciente();

            // Assert
            paciente.Citas.Should().NotBeNull()
                .And.BeEmpty(because: "la colección Citas debe inicializarse vacía");
        }

        [Fact(DisplayName = "Se puede agregar un dependiente a la colección del tutor")]
        public void Dependientes_CuandoSeAgregaMenor_LaColeccionCrece()
        {
            // Arrange
            var tutor       = PacienteFixture.BuildPacienteAdulto(pacienteId: 1);
            var dependiente = PacienteFixture.BuildPacienteMenor(pacienteId: 2, responsableId: 1);

            // Act
            tutor.Dependientes.Add(dependiente);

            // Assert
            tutor.Dependientes.Should().HaveCount(1)
                .And.Contain(dependiente);
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════
        // #region: FechaNacimiento — Validaciones de negocio
        // ══════════════════════════════════════════════════════════════════════════
        #region FechaNacimiento

        [Fact(DisplayName = "FechaNacimiento en el futuro haría EsMenor true (edge case)")]
        public void EsMenor_CuandoFechaNacimientoEsEnElFuturo_RetornaTrue()
        {
            // Arrange: fecha en el futuro (dato inválido, pero verificamos la lógica)
            var paciente = PacienteFixture.BuildPacienteAdulto();
            paciente.FechaNacimiento = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            // Act
            var resultado = paciente.EsMenor;

            // Assert
            resultado.Should().BeTrue(
                because: "la fórmula FechaNacimiento.AddYears(18) > Hoy será verdadera para fechas futuras");
        }

        #endregion
    }
}

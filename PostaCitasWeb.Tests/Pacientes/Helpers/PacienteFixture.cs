using PostaCitasWeb.Entities;
using System;
using System.Collections.Generic;

namespace PostaCitasWeb.Tests.Pacientes.Helpers
{
    /// <summary>
    /// Fixture centralizado con datos de prueba reutilizables para el módulo Pacientes.
    /// Proporciona constructores de entidades con valores por defecto válidos.
    /// </summary>
    public static class PacienteFixture
    {
        // ─── Constantes de prueba ────────────────────────────────────────────────

        public const string DniValido         = "12345678";
        public const string DniValido2        = "87654321";
        public const string DniInvalidoCorto  = "1234567";  // 7 dígitos
        public const string DniInvalidoLargo  = "123456789"; // 9 dígitos
        public const string DniInvalidoLetras = "1234567A";

        public const string CelularValido         = "987654321";  // 9 dígitos
        public const string CelularValido15Digitos = "999999999999999"; // 15 dígitos (máximo)
        public const string CelularInvalidoCorto   = "12345678";  // 8 dígitos
        public const string CelularInvalidoLetras  = "98765432A";

        public const string PasswordValida         = "Password1";  // 8+ chars
        public const string PasswordInvalidaCorta  = "Abc123";     // < 8 chars

        // ─── Fechas representativas ──────────────────────────────────────────────

        /// <summary>Fecha de nacimiento para un paciente adulto (34 años).</summary>
        public static readonly DateOnly FechaNacimientoAdulto = new DateOnly(1990, 6, 15);

        /// <summary>Fecha de nacimiento para un paciente menor de edad (10 años).</summary>
        public static readonly DateOnly FechaNacimientoMenor =
            DateOnly.FromDateTime(DateTime.Now.AddYears(-10));

        /// <summary>Fecha de nacimiento exactamente en el límite (17 años 364 días).</summary>
        public static readonly DateOnly FechaNacimientoCasiBorde =
            DateOnly.FromDateTime(DateTime.Now.AddYears(-18).AddDays(1));

        /// <summary>Fecha de nacimiento exactamente al cumplir 18 (adulto).</summary>
        public static readonly DateOnly FechaNacimientoExacto18 =
            DateOnly.FromDateTime(DateTime.Now.AddYears(-18));

        // ─── Constructores de entidades ──────────────────────────────────────────

        /// <summary>Construye un <see cref="Usuario"/> con Rol.Paciente válido.</summary>
        public static Usuario BuildUsuario(
            int id = 0,
            string dni = DniValido,
            string celular = CelularValido,
            bool activo = true)
        {
            var u = new Usuario
            {
                DNI         = dni,
                NombreUsuario = $"user_{dni}",
                PasswordHash  = BCrypt.Net.BCrypt.HashPassword("Password1"),
                Rol         = Rol.Paciente,
                Activo      = activo,
                Celular     = celular,
                FechaCreacion = DateTime.UtcNow
            };
            // Solo fijar ID si se pasa un valor > 0 (para tests que crean el mock directamente)
            if (id > 0) u.UsuarioId = id;
            return u;
        }

        /// <summary>Construye un <see cref="Paciente"/> adulto válido.</summary>
        public static Paciente BuildPacienteAdulto(
            int pacienteId = 1,
            int usuarioId = 10,
            string dni = DniValido,
            int? responsableId = null)
        {
            return new Paciente
            {
                PacienteId      = pacienteId,
                UsuarioId       = usuarioId,
                DNI             = dni,
                Nombres         = "Juan Carlos",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "García",
                FechaNacimiento = FechaNacimientoAdulto,
                TieneSIS        = true,
                PostaAsociadaId = null,
                ResponsableId   = responsableId,
                Usuario         = BuildUsuario(usuarioId, dni)
            };
        }

        /// <summary>Construye un <see cref="Paciente"/> menor de edad con responsable.</summary>
        public static Paciente BuildPacienteMenor(
            int pacienteId = 2,
            int usuarioId = 20,
            string dni = DniValido2,
            int responsableId = 1)
        {
            return new Paciente
            {
                PacienteId      = pacienteId,
                UsuarioId       = usuarioId,
                DNI             = dni,
                Nombres         = "Pedrito",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "García",
                FechaNacimiento = FechaNacimientoMenor,
                TieneSIS        = true,
                PostaAsociadaId = null,
                ResponsableId   = responsableId,
                Usuario         = BuildUsuario(usuarioId, dni)
            };
        }

        /// <summary>Construye una lista de pacientes de prueba variada.</summary>
        public static List<Paciente> BuildPacienteLista()
        {
            return new List<Paciente>
            {
                BuildPacienteAdulto(1, 10, "11111111"),
                BuildPacienteAdulto(2, 11, "22222222"),
                BuildPacienteMenor(3, 12, "33333333", responsableId: 1)
            };
        }
    }
}

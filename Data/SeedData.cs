using System;
using System.Collections.Generic;
using PostaCitasWeb.Entities;

namespace PostaCitasWeb.Data
{
    public static class SeedData
    {
        public static List<Usuario> GetSeedUsuarios()
        {
            var usuarios = new List<Usuario>();
            string defaultHash = "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82";

            usuarios.Add(new Usuario
            {
                UsuarioId = 10,
                DNI = "12345678",
                NombreUsuario = "juan_perez",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "999888777",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 11,
                DNI = "87654321",
                NombreUsuario = "maria_lopez",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "988777666",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 12,
                DNI = "11223344",
                NombreUsuario = "carlos_rodriguez",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "977666555",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 13,
                DNI = "99887766",
                NombreUsuario = "ana_fernandez",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "966555444",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 14,
                DNI = "45678901",
                NombreUsuario = "luis_torres",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "955444333",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 15,
                DNI = "56789012",
                NombreUsuario = "carmen_gomez",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "944333222",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 16,
                DNI = "67890123",
                NombreUsuario = "jorge_flores",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "933222111",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 17,
                DNI = "78901234",
                NombreUsuario = "rosa_quispe",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "922111000",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 18,
                DNI = "89012345",
                NombreUsuario = "pedro_diaz",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "911000999",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            usuarios.Add(new Usuario
            {
                UsuarioId = 19,
                DNI = "90123456",
                NombreUsuario = "sofia_rojas",
                PasswordHash = defaultHash,
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "900999888",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            return usuarios;
        }

        public static List<Paciente> GetSeedPacientes()
        {
            var pacientes = new List<Paciente>();

            pacientes.Add(new Paciente
            {
                PacienteId = 10,
                UsuarioId = 10,
                DNI = "12345678",
                Nombres = "Juan",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "García",
                FechaNacimiento = new DateOnly(1985, 5, 10),
                TieneSIS = true
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 11,
                UsuarioId = 11,
                DNI = "87654321",
                Nombres = "María",
                ApellidoPaterno = "López",
                ApellidoMaterno = "Martínez",
                FechaNacimiento = new DateOnly(1992, 8, 24),
                TieneSIS = false
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 12,
                UsuarioId = 12,
                DNI = "11223344",
                Nombres = "Carlos",
                ApellidoPaterno = "Rodríguez",
                ApellidoMaterno = "Sánchez",
                FechaNacimiento = new DateOnly(1978, 12, 3),
                TieneSIS = true
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 13,
                UsuarioId = 13,
                DNI = "99887766",
                Nombres = "Ana",
                ApellidoPaterno = "Fernández",
                ApellidoMaterno = "Díaz",
                FechaNacimiento = new DateOnly(1989, 4, 15),
                TieneSIS = true
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 14,
                UsuarioId = 14,
                DNI = "45678901",
                Nombres = "Luis",
                ApellidoPaterno = "Torres",
                ApellidoMaterno = "Ruiz",
                FechaNacimiento = new DateOnly(1995, 11, 2),
                TieneSIS = false
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 15,
                UsuarioId = 15,
                DNI = "56789012",
                Nombres = "Carmen",
                ApellidoPaterno = "Gómez",
                ApellidoMaterno = "Quispe",
                FechaNacimiento = new DateOnly(2001, 7, 18),
                TieneSIS = true
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 16,
                UsuarioId = 16,
                DNI = "67890123",
                Nombres = "Jorge",
                ApellidoPaterno = "Flores",
                ApellidoMaterno = "Chávez",
                FechaNacimiento = new DateOnly(1983, 2, 28),
                TieneSIS = false
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 17,
                UsuarioId = 17,
                DNI = "78901234",
                Nombres = "Rosa",
                ApellidoPaterno = "Quispe",
                ApellidoMaterno = "Mamani",
                FechaNacimiento = new DateOnly(1974, 9, 5),
                TieneSIS = true
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 18,
                UsuarioId = 18,
                DNI = "89012345",
                Nombres = "Pedro",
                ApellidoPaterno = "Díaz",
                ApellidoMaterno = "Vargas",
                FechaNacimiento = new DateOnly(1998, 6, 12),
                TieneSIS = true
            });

            pacientes.Add(new Paciente
            {
                PacienteId = 19,
                UsuarioId = 19,
                DNI = "90123456",
                Nombres = "Sofía",
                ApellidoPaterno = "Rojas",
                ApellidoMaterno = "Castro",
                FechaNacimiento = new DateOnly(1990, 10, 30),
                TieneSIS = false
            });

            return pacientes;
        }
    }
}

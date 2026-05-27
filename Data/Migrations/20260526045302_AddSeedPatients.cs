using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PostaCitasWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "UsuarioId", "Activo", "Celular", "DNI", "FechaCreacion", "NombreUsuario", "PasswordHash", "Rol" },
                values: new object[,]
                {
                    { 10, true, "999888777", "12345678", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "juan_perez", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 11, true, "988777666", "87654321", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "maria_lopez", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 12, true, "977666555", "11223344", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "carlos_rodriguez", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 13, true, "966555444", "99887766", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ana_fernandez", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 14, true, "955444333", "45678901", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "luis_torres", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 15, true, "944333222", "56789012", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "carmen_gomez", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 16, true, "933222111", "67890123", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "jorge_flores", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 17, true, "922111000", "78901234", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "rosa_quispe", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 18, true, "911000999", "89012345", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pedro_diaz", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 },
                    { 19, true, "900999888", "90123456", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sofia_rojas", "$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82", 0 }
                });

            migrationBuilder.InsertData(
                table: "Pacientes",
                columns: new[] { "PacienteId", "ApellidoMaterno", "ApellidoPaterno", "DNI", "FechaNacimiento", "Nombres", "PostaAsociadaId", "ResponsableId", "TieneSIS", "UsuarioId" },
                values: new object[,]
                {
                    { 10, "García", "Pérez", "12345678", new DateOnly(1985, 5, 10), "Juan", null, null, true, 10 },
                    { 11, "Martínez", "López", "87654321", new DateOnly(1992, 8, 24), "María", null, null, false, 11 },
                    { 12, "Sánchez", "Rodríguez", "11223344", new DateOnly(1978, 12, 3), "Carlos", null, null, true, 12 },
                    { 13, "Díaz", "Fernández", "99887766", new DateOnly(1989, 4, 15), "Ana", null, null, true, 13 },
                    { 14, "Ruiz", "Torres", "45678901", new DateOnly(1995, 11, 2), "Luis", null, null, false, 14 },
                    { 15, "Quispe", "Gómez", "56789012", new DateOnly(2001, 7, 18), "Carmen", null, null, true, 15 },
                    { 16, "Chávez", "Flores", "67890123", new DateOnly(1983, 2, 28), "Jorge", null, null, false, 16 },
                    { 17, "Mamani", "Quispe", "78901234", new DateOnly(1974, 9, 5), "Rosa", null, null, true, 17 },
                    { 18, "Vargas", "Díaz", "89012345", new DateOnly(1998, 6, 12), "Pedro", null, null, true, 18 },
                    { 19, "Castro", "Rojas", "90123456", new DateOnly(1990, 10, 30), "Sofía", null, null, false, 19 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 19);
        }
    }
}

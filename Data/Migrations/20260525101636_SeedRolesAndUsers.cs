using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PostaCitasWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EspecialidadId",
                table: "Citas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Medicos",
                columns: new[] { "MedicoId", "Activo", "ApellidoMaterno", "ApellidoPaterno", "CMP", "Nombres" },
                values: new object[] { 1, true, "", "Ramírez", "CMP001", "Carlos" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "UsuarioId", "Activo", "Celular", "DNI", "FechaCreacion", "NombreUsuario", "PasswordHash", "Rol" },
                values: new object[,]
                {
                    { 2, true, "911111111", "11111111", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admision", "Admision123!", 1 },
                    { 3, true, "922222222", "22222222", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "enfermeria", "Enfermeria123!", 2 },
                    { 4, true, "933333333", "33333333", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "medico", "Medico123!", 4 },
                    { 5, true, "944444444", "44444444", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "paciente", "Paciente123!", 0 }
                });

            migrationBuilder.InsertData(
                table: "Pacientes",
                columns: new[] { "PacienteId", "ApellidoMaterno", "ApellidoPaterno", "DNI", "FechaNacimiento", "Nombres", "PostaAsociadaId", "ResponsableId", "TieneSIS", "UsuarioId" },
                values: new object[] { 1, "García", "Pérez", "44444444", new DateOnly(1990, 1, 1), "Juan", null, null, true, 5 });

            migrationBuilder.InsertData(
                table: "ProgramacionesOperativas",
                columns: new[] { "ProgramacionId", "CreadaPorUsuarioId", "CuposTotal", "DuracionMinutos", "EspecialidadId", "Fecha", "FechaCreacion", "Habilitada", "MedicoId", "Turno" },
                values: new object[] { 1, 1, 10, 20, 1, new DateOnly(2030, 1, 2), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_EspecialidadId",
                table: "Citas",
                column: "EspecialidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Especialidades_EspecialidadId",
                table: "Citas",
                column: "EspecialidadId",
                principalTable: "Especialidades",
                principalColumn: "EspecialidadId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Especialidades_EspecialidadId",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_EspecialidadId",
                table: "Citas");

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "PacienteId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProgramacionesOperativas",
                keyColumn: "ProgramacionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Medicos",
                keyColumn: "MedicoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "EspecialidadId",
                table: "Citas");
        }
    }
}

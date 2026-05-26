using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PostaCitasWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    MedicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CMP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.MedicoId);
                });

            migrationBuilder.CreateTable(
                name: "UPS",
                columns: table => new
                {
                    UPSId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UPS", x => x.UPSId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DNI = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Celular = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    EspecialidadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UPSId = table.Column<int>(type: "int", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.EspecialidadId);
                    table.ForeignKey(
                        name: "FK_Especialidades_UPS_UPSId",
                        column: x => x.UPSId,
                        principalTable: "UPS",
                        principalColumn: "UPSId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    PacienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    TieneSIS = table.Column<bool>(type: "bit", nullable: false),
                    PostaAsociadaId = table.Column<int>(type: "int", nullable: true),
                    ResponsableId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.PacienteId);
                    table.ForeignKey(
                        name: "FK_Pacientes_Pacientes_ResponsableId",
                        column: x => x.ResponsableId,
                        principalTable: "Pacientes",
                        principalColumn: "PacienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pacientes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgramacionesOperativas",
                columns: table => new
                {
                    ProgramacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EspecialidadId = table.Column<int>(type: "int", nullable: false),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    Turno = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    CuposTotal = table.Column<int>(type: "int", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false),
                    Habilitada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreadaPorUsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesOperativas", x => x.ProgramacionId);
                    table.ForeignKey(
                        name: "FK_ProgramacionesOperativas_Especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "Especialidades",
                        principalColumn: "EspecialidadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesOperativas_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "MedicoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacionesOperativas_Usuarios_CreadaPorUsuarioId",
                        column: x => x.CreadaPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvisosAtencionInmediata",
                columns: table => new
                {
                    AvisoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EstadoAviso = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvisosAtencionInmediata", x => x.AvisoId);
                    table.ForeignKey(
                        name: "FK_AvisosAtencionInmediata_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "PacienteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SlotsDisponibles",
                columns: table => new
                {
                    SlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramacionId = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeOnly>(type: "time", nullable: false),
                    CuposDisponibles = table.Column<int>(type: "int", nullable: false),
                    CuposTotal = table.Column<int>(type: "int", nullable: false),
                    EsSobrecupo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotsDisponibles", x => x.SlotId);
                    table.CheckConstraint("CK_Slots_CuposDisponibles", "[CuposDisponibles] >= 0");
                    table.ForeignKey(
                        name: "FK_SlotsDisponibles_ProgramacionesOperativas_ProgramacionId",
                        column: x => x.ProgramacionId,
                        principalTable: "ProgramacionesOperativas",
                        principalColumn: "ProgramacionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    CitaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    EstadoCita = table.Column<int>(type: "int", nullable: false),
                    OrigenReserva = table.Column<int>(type: "int", nullable: false),
                    FechaReserva = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EsSobrecupo = table.Column<bool>(type: "bit", nullable: false),
                    RegistradaPorUsuarioId = table.Column<int>(type: "int", nullable: true),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.CitaId);
                    table.ForeignKey(
                        name: "FK_Citas_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "PacienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_SlotsDisponibles_SlotId",
                        column: x => x.SlotId,
                        principalTable: "SlotsDisponibles",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Usuarios_RegistradaPorUsuarioId",
                        column: x => x.RegistradaPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEstadosCita",
                columns: table => new
                {
                    HistorialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitaId = table.Column<int>(type: "int", nullable: false),
                    EstadoAnterior = table.Column<int>(type: "int", nullable: false),
                    EstadoNuevo = table.Column<int>(type: "int", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEstadosCita", x => x.HistorialId);
                    table.ForeignKey(
                        name: "FK_HistorialEstadosCita_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "CitaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstadosCita_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitaId = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_Tickets_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "CitaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Triajes",
                columns: table => new
                {
                    TriajeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitaId = table.Column<int>(type: "int", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Talla = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Temperatura = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    PresionSistolica = table.Column<int>(type: "int", nullable: false),
                    PresionDiastolica = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EnfermeriaUsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triajes", x => x.TriajeId);
                    table.ForeignKey(
                        name: "FK_Triajes_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "CitaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triajes_Usuarios_EnfermeriaUsuarioId",
                        column: x => x.EnfermeriaUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "UPS",
                columns: new[] { "UPSId", "Activa", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Medicina General" },
                    { 2, true, "Odontología" },
                    { 3, true, "Obstetricia" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "UsuarioId", "Activo", "Celular", "DNI", "FechaCreacion", "NombreUsuario", "PasswordHash", "Rol" },
                values: new object[] { 1, true, "000000000", "00000000", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin", "Admin123!", 3 });

            migrationBuilder.InsertData(
                table: "Especialidades",
                columns: new[] { "EspecialidadId", "Activa", "DuracionMinutos", "Nombre", "UPSId" },
                values: new object[,]
                {
                    { 1, true, 20, "Medicina General", 1 },
                    { 2, true, 25, "Odontología", 2 },
                    { 3, true, 20, "Obstetricia", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvisosAtencionInmediata_PacienteId",
                table: "AvisosAtencionInmediata",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_EstadoCita",
                table: "Citas",
                column: "EstadoCita");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_PacienteId",
                table: "Citas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_PacienteId_SlotId",
                table: "Citas",
                columns: new[] { "PacienteId", "SlotId" },
                unique: true,
                filter: "[EstadoCita] IN (0, 1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_RegistradaPorUsuarioId",
                table: "Citas",
                column: "RegistradaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_SlotId",
                table: "Citas",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Especialidades_UPSId",
                table: "Especialidades",
                column: "UPSId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstadosCita_CitaId",
                table: "HistorialEstadosCita",
                column: "CitaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstadosCita_UsuarioId",
                table: "HistorialEstadosCita",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_CMP",
                table: "Medicos",
                column: "CMP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_ResponsableId",
                table: "Pacientes",
                column: "ResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_UsuarioId",
                table: "Pacientes",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesOperativas_CreadaPorUsuarioId",
                table: "ProgramacionesOperativas",
                column: "CreadaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesOperativas_EspecialidadId",
                table: "ProgramacionesOperativas",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesOperativas_MedicoId",
                table: "ProgramacionesOperativas",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotsDisponibles_EsSobrecupo",
                table: "SlotsDisponibles",
                column: "EsSobrecupo");

            migrationBuilder.CreateIndex(
                name: "IX_SlotsDisponibles_ProgramacionId",
                table: "SlotsDisponibles",
                column: "ProgramacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CitaId",
                table: "Tickets",
                column: "CitaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Codigo",
                table: "Tickets",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Triajes_CitaId",
                table: "Triajes",
                column: "CitaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Triajes_EnfermeriaUsuarioId",
                table: "Triajes",
                column: "EnfermeriaUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_DNI",
                table: "Usuarios",
                column: "DNI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvisosAtencionInmediata");

            migrationBuilder.DropTable(
                name: "HistorialEstadosCita");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Triajes");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "SlotsDisponibles");

            migrationBuilder.DropTable(
                name: "ProgramacionesOperativas");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "UPS");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Entities;
using System.Reflection;
using System;

namespace PostaCitasWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<UPS> UPS { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<ProgramacionOperativa> ProgramacionesOperativas { get; set; }
        public DbSet<SlotDisponible> SlotsDisponibles { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Triaje> Triajes { get; set; }
        public DbSet<HistorialEstadoCita> HistorialEstadosCita { get; set; }
        public DbSet<AvisoAtencionInmediata> AvisosAtencionInmediata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                UsuarioId = 1,
                DNI = "00000000",
                NombreUsuario = "admin",
                PasswordHash = "Admin123!",
                Rol = Rol.Administrador,
                Activo = true,
                Celular = "000000000",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Usuario
            {
                UsuarioId = 2,
                DNI = "11111111",
                NombreUsuario = "admision",
                PasswordHash = "Admision123!",
                Rol = Rol.Admision,
                Activo = true,
                Celular = "911111111",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Usuario
            {
                UsuarioId = 3,
                DNI = "22222222",
                NombreUsuario = "enfermeria",
                PasswordHash = "Enfermeria123!",
                Rol = Rol.Enfermeria,
                Activo = true,
                Celular = "922222222",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Usuario
            {
                UsuarioId = 4,
                NombreUsuario = "medico",
                DNI = "33333333",
                PasswordHash = "Medico123!",
                Rol = Rol.Medico,
                Activo = true,
                Celular = "933333333",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Usuario
            {
                UsuarioId = 5,
                DNI = "44444444",
                NombreUsuario = "paciente",
                PasswordHash = "Paciente123!",
                Rol = Rol.Paciente,
                Activo = true,
                Celular = "944444444",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            modelBuilder.Entity<Medico>().HasData(new Medico
            {
                MedicoId = 1,
                Nombres = "Carlos",
                ApellidoPaterno = "Ramírez",
                ApellidoMaterno = "",
                CMP = "CMP001",
                Activo = true
            });

            modelBuilder.Entity<UPS>().HasData(
                new UPS { UPSId = 1, Nombre = "Medicina General", Activa = true },
                new UPS { UPSId = 2, Nombre = "Odontología", Activa = true },
                new UPS { UPSId = 3, Nombre = "Obstetricia", Activa = true }
            );

            modelBuilder.Entity<Especialidad>().HasData(
                new Especialidad { EspecialidadId = 1, UPSId = 1, Nombre = "Medicina General", DuracionMinutos = 20, Activa = true },
                new Especialidad { EspecialidadId = 2, UPSId = 2, Nombre = "Odontología", DuracionMinutos = 25, Activa = true },
                new Especialidad { EspecialidadId = 3, UPSId = 3, Nombre = "Obstetricia", DuracionMinutos = 20, Activa = true }
            );

            modelBuilder.Entity<ProgramacionOperativa>().HasData(new ProgramacionOperativa
            {
                ProgramacionId = 1,
                EspecialidadId = 1,
                MedicoId = 1,
                Turno = Turno.Manana,
                Fecha = new DateOnly(2030, 1, 2),
                CuposTotal = 10,
                DuracionMinutos = 20,
                Habilitada = true,
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreadaPorUsuarioId = 1
            });

            modelBuilder.Entity<Paciente>().HasData(new Paciente
            {
                PacienteId = 1,
                UsuarioId = 5,
                DNI = "44444444",
                Nombres = "Juan",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "García",
                FechaNacimiento = new DateOnly(1990, 1, 1),
                TieneSIS = true,
                PostaAsociadaId = null,
                ResponsableId = null
            });

            modelBuilder.Entity<Usuario>().HasData(SeedData.GetSeedUsuarios().ToArray());
            modelBuilder.Entity<Paciente>().HasData(SeedData.GetSeedPacientes().ToArray());
        }
    }
}

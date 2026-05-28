using Microsoft.EntityFrameworkCore;
using PostaCitasWeb.Data;
using PostaCitasWeb.Entities;
using System;
using System.Threading.Tasks;

namespace PostaCitasWeb.Tests.Helpers
{
    /// <summary>
    /// Helper para sembrar datos de prueba en la base de datos.
    /// Cada prueba de integración debe usar este helper para insertar sus propios datos.
    /// </summary>
    public static class SeedDataHelper
    {
        /// <summary>
        /// Crea un usuario de prueba con el rol especificado.
        /// </summary>
        public static Usuario CreateUsuario(Rol rol, bool activo = true, string dni = "12345678")
        {
            return new Usuario
            {
                DNI = dni,
                NombreUsuario = $"user_{dni}",
                PasswordHash = "hashed_password",
                Rol = rol,
                Activo = activo,
                Celular = "999999999",
                FechaCreacion = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Crea un paciente de prueba asociado a un usuario.
        /// </summary>
        public static Paciente CreatePaciente(int usuarioId, string dni = "12345678", DateOnly? fechaNacimiento = null)
        {
            return new Paciente
            {
                UsuarioId = usuarioId,
                DNI = dni,
                Nombres = "Juan",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "García",
                FechaNacimiento = fechaNacimiento ?? new DateOnly(1990, 1, 1),
                TieneSIS = true,
                PostaAsociadaId = null,
                ResponsableId = null
            };
        }

        /// <summary>
        /// Crea un paciente menor de edad para pruebas de RN03.
        /// </summary>
        public static Paciente CreatePacienteMenor(int usuarioId, int? responsableId = null, string dni = "87654321")
        {
            var fechaNacimiento = DateOnly.FromDateTime(DateTime.Now.AddYears(-10)); // 10 años
            return new Paciente
            {
                UsuarioId = usuarioId,
                DNI = dni,
                Nombres = "Pedrito",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "García",
                FechaNacimiento = fechaNacimiento,
                TieneSIS = true,
                PostaAsociadaId = null,
                ResponsableId = responsableId
            };
        }

        /// <summary>
        /// Crea una especialidad de prueba.
        /// </summary>
        public static Especialidad CreateEspecialidad(int upsId, string nombre = "Medicina General")
        {
            return new Especialidad
            {
                UPSId = upsId,
                Nombre = nombre,
                DuracionMinutos = 20,
                Activa = true
            };
        }

        /// <summary>
        /// Crea un médico de prueba.
        /// </summary>
        public static Medico CreateMedico(string nombres = "Carlos", string apellidoPaterno = "Ramírez")
        {
            return new Medico
            {
                Nombres = nombres,
                ApellidoPaterno = apellidoPaterno,
                ApellidoMaterno = "",
                CMP = "CMP001",
                Activo = true
            };
        }

        /// <summary>
        /// Crea una UPS de prueba.
        /// </summary>
        public static UPS CreateUPS(string nombre = "Medicina General")
        {
            return new UPS
            {
                Nombre = nombre,
                Activa = true
            };
        }

        /// <summary>
        /// Crea una programación operativa de prueba.
        /// </summary>
        public static ProgramacionOperativa CreateProgramacion(
            int especialidadId,
            int medicoId,
            Turno turno,
            DateOnly fecha,
            int cuposTotal = 10,
            int duracionMinutos = 20,
            bool habilitada = true,
            int creadaPorUsuarioId = 1)
        {
            return new ProgramacionOperativa
            {
                EspecialidadId = especialidadId,
                MedicoId = medicoId,
                Turno = turno,
                Fecha = fecha,
                CuposTotal = cuposTotal,
                DuracionMinutos = duracionMinutos,
                Habilitada = habilitada,
                FechaCreacion = DateTime.UtcNow,
                CreadaPorUsuarioId = creadaPorUsuarioId
            };
        }

        /// <summary>
        /// Crea un slot disponible de prueba.
        /// </summary>
        public static SlotDisponible CreateSlot(
            int programacionId,
            TimeOnly horaInicio,
            TimeOnly horaFin,
            int cuposDisponibles = 10,
            int cuposTotal = 10,
            bool esSobrecupo = false)
        {
            return new SlotDisponible
            {
                ProgramacionId = programacionId,
                HoraInicio = horaInicio,
                HoraFin = horaFin,
                CuposDisponibles = cuposDisponibles,
                CuposTotal = cuposTotal,
                EsSobrecupo = esSobrecupo
            };
        }

        /// <summary>
        /// Crea una cita de prueba.
        /// </summary>
        public static Cita CreateCita(
            int pacienteId,
            int slotId,
            int especialidadId,
            EstadoCita estado = EstadoCita.Pendiente,
            OrigenReserva origen = OrigenReserva.Web)
        {
            return new Cita
            {
                PacienteId = pacienteId,
                SlotId = slotId,
                EspecialidadId = especialidadId,
                EstadoCita = estado,
                OrigenReserva = origen,
                FechaReserva = DateTime.UtcNow,
                EsSobrecupo = false,
                RegistradaPorUsuarioId = null,
                FechaUltimaActualizacion = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Crea un ticket de prueba.
        /// </summary>
        public static Ticket CreateTicket(int citaId, string codigo = "TC-20260527-12345")
        {
            return new Ticket
            {
                CitaId = citaId,
                Codigo = codigo,
                FechaEmision = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Crea un triaje de prueba.
        /// </summary>
        public static Triaje CreateTriaje(
            int citaId,
            int enfermeriaUsuarioId,
            decimal peso = 70,
            decimal talla = 1.75m,
            decimal temperatura = 36.5m,
            int presionSistolica = 120,
            int presionDiastolica = 80,
            string? observacion = null)
        {
            return new Triaje
            {
                CitaId = citaId,
                EnfermeriaUsuarioId = enfermeriaUsuarioId,
                Peso = peso,
                Talla = talla,
                Temperatura = temperatura,
                PresionSistolica = presionSistolica,
                PresionDiastolica = presionDiastolica,
                Observacion = observacion,
                FechaRegistro = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Crea un historial de estado de cita de prueba.
        /// </summary>
        public static HistorialEstadoCita CreateHistorialEstadoCita(
            int citaId,
            EstadoCita estadoAnterior,
            EstadoCita estadoNuevo,
            int usuarioId,
            string? observacion = null)
        {
            return new HistorialEstadoCita
            {
                CitaId = citaId,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = estadoNuevo,
                FechaCambio = DateTime.UtcNow,
                UsuarioId = usuarioId,
                Observacion = observacion
            };
        }

        /// <summary>
        /// Crea un aviso de atención inmediata de prueba.
        /// </summary>
        public static AvisoAtencionInmediata CreateAviso(
            int pacienteId,
            string motivo = "Dolor abdominal",
            EstadoAviso estado = EstadoAviso.Pendiente)
        {
            return new AvisoAtencionInmediata
            {
                PacienteId = pacienteId,
                Motivo = motivo,
                EstadoAviso = estado,
                FechaEnvio = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Sembrará datos básicos en el contexto (usuarios, UPS, especialidades, médicos).
        /// Útil para pruebas de integración que requieren datos de referencia.
        /// </summary>
        public static async Task SeedBasicDataAsync(AppDbContext context)
        {
            // UPS
            if (!await context.UPS.AnyAsync())
            {
                context.UPS.AddRange(
                    CreateUPS("Medicina General"),
                    CreateUPS("Odontología"),
                    CreateUPS("Obstetricia")
                );
                await context.SaveChangesAsync();
            }

            // Especialidades
            if (!await context.Especialidades.AnyAsync())
            {
                var ups = await context.UPS.ToListAsync();
                context.Especialidades.AddRange(
                    CreateEspecialidad(ups[0].UPSId, "Medicina General"),
                    CreateEspecialidad(ups[1].UPSId, "Odontología"),
                    CreateEspecialidad(ups[2].UPSId, "Obstetricia")
                );
                await context.SaveChangesAsync();
            }

            // Médicos
            if (!await context.Medicos.AnyAsync())
            {
                context.Medicos.AddRange(
                    CreateMedico("Carlos", "Ramírez"),
                    CreateMedico("María", "López")
                );
                await context.SaveChangesAsync();
            }

            // Usuarios de prueba
            if (!await context.Usuarios.AnyAsync())
            {
                context.Usuarios.AddRange(
                    CreateUsuario(Rol.Administrador, true, "00000001"),
                    CreateUsuario(Rol.Admision, true, "00000002"),
                    CreateUsuario(Rol.Enfermeria, true, "00000003"),
                    CreateUsuario(Rol.Medico, true, "00000004"),
                    CreateUsuario(Rol.Paciente, true, "00000005")
                );
                await context.SaveChangesAsync();
            }
        }
    }
}

# ESPECIFICACIÓN DE PRUEBAS

Proyecto: Sistema Web de Gestión de Citas Médicas – Posta de Salud (PostaCitasWeb)  
Versión: 1.0  
Enfoque: Specification-Driven Development (SpecDD) + Behavior-Driven Development (BDD)  
Frameworks sugeridos: xUnit · Moq · FluentAssertions · Respawn (reset de BD en integración)

---

## PROPÓSITO

Este documento define el comportamiento esperado del sistema **antes** de que se escriba el código de implementación. Cada escenario es una especificación ejecutable: si el código la pasa, el comportamiento es correcto. Si la falla, el código está mal, no la prueba.

Las pruebas están organizadas por tipo (unitaria / integración) y asociadas a las reglas de negocio de `03_reglas_negocio.md`. Ningún escenario introduce reglas fuera de ese documento.

---

## CONVENCIONES

- `[U-XX]` → Prueba unitaria numerada.
- `[I-XX]` → Prueba de integración numerada.
- `RNxx` → Regla de negocio asociada del documento `03_reglas_negocio.md`.
- **Dado / Cuando / Entonces** → Sintaxis Gherkin en español (alias de Given / When / Then).
- `Y` → Alias de `And` en Gherkin.

---

## PARTE 1 — PRUEBAS UNITARIAS

Alcance: métodos de la capa `Services` probados en aislamiento. Las dependencias de repositorio se reemplazan con mocks (`Moq`). No se toca base de datos.

Estructura de proyecto sugerida:

```
PostaCitasWeb.Tests/
└── Unit/
    ├── CitaServiceTests.cs
    ├── DisponibilidadServiceTests.cs
    ├── AuthServiceTests.cs
    ├── TriajeServiceTests.cs
    └── AvisoServiceTests.cs
```

---

### MÓDULO: AUTENTICACIÓN

---

#### [U-01] — Usuario inactivo no puede iniciar sesión

**Regla asociada:** RN01 — Acceso controlado

```gherkin
Escenario: Paciente con cuenta no habilitada intenta iniciar sesión
  Dado    que existe un Usuario con DNI "47110001" y Activo = false
  Cuando  AuthService.LoginAsync("47110001", "clave123") es invocado
  Entonces el resultado debe ser un error de autenticación
  Y       el mensaje debe indicar que la cuenta no está habilitada
```

**Clase:** `AuthServiceTests`  
**Método bajo prueba:** `AuthService.LoginAsync(string dni, string password)`  
**Mock:** `IUsuarioRepository.GetByDniAsync()` devuelve usuario con `Activo = false`  
**Aserción:** resultado es `null` o excepción de tipo `UnauthorizedAccessException`

---

#### [U-02] — Recuperación de contraseña requiere DNI y celular coincidentes

**Regla asociada:** RN02-A — Recuperación de acceso

```gherkin
Escenario: Paciente proporciona celular que no coincide con el registrado
  Dado    que existe un Usuario con DNI "47110001" y Celular registrado "987654321"
  Cuando  AuthService.SolicitarRecuperacionAsync("47110001", "999999999") es invocado
  Entonces el resultado debe ser un error de validación
  Y       no debe generarse ningún token de recuperación

Escenario: Paciente proporciona celular correcto
  Dado    que existe un Usuario con DNI "47110001" y Celular registrado "987654321"
  Cuando  AuthService.SolicitarRecuperacionAsync("47110001", "987654321") es invocado
  Entonces el resultado debe ser exitoso
  Y       debe retornarse un token temporal para cambio de contraseña
```

**Clase:** `AuthServiceTests`  
**Método bajo prueba:** `AuthService.SolicitarRecuperacionAsync(string dni, string celular)`

---

#### [U-03] — Paciente no puede modificar campos bloqueados

**Regla asociada:** RN02 — Gestión de cuenta

```gherkin
Escenario: Paciente intenta actualizar su DNI
  Dado    que existe un Paciente autenticado con PacienteId = 5
  Cuando  PacienteService.ActualizarDatosAsync(5, { DNI = "99999999" }) es invocado
  Entonces el servicio debe lanzar una excepción de tipo InvalidOperationException
  Y       el DNI del paciente no debe haber cambiado

Escenario: Paciente actualiza su número celular correctamente
  Dado    que existe un Paciente autenticado con PacienteId = 5
  Cuando  PacienteService.ActualizarDatosAsync(5, { Celular = "912345678" }) es invocado
  Entonces el resultado debe ser exitoso
  Y       el Celular del Usuario asociado debe ser "912345678"
```

**Clase:** `AuthServiceTests` / `PacienteServiceTests`  
**Campos bloqueados a validar:** `DNI`, `Nombres`, `ApellidoPaterno`, `ApellidoMaterno`, `FechaNacimiento`

---

#### [U-04] — Solo adulto responsable puede reservar para un menor

**Regla asociada:** RN03 — Gestión de menores

```gherkin
Escenario: Paciente adulto no vinculado intenta reservar para un menor
  Dado    que existe un Paciente menor con PacienteId = 10
  Y       existe un Paciente adulto con PacienteId = 7 sin vínculo de responsabilidad
  Cuando  CitaService.ReservarAsync(slotId: 3, pacienteId: 10, solicitanteId: 7) es invocado
  Entonces el servicio debe retornar un error de autorización
  Y       no debe crearse ninguna cita

Escenario: Adulto responsable vinculado reserva para su menor
  Dado    que existe un Paciente menor con PacienteId = 10 y ResponsableId = 20
  Y       existe un Paciente adulto con PacienteId = 20
  Cuando  CitaService.ReservarAsync(slotId: 3, pacienteId: 10, solicitanteId: 20) es invocado
  Entonces la reserva debe crearse correctamente
```

---

### MÓDULO: DISPONIBILIDAD

---

#### [U-05] — Slots de sobrecupo no son visibles para pacientes

**Regla asociada:** RN16 — Sobrecupo oculto

```gherkin
Escenario: Paciente consulta disponibilidad y no debe ver slots de sobrecupo
  Dado    que existen 3 SlotDisponible para la programación 15
          con EsSobrecupo = false en 2 slots y EsSobrecupo = true en 1 slot
  Cuando  DisponibilidadService.ObtenerSlotsAsync(programacionId: 15, rol: Paciente) es invocado
  Entonces el resultado debe contener exactamente 2 slots
  Y       ningún slot del resultado debe tener EsSobrecupo = true

Escenario: Admisión consulta disponibilidad y sí puede ver slots de sobrecupo
  Dado    que existen 3 SlotDisponible para la programación 15
          con 2 slots normales y 1 de sobrecupo
  Cuando  DisponibilidadService.ObtenerSlotsAsync(programacionId: 15, rol: Admision) es invocado
  Entonces el resultado debe contener los 3 slots
```

**Clase:** `DisponibilidadServiceTests`  
**Método bajo prueba:** `DisponibilidadService.ObtenerSlotsAsync(int programacionId, Rol rol)`

---

#### [U-06] — No se puede modificar disponibilidad de una jornada ya iniciada

**Regla asociada:** RN09 — Modificación futura / RN18 — Rotación operativa

```gherkin
Escenario: Admisión intenta ajustar una programación cuya fecha ya pasó
  Dado    que existe una ProgramacionOperativa con Fecha = hoy - 1 día
  Cuando  DisponibilidadService.AjustarProgramacionAsync(programacionId: 8, nuevosMedico: 3) es invocado
  Entonces el servicio debe retornar un error de validación
  Y       la programación no debe haber sido modificada

Escenario: Admisión ajusta una programación futura
  Dado    que existe una ProgramacionOperativa con Fecha = hoy + 3 días
  Cuando  DisponibilidadService.AjustarProgramacionAsync(programacionId: 9, nuevoMedicoId: 3) es invocado
  Entonces la operación debe completarse sin errores
```

**Clase:** `DisponibilidadServiceTests`  
**Método bajo prueba:** `DisponibilidadService.AjustarProgramacionAsync(...)`

---

#### [U-07] — Generación automática de slots respeta turnos y duración

**Regla asociada:** RN08, RN27, RN28, RN29

```gherkin
Escenario: Se generan slots para turno mañana con duración de 20 minutos
  Dado    que existe una ProgramacionOperativa con Turno = Mañana y DuracionMinutos = 20
  Cuando  DisponibilidadService.GenerarSlotsAsync(programacionId: 5) es invocado
  Entonces el primer slot debe iniciar a las 08:00
  Y       el segundo slot debe iniciar a las 08:20
  Y       el último slot debe finalizar a las 13:40 o antes
  Y       ningún slot debe iniciar después de las 13:30

Escenario: Se generan slots para turno tarde con duración de 25 minutos
  Dado    que existe una ProgramacionOperativa con Turno = Tarde y DuracionMinutos = 25
  Cuando  DisponibilidadService.GenerarSlotsAsync(programacionId: 6) es invocado
  Entonces el primer slot debe iniciar a las 15:00
  Y       el segundo slot debe iniciar a las 15:25
  Y       ningún slot debe iniciar después de las 19:00
```

**Clase:** `DisponibilidadServiceTests`  
**Método bajo prueba:** `DisponibilidadService.GenerarSlotsAsync(int programacionId)`

---

#### [U-08] — Admisión no puede crear programación, solo habilitarla

**Regla asociada:** RN06 — Restricción de publicación

```gherkin
Escenario: Admisión intenta crear una nueva ProgramacionOperativa
  Dado    que el usuario autenticado tiene Rol = Admision
  Cuando  DisponibilidadService.CrearProgramacionAsync(nueva programacion) es invocado
  Entonces el servicio debe lanzar una excepción de tipo UnauthorizedAccessException
  Y       no debe insertarse ningún registro en ProgramacionesOperativas

Escenario: Admisión habilita una programación existente
  Dado    que existe una ProgramacionOperativa con ProgramacionId = 12 y Habilitada = false
  Y       el usuario autenticado tiene Rol = Admision
  Cuando  DisponibilidadService.HabilitarProgramacionAsync(programacionId: 12) es invocado
  Entonces Habilitada debe ser true
```

---

### MÓDULO: RESERVA DE CITAS

---

#### [U-09] — No se puede reservar si el slot no tiene cupos disponibles

**Regla asociada:** RN10 — Orden de reserva / RN04 — Stock compartido

```gherkin
Escenario: Paciente intenta reservar un slot agotado
  Dado    que existe un SlotDisponible con SlotId = 7 y CuposDisponibles = 0
  Cuando  CitaService.ReservarAsync(slotId: 7, pacienteId: 2) es invocado
  Entonces el servicio debe retornar un error indicando cupo agotado
  Y       no debe crearse ninguna Cita
  Y       CuposDisponibles del slot debe seguir siendo 0

Escenario: Paciente reserva un slot con cupos disponibles
  Dado    que existe un SlotDisponible con SlotId = 8 y CuposDisponibles = 3
  Cuando  CitaService.ReservarAsync(slotId: 8, pacienteId: 3) es invocado
  Entonces debe crearse una Cita con EstadoCita = Pendiente
  Y       CuposDisponibles del slot debe decrementarse a 2
```

**Clase:** `CitaServiceTests`  
**Método bajo prueba:** `CitaService.ReservarAsync(int slotId, int pacienteId)`

---

#### [U-10] — Reserva genera ticket obligatoriamente

**Regla asociada:** RN12 — Ticket obligatorio

```gherkin
Escenario: Confirmación de reserva produce ticket
  Dado    que existe un SlotDisponible con SlotId = 10 y CuposDisponibles = 2
  Y       existe un Paciente con PacienteId = 4 y usuario activo
  Cuando  CitaService.ReservarAsync(slotId: 10, pacienteId: 4) es invocado
  Entonces debe crearse una Cita con EstadoCita = Pendiente
  Y       debe crearse un Ticket asociado a esa Cita
  Y       el Ticket debe tener un Codigo no nulo y no vacío
```

**Clase:** `CitaServiceTests`

---

#### [U-11] — No se permiten reservas duplicadas activas en el mismo slot

**Regla asociada:** RN31 — Integridad

```gherkin
Escenario: Paciente intenta reservar dos veces el mismo slot
  Dado    que existe una Cita activa con PacienteId = 5 y SlotId = 11 y EstadoCita = Pendiente
  Cuando  CitaService.ReservarAsync(slotId: 11, pacienteId: 5) es invocado nuevamente
  Entonces el servicio debe retornar un error de duplicidad
  Y       no debe crearse una segunda Cita

Escenario: Paciente puede reservar el mismo slot si su cita anterior fue cancelada
  Dado    que existe una Cita con PacienteId = 5, SlotId = 11 y EstadoCita = Cancelada
  Cuando  CitaService.ReservarAsync(slotId: 11, pacienteId: 5) es invocado
  Entonces debe crearse una nueva Cita con EstadoCita = Pendiente
```

#### [U-11B] — Validar ventana estricta de fechas para reserva web

**Regla asociada:** RN37 — Ventana estricta de reserva

Escenario: Paciente intenta reservar de Lunes a Viernes para una fecha futura no permitida
  Dado    que la fecha actual del sistema es Martes
  Y       existe un SlotDisponible para el día Miércoles
  Cuando  CitaService.ReservarAsync() es invocado para ese slot
  Entonces el servicio debe retornar un error de validación de fecha
  Y       no debe crearse ninguna Cita

Escenario: Paciente reserva un Sábado para el día Lunes
  Dado    que la fecha actual del sistema es Sábado
  Y       existe un SlotDisponible para el día Lunes (hoy + 2 días)
  Cuando  CitaService.ReservarAsync() es invocado para ese slot
  Entonces la operación debe ser exitosa
  Y       debe crearse una Cita con EstadoCita = Pendiente

Escenario: Paciente intenta reservar un Domingo
  Dado    que la fecha actual del sistema es Domingo
  Cuando  CitaService.ReservarAsync() es invocado para cualquier slot
  Entonces el servicio debe retornar un error indicando que no hay atención web

**Clase:** `CitaServiceTests`

---

#### [U-12] — Cancelación solo permitida antes del inicio del triaje

**Regla asociada:** RN13 — Restricción de cancelación / RN36 — Inicio de triaje

```gherkin
Escenario: Paciente cancela cita con turno mañana antes de las 07:40
  Dado    que existe una Cita con EstadoCita = Pendiente y turno Mañana
  Y       la hora actual del sistema es 07:15
  Cuando  CitaService.CancelarAsync(citaId: 1, pacienteId: 5) es invocado
  Entonces la Cita debe quedar con EstadoCita = Cancelada
  Y       CuposDisponibles del slot debe incrementarse en 1

Escenario: Paciente intenta cancelar cita de turno mañana después de las 07:40
  Dado    que existe una Cita con EstadoCita = Pendiente y turno Mañana
  Y       la hora actual del sistema es 08:05
  Cuando  CitaService.CancelarAsync(citaId: 1, pacienteId: 5) es invocado
  Entonces el servicio debe retornar un error indicando que la ventana de cancelación cerró
  Y       la Cita debe mantener EstadoCita = Pendiente
  Y       CuposDisponibles no debe cambiar

Escenario: Paciente cancela cita con turno tarde antes de las 14:40
  Dado    que existe una Cita con EstadoCita = Pendiente y turno Tarde
  Y       la hora actual del sistema es 14:10
  Cuando  CitaService.CancelarAsync(citaId: 2, pacienteId: 5) es invocado
  Entonces la Cita debe quedar con EstadoCita = Cancelada
  Y       CuposDisponibles del slot debe incrementarse en 1

Escenario: Paciente intenta cancelar después de las 14:40 turno tarde
  Dado    que existe una Cita con EstadoCita = Pendiente y turno Tarde
  Y       la hora actual del sistema es 15:20
  Cuando  CitaService.CancelarAsync(citaId: 2, pacienteId: 5) es invocado
  Entonces el servicio debe retornar un error de ventana cerrada
```

**Clase:** `CitaServiceTests`  
**Método bajo prueba:** `CitaService.CancelarAsync(int citaId, int pacienteId)`  
**Nota de implementación:** el servicio debe recibir `IDateTimeProvider` inyectable para poder controlar la hora en pruebas sin depender del reloj del sistema.

---

#### [U-13] — Cancelación libera cupo en el stock compartido

**Regla asociada:** RN14 — Liberación de cupo / RN04 — Stock compartido

```gherkin
Escenario: Cancelación de cita web restaura cupo compartido
  Dado    que un Paciente tiene una Cita activa con OrigenReserva = Web en SlotId = 15
  Y       el SlotId = 15 tiene CuposDisponibles = 1
  Cuando  CitaService.CancelarAsync(citaId, pacienteId) es invocado dentro de la ventana permitida
  Entonces CuposDisponibles del SlotId = 15 debe ser 2

Escenario: Cancelación de cita presencial también libera cupo compartido
  Dado    que una Cita tiene OrigenReserva = Presencial en SlotId = 15
  Y       el SlotId = 15 tiene CuposDisponibles = 0
  Cuando  la cita es cancelada por Admisión
  Entonces CuposDisponibles del SlotId = 15 debe ser 1
```

---

#### [U-14] — Trazabilidad: cada cambio de estado genera historial

**Regla asociada:** RN30 — Seguimiento / RN32 — Auditoría

```gherkin
Escenario: Al cancelar una cita se registra el cambio en el historial
  Dado    que existe una Cita con CitaId = 20 y EstadoCita = Pendiente
  Cuando  CitaService.CancelarAsync(citaId: 20, pacienteId: 5) es invocado
  Entonces debe existir un registro en HistorialEstadosCita con:
           CitaId = 20
           EstadoAnterior = Pendiente
           EstadoNuevo = Cancelada
           UsuarioId = 5

Escenario: Al registrar triaje se registra el cambio de estado en historial
  Dado    que existe una Cita con CitaId = 21 y EstadoCita = Pendiente
  Cuando  TriajeService.RegistrarAsync(citaId: 21, datosMedicos, enfermeriaUsuarioId: 9) es invocado
  Entonces debe existir un registro en HistorialEstadosCita con:
           EstadoAnterior = Pendiente
           EstadoNuevo = EnTriaje
```

---

### MÓDULO: TRIAJE

---

#### [U-15] — Solo enfermería puede registrar triaje

**Regla asociada:** RN20 — Registro exclusivo

```gherkin
Escenario: Usuario con rol Paciente intenta registrar triaje
  Dado    que el usuario autenticado tiene UsuarioId = 6 y Rol = Paciente
  Y       existe una Cita con CitaId = 30 y EstadoCita = Pendiente
  Cuando  TriajeService.RegistrarAsync(citaId: 30, datosMedicos, solicitanteId: 6) es invocado
  Entonces el servicio debe lanzar una excepción de tipo UnauthorizedAccessException
  Y       no debe crearse ningún registro en Triajes

Escenario: Enfermera registra triaje correctamente
  Dado    que el usuario autenticado tiene UsuarioId = 9 y Rol = Enfermeria
  Y       existe una Cita con CitaId = 30 y EstadoCita = Pendiente
  Cuando  TriajeService.RegistrarAsync(citaId: 30, datosMedicos, solicitanteId: 9) es invocado
  Entonces debe crearse un Triaje vinculado a CitaId = 30
  Y       EstadoCita de la cita debe cambiar a EnTriaje
```

**Clase:** `TriajeServiceTests`  
**Método bajo prueba:** `TriajeService.RegistrarAsync(int citaId, TriajeDto datos, int solicitanteId)`

---

#### [U-16] — El triaje cambia el estado de la cita según la secuencia permitida

**Regla asociada:** RN21 — Estados permitidos

```gherkin
Escenario: Enfermería registra triaje y el estado pasa de Pendiente a EnTriaje
  Dado    que existe una Cita con EstadoCita = Pendiente
  Cuando  TriajeService.RegistrarAsync es invocado correctamente
  Entonces EstadoCita debe ser EnTriaje

Escenario: Enfermería marca paciente como listo para atención
  Dado    que existe una Cita con EstadoCita = EnTriaje
  Cuando  CitaService.ActualizarEstadoAsync(citaId, ListoAtencion, enfermeriaUsuarioId) es invocado
  Entonces EstadoCita debe ser ListoAtencion

Escenario: No se puede pasar directamente de Pendiente a ListoAtencion
  Dado    que existe una Cita con EstadoCita = Pendiente
  Cuando  CitaService.ActualizarEstadoAsync(citaId, ListoAtencion, enfermeriaUsuarioId) es invocado
  Entonces el servicio debe retornar un error de transición de estado inválida
```

---

### MÓDULO: AVISOS DE ATENCIÓN INMEDIATA

---

#### [U-17] — El aviso no genera cita ni modifica disponibilidad

**Regla asociada:** RN24 — Aviso informativo / RN25 — Sin prioridad automática

```gherkin
Escenario: Paciente envía aviso de atención inmediata
  Dado    que existe un Paciente con PacienteId = 3 y usuario activo
  Y       existe un SlotDisponible con SlotId = 5 y CuposDisponibles = 4
  Cuando  AvisoService.EnviarAsync(pacienteId: 3, motivo: "Dolor fuerte") es invocado
  Entonces debe crearse un AvisoAtencionInmediata con EstadoAviso = Pendiente
  Y       no debe crearse ninguna Cita
  Y       CuposDisponibles del SlotId = 5 debe seguir siendo 4
```

**Clase:** `AvisoServiceTests`  
**Método bajo prueba:** `AvisoService.EnviarAsync(int pacienteId, string motivo)`

---

#### [U-18] — Los avisos solo son visibles para enfermería

**Regla asociada:** RN26 — Visualización restringida

```gherkin
Escenario: Usuario con rol Paciente intenta listar los avisos
  Dado    que el usuario autenticado tiene Rol = Paciente
  Cuando  AvisoService.ObtenerTodosAsync(solicitanteId, Rol.Paciente) es invocado
  Entonces el servicio debe lanzar UnauthorizedAccessException

Escenario: Enfermera lista avisos correctamente
  Dado    que el usuario autenticado tiene Rol = Enfermeria
  Y       existen 3 avisos en estado Pendiente
  Cuando  AvisoService.ObtenerTodosAsync(solicitanteId, Rol.Enfermeria) es invocado
  Entonces el resultado debe contener los 3 avisos
```

---

## PARTE 2 — PRUEBAS DE INTEGRACIÓN

Alcance: pruebas que ejercen la pila completa: `Service → Repository → AppDbContext → SQL Server (LocalDB o SQL Server en memoria)`. Se usa `Respawn` para resetear la base de datos entre pruebas y un `WebApplicationFactory<Program>` si se prueban controladores.

Estructura de proyecto sugerida:

```
PostaCitasWeb.Tests/
└── Integration/
    ├── CitaIntegrationTests.cs
    ├── DisponibilidadIntegrationTests.cs
    ├── TriajeIntegrationTests.cs
    ├── AuthIntegrationTests.cs
    └── Helpers/
        ├── TestDbFactory.cs       ← AppDbContext con LocalDB de prueba
        └── SeedDataHelper.cs      ← Datos mínimos por escenario
```

**Configuración base:**

```csharp
// Helpers/TestDbFactory.cs
public class TestDbFactory : IAsyncLifetime
{
    protected AppDbContext Context { get; private set; }
    private Respawner _respawner;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PostaCitasWeb_Test;Trusted_Connection=True;")
            .Options;
        Context = new AppDbContext(options);
        await Context.Database.MigrateAsync();
        _respawner = await Respawner.CreateAsync(Context.Database.GetConnectionString()!);
    }

    public async Task DisposeAsync() => await _respawner.ResetAsync(Context.Database.GetConnectionString()!);
}
```

---

### MÓDULO: RESERVA DE CITAS (INTEGRACIÓN)

---

#### [I-01] — Reserva web decrementa cupo en base de datos y persiste ticket

**Regla asociada:** RN04, RN12

```gherkin
Escenario: Flujo completo de reserva web persiste cita y ticket en base de datos
  Dado    que en base de datos existe:
            - Un Usuario activo con Rol = Paciente y PacienteId = 1
            - Una ProgramacionOperativa habilitada con ProgramacionId = 1
            - Un SlotDisponible con SlotId = 1, CuposDisponibles = 3, EsSobrecupo = false
  Cuando  CitaService.ReservarAsync(slotId: 1, pacienteId: 1) es invocado
  Entonces en la tabla Citas debe existir 1 registro con:
            - PacienteId = 1
            - SlotId = 1
            - EstadoCita = Pendiente
            - OrigenReserva = Web
  Y       en la tabla Tickets debe existir 1 registro vinculado a esa Cita
  Y       en la tabla SlotsDisponibles, el registro con SlotId = 1 debe tener CuposDisponibles = 2
```

**Clase:** `CitaIntegrationTests`  
**Verificación:** consultas directas al `AppDbContext` de prueba tras ejecutar el servicio.

---

#### [I-02] — Reserva presencial usa el mismo stock que la reserva web

**Regla asociada:** RN04 — Stock compartido

```gherkin
Escenario: Una reserva web y una presencial comparten el mismo contador de cupos
  Dado    que existe un SlotDisponible con SlotId = 2 y CuposDisponibles = 2
  Cuando  CitaService.ReservarAsync(slotId: 2, pacienteId: 1) es invocado (origen Web)
  Y       CitaService.RegistrarPresencialAsync(slotId: 2, pacienteId: 2, admisionUsuarioId: 10)
          es invocado (origen Presencial)
  Entonces CuposDisponibles del SlotId = 2 debe ser 0
  Y       deben existir 2 Citas en base de datos para ese slot
  Y       una Cita debe tener OrigenReserva = Web y la otra OrigenReserva = Presencial
```

---

#### [I-03] — Cancelación restaura cupo y persiste historial

**Regla asociada:** RN13, RN14, RN30

```gherkin
Escenario: Cancelación dentro de ventana horaria actualiza cupo y registra historial
  Dado    que existe una Cita con CitaId = 5, EstadoCita = Pendiente y turno Mañana
  Y       el SlotDisponible vinculado tiene CuposDisponibles = 1
  Y       la hora actual simulada es 07:00
  Cuando  CitaService.CancelarAsync(citaId: 5, pacienteId: 1) es invocado
  Entonces la Cita en base de datos debe tener EstadoCita = Cancelada
  Y       el SlotDisponible debe tener CuposDisponibles = 2
  Y       en HistorialEstadosCita debe existir 1 registro con:
            EstadoAnterior = Pendiente
            EstadoNuevo = Cancelada
            CitaId = 5
```

---

#### [I-04] — No se persiste segunda reserva activa para el mismo paciente y slot

**Regla asociada:** RN31 — Integridad

```gherkin
Escenario: Intento de doble reserva al mismo slot es rechazado por la base de datos
  Dado    que existe una Cita con PacienteId = 3, SlotId = 7 y EstadoCita = Pendiente
  Y       el SlotDisponible tiene CuposDisponibles = 5
  Cuando  CitaService.ReservarAsync(slotId: 7, pacienteId: 3) es invocado por segunda vez
  Entonces se debe lanzar una excepción (de servicio o de índice único de BD)
  Y       la tabla Citas debe contener exactamente 1 registro activo para PacienteId = 3 y SlotId = 7
  Y       CuposDisponibles debe seguir siendo 4 (solo el primer decremento aplicó)
```

**Nota:** esta prueba verifica que tanto la validación de servicio como el índice único condicional `UX_Citas_PacienteSlotActiva` funcionen correctamente en conjunto.

---

### MÓDULO: DISPONIBILIDAD (INTEGRACIÓN)

---

#### [I-05] — Generación de slots persiste correctamente en base de datos

**Regla asociada:** RN08, RN27, RN28, RN29

```gherkin
Escenario: Se generan y persisten slots para turno mañana con 20 minutos de duración
  Dado    que existe una ProgramacionOperativa con:
            Turno = Mañana, DuracionMinutos = 20, CuposTotal = 2, Habilitada = true
  Cuando  DisponibilidadService.GenerarSlotsAsync(programacionId) es invocado
  Entonces en la tabla SlotsDisponibles deben existir slots que:
            - El primero inicia a las 08:00
            - Cada slot tiene CuposTotal = 2 y CuposDisponibles = 2
            - Ningún slot tiene HoraInicio después de las 13:30
            - EsSobrecupo = false en todos
```

---

#### [I-06] — Habilitar programación la deja visible para consultas de paciente

**Regla asociada:** RN05, RN06

```gherkin
Escenario: Programación habilitada aparece en la consulta de disponibilidad del paciente
  Dado    que existen dos ProgramacionesOperativas:
            - ProgramacionId = 10 con Habilitada = false
            - ProgramacionId = 11 con Habilitada = true
  Y       ambas tienen SlotDisponibles con EsSobrecupo = false
  Cuando  DisponibilidadService.ObtenerSlotsAsync(especialidadId, turno, rol: Paciente) es invocado
  Entonces solo deben retornarse los slots pertenecientes a ProgramacionId = 11
```

---

### MÓDULO: TRIAJE (INTEGRACIÓN)

---

#### [I-07] — Registro de triaje persiste datos y actualiza estado de cita

**Regla asociada:** RN19, RN20, RN22, RN30

```gherkin
Escenario: Enfermería registra triaje y la cita cambia de estado en base de datos
  Dado    que existe una Cita con CitaId = 8 y EstadoCita = Pendiente
  Y       existe un Usuario con UsuarioId = 9 y Rol = Enfermeria
  Cuando  TriajeService.RegistrarAsync(citaId: 8, datos, enfermeriaUsuarioId: 9) es invocado
  Entonces en la tabla Triajes debe existir 1 registro con CitaId = 8
  Y       los campos Peso, Talla, Temperatura, PresionSistolica, PresionDiastolica deben persistir
  Y       la Cita en base de datos debe tener EstadoCita = EnTriaje
  Y       en HistorialEstadosCita debe existir 1 registro con:
            CitaId = 8, EstadoAnterior = Pendiente, EstadoNuevo = EnTriaje, UsuarioId = 9
```

---

#### [I-08] — No se puede registrar segundo triaje para la misma cita

**Regla asociada:** RN19 (relación 1:1)

```gherkin
Escenario: Intento de registrar triaje duplicado es rechazado
  Dado    que ya existe un Triaje vinculado a CitaId = 8
  Cuando  TriajeService.RegistrarAsync(citaId: 8, nuevosDatos, enfermeriaUsuarioId: 9) es invocado
  Entonces se debe lanzar una excepción indicando que la cita ya tiene triaje registrado
  Y       la tabla Triajes debe seguir teniendo exactamente 1 registro para CitaId = 8
```

---

### MÓDULO: AUTENTICACIÓN (INTEGRACIÓN)

---

#### [I-09] — Usuario con Activo = false no puede autenticarse

**Regla asociada:** RN01 — Acceso controlado

```gherkin
Escenario: Login con cuenta inactiva no produce sesión
  Dado    que en base de datos existe un Usuario con DNI "47110002" y Activo = false
  Cuando  AuthService.LoginAsync("47110002", "clave123") es invocado contra la base de datos real
  Entonces el resultado debe ser null o excepción de acceso denegado
  Y       no debe generarse ninguna cookie de sesión
```

---

#### [I-10] — Habilitación de usuario por Admisión persiste en base de datos

**Regla asociada:** RN01 — Acceso controlado

```gherkin
Escenario: Admisión activa la cuenta de un paciente
  Dado    que existe un Usuario con UsuarioId = 15 y Activo = false
  Y       el usuario actuante tiene Rol = Admision
  Cuando  AuthService.HabilitarUsuarioAsync(usuarioId: 15, admisionId) es invocado
  Entonces en la tabla Usuarios el registro con UsuarioId = 15 debe tener Activo = true
```

---



## PARTE 3 — MATRIZ DE COBERTURA

Cruce entre reglas de negocio y pruebas que las cubren.

| Regla | Descripción resumida | Pruebas unitarias | Pruebas integración |
|---|---|---|---|
| RN01 | Acceso controlado | U-01 | I-09, I-10 |
| RN02 | Campos no modificables | U-03 | — |
| RN02-A | Recuperación por celular | U-02 | — |
| RN03 | Menores y responsable | U-04 | — |
| RN04 | Stock compartido | U-09, U-13 | I-01, I-02, I-03 |
| RN05 | Publicación operativa | U-08 | I-06 |
| RN06 | Admisión no crea programación | U-08 | — |
| RN08 | Generación automática de slots | U-07 | I-05 |
| RN09 | Solo jornadas futuras modificables | U-06 | — |
| RN10 | Verificación de cupos al confirmar | U-09 | I-01 |
| RN12 | Ticket obligatorio | U-10 | I-01 |
| RN13 | Ventana de cancelación | U-12 | I-03 |
| RN14 | Liberación de cupo al cancelar | U-13 | I-03 |
| RN16 | Sobrecupo oculto para paciente | U-05 | I-06 |
| RN18 | Rotación solo afecta futuro | U-06 | — |
| RN19 | Un triaje por cita | U-15, U-16 | I-07, I-08 |
| RN20 | Solo enfermería registra triaje | U-15 | I-07 |
| RN21 | Estados válidos y secuencia | U-16 | I-07 |
| RN24 | Aviso no genera cita | U-17 | — |
| RN25 | Aviso no altera orden de atención | U-17 | — |
| RN26 | Avisos solo visibles para enfermería | U-18 | — |
| RN27 | Dos turnos: mañana y tarde | U-07 | I-05 |
| RN28 | Horarios operativos por turno | U-07, U-12 | I-05 |
| RN29 | Duración configurable por especialidad | U-07 | I-05 |
| RN30 | Historial de cambios de estado | U-14 | I-03, I-07 |
| RN31 | Sin reservas duplicadas activas | U-11 | I-04 |
| RN32 | Auditoría de acciones admin | U-14 | I-07 |
| RN36 | Triaje 20 min antes del turno | U-12 | I-03 |

---

## PARTE 4 — NOTAS DE IMPLEMENTACIÓN

### Control del tiempo en pruebas (IDateTimeProvider)

Las pruebas U-12 e I-03 dependen de la hora actual del sistema para validar la ventana de cancelación (RN13, RN36). Para hacer esto testeable, el servicio debe recibir una abstracción del reloj:

```csharp
// Abstracción
public interface IDateTimeProvider
{
    DateTime Now { get; }
}

// Producción
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}

// Pruebas
public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get; set; }
}
```

Registro en `Program.cs`:
```csharp
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
```

En cada prueba que requiera control de tiempo:
```csharp
var fakeClock = new FakeDateTimeProvider { Now = new DateTime(2025, 6, 1, 7, 15, 0) };
var service = new CitaService(mockCitaRepo.Object, mockSlotRepo.Object, fakeClock);
```

---

### Orden de ejecución de pruebas de integración

Las pruebas de integración deben ser independientes entre sí. Cada test resetea la base de datos usando `Respawn` antes de insertar sus datos de semilla propios mediante `SeedDataHelper`. Nunca dependen del estado dejado por otra prueba.

---

### Nomenclatura de métodos de prueba

Convención: `MetodoBajoPrueba_Escenario_ResultadoEsperado`

```csharp
// Ejemplos
public async Task ReservarAsync_CupoAgotado_RetornaErrorSinCrearCita()
public async Task ReservarAsync_SlotDisponible_DecrementaCupoYCreaTicket()
public async Task CancelarAsync_DespuesVentanaMañana_RetornaErrorSinCambiarEstado()
public async Task RegistrarTriaje_RolPaciente_LanzaUnauthorizedException()
```

---

### Herramientas recomendadas

| Herramienta | Versión sugerida | Propósito |
|---|---|---|
| `xUnit` | 2.x | Framework de pruebas principal |
| `Moq` | 4.x | Mocking de repositorios en pruebas unitarias |
| `FluentAssertions` | 6.x | Aserciones legibles y descriptivas |
| `Respawn` | 6.x | Reset de base de datos entre pruebas de integración |
| `Microsoft.AspNetCore.Mvc.Testing` | 10.x | `WebApplicationFactory` para pruebas de controladores |
| `Bogus` | 34.x | Generación de datos de prueba realistas (opcional) |

---

*Documento generado en base a: 00_constitucion.md · 01_requisitos.md · 02_casos_uso.md · 03_reglas_negocio.md · 04_modelo_dominio.md · 05_arquitectura.md · 06_base_datos.md — Versión 1.0*
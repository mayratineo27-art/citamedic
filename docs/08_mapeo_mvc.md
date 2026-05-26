# MAPEO MVC — CONTRATO UI / BACKEND

Proyecto: Sistema Web de Gestión de Citas Médicas – Posta de Salud (PostaCitasWeb)  
Versión: 1.0  
Arquitectura: MVC — ASP.NET Core / Entity Framework Core / SQL Server  
Referencia: 04_modelo_dominio.md · 05_arquitectura.md · 02_casos_uso.md · 03_reglas_negocio.md

---

## PROPÓSITO

Este documento define el **contrato de datos entre la vista y el controlador** para cada flujo funcional del sistema. Especifica los ViewModels (DTOs de presentación) con sus atributos y anotaciones de validación, los controladores con sus métodos de acción, y la estructura de vistas Razor. Ningún ViewModel expone directamente una entidad de dominio (ver decisión arquitectónica en `05_arquitectura.md`, sección 7).

---

## PRINCIPIOS DE ESTE DOCUMENTO

- Las entidades `Models/` **nunca** se pasan a la vista. Siempre se usa un ViewModel.
- Los campos no modificables por regla de negocio (RN02) se exponen como solo lectura en el ViewModel y no se incluyen en los formularios POST.
- Los campos sensibles (`PasswordHash`, `EsSobrecupo`, `UPSId`) nunca forman parte de un ViewModel dirigido al rol `Paciente`.
- Cada ViewModel lleva las Data Annotations mínimas necesarias para que `ModelState.IsValid` capture errores antes de llegar a la capa de servicio.
- Las reglas de negocio complejas (unicidad, ventana de cancelación, etc.) se validan en `Services`, no en anotaciones.

---

## 1. VIEWMODELS

Los ViewModels residen en la carpeta `ViewModels/` organizada por módulo funcional, en coherencia con la estructura de `05_arquitectura.md`.

```
ViewModels/
├── Auth/
│   ├── LoginViewModel.cs
│   ├── SolicitudAccesoViewModel.cs
│   └── RecuperarAccesoViewModel.cs
├── Pacientes/
│   ├── PerfilPacienteViewModel.cs
│   ├── ActualizarPerfilViewModel.cs
│   └── DependienteViewModel.cs
├── Citas/
│   ├── EspecialidadItemViewModel.cs
│   ├── ConsultaDisponibilidadViewModel.cs
│   ├── SlotDisponibleViewModel.cs
│   ├── ReservaCitaViewModel.cs
│   ├── ConfirmacionReservaViewModel.cs
│   ├── TicketViewModel.cs
│   ├── CitaResumenViewModel.cs
│   └── RegistroCitaPresencialViewModel.cs
├── Disponibilidad/
│   ├── ProgramacionOperativaFormViewModel.cs
│   ├── HabilitarDisponibilidadViewModel.cs
│   └── AjusteDisponibilidadViewModel.cs
├── Triaje/
│   ├── BusquedaCitaTriajeViewModel.cs
│   ├── RegistroTriajeViewModel.cs
│   └── ActualizarEstadoCitaViewModel.cs
├── Avisos/
│   ├── EnviarAvisoViewModel.cs
│   └── AvisoItemViewModel.cs
└── Admin/
    ├── UpsFormViewModel.cs
    ├── EspecialidadFormViewModel.cs
    ├── MedicoFormViewModel.cs
    └── UsuarioAdminViewModel.cs
```

---

### 1.1 MÓDULO AUTENTICACIÓN

---

#### `LoginViewModel`

Recibe las credenciales para autenticación (CU02, RF02).

```csharp
public class LoginViewModel
{
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener solo dígitos.")]
    public string DNI { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    // No se expone: PasswordHash, Rol, Activo
}
```

---

#### `SolicitudAccesoViewModel`

Solicitud de habilitación de cuenta enviada a Admisión (CU01, RF01, RN01).

```csharp
public class SolicitudAccesoViewModel
{
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8)]
    [RegularExpression(@"^\d{8}$")]
    public string DNI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número celular es obligatorio.")]
    [StringLength(15)]
    [Phone(ErrorMessage = "Formato de celular inválido.")]
    public string Celular { get; set; } = string.Empty;

    // Admisión aprueba; el campo Activo lo gestiona el servicio (RN01)
}
```

---

#### `RecuperarAccesoViewModel`

Recuperación de contraseña mediante DNI + celular (RF03, RN02-A). Flujo de dos pasos.

```csharp
// Paso 1: validación de identidad
public class RecuperarAccesoViewModel
{
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8)]
    [RegularExpression(@"^\d{8}$")]
    public string DNI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El celular registrado es obligatorio.")]
    [Phone]
    public string CelularRegistrado { get; set; } = string.Empty;
}

// Paso 2: nueva contraseña (solo se muestra si paso 1 fue validado)
public class NuevaPasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres.")]
    public string NuevaPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmarPassword { get; set; } = string.Empty;

    // Token temporal generado por el servicio tras validar paso 1
    [Required]
    public string TokenRecuperacion { get; set; } = string.Empty;
}
```

---

### 1.2 MÓDULO PACIENTES

---

#### `PerfilPacienteViewModel`

Vista de solo lectura del perfil del paciente (RF05). Los campos inmutables según RN02 solo se muestran, nunca se envían en POST.

```csharp
public class PerfilPacienteViewModel
{
    // Campos de solo lectura — no modificables (RN02)
    public string DNI { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public DateOnly FechaNacimiento { get; set; }

    // Campos informativos
    public bool TieneSIS { get; set; }
    public string? PostaAsociada { get; set; }

    // Campo modificable expuesto para referencia
    public string Celular { get; set; } = string.Empty;
}
```

---

#### `ActualizarPerfilViewModel`

Solo expone los campos que el paciente puede modificar (RF06, RN02).

```csharp
public class ActualizarPerfilViewModel
{
    [Required(ErrorMessage = "El número celular es obligatorio.")]
    [Phone(ErrorMessage = "Formato de celular inválido.")]
    [StringLength(15)]
    public string NuevoCelular { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string? NuevaPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public string? ConfirmarPassword { get; set; }

    // DNI, Nombres y FechaNacimiento NO se incluyen (RN02)
}
```

---

#### `DependienteViewModel`

Registro y gestión de pacientes menores por responsable (RF04, RN03).

```csharp
public class DependienteViewModel
{
    [Required(ErrorMessage = "El DNI del menor es obligatorio.")]
    [StringLength(8, MinimumLength = 8)]
    [RegularExpression(@"^\d{8}$")]
    public string DNI { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ApellidoPaterno { get; set; } = string.Empty;

    [StringLength(50)]
    public string ApellidoMaterno { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateOnly FechaNacimiento { get; set; }

    // Calculado en servicio: EsMenor debe ser true (RN03)
    // ResponsableId se asigna desde el ClaimsPrincipal del usuario en sesión
}
```

---

### 1.3 MÓDULO CITAS

---

#### `ConsultaDisponibilidadViewModel`

Parámetros de búsqueda que el paciente envía para consultar cupos (CU05, RF08, RN11).

```csharp
public class ConsultaDisponibilidadViewModel
{
    [Required(ErrorMessage = "Seleccione una especialidad.")]
    public int EspecialidadId { get; set; }

    [Required(ErrorMessage = "Seleccione un turno.")]
    public Turno Turno { get; set; }

    [Required(ErrorMessage = "Seleccione una fecha.")]
    public DateOnly Fecha { get; set; }

    // Para re-poblar el select de especialidades en la vista
    public IEnumerable<EspecialidadItemViewModel> EspecialidadesDisponibles { get; set; }
        = Enumerable.Empty<EspecialidadItemViewModel>();

    // Resultados: llenados por el controlador al hacer GET con parámetros
    public IEnumerable<SlotDisponibleViewModel> Slots { get; set; }
        = Enumerable.Empty<SlotDisponibleViewModel>();
}
```

---

#### `EspecialidadItemViewModel`

Proyección mínima de Especialidad para poblar listas desplegables (RF07, RN07). No expone UPSId.

```csharp
public class EspecialidadItemViewModel
{
    public int EspecialidadId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    // UPSId y UPS.Nombre NO se exponen al paciente (RN07)
}
```

---

#### `SlotDisponibleViewModel`

Tarjeta de horario disponible que el paciente visualiza y selecciona (CU06, RF08, RF09, RN16).

```csharp
public class SlotDisponibleViewModel
{
    public int SlotId { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
    public string NombreMedico { get; set; } = string.Empty;

    // EsSobrecupo NO se expone (RN16)
    // ProgramacionId NO se expone
}
```

---

#### `ReservaCitaViewModel`

Datos enviados por POST para confirmar una reserva web (CU06, RF09, RN10, RN12).

```csharp
public class ReservaCitaViewModel
{
    [Required]
    public int SlotId { get; set; }

    // PacienteId se resuelve desde ClaimsPrincipal en el controlador;
    // no se envía desde el formulario para evitar manipulación.
    // Si la reserva es para un dependiente, se incluye:
    public int? PacienteDependienteId { get; set; }

    // Campos de confirmación para mostrar resumen antes de confirmar
    public string EspecialidadNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
}
```

---

#### `TicketViewModel`

Comprobante de reserva generado automáticamente (RF12, RN12, CU09).

```csharp
public class TicketViewModel
{
    public string Codigo { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public DateOnly FechaCita { get; set; }
    public TimeOnly HoraCita { get; set; }
    public string Turno { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string NombrePaciente { get; set; } = string.Empty;
    // OrigenReserva, CitaId no se exponen en el ticket imprimible
}
```

---

#### `CitaResumenViewModel`

Lista de citas del paciente con su estado para seguimiento (RF19, CU15, RN30).

```csharp
public class CitaResumenViewModel
{
    public int CitaId { get; set; }
    public string EspecialidadNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public DateOnly FechaCita { get; set; }
    public TimeOnly HoraCita { get; set; }
    public string EstadoCita { get; set; } = string.Empty;   // Nombre legible del enum
    public string CodigoTicket { get; set; } = string.Empty;
    public bool PuedeCancelar { get; set; }                  // Calculado por servicio (RN13, RN36)
}
```

---

#### `RegistroCitaPresencialViewModel`

Datos que Admisión ingresa para registrar una cita presencial (RF10, CU07, RN04).

```csharp
public class RegistroCitaPresencialViewModel
{
    [Required(ErrorMessage = "Ingrese el DNI del paciente.")]
    [StringLength(8, MinimumLength = 8)]
    [RegularExpression(@"^\d{8}$")]
    public string DNIPaciente { get; set; } = string.Empty;

    // Poblado por AJAX/búsqueda tras ingresar DNI
    public string? NombrePaciente { get; set; }
    public int? PacienteId { get; set; }

    [Required(ErrorMessage = "Seleccione una especialidad.")]
    public int EspecialidadId { get; set; }

    [Required(ErrorMessage = "Seleccione un turno.")]
    public Turno Turno { get; set; }

    [Required(ErrorMessage = "Seleccione una fecha.")]
    public DateOnly Fecha { get; set; }

    [Required(ErrorMessage = "Seleccione un horario.")]
    public int SlotId { get; set; }

    public IEnumerable<EspecialidadItemViewModel> Especialidades { get; set; }
        = Enumerable.Empty<EspecialidadItemViewModel>();

    public IEnumerable<SlotDisponibleViewModel> Slots { get; set; }
        = Enumerable.Empty<SlotDisponibleViewModel>();

    // OrigenReserva = Presencial lo asigna el servicio (RN04)
}
```

---

### 1.4 MÓDULO DISPONIBILIDAD

---

#### `ProgramacionOperativaFormViewModel`

Configuración de jornadas futuras por Administrador (RF15A, CU10, RN17). Admisión no accede a este form (RN06).

```csharp
public class ProgramacionOperativaFormViewModel
{
    [Required(ErrorMessage = "Seleccione una especialidad.")]
    public int EspecialidadId { get; set; }

    [Required(ErrorMessage = "Seleccione un médico.")]
    public int MedicoId { get; set; }

    [Required(ErrorMessage = "Seleccione el turno.")]
    public Turno Turno { get; set; }

    [Required(ErrorMessage = "Ingrese la fecha.")]
    public DateOnly Fecha { get; set; }

    [Required]
    [Range(1, 50, ErrorMessage = "Los cupos deben estar entre 1 y 50.")]
    public int CuposTotal { get; set; }

    [Required]
    [Range(5, 120, ErrorMessage = "La duración debe estar entre 5 y 120 minutos.")]
    public int DuracionMinutos { get; set; }

    // Listas para poblar selects
    public IEnumerable<EspecialidadItemViewModel> Especialidades { get; set; }
        = Enumerable.Empty<EspecialidadItemViewModel>();

    public IEnumerable<MedicoItemViewModel> Medicos { get; set; }
        = Enumerable.Empty<MedicoItemViewModel>();

    // Habilitada se inicializa en false; Admisión la activa por separado (RN05, RN06)
    // CreadaPorUsuarioId se resuelve desde ClaimsPrincipal
}

public class MedicoItemViewModel
{
    public int MedicoId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CMP { get; set; } = string.Empty;
}
```

---

#### `HabilitarDisponibilidadViewModel`

Admisión confirma la apertura de cupos para una jornada existente (RF15, CU11, RN05, RN06).

```csharp
public class HabilitarDisponibilidadViewModel
{
    public int ProgramacionId { get; set; }
    public string EspecialidadNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public Turno Turno { get; set; }
    public int CuposTotal { get; set; }
    public int DuracionMinutos { get; set; }
    public bool YaHabilitada { get; set; }  // Para mostrar advertencia si ya está activa

    // Admisión solo confirma; no modifica ningún campo de la programación (RN06)
}
```

---

#### `AjusteDisponibilidadViewModel`

Admisión ajusta cupos de jornadas futuras por rotación o ausencia (RF16, CU12, RN09, RN18).

```csharp
public class AjusteDisponibilidadViewModel
{
    [Required]
    public int ProgramacionId { get; set; }

    public string EspecialidadNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public Turno Turno { get; set; }

    [Required(ErrorMessage = "Seleccione el médico disponible.")]
    public int NuevoMedicoId { get; set; }

    [Required]
    [Range(0, 50)]
    public int NuevosCuposTotal { get; set; }

    [Required(ErrorMessage = "Indique el motivo del ajuste.")]
    [StringLength(200)]
    public string Motivo { get; set; } = string.Empty;

    public IEnumerable<MedicoItemViewModel> Medicos { get; set; }
        = Enumerable.Empty<MedicoItemViewModel>();

    // Solo jornadas con Fecha > hoy son modificables (RN09); validación en servicio
}
```

---

### 1.5 MÓDULO TRIAJE

---

#### `BusquedaCitaTriajeViewModel`

Búsqueda de cita activa por DNI del paciente para iniciar triaje (CU13, RF17, RN19, RN20).

```csharp
public class BusquedaCitaTriajeViewModel
{
    [Required(ErrorMessage = "Ingrese el DNI del paciente.")]
    [StringLength(8, MinimumLength = 8)]
    [RegularExpression(@"^\d{8}$")]
    public string DNIPaciente { get; set; } = string.Empty;

    // Resultados poblados por el controlador
    public IEnumerable<CitaTriajeItemViewModel> CitasEncontradas { get; set; }
        = Enumerable.Empty<CitaTriajeItemViewModel>();
}

public class CitaTriajeItemViewModel
{
    public int CitaId { get; set; }
    public string NombrePaciente { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;
    public TimeOnly HoraCita { get; set; }
    public EstadoCita EstadoActual { get; set; }
}
```

---

#### `RegistroTriajeViewModel`

Formulario de registro de signos vitales por Enfermería (RF17, RN20, RN22).

```csharp
public class RegistroTriajeViewModel
{
    [Required]
    public int CitaId { get; set; }

    // Datos de contexto (solo lectura en la vista)
    public string NombrePaciente { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;
    public TimeOnly HoraCita { get; set; }

    [Required(ErrorMessage = "El peso es obligatorio.")]
    [Range(1.0, 300.0, ErrorMessage = "Peso fuera de rango (1–300 kg).")]
    [DisplayFormat(DataFormatString = "{0:F1}")]
    public decimal Peso { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria.")]
    [Range(30.0, 250.0, ErrorMessage = "Talla fuera de rango (30–250 cm).")]
    [DisplayFormat(DataFormatString = "{0:F1}")]
    public decimal Talla { get; set; }

    [Required(ErrorMessage = "La temperatura es obligatoria.")]
    [Range(30.0, 45.0, ErrorMessage = "Temperatura fuera de rango (30–45 °C).")]
    [DisplayFormat(DataFormatString = "{0:F1}")]
    public decimal Temperatura { get; set; }

    [Required(ErrorMessage = "La presión sistólica es obligatoria.")]
    [Range(50, 300, ErrorMessage = "Presión sistólica fuera de rango.")]
    public int PresionSistolica { get; set; }

    [Required(ErrorMessage = "La presión diastólica es obligatoria.")]
    [Range(30, 200, ErrorMessage = "Presión diastólica fuera de rango.")]
    public int PresionDiastolica { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }

    // EnfermeriaUsuarioId se resuelve desde ClaimsPrincipal (RN20)
    // Al guardar, EstadoCita → EnTriaje (RN21, RN30)
}
```

---

#### `ActualizarEstadoCitaViewModel`

Actualización manual del estado de la cita por Enfermería (RF18, CU14, RN21).

```csharp
public class ActualizarEstadoCitaViewModel
{
    [Required]
    public int CitaId { get; set; }

    [Required(ErrorMessage = "Seleccione el nuevo estado.")]
    public EstadoCita NuevoEstado { get; set; }

    [StringLength(300)]
    public string? Observacion { get; set; }

    // Estados válidos expuestos al actor Enfermería: EnTriaje, ListoAtencion, NoAsistio
    // Cancelada no es opción en este form (se cancela desde el módulo de citas)
}
```

---

### 1.6 MÓDULO AVISOS

---

#### `EnviarAvisoViewModel`

El paciente registra un aviso de atención inmediata (RF20, CU16, RN24, RN25).

```csharp
public class EnviarAvisoViewModel
{
    [Required(ErrorMessage = "Describa brevemente el motivo.")]
    [StringLength(300, ErrorMessage = "Máximo 300 caracteres.")]
    public string Motivo { get; set; } = string.Empty;

    // PacienteId se resuelve desde ClaimsPrincipal
    // EstadoAviso = Pendiente lo asigna el servicio (RN26)
    // No genera Cita (RN24); validación en servicio
}
```

---

#### `AvisoItemViewModel`

Ítem del panel de avisos visible solo para Enfermería (RF21, CU17, RN26).

```csharp
public class AvisoItemViewModel
{
    public int AvisoId { get; set; }
    public string NombrePaciente { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public EstadoAviso EstadoActual { get; set; }
    public DateTime FechaEnvio { get; set; }
}
```

---

### 1.7 MÓDULO ADMINISTRACIÓN

---

#### `UpsFormViewModel`

Creación y edición de UPS (RF13, CU18). Solo Administrador.

```csharp
public class UpsFormViewModel
{
    public int? UPSId { get; set; }   // null = creación, valor = edición

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;
}
```

---

#### `EspecialidadFormViewModel`

Gestión de especialidades (RF14, CU19, RN29). Solo Administrador.

```csharp
public class EspecialidadFormViewModel
{
    public int? EspecialidadId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione una UPS.")]
    public int UPSId { get; set; }

    [Required(ErrorMessage = "La duración es obligatoria.")]
    [Range(5, 120, ErrorMessage = "La duración debe estar entre 5 y 120 minutos.")]
    public int DuracionMinutos { get; set; }

    public bool Activa { get; set; } = true;

    public IEnumerable<UpsItemViewModel> UPSDisponibles { get; set; }
        = Enumerable.Empty<UpsItemViewModel>();
}

public class UpsItemViewModel
{
    public int UPSId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
```

---

#### `UsuarioAdminViewModel`

Gestión de usuarios internos (Enfermería, Admisión) y habilitación de pacientes (RF22, CU18, RN01).

```csharp
public class UsuarioAdminViewModel
{
    public int UsuarioId { get; set; }
    public string DNI { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Celular y PasswordHash NO se exponen en este ViewModel
}
```

---

## 2. CONTROLADORES Y ACCIONES

Cada controlador corresponde a un módulo funcional definido en `00_constitucion.md` y `05_arquitectura.md`. Se indica el método HTTP, la acción, el ViewModel de entrada/salida y la vista Razor retornada.

---

### 2.1 `AuthController`

Autorización: acceso público en Login y SolicitudAcceso. Roles específicos en las demás acciones.

| Método | Acción | ViewModel entrada | Vista retornada | Descripción |
|--------|--------|-------------------|-----------------|-------------|
| GET | `Login()` | — | `Auth/Login` | Muestra formulario de login |
| POST | `Login(LoginViewModel vm)` | `LoginViewModel` | Redirect → Home o `Auth/Login` | Valida credenciales, crea cookie (CU02) |
| GET | `SolicitudAcceso()` | — | `Auth/SolicitudAcceso` | Muestra form de solicitud (CU01) |
| POST | `SolicitudAcceso(SolicitudAccesoViewModel vm)` | `SolicitudAccesoViewModel` | `Auth/SolicitudEnviada` | Registra solicitud, Admisión la aprueba (RN01) |
| GET | `RecuperarAcceso()` | — | `Auth/RecuperarAcceso` | Paso 1: validación identidad (RF03) |
| POST | `RecuperarAcceso(RecuperarAccesoViewModel vm)` | `RecuperarAccesoViewModel` | `Auth/NuevaPassword` o error | Valida DNI + celular (RN02-A) |
| POST | `NuevaPassword(NuevaPasswordViewModel vm)` | `NuevaPasswordViewModel` | Redirect → `Auth/Login` | Actualiza contraseña |
| POST | `Logout()` | — | Redirect → `Auth/Login` | Cierra sesión |

---

### 2.2 `PacientesController`

Autorización: `[Authorize(Roles = "Paciente")]`

| Método | Acción | ViewModel entrada | Vista retornada | Descripción |
|--------|--------|-------------------|-----------------|-------------|
| GET | `Perfil()` | — | `Pacientes/Perfil` | Muestra datos del paciente (RF05) |
| GET | `EditarPerfil()` | — | `Pacientes/EditarPerfil` | Form edición campos permitidos (RF06) |
| POST | `EditarPerfil(ActualizarPerfilViewModel vm)` | `ActualizarPerfilViewModel` | Redirect → `Perfil` o error | Actualiza celular/contraseña (RN02) |
| GET | `Dependientes()` | — | `Pacientes/Dependientes` | Lista menores asociados (RF04) |
| GET | `AgregarDependiente()` | — | `Pacientes/AgregarDependiente` | Form nuevo dependiente (CU03) |
| POST | `AgregarDependiente(DependienteViewModel vm)` | `DependienteViewModel` | Redirect → `Dependientes` | Registra menor (RN03) |

---

### 2.3 `CitasController`

Autorización mixta: `Paciente` para reserva web; `Admision` para presencial.

| Método | Acción | ViewModel entrada | Vista retornada | Descripción |
|--------|--------|-------------------|-----------------|-------------|
| GET | `Disponibilidad()` | — | `Citas/Disponibilidad` | Form inicial de búsqueda (CU04, CU05) |
| GET | `Disponibilidad(ConsultaDisponibilidadViewModel vm)` | `ConsultaDisponibilidadViewModel` | `Citas/Disponibilidad` | Muestra slots filtrados (RF07, RF08, RN07) |
| GET | `Reservar(int slotId)` | — | `Citas/Reservar` | Muestra resumen antes de confirmar (CU06) |
| POST | `Confirmar(ReservaCitaViewModel vm)` | `ReservaCitaViewModel` | Redirect → `Ticket` | Confirma reserva, crea ticket (RF09, RN10–RN12) |
| GET | `Ticket(int ticketId)` | — | `Citas/Ticket` | Muestra comprobante (RF12, CU09) |
| GET | `MisCitas()` | — | `Citas/MisCitas` | Lista citas y estado (CU15, RF19) |
| POST | `Cancelar(int citaId)` | — | Redirect → `MisCitas` | Cancela cita si aplica (RF11, RN13, RN36) |
| GET | `Presencial()` | — | `Citas/Presencial` | Form cita presencial — solo Admisión (CU07) |
| POST | `Presencial(RegistroCitaPresencialViewModel vm)` | `RegistroCitaPresencialViewModel` | Redirect → `Ticket` | Registra cita presencial (RF10, RN04) |
| GET | `Sobrecupo()` | — | `Citas/Sobrecupo` | Form sobrecupo — solo Admisión (RF24, RN15) |
| POST | `Sobrecupo(int slotId, int pacienteId)` | parámetros simples | Redirect → `Ticket` | Registra excepción (RN15, RN16) |

---

### 2.4 `DisponibilidadController`

Autorización: `Administrador` para Configurar; `Admision` para Habilitar y Ajustar.

| Método | Acción | ViewModel entrada | Vista retornada | Descripción |
|--------|--------|-------------------|-----------------|-------------|
| GET | `Configurar()` | — | `Disponibilidad/Configurar` | Lista programaciones — solo Admin (RF15A) |
| GET | `CrearProgramacion()` | — | `Disponibilidad/CrearProgramacion` | Form nueva programación (CU10) |
| POST | `CrearProgramacion(ProgramacionOperativaFormViewModel vm)` | `ProgramacionOperativaFormViewModel` | Redirect → `Configurar` | Crea programación (RN17, RN06) |
| GET | `Habilitar()` | — | `Disponibilidad/Habilitar` | Lista programaciones pendientes — Admisión (RF15, CU11) |
| POST | `Habilitar(int programacionId)` | parámetro simple | Redirect → `Habilitar` | Activa disponibilidad (RN05, RN06) |
| GET | `Ajustar(int programacionId)` | — | `Disponibilidad/Ajustar` | Form ajuste — Admisión (RF16, CU12) |
| POST | `Ajustar(AjusteDisponibilidadViewModel vm)` | `AjusteDisponibilidadViewModel` | Redirect → `Habilitar` | Aplica ajuste (RN09, RN18) |

---

### 2.5 `TriajeController`

Autorización: `[Authorize(Roles = "Enfermeria")]`

| Método | Acción | ViewModel entrada | Vista retornada | Descripción |
|--------|--------|-------------------|-----------------|-------------|
| GET | `Buscar()` | — | `Triaje/Buscar` | Form búsqueda por DNI (CU13) |
| GET | `Buscar(BusquedaCitaTriajeViewModel vm)` | `BusquedaCitaTriajeViewModel` | `Triaje/Buscar` | Muestra citas encontradas |
| GET | `Registrar(int citaId)` | — | `Triaje/Registrar` | Form registro de signos vitales (RF17) |
| POST | `Registrar(RegistroTriajeViewModel vm)` | `RegistroTriajeViewModel` | Redirect → `Buscar` | Guarda triaje, cambia estado (RN19–RN22, RN30) |
| POST | `ActualizarEstado(ActualizarEstadoCitaViewModel vm)` | `ActualizarEstadoCitaViewModel` | Redirect → `Buscar` | Actualiza estado (RF18, CU14, RN21) |

---

### 2.6 `AvisosController`

Autorización mixta: `Paciente` para enviar; `Enfermeria` para visualizar panel.

| Método | Acción | ViewModel entrada | Vista retornada | Descripción |
|--------|--------|-------------------|-----------------|-------------|
| GET | `Enviar()` | — | `Avisos/Enviar` | Form aviso — Paciente (RF20, CU16) |
| POST | `Enviar(EnviarAvisoViewModel vm)` | `EnviarAvisoViewModel` | `Avisos/AvisoEnviado` | Registra aviso (RN24, RN25) |
| GET | `Panel()` | — | `Avisos/Panel` | Lista avisos — solo Enfermería (RF21, CU17, RN26) |
| POST | `ActualizarAviso(int avisoId, EstadoAviso nuevoEstado)` | parámetros simples | Redirect → `Panel` | Cambia estado del aviso (RN26) |

---

### 2.7 `AdminController`

Autorización: `[Authorize(Roles = "Administrador")]`

| Método | Acción | ViewModel entrada | Vista retornada | Descripción |
|--------|--------|-------------------|-----------------|-------------|
| GET | `Ups()` | — | `Admin/Ups` | Lista UPS (RF13) |
| GET | `CrearUps()` | — | `Admin/UpsForm` | Form crear UPS |
| POST | `CrearUps(UpsFormViewModel vm)` | `UpsFormViewModel` | Redirect → `Ups` | Crea UPS |
| GET | `EditarUps(int id)` | — | `Admin/UpsForm` | Form editar UPS |
| POST | `EditarUps(UpsFormViewModel vm)` | `UpsFormViewModel` | Redirect → `Ups` | Actualiza UPS |
| GET | `Especialidades()` | — | `Admin/Especialidades` | Lista especialidades (RF14) |
| GET | `CrearEspecialidad()` | — | `Admin/EspecialidadForm` | Form crear especialidad |
| POST | `CrearEspecialidad(EspecialidadFormViewModel vm)` | `EspecialidadFormViewModel` | Redirect → `Especialidades` | Crea especialidad (RN29) |
| GET | `EditarEspecialidad(int id)` | — | `Admin/EspecialidadForm` | Form editar especialidad |
| POST | `EditarEspecialidad(EspecialidadFormViewModel vm)` | `EspecialidadFormViewModel` | Redirect → `Especialidades` | Actualiza especialidad |
| GET | `Usuarios()` | — | `Admin/Usuarios` | Lista usuarios (RF22) |
| POST | `HabilitarUsuario(int usuarioId)` | parámetro simple | Redirect → `Usuarios` | Activa cuenta (RN01) |
| POST | `DeshabilitarUsuario(int usuarioId)` | parámetro simple | Redirect → `Usuarios` | Desactiva cuenta |

---

## 3. ESTRUCTURA DE VISTAS RAZOR

Organización de archivos `.cshtml` por carpeta de controlador, con indicación del `@model` esperado.

```
Views/
│
├── Auth/
│   ├── Login.cshtml                   @model LoginViewModel
│   ├── SolicitudAcceso.cshtml         @model SolicitudAccesoViewModel
│   ├── SolicitudEnviada.cshtml        — (confirmación sin modelo)
│   ├── RecuperarAcceso.cshtml         @model RecuperarAccesoViewModel
│   └── NuevaPassword.cshtml           @model NuevaPasswordViewModel
│
├── Pacientes/
│   ├── Perfil.cshtml                  @model PerfilPacienteViewModel
│   ├── EditarPerfil.cshtml            @model ActualizarPerfilViewModel
│   ├── Dependientes.cshtml            @model IEnumerable<DependienteViewModel>
│   └── AgregarDependiente.cshtml      @model DependienteViewModel
│
├── Citas/
│   ├── Disponibilidad.cshtml          @model ConsultaDisponibilidadViewModel
│   ├── Reservar.cshtml                @model ReservaCitaViewModel
│   ├── Ticket.cshtml                  @model TicketViewModel
│   ├── MisCitas.cshtml                @model IEnumerable<CitaResumenViewModel>
│   ├── Presencial.cshtml              @model RegistroCitaPresencialViewModel
│   └── Sobrecupo.cshtml               — (form simple, parámetros directos)
│
├── Disponibilidad/
│   ├── Configurar.cshtml              @model IEnumerable<HabilitarDisponibilidadViewModel>
│   ├── CrearProgramacion.cshtml       @model ProgramacionOperativaFormViewModel
│   ├── Habilitar.cshtml               @model IEnumerable<HabilitarDisponibilidadViewModel>
│   └── Ajustar.cshtml                 @model AjusteDisponibilidadViewModel
│
├── Triaje/
│   ├── Buscar.cshtml                  @model BusquedaCitaTriajeViewModel
│   ├── Registrar.cshtml               @model RegistroTriajeViewModel
│   └── _ActualizarEstado.cshtml       @model ActualizarEstadoCitaViewModel  (partial)
│
├── Avisos/
│   ├── Enviar.cshtml                  @model EnviarAvisoViewModel
│   ├── AvisoEnviado.cshtml            — (confirmación sin modelo)
│   └── Panel.cshtml                   @model IEnumerable<AvisoItemViewModel>
│
├── Admin/
│   ├── Ups.cshtml                     @model IEnumerable<UpsFormViewModel>
│   ├── UpsForm.cshtml                 @model UpsFormViewModel
│   ├── Especialidades.cshtml          @model IEnumerable<EspecialidadFormViewModel>
│   ├── EspecialidadForm.cshtml        @model EspecialidadFormViewModel
│   ├── Usuarios.cshtml                @model IEnumerable<UsuarioAdminViewModel>
│   └── _HabilitarUsuario.cshtml       — (partial dentro de tabla)
│
└── Shared/
    ├── _Layout.cshtml                 — Layout principal por rol
    ├── _LayoutAdmin.cshtml            — Layout para panel administrativo
    ├── _ValidationScriptsPartial.cshtml
    ├── _NavPaciente.cshtml            — Partial: barra de navegación Paciente
    ├── _NavAdmision.cshtml            — Partial: barra de navegación Admisión
    ├── _NavEnfermeria.cshtml          — Partial: barra de navegación Enfermería
    ├── _NavAdmin.cshtml               — Partial: barra de navegación Administrador
    ├── Error.cshtml                   — Vista de error general
    └── Denegado.cshtml                — Acceso denegado (403)
```

---

## 4. FLUJOS DE NAVEGACIÓN

Se describen los flujos principales indicando el recorrido de vistas que completa el usuario en cada caso de uso.

---

### Flujo 1 — Reserva de cita web (CU04 → CU05 → CU06 → CU09)

Actor: Paciente

```
[Auth/Login]
    ↓  POST credenciales válidas
[Citas/Disponibilidad]  (GET — selección de especialidad y turno)
    ↓  GET con ConsultaDisponibilidadViewModel completado
[Citas/Disponibilidad]  (vista con tarjetas de slots)
    ↓  Paciente selecciona SlotId → GET /Citas/Reservar?slotId=X
[Citas/Reservar]        (muestra resumen: médico, hora, fecha)
    ↓  POST ReservaCitaViewModel
[Citas/Ticket]          (muestra código, especialidad, médico, hora)
```

Reglas aplicadas en el flujo: RN04, RN07, RN10, RN11, RN12, RN16, RN31.

---

### Flujo 2 — Registro de cita presencial (CU07)

Actor: Admisión

```
[Auth/Login]
    ↓
[Citas/Presencial]  (GET — busca paciente por DNI)
    ↓  DNI ingresado → AJAX busca nombre y PacienteId
[Citas/Presencial]  (selección especialidad, turno, fecha, slot)
    ↓  POST RegistroCitaPresencialViewModel
[Citas/Ticket]      (ticket generado para entregar al paciente)
```

Reglas aplicadas: RN04, RN06, RN12.

---

### Flujo 3 — Configuración y habilitación de disponibilidad (CU10 → CU11)

Actor: Administrador configura / Admisión habilita

```
[Admin → Disponibilidad/Configurar]
    ↓  Admin: GET CrearProgramacion
[Disponibilidad/CrearProgramacion]
    ↓  POST ProgramacionOperativaFormViewModel
[Disponibilidad/Configurar]   (lista actualizada; Habilitada = false)

--- Acción posterior de Admisión ---

[Disponibilidad/Habilitar]
    ↓  Admisión ve lista de programaciones no habilitadas
[Disponibilidad/Habilitar]
    ↓  POST /Disponibilidad/Habilitar/{programacionId}
[Disponibilidad/Habilitar]    (lista actualizada; cupos ahora visibles para pacientes)
```

Reglas aplicadas: RN05, RN06, RN08, RN09, RN17.

---

### Flujo 4 — Triaje y actualización de estado (CU13 → CU14)

Actor: Enfermería

```
[Auth/Login]
    ↓
[Triaje/Buscar]         (GET — form búsqueda por DNI)
    ↓  GET con DNI → lista de citas encontradas
[Triaje/Buscar]         (muestra CitaTriajeItemViewModel)
    ↓  Selecciona cita → GET /Triaje/Registrar?citaId=X
[Triaje/Registrar]      (form RegistroTriajeViewModel)
    ↓  POST datos de signos vitales
[Triaje/Buscar]         (estado de cita → EnTriaje automáticamente)
    ↓  POST ActualizarEstadoCitaViewModel (ListoAtencion / NoAsistio)
[Triaje/Buscar]         (estado actualizado, trazado en HistorialEstadoCita)
```

Reglas aplicadas: RN19, RN20, RN21, RN22, RN30, RN36.

---

### Flujo 5 — Cancelación de cita (CU08)

Actor: Paciente

```
[Citas/MisCitas]   (lista CitaResumenViewModel con PuedeCancelar calculado)
    ↓  Si PuedeCancelar = true → POST /Citas/Cancelar/{citaId}
[Citas/MisCitas]   (estado actualizado → Cancelada; cupo devuelto)
```

Reglas aplicadas: RN13, RN14, RN36.

---

### Flujo 6 — Aviso de atención inmediata (CU16 → CU17)

Actor: Paciente envía / Enfermería visualiza

```
[Avisos/Enviar]         (GET — Paciente)
    ↓  POST EnviarAvisoViewModel
[Avisos/AvisoEnviado]   (confirmación sin modelo)

--- Panel Enfermería (independiente) ---

[Avisos/Panel]          (GET — lista AvisoItemViewModel; EstadoAviso = Pendiente)
    ↓  POST /Avisos/ActualizarAviso → estado Visualizado → Cerrado
[Avisos/Panel]          (lista actualizada)
```

Reglas aplicadas: RN24, RN25, RN26.

---

## 5. CONVENCIONES DE VIEWMODELS

| Convención | Criterio |
|------------|----------|
| Nombre del ViewModel | `{Acción}{Entidad}ViewModel` — ejemplo: `ReservaCitaViewModel`, `RegistroTriajeViewModel` |
| Campos no modificables (RN02) | Incluidos solo como `{ get; set; }` en el ViewModel de lectura; ausentes en el ViewModel de escritura |
| PacienteId / UsuarioId | Nunca proveniente del formulario HTML; siempre resuelto desde `User.FindFirst(ClaimTypes.NameIdentifier)` en el controlador |
| Campos sensibles | `PasswordHash`, `EsSobrecupo`, `UPSId` (para Paciente) nunca en ViewModels expuestos al cliente |
| Listas de selección | Propiedades `IEnumerable<XxxItemViewModel>` incluidas en el mismo ViewModel para poblar `<select>` sin ViewBag |
| Validación de reglas complejas | Solo `[Required]`, `[Range]`, `[StringLength]`, `[RegularExpression]` y `[Compare]` en anotaciones; lógica de negocio (duplicidad, ventana de cancelación) en `Services` |
| Enums en la vista | Se exponen como `enum` en el ViewModel; la vista usa `Html.GetEnumSelectList<T>()` o `@Html.DisplayFor()` |

---

*Documento generado en base a: 00_constitucion.md · 01_requisitos.md · 02_casos_uso.md · 03_reglas_negocio.md · 04_modelo_dominio.md · 05_arquitectura.md · 07_pruebas.md — Versión 1.0*
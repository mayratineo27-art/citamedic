# Requirements Document
## Sistema Web de Gestión de Citas Médicas — PostaCitasWeb — Iteración 2

**Proyecto:** PostaCitasWeb — Posta Los Licenciados  
**Versión:** 2.0  
**Fecha:** 2025  
**Tipo:** Feature — Requirements-First  
**Tecnología:** ASP.NET Core MVC (.NET 10), SQL Server, Entity Framework Core

---

## Introduction

La Iteración 2 refina el alcance funcional del sistema PostaCitasWeb, corrige problemas arquitecturales identificados en la Iteración 1 y cierra el conjunto de actores a tres roles operativos: **Paciente**, **Admisión** y **Administrador**. Se eliminan los módulos de Enfermería (triaje digital, Kanban, avisos) y el rol Médico. Se introduce la tabla `TutorDependiente` para gestionar vínculos tutor-menor de forma explícita, reemplazando la auto-referencia `Paciente.ResponsableId`. Se añaden flujos de activación/recuperación de cuenta sin dependencia de email ni SMS, y se refuerza la cobertura de pruebas con propiedades de corrección ejecutables sobre funciones puras de dominio.

---

## Glossary

- **Sistema**: PostaCitasWeb — aplicación ASP.NET Core MVC objeto de esta especificación.
- **Paciente**: persona registrada en el padrón de la posta que puede activar cuenta, reservar citas y gestionar dependientes.
- **Admisión**: personal de ventanilla con acceso al panel de admisión.
- **Administrador**: usuario con privilegios de configuración total del sistema.
- **Tutor**: paciente adulto registrado en `TutorDependiente` como responsable de un dependiente.
- **Dependiente**: paciente menor de edad vinculado a un Tutor en `TutorDependiente`.
- **TutorDependiente**: tabla de vínculo explícito entre Tutor y Dependiente (reemplaza `Paciente.ResponsableId`).
- **UPS**: Unidad Prestadora de Servicios — unidad organizativa interna, no visible para el Paciente.
- **Especialidad**: servicio médico visible para el Paciente, asociado a una UPS.
- **ProgramacionOperativa**: plantilla de disponibilidad futura configurada por el Administrador.
- **SlotDisponible**: cupo horario individual generado a partir de una ProgramacionOperativa.
- **Cita**: reserva de un Paciente en un SlotDisponible, originada vía web o presencial.
- **Ticket**: comprobante generado automáticamente al confirmar una Cita.
- **HistorialEstadoCita**: registro de auditoría de cada cambio de estado de una Cita.
- **Turno**: período de atención — `Mañana` (08:00–13:30) o `Tarde` (15:00–19:00).
- **VentanaReserva**: rango de días en que el Paciente puede reservar (Lun–Vie: mismo día; Sáb: mismo día y próximo lunes; Dom: bloqueado).
- **VentanaCancelacion**: límite horario hasta el que el Paciente puede cancelar (07:40 turno Mañana; 14:40 turno Tarde).
- **Activador**: flujo `AuthController.ActivarCuenta` que detecta si el Paciente activa cuenta nueva o recupera contraseña existente.
- **Validator**: componente de servicio que valida datos de activación/recuperación contra el padrón.
- **TicketGenerator**: componente de servicio que genera el código único de Ticket.
- **SlotGenerator**: componente de servicio que genera SlotDisponibles a partir de una ProgramacionOperativa.
- **CancelacionService**: componente de servicio que evalúa y ejecuta cancelaciones de Cita.
- **ReservaService**: componente de servicio que ejecuta la reserva de Cita y genera el Ticket en la misma transacción.
- **SesionDelegada**: mecanismo por el que el Tutor opera en nombre de un Dependiente dentro de la misma sesión autenticada.
- **BCrypt**: algoritmo de hash de contraseñas utilizado en el seed y en el registro de usuarios.
- **Padrón**: base de datos de pacientes registrados en la posta, fuente de verdad para activación de cuentas.

---

## Cambios de Dominio respecto a Iteración 1

Los siguientes cambios estructurales son prerequisito de todos los requisitos de esta iteración:

| Cambio | Descripción |
|--------|-------------|
| Nueva tabla `TutorDependiente` | `TutorId` FK→Paciente, `DependienteId` FK→Paciente, `FechaRegistro` |
| Eliminar `Paciente.ResponsableId` | Se reemplaza por `TutorDependiente` |
| Eliminar `Paciente.PostaAsociadaId` | FK huérfana sin uso operativo |
| Índice único `Ticket.CitaId` | Refuerza relación 1:1 en BD |
| Índice único `Triaje.CitaId` | Refuerza relación 1:1 en BD |
| Seed con contraseñas BCrypt | Reemplaza contraseñas en texto plano |
| Desactivar `TriajeController` | Fuera del alcance de Iteración 2 |
| Desactivar `EnfermeriaController` | Fuera del alcance de Iteración 2 |
| Nuevo flujo `AuthController.ActivarCuenta` | Activación y recuperación unificadas |
| Nuevo `RF_RST` en `AdmisionController` | Reset de contraseña desde ventanilla |

---

## Requirements

---

### Requisito 1 — Activación de Cuenta y Recuperación de Contraseña (RF_ACT)

**Historia de usuario:** Como Paciente registrado en el padrón de la posta, quiero activar mi cuenta o recuperar mi contraseña usando mis datos personales, para acceder al sistema sin depender de email ni SMS.

#### Criterios de Aceptación

1. WHEN el Paciente envía DNI, fecha de nacimiento y nombres en el formulario de activación, THE Validator SHALL verificar que los datos coincidan con un registro existente en el Padrón.
2. IF los datos enviados no coinciden con ningún registro del Padrón, THEN THE Sistema SHALL mostrar el mensaje "Datos no encontrados en el padrón de la posta" sin revelar si el DNI existe.
3. WHEN el Validator confirma que los datos coinciden y el Usuario asociado tiene `Activo = false`, THE Sistema SHALL activar la cuenta estableciendo `Activo = true` y solicitando al Paciente que defina su contraseña.
4. WHEN el Validator confirma que los datos coinciden y el Usuario asociado tiene `Activo = true`, THE Sistema SHALL iniciar el flujo de recuperación de contraseña solicitando al Paciente que defina una nueva contraseña.
5. THE Sistema SHALL detectar automáticamente si el flujo es activación o recuperación sin requerir que el Paciente seleccione la opción manualmente.
6. WHEN el Paciente define una nueva contraseña, THE Sistema SHALL almacenarla usando BCrypt antes de confirmar la operación.
7. IF el Dependiente intenta activar cuenta directamente, THEN THE Sistema SHALL rechazar la operación con el mensaje "Los menores de edad no pueden activar cuenta directamente".
8. THE Validator SHALL exponer la función pura `ValidarDatosActivacion(dni, fechaNac, nombres, paciente) → bool` con cobertura de prueba del 100%.
9. THE Sistema SHALL mostrar en la pantalla de inicio de sesión dos enlaces independientes debajo del formulario de credenciales: "Activar cuenta" y "¿Olvidaste tu contraseña?", ambos dirigiendo a `AuthController.ActivarCuenta`.
10. THE Sistema SHALL presentar el mismo formulario `AuthController.ActivarCuenta` independientemente del enlace que el Paciente haya utilizado para acceder, siendo el campo `Activo` del Usuario el único determinante del flujo ejecutado (criterios 3 y 4).

---

### Requisito 2 — Inicio de Sesión (RF01)

**Historia de usuario:** Como usuario del sistema (Paciente, Admisión o Administrador), quiero ingresar mis credenciales, para acceder a las funciones correspondientes a mi rol.

#### Criterios de Aceptación

1. WHEN el usuario envía credenciales válidas, THE Sistema SHALL autenticar la sesión y redirigir al panel correspondiente al rol del usuario.
2. IF las credenciales son incorrectas, THEN THE Sistema SHALL mostrar el mensaje "Credenciales inválidas" sin especificar cuál campo es incorrecto.
3. IF el Usuario tiene `Activo = false`, THEN THE Sistema SHALL rechazar el acceso con el mensaje "Cuenta no habilitada. Contacte a Admisión".
4. WHILE el usuario tiene sesión activa, THE Sistema SHALL mantener la identidad y el rol en la sesión hasta que se ejecute el cierre de sesión.

---

### Requisito 3 — Cierre de Sesión (RF02)

**Historia de usuario:** Como usuario autenticado, quiero cerrar mi sesión, para proteger mi cuenta al terminar de usar el sistema.

#### Criterios de Aceptación

1. WHEN el usuario ejecuta el cierre de sesión, THE Sistema SHALL invalidar la sesión activa y redirigir a la página de inicio de sesión.
2. WHEN la SesionDelegada está activa y el Tutor ejecuta el cierre de sesión, THE Sistema SHALL invalidar tanto la sesión delegada como la sesión principal del Tutor.

---

### Requisito 4 — Actualización de Datos de Perfil (RF05)

**Historia de usuario:** Como Paciente, quiero actualizar mi número de celular y contraseña desde mi perfil, para mantener mis datos de contacto vigentes.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir al Paciente modificar únicamente el número de celular y la contraseña desde la vista de perfil.
2. IF el Paciente intenta modificar DNI, nombres o fecha de nacimiento, THEN THE Sistema SHALL rechazar la operación y mostrar el mensaje "Este campo no puede ser modificado".
3. WHEN el Paciente actualiza la contraseña, THE Sistema SHALL almacenarla usando BCrypt antes de confirmar el cambio.
4. WHEN el Paciente actualiza el número de celular, THE Sistema SHALL validar que el valor tenga entre 9 y 15 dígitos numéricos antes de guardar.

---

### Requisito 5 — Reset de Contraseña por Admisión (RF_RST)

**Historia de usuario:** Como personal de Admisión, quiero resetear la contraseña de un Paciente desde ventanilla, para asistir a pacientes que no pueden recuperar acceso por sí mismos.

#### Criterios de Aceptación

1. WHEN Admisión busca un Paciente por DNI y ejecuta el reset de contraseña, THE Sistema SHALL generar una contraseña temporal y almacenarla usando BCrypt.
2. THE Sistema SHALL mostrar la contraseña temporal a Admisión en pantalla para que pueda comunicarla al Paciente presencialmente.
3. THE Sistema SHALL registrar en `HistorialEstadoCita` — o en la tabla de auditoría correspondiente — el usuario de Admisión que ejecutó el reset y la fecha/hora de la operación.
4. IF el DNI ingresado no corresponde a ningún Paciente registrado, THEN THE Sistema SHALL mostrar el mensaje "Paciente no encontrado".

---

### Requisito 6 — Consulta de Perfil del Paciente (RF04)

**Historia de usuario:** Como Paciente, quiero consultar mi perfil, para verificar mis datos personales registrados en el sistema.

#### Criterios de Aceptación

1. WHEN el Paciente accede a la vista de perfil, THE Sistema SHALL mostrar: nombres completos, DNI, número de historia clínica y estado de afiliación SIS.
2. THE Sistema SHALL mostrar los campos DNI, nombres y fecha de nacimiento como solo lectura, sin controles de edición.
3. WHILE el Paciente opera en SesionDelegada sobre un Dependiente, THE Sistema SHALL mostrar el perfil del Dependiente, no el del Tutor.

---

### Requisito 7 — Sesión Delegada Tutor–Dependiente (RF36)

**Historia de usuario:** Como Tutor, quiero cambiar mi sesión al perfil de un Dependiente registrado, para gestionar sus citas sin necesidad de credenciales separadas.

#### Criterios de Aceptación

1. WHEN el Tutor selecciona un Dependiente de su lista, THE Sistema SHALL verificar la existencia del vínculo en `TutorDependiente` antes de activar la SesionDelegada.
2. IF el vínculo no existe en `TutorDependiente`, THEN THE Sistema SHALL rechazar el cambio de sesión con el mensaje "No tiene vínculo registrado con este paciente".
3. WHILE la SesionDelegada está activa, THE Sistema SHALL ejecutar todas las operaciones de reserva, cancelación y consulta en nombre del Dependiente.
4. WHEN el Tutor ejecuta "Volver a mi perfil", THE Sistema SHALL restaurar la sesión original del Tutor sin requerir nueva autenticación.
5. THE Sistema SHALL mostrar de forma visible en la interfaz que la sesión activa corresponde a un Dependiente, indicando el nombre del Dependiente.
6. IF el Dependiente intenta activar SesionDelegada sobre otro paciente, THEN THE Sistema SHALL rechazar la operación.

---

### Requisito 8 — Consulta de Especialidades Disponibles (RF07)

**Historia de usuario:** Como Paciente, quiero consultar las especialidades disponibles, para iniciar el proceso de reserva de cita.

#### Criterios de Aceptación

1. WHEN el Paciente accede al módulo de citas, THE Sistema SHALL mostrar únicamente las especialidades con `Activa = true` y con al menos un SlotDisponible con `CuposDisponibles > 0` dentro de la VentanaReserva vigente.
2. THE Sistema SHALL ocultar la UPS asociada a cada especialidad en todas las vistas del Paciente.
3. IF no existen especialidades con disponibilidad en la VentanaReserva vigente, THEN THE Sistema SHALL mostrar el mensaje "No hay especialidades con disponibilidad para hoy".
4. WHEN el día de la semana es domingo, THE Sistema SHALL bloquear el acceso al flujo de reserva y mostrar el mensaje "Las reservas web no están disponibles los domingos".

---

### Requisito 9 — Consulta de Disponibilidad por Especialidad y Turno (RF08)

**Historia de usuario:** Como Paciente, quiero consultar los turnos y horarios disponibles para una especialidad, para elegir el slot que mejor se adapte a mi disponibilidad.

#### Criterios de Aceptación

1. WHEN el Paciente selecciona una especialidad, THE Sistema SHALL mostrar los turnos disponibles (Mañana y/o Tarde) con sus SlotDisponibles que tengan `CuposDisponibles > 0`.
2. THE Sistema SHALL mostrar el turno Mañana con horario 08:00–13:30 y el turno Tarde con horario 15:00–19:00.
3. THE Sistema SHALL ocultar los SlotDisponibles con `EsSobrecupo = true` en todas las vistas del Paciente.
4. IF un turno no tiene SlotDisponibles con `CuposDisponibles > 0`, THEN THE Sistema SHALL mostrar ese turno como "Sin disponibilidad" sin ocultarlo completamente.
5. WHILE el Paciente consulta disponibilidad, THE Sistema SHALL mostrar únicamente slots dentro de la VentanaReserva vigente según el día de la semana actual.

---

### Requisito 10 — Reserva de Cita Web (RF09)

**Historia de usuario:** Como Paciente, quiero reservar una cita médica siguiendo un flujo guiado, para asegurar mi atención en la especialidad y horario elegidos.

#### Criterios de Aceptación

1. THE Sistema SHALL implementar el flujo de reserva en el orden: Especialidad → Turno → Slot → Confirmación, sin permitir saltar pasos.
2. WHEN el Paciente confirma la reserva, THE ReservaService SHALL decrementar `SlotDisponible.CuposDisponibles` en 1 y crear la Cita en la misma transacción de base de datos.
3. WHEN la Cita es creada exitosamente, THE TicketGenerator SHALL generar el Ticket en la misma transacción, antes de confirmar al Paciente.
4. IF el Paciente ya tiene una Cita activa (estado `Pendiente`) para la misma fecha, THEN THE Sistema SHALL rechazar la reserva con el mensaje "Ya tiene una cita activa para esta fecha".
5. IF `SlotDisponible.CuposDisponibles` es 0 al momento de confirmar, THEN THE Sistema SHALL rechazar la reserva con el mensaje "El cupo seleccionado ya no está disponible. Por favor elija otro horario".
6. WHEN el día de la semana es sábado, THE Sistema SHALL permitir reservar únicamente slots del día sábado actual y del próximo lunes.
7. WHEN el día de la semana es domingo, THE Sistema SHALL bloquear el flujo de reserva completo.
8. THE Sistema SHALL registrar el origen de la Cita como `OrigenReserva.Web`.
9. THE Sistema SHALL registrar el primer cambio de estado en `HistorialEstadoCita` con estado `Pendiente` al crear la Cita.
10. THE SlotGenerator SHALL exponer la función pura `GenerarSlots(horaInicioTurno, duracionMinutos, cuposTotal) → IEnumerable<SlotDto>` con cobertura de prueba del 100%.
11. THE TicketGenerator SHALL exponer la función pura `GenerarCodigoTicket(fecha) → string` con cobertura de prueba del 100%.

---

### Requisito 11 — Registro de Cita Presencial por Admisión (RF10)

**Historia de usuario:** Como personal de Admisión, quiero registrar citas de pacientes que se presentan en ventanilla, para integrar la atención presencial al mismo stock de cupos que la reserva web.

#### Criterios de Aceptación

1. WHEN Admisión ingresa el DNI de un Paciente, THE Sistema SHALL precargar automáticamente los datos del Paciente desde la base de datos si el DNI existe.
2. WHEN Admisión confirma el registro de la cita presencial, THE ReservaService SHALL decrementar `SlotDisponible.CuposDisponibles` en 1 y crear la Cita en la misma transacción de base de datos.
3. WHEN la Cita presencial es creada, THE TicketGenerator SHALL generar el Ticket en la misma transacción.
4. THE Sistema SHALL registrar el origen de la Cita como `OrigenReserva.Presencial` y el `RegistradaPorUsuarioId` con el identificador del usuario de Admisión.
5. IF el Paciente ya tiene una Cita activa (estado `Pendiente`) para la misma fecha, THEN THE Sistema SHALL mostrar una advertencia a Admisión y requerir confirmación explícita antes de proceder.
6. IF `SlotDisponible.CuposDisponibles` es 0, THEN THE Sistema SHALL permitir a Admisión registrar la cita como sobrecupo, estableciendo `EsSobrecupo = true`.

---

### Requisito 12 — Cancelación de Cita por Paciente (RF11-P)

**Historia de usuario:** Como Paciente, quiero cancelar una cita activa dentro del horario permitido, para liberar el cupo y permitir que otro paciente lo use.

#### Criterios de Aceptación

1. WHEN el Paciente solicita cancelar una Cita con estado `Pendiente`, THE CancelacionService SHALL verificar que la hora actual sea anterior a la VentanaCancelacion del turno correspondiente.
2. IF la hora actual es igual o posterior a la VentanaCancelacion del turno, THEN THE Sistema SHALL rechazar la cancelación con el mensaje "El plazo para cancelar esta cita ha vencido (turno Mañana: hasta 07:40 / turno Tarde: hasta 14:40)".
3. WHEN la cancelación es aprobada, THE CancelacionService SHALL cambiar el estado de la Cita a `Cancelada` e incrementar `SlotDisponible.CuposDisponibles` en 1 en la misma transacción.
4. WHEN la cancelación es ejecutada, THE Sistema SHALL registrar el cambio de estado en `HistorialEstadoCita` con el identificador del Paciente y la fecha/hora.
5. THE CancelacionService SHALL exponer la función pura `ObtenerHoraLimiteCancelacion(turno) → TimeOnly` con cobertura de prueba del 100%.
6. THE CancelacionService SHALL exponer la función pura `EsDentroDeVentanaCancelacion(turno, horaActual, esAdmision) → bool` con cobertura de prueba del 100%.

---

### Requisito 13 — Cancelación de Cita por Admisión (RF11-A)

**Historia de usuario:** Como personal de Admisión, quiero cancelar cualquier cita activa sin restricción horaria, para gestionar situaciones operativas que el Paciente no puede resolver por sí mismo.

#### Criterios de Aceptación

1. WHEN Admisión cancela una Cita con estado `Pendiente` o `EnTriaje`, THE CancelacionService SHALL cambiar el estado a `Cancelada` e incrementar `SlotDisponible.CuposDisponibles` en 1 en la misma transacción.
2. THE Sistema SHALL permitir a Admisión cancelar citas sin verificar la VentanaCancelacion horaria.
3. WHEN Admisión ejecuta la cancelación, THE Sistema SHALL registrar el cambio de estado en `HistorialEstadoCita` con el identificador del usuario de Admisión y la fecha/hora.
4. THE CancelacionService SHALL evaluar `EsDentroDeVentanaCancelacion(turno, horaActual, esAdmision: true) → true` para cualquier hora cuando `esAdmision` es `true`.

---

### Requisito 14 — Generación Automática de Ticket (RF12)

**Historia de usuario:** Como sistema, quiero generar un ticket automáticamente al confirmar cualquier reserva, para garantizar que el Paciente tenga un comprobante formal de su cita.

#### Criterios de Aceptación

1. WHEN una Cita es confirmada (web o presencial), THE TicketGenerator SHALL generar un Ticket con código único en la misma transacción de base de datos que crea la Cita.
2. THE Ticket SHALL contener: código único, especialidad, nombre del médico, fecha de la cita, hora de inicio del slot y turno.
3. THE Sistema SHALL garantizar que exista exactamente un Ticket por Cita mediante el índice único en `Ticket.CitaId`.
4. IF la generación del Ticket falla, THEN THE Sistema SHALL revertir la transacción completa y mostrar el mensaje "Error al confirmar la reserva. Intente nuevamente".
5. THE TicketGenerator SHALL exponer la función pura `GenerarCodigoTicket(fecha) → string` que produzca códigos únicos y deterministas para la misma fecha de entrada, con cobertura de prueba del 100%.
6. FOR ALL fechas válidas de entrada, `GenerarCodigoTicket(fecha)` SHALL producir un código con formato `TKT-YYYYMMDD-NNNN` donde NNNN es un secuencial de 4 dígitos (propiedad de formato verificable por PBT).

---

### Requisito 15 — Consulta del Estado de Cita por Paciente (RF19)

**Historia de usuario:** Como Paciente, quiero consultar el estado actual de mis citas, para hacer seguimiento a mi atención médica.

#### Criterios de Aceptación

1. WHEN el Paciente accede a la vista de mis citas, THE Sistema SHALL mostrar todas las Citas del Paciente con su estado actual, especialidad, médico, fecha y turno.
2. THE Sistema SHALL mostrar el historial de cambios de estado de cada Cita ordenado cronológicamente de más reciente a más antiguo.
3. WHILE el Paciente opera en SesionDelegada, THE Sistema SHALL mostrar las citas del Dependiente activo, no las del Tutor.
4. THE Sistema SHALL mostrar el Ticket asociado a cada Cita confirmada con opción de visualización para impresión.

---

### Requisito 16 — Gestión de UPS (RF13)

**Historia de usuario:** Como Administrador, quiero gestionar las Unidades Prestadoras de Servicios, para estructurar la oferta de servicios de la posta.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir al Administrador crear, editar y desactivar UPS desde el panel de administración.
2. WHEN el Administrador desactiva una UPS, THE Sistema SHALL desactivar también todas las Especialidades asociadas a esa UPS.
3. IF una UPS tiene Especialidades con ProgramacionesOperativas futuras habilitadas, THEN THE Sistema SHALL advertir al Administrador antes de permitir la desactivación.
4. THE Sistema SHALL ocultar las UPS en todas las vistas accesibles por el Paciente.

---

### Requisito 17 — Gestión de Especialidades (RF14)

**Historia de usuario:** Como Administrador, quiero configurar las especialidades médicas, para definir los servicios disponibles para los pacientes.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir al Administrador crear, editar y desactivar Especialidades, incluyendo nombre, UPS asociada y duración de atención en minutos.
2. WHEN el Administrador modifica la duración de atención de una Especialidad, THE Sistema SHALL aplicar el nuevo valor únicamente a las ProgramacionesOperativas futuras.
3. IF el Administrador intenta desactivar una Especialidad con Citas activas (estado `Pendiente`) en fechas futuras, THEN THE Sistema SHALL rechazar la operación con el mensaje "Existen citas activas para esta especialidad".
4. THE Sistema SHALL validar que `DuracionMinutos` sea un valor entero positivo mayor a 0.

---

### Requisito 18 — Configuración de Programación Operativa (RF15A)

**Historia de usuario:** Como Administrador, quiero configurar la programación operativa futura, para establecer la oferta base de turnos médicos.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir al Administrador crear ProgramacionesOperativas especificando: Especialidad, Médico, Turno (Mañana/Tarde), Fecha, CuposTotal y DuracionMinutos.
2. WHEN el Administrador crea una ProgramacionOperativa, THE SlotGenerator SHALL generar automáticamente los SlotDisponibles dividiendo el turno según la duración configurada.
3. THE Sistema SHALL rechazar la creación de ProgramacionesOperativas con fecha igual o anterior a la fecha actual del servidor.
4. IF ya existe una ProgramacionOperativa para la misma Especialidad, Médico, Turno y Fecha, THEN THE Sistema SHALL rechazar la creación con el mensaje "Ya existe una programación para esta combinación".
5. THE Sistema SHALL registrar `CreadaPorUsuarioId` con el identificador del Administrador y `FechaCreacion` con la fecha/hora del servidor al crear cada ProgramacionOperativa.
6. THE SlotGenerator SHALL exponer la función pura `GenerarSlots(horaInicioTurno, duracionMinutos, cuposTotal) → IEnumerable<SlotDto>` con cobertura de prueba del 100%.
7. FOR ALL combinaciones válidas de `horaInicioTurno`, `duracionMinutos` y `cuposTotal`, la suma de `CuposTotal` de todos los SlotDtos generados SHALL ser igual a `cuposTotal` (propiedad de invariante verificable por PBT).

---

### Requisito 19 — Habilitación de Disponibilidad por Admisión (RF15)

**Historia de usuario:** Como personal de Admisión, quiero habilitar la disponibilidad configurada por el Administrador, para abrir los cupos al público web y presencial antes de la jornada.

#### Criterios de Aceptación

1. WHEN Admisión selecciona una ProgramacionOperativa con `Habilitada = false` y la habilita, THE Sistema SHALL establecer `Habilitada = true` y exponer los SlotDisponibles asociados.
2. THE Sistema SHALL mostrar a Admisión únicamente ProgramacionesOperativas con `Habilitada = false` y fecha igual o posterior a la fecha actual.
3. IF la ProgramacionOperativa no tiene SlotDisponibles generados, THEN THE Sistema SHALL generar los slots automáticamente al habilitar.
4. THE Sistema SHALL impedir que Admisión cree nuevas ProgramacionesOperativas; solo puede habilitar las existentes.

---

### Requisito 20 — Ajuste de Cupos por Admisión (RF16)

**Historia de usuario:** Como personal de Admisión, quiero ajustar los cupos de programaciones futuras, para reflejar cambios operativos como rotaciones o ausencias de médicos.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir a Admisión modificar `CuposTotal` de ProgramacionesOperativas con fecha estrictamente posterior a la fecha actual del servidor.
2. IF Admisión intenta modificar una ProgramacionOperativa con fecha igual o anterior a la fecha actual, THEN THE Sistema SHALL rechazar la operación con el mensaje "Solo se pueden ajustar programaciones futuras".
3. WHEN Admisión reduce `CuposTotal` de una ProgramacionOperativa, THE Sistema SHALL verificar que el nuevo valor no sea menor al número de Citas activas ya registradas para esa programación.
4. IF el nuevo `CuposTotal` es menor al número de Citas activas, THEN THE Sistema SHALL rechazar el ajuste con el mensaje "No se puede reducir cupos por debajo de las citas ya registradas".

---

### Requisito 21 — Gestión de Médicos (RF_M)

**Historia de usuario:** Como Administrador, quiero gestionar el catálogo de médicos, para asignarlos a las programaciones operativas.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir al Administrador crear, editar y desactivar médicos, incluyendo nombres, apellidos y CMP.
2. THE Sistema SHALL validar que el CMP sea único en el sistema antes de crear o editar un médico.
3. IF el Administrador intenta desactivar un médico con ProgramacionesOperativas futuras habilitadas, THEN THE Sistema SHALL advertir al Administrador antes de permitir la desactivación.
4. WHEN un médico es desactivado, THE Sistema SHALL mantener las Citas existentes asociadas a ese médico sin modificar su estado.

---

### Requisito 22 — Gestión de Usuarios Internos (RF22)

**Historia de usuario:** Como Administrador, quiero gestionar las cuentas de usuarios internos (Admisión y Administrador), para controlar el acceso operativo al sistema.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir al Administrador crear, editar y desactivar usuarios con roles `Admision` y `Administrador`.
2. THE Sistema SHALL validar que el DNI del nuevo usuario sea único en la tabla `Usuario` antes de crear la cuenta.
3. WHEN el Administrador crea un usuario interno, THE Sistema SHALL almacenar la contraseña inicial usando BCrypt.
4. IF el Administrador intenta desactivar su propia cuenta, THEN THE Sistema SHALL rechazar la operación con el mensaje "No puede desactivar su propia cuenta".

---

### Requisito 23 — Gestión de Vínculos Tutor–Dependiente por Administrador (RF_TD-A)

**Historia de usuario:** Como Administrador, quiero gestionar los vínculos entre tutores y dependientes, para mantener el padrón de relaciones familiares actualizado.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir al Administrador crear vínculos en `TutorDependiente` especificando el DNI del Tutor y el DNI del Dependiente.
2. THE Sistema SHALL validar que tanto el Tutor como el Dependiente existan como Pacientes registrados antes de crear el vínculo.
3. THE Sistema SHALL validar que el Dependiente tenga `EsMenor = true` antes de crear el vínculo.
4. IF ya existe un vínculo activo entre el mismo Tutor y Dependiente, THEN THE Sistema SHALL rechazar la creación con el mensaje "El vínculo ya existe".
5. THE Sistema SHALL permitir al Administrador eliminar vínculos existentes en `TutorDependiente`.
6. THE Sistema SHALL registrar `FechaRegistro` con la fecha/hora del servidor al crear cada vínculo.

---

### Requisito 24 — Registro de Vínculo Tutor–Dependiente por Admisión (RF_TD-ADM)

**Historia de usuario:** Como personal de Admisión, quiero registrar vínculos tutor-dependiente desde ventanilla, para asistir a familias que se presentan presencialmente.

#### Criterios de Aceptación

1. THE Sistema SHALL permitir a Admisión crear vínculos en `TutorDependiente` especificando el DNI del Tutor y el DNI del Dependiente.
2. THE Sistema SHALL aplicar las mismas validaciones que el Administrador: existencia de ambos pacientes, `EsMenor = true` del Dependiente y unicidad del vínculo.
3. THE Sistema SHALL registrar el usuario de Admisión que creó el vínculo y la fecha/hora de la operación.

---

### Requisito 25 — Vista Global de Citas del Día por Admisión (RF27)

**Historia de usuario:** Como personal de Admisión, quiero ver todas las citas del día con filtro por DNI, para gestionar la agenda global de la posta y resolver conflictos diarios.

#### Criterios de Aceptación

1. WHEN Admisión accede al panel de citas del día, THE Sistema SHALL mostrar todas las Citas (web y presenciales) con fecha igual a la fecha actual del servidor.
2. THE Sistema SHALL mostrar por cada Cita: nombre del Paciente, DNI, especialidad, médico, turno, hora del slot, estado actual y origen (web/presencial).
3. WHEN Admisión ingresa un DNI en el filtro, THE Sistema SHALL filtrar la lista en tiempo real mostrando únicamente las Citas del Paciente con ese DNI.
4. THE Sistema SHALL ordenar la lista de citas por turno (Mañana primero, Tarde después) y dentro de cada turno por hora de inicio del slot de forma ascendente.

---

### Requisito 26 — Búsqueda de Historia Clínica por DNI (RF28)

**Historia de usuario:** Como personal de Admisión, quiero buscar el número de historia clínica de un paciente ingresando su DNI, para localizar físicamente el expediente en el archivo.

#### Criterios de Aceptación

1. WHEN Admisión ingresa un DNI en el buscador de historia clínica, THE Sistema SHALL retornar el número de historia clínica del Paciente en formato `HC-XXXX`.
2. IF el DNI no corresponde a ningún Paciente registrado, THEN THE Sistema SHALL mostrar el mensaje "Paciente no encontrado en el sistema".
3. THE Sistema SHALL precargar automáticamente los datos del Paciente (nombres, DNI, historia clínica) cuando el DNI existe en la base de datos.

---

### Requisito 27 — Búsqueda Rápida de Paciente en Panel de Admisión (RF31)

**Historia de usuario:** Como personal de Admisión, quiero disponer de un buscador rápido en el panel principal, para encontrar pacientes por DNI sin navegar por menús.

#### Criterios de Aceptación

1. THE Sistema SHALL mostrar un campo de búsqueda accesible desde el panel principal de Admisión sin requerir navegación adicional.
2. WHEN Admisión ingresa un DNI completo (8 dígitos), THE Sistema SHALL mostrar los datos del Paciente: nombres, DNI, historia clínica y estado de cuenta.
3. IF el DNI ingresado no corresponde a ningún Paciente, THEN THE Sistema SHALL mostrar el mensaje "No se encontró ningún paciente con ese DNI".
4. WHEN Admisión selecciona un resultado de búsqueda, THE Sistema SHALL ofrecer acciones directas: registrar cita presencial, cancelar cita activa y resetear contraseña.

---

## Requisitos No Funcionales

### RNF01 — Rendimiento

THE Sistema SHALL responder a cualquier solicitud de usuario en un tiempo máximo de 3 segundos bajo condiciones normales de operación (hasta 50 usuarios concurrentes).

### RNF02 — Seguridad

THE Sistema SHALL requerir autenticación válida para acceder a cualquier vista o endpoint que no sea la página de inicio de sesión o el formulario de activación de cuenta.

### RNF03 — Integridad Transaccional

WHEN se ejecuta una operación de reserva o cancelación, THE Sistema SHALL garantizar que el decremento/incremento de `CuposDisponibles` y la creación/actualización de la Cita ocurran en la misma transacción de base de datos.

### RNF04 — Cobertura de Pruebas

THE Sistema SHALL mantener una cobertura de pruebas automatizadas de al menos 50% sobre el total del proyecto, al menos 85% sobre la capa de Services, y 100% sobre las seis funciones puras de dominio definidas en esta especificación.

### RNF05 — Trazabilidad

THE Sistema SHALL registrar en `HistorialEstadoCita` cada cambio de estado de una Cita, incluyendo el identificador del usuario que realizó el cambio y la fecha/hora del servidor.

### RNF06 — Compatibilidad

THE Sistema SHALL ser compatible con los navegadores Google Chrome (versión 120+), Microsoft Edge (versión 120+) y Mozilla Firefox (versión 120+).

### RNF07 — Usabilidad del Flujo de Reserva

THE Sistema SHALL implementar el flujo de reserva web como un asistente de pasos (wizard) que no requiera recarga completa de página entre pasos, usando peticiones asíncronas o parciales de Razor.

### RNF08 — Impresión de Ticket

THE Sistema SHALL proveer una vista de Ticket optimizada para impresión que oculte la navegación y elementos de UI no esenciales al imprimir desde el navegador.

---

## Propiedades de Corrección — Funciones Puras de Dominio (PBT)

Las siguientes funciones puras deben alcanzar cobertura del 100% mediante pruebas basadas en propiedades (Property-Based Testing) usando xUnit + FsCheck o similar. Cada función es independiente de infraestructura y no realiza llamadas a base de datos.

### PBT-01 — `ObtenerHoraLimiteCancelacion(turno) → TimeOnly`

| Propiedad | Descripción |
|-----------|-------------|
| **Determinismo** | Para el mismo valor de `turno`, la función siempre retorna el mismo `TimeOnly`. |
| **Turno Mañana** | `ObtenerHoraLimiteCancelacion(Turno.Manana)` SHALL retornar `07:40`. |
| **Turno Tarde** | `ObtenerHoraLimiteCancelacion(Turno.Tarde)` SHALL retornar `14:40`. |
| **Exhaustividad** | Para todo valor del enum `Turno`, la función SHALL retornar un `TimeOnly` válido sin lanzar excepción. |

### PBT-02 — `EsDentroDeVentanaReserva(diaSemana, turno, horaActual) → bool`

| Propiedad | Descripción |
|-----------|-------------|
| **Domingo bloqueado** | Para `diaSemana = DayOfWeek.Sunday`, la función SHALL retornar `false` para cualquier turno y hora. |
| **Lunes–Viernes** | Para `diaSemana` en {Lunes, Martes, Miércoles, Jueves, Viernes}, la función SHALL retornar `true` si `horaActual` es anterior a la hora de inicio del turno. |
| **Sábado** | Para `diaSemana = DayOfWeek.Saturday`, la función SHALL retornar `true` para turnos del sábado y del próximo lunes. |
| **Idempotencia** | Llamar la función dos veces con los mismos parámetros SHALL producir el mismo resultado. |

### PBT-03 — `EsDentroDeVentanaCancelacion(turno, horaActual, esAdmision) → bool`

| Propiedad | Descripción |
|-----------|-------------|
| **Admisión siempre puede** | Para `esAdmision = true`, la función SHALL retornar `true` para cualquier turno y hora. |
| **Paciente antes del límite** | Para `esAdmision = false` y `horaActual < ObtenerHoraLimiteCancelacion(turno)`, la función SHALL retornar `true`. |
| **Paciente después del límite** | Para `esAdmision = false` y `horaActual >= ObtenerHoraLimiteCancelacion(turno)`, la función SHALL retornar `false`. |
| **Consistencia con PBT-01** | El resultado de esta función SHALL ser consistente con el valor retornado por `ObtenerHoraLimiteCancelacion`. |

### PBT-04 — `ValidarDatosActivacion(dni, fechaNac, nombres, paciente) → bool`

| Propiedad | Descripción |
|-----------|-------------|
| **Coincidencia exacta** | La función SHALL retornar `true` únicamente cuando `dni`, `fechaNac` y `nombres` coinciden exactamente con los campos del `paciente`. |
| **Fallo parcial** | Si cualquiera de los tres campos no coincide, la función SHALL retornar `false`. |
| **Null safety** | Para cualquier parámetro nulo, la función SHALL retornar `false` sin lanzar excepción. |
| **Idempotencia** | Llamar la función dos veces con los mismos parámetros SHALL producir el mismo resultado. |

### PBT-05 — `GenerarCodigoTicket(fecha) → string`

| Propiedad | Descripción |
|-----------|-------------|
| **Formato** | Para toda fecha válida, el código generado SHALL coincidir con el patrón `TKT-YYYYMMDD-NNNN`. |
| **No vacío** | La función SHALL retornar un string no nulo y no vacío para cualquier fecha válida. |
| **Determinismo de prefijo** | El segmento `YYYYMMDD` del código SHALL corresponder a la fecha de entrada. |

### PBT-06 — `GenerarSlots(horaInicioTurno, duracionMinutos, cuposTotal) → IEnumerable<SlotDto>`

| Propiedad | Descripción |
|-----------|-------------|
| **Invariante de cupos** | La suma de `CuposTotal` de todos los SlotDtos generados SHALL ser igual al parámetro `cuposTotal`. |
| **Cobertura de turno** | El `HoraFin` del último slot SHALL ser menor o igual a la hora de fin del turno correspondiente. |
| **Sin solapamiento** | Para todo par de SlotDtos generados, los rangos `[HoraInicio, HoraFin)` no SHALL solaparse. |
| **Orden ascendente** | Los SlotDtos SHALL estar ordenados por `HoraInicio` de forma ascendente. |
| **Duración correcta** | Para todo SlotDto, `HoraFin - HoraInicio` SHALL ser igual a `duracionMinutos`. |
| **Parámetros inválidos** | Para `duracionMinutos <= 0` o `cuposTotal <= 0`, la función SHALL lanzar `ArgumentException`. |
| **Round-trip** | El número de slots generados SHALL ser igual a `floor((horaFinTurno - horaInicioTurno).TotalMinutes / duracionMinutos)`. |

---

## Matriz de Trazabilidad

| Requisito | RF / Regla | Actor | Módulo | PBT |
|-----------|-----------|-------|--------|-----|
| Req. 1 — Activación/Recuperación | RF_ACT, RN01, RN01-A, RN41 | Paciente | M1 | PBT-04 |
| Req. 2 — Login | RF01 | Todos | M1 | — |
| Req. 3 — Logout | RF02 | Todos | M1 | — |
| Req. 4 — Actualizar perfil | RF05, RN02 | Paciente | M1 | — |
| Req. 5 — Reset contraseña Admisión | RF_RST, RN01-B | Admisión | M1 | — |
| Req. 6 — Consultar perfil | RF04 | Paciente | M2 | — |
| Req. 7 — Sesión delegada | RF36, RN03, RN42, RN43 | Paciente/Tutor | M2 | — |
| Req. 8 — Especialidades disponibles | RF07, RN07, RN37 | Paciente | M3 | PBT-02 |
| Req. 9 — Disponibilidad por turno | RF08, RN16, RN27, RN28 | Paciente | M3 | — |
| Req. 10 — Reserva web | RF09, RN04, RN11, RN12, RN31, RN37 | Paciente | M3 | PBT-05, PBT-06 |
| Req. 11 — Cita presencial | RF10, RN04, RN40 | Admisión | M3 | — |
| Req. 12 — Cancelación Paciente | RF11-P, RN13, RN14, RN36 | Paciente | M3 | PBT-01, PBT-03 |
| Req. 13 — Cancelación Admisión | RF11-A, RN44 | Admisión | M3 | PBT-03 |
| Req. 14 — Generación Ticket | RF12, RN12 | Sistema | M3 | PBT-05 |
| Req. 15 — Estado de cita | RF19, RN30 | Paciente | M3 | — |
| Req. 16 — Gestión UPS | RF13 | Administrador | M4 | — |
| Req. 17 — Gestión Especialidades | RF14, RN29 | Administrador | M4 | — |
| Req. 18 — Programación operativa | RF15A, RN08, RN17, RN09 | Administrador | M4 | PBT-06 |
| Req. 19 — Habilitar disponibilidad | RF15, RN05, RN06 | Admisión | M4 | — |
| Req. 20 — Ajuste de cupos | RF16, RN09, RN18 | Admisión | M4 | — |
| Req. 21 — Gestión médicos | RF_M | Administrador | M5 | — |
| Req. 22 — Gestión usuarios internos | RF22 | Administrador | M5 | — |
| Req. 23 — Vínculos TD Administrador | RF_TD-A, RN45 | Administrador | M5 | — |
| Req. 24 — Vínculos TD Admisión | RF_TD-ADM, RN45 | Admisión | M6 | — |
| Req. 25 — Vista citas del día | RF27 | Admisión | M6 | — |
| Req. 26 — Búsqueda historia clínica | RF28, RN40 | Admisión | M6 | — |
| Req. 27 — Búsqueda rápida paciente | RF31 | Admisión | M6 | — |

---

## Fuera del Alcance de esta Iteración

Los siguientes elementos quedan explícitamente excluidos y no deben implementarse:

- Rol Médico y RF30 (agenda diaria del médico)
- Módulo de Enfermería: triaje digital, tablero Kanban, avisos de atención inmediata
- RF17, RF18, RF20, RF21, RF25, RF26, RF32 (todos los RF de Enfermería)
- Historia clínica electrónica
- Módulos de farmacia o recetas médicas
- Notificaciones por email o SMS
- Sobrecupos visibles para el Paciente

---

*Documento generado conforme al workflow Requirements-First — Spec-Driven Development (SpecDD)*  
*Tecnología: ASP.NET Core MVC (.NET 10) · SQL Server · Entity Framework Core*

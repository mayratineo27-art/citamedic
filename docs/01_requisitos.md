# REQUISITOS DEL SISTEMA

Proyecto:
Sistema Web de Gestión de Citas Médicas – Posta de Salud

Versión:
1.0

---

# OBJETIVO

Definir las funcionalidades y restricciones necesarias para implementar el sistema.

---

# ACTORES

- Paciente
- Responsable del menor
- Admisión
- Enfermería
- Administrador

---

# REQUISITOS FUNCIONALES


## RF01 Iniciar sesión

Actor:
Todos

Descripción:
Permitir acceso según rol.

Prioridad:
Alta

---

## RF03 Gestionar menores

Actor:
Responsable

Descripción:

Permitir administrar pacientes menores.

Acciones:

- Registrar dependiente.
- Reservar.
- Visualizar historial de citas.

Prioridad:
Alta

---

# MÓDULO 2 — PACIENTES

## RF04 Consultar perfil

Actor:
Paciente

Descripción: 
Permitir que el paciente consulte su perfil desde su dashboard.

Visualizar:

- Nombres y Apellidos
- Dni
- Nro. Historia Clínica
- Seguro de Salud SIS
- Establecimiento asignado
- Dirección domiciliaria

Prioridad:
Media

---

## RF05 Actualizar datos permitidos

Actor:
Paciente

Descripción:

Modificar:

- Número celular
- Contraseña

Restricción:

No modificar:

- DNI
- Nombre
- Fecha de nacimiento

Prioridad:
Media

---

# MÓDULO 3 — CITAS

## RF07 Consultar especialidades

Actor:
Paciente

Descripción:

Visualizar especialidades disponibles.

Restricción:

No visualizar UPS.

Prioridad:
Alta

---

## RF08 Consultar disponibilidad

Actor:
Paciente

Descripción:

Consultar cupos por:

- Especialidad
- Turno
- Médico
- Horario

Prioridad:
Alta

---

## RF09 Reservar cita

Actor:
Paciente

Descripción:

Reservar mediante:

Especialidad

↓

Turno

↓

Tarjeta disponible

↓

Confirmar

Reglas:

- Descontar cupo.
- Generar ticket.

Prioridad:
Alta

---

## RF10 Registrar cita presencial

Actor:
Admisión

Descripción:

Registrar reservas realizadas presencialmente.

Reglas:

- Compartir stock con web.

Prioridad:
Alta

---

## RF11 Cancelar cita

Actor:
Paciente

Descripción:

Cancelar reserva.

Reglas:

- Liberar cupo.
- Solo antes del inicio del triaje.

Prioridad:
Alta

---

## RF12 Generar ticket

Actor:
Sistema

Descripción:

Emitir comprobante.

Datos:

- Especialidad
- Médico
- Hora
- Fecha

Prioridad:
Alta

---

# MÓDULO 4 — DISPONIBILIDAD

## RF13 Gestionar UPS

Actor:
Administrador

Descripción:

Gestionar:

- Crear
- Editar
- Desactivar

Prioridad:
Alta

---

## RF14 Gestionar especialidades

Actor:
Administrador

Descripción:

Configurar:

- Nombre
- UPS
- Duración atención

Prioridad:
Alta

---

## RF15 Habilitar disponibilidad

Actor:
Admisión

Descripción:

Habilitar la disponibilidad previamente configurada para la jornada correspondiente.

Incluye:

- Abrir cupos.
- Asociar médico disponible.
- Confirmar publicación.

Restricción:

No podrá crear horarios fuera de la planificación establecida.

---

## RF15A Configurar programación operativa

Actor:
Administrador

Descripción:

Configurar programación futura.

Incluye:

- Especialidad
- Médico
- Turno
- Duración
- Cupos

---

## RF16 Ajustar disponibilidad

Actor:
Admisión

Descripción:

Modificar disponibilidad futura.

Motivos:

- Rotación
- Ausencia
- Reprogramación

Restricción:

No modificar atención iniciada.

Prioridad:
Alta

---

# MÓDULO 5 — TRIAJE

## RF17 Registrar triaje

Actor:
Enfermería

Descripción:

Registrar:

- Peso
- Talla
- Temperatura
- Presión
- Observación

Prioridad:
Alta

---

## RF18 Actualizar estado de atención

Actor:
Enfermería

Estados:

- Pendiente
- En triaje
- Listo atención
- No asistió

Prioridad:
Alta

---

## RF19 Consultar trazabilidad

Actor:
Paciente

Descripción:

Visualizar estados.

Prioridad:
Media

---

# MÓDULO 6 — AVISO DE ATENCIÓN

## RF20 Registrar aviso de atención inmediata

Actor:
Paciente

Descripción:

Enviar aviso operativo.

Restricción:

No genera cita.

Prioridad:
Baja

---

## RF21 Visualizar avisos

Actor:
Enfermería

Descripción:

Visualizar panel.

Estados:

- Pendiente
- Visualizado
- Cerrado

Prioridad:
Baja

---

# MÓDULO 7 — ADMINISTRACIÓN

## RF22 Gestionar usuarios

Actor:
Administrador

Descripción:

Administrar:

- Pacientes
- Enfermería
- Admisión

Prioridad:
Media

---

## RF23 Configurar duración de atención

Actor:
Administrador

Descripción:

Configurar duración.

Ejemplo:

Medicina:
20 minutos

Prioridad:
Alta

---

## RF24 Gestionar sobrecupos

Actor:
Admisión

Descripción:

Registrar excepción.

Restricción:

No visible al paciente.

Prioridad:
Baja

---



# MÓDULO 8 — DERIVADOS 

## RF25 Tablero Kanban de Atención
Actor: Enfermería
Descripción: El sistema debe proveer una vista Kanban ("Falta Triaje", "En Triaje", "Listo Atención", "Finalizados") para que enfermería mueva pacientes entre estados rápidamente.
Trazabilidad: CU17
Prioridad: Alta

## RF36 Gestión de Sesión Delegada (Mi Familia)
Actor: Paciente (Responsable)
Descripción: El sistema debe permitir a un tutor cambiar su sesión actual a la de un dependiente menor de edad registrado, operando en nombre de este, y regresar a la sesión original con un solo clic.
Trazabilidad: CU22
Prioridad: Alta

## RF27 Visualización Global y Filtrado de Citas
Actor: Admisión
Descripción: El sistema debe listar todas las citas de ambas modalidades (web y presencial) y permitir un filtrado en tiempo real utilizando el DNI del paciente.
Trazabilidad: CU19
Prioridad: Alta

## RF28 Consulta de Historia Clínica por DNI
Actor: Admisión
Descripción: Proveer una funcionalidad de búsqueda para obtener de inmediato el Identificador de Historia Clínica (Ej. HC-XXXX) a partir del DNI, facilitando la ubicación física del expediente.
Trazabilidad: CU20
Prioridad: Media

## RF29 Vista Administrativa de Oferta en Tiempo Real
Actor: Admisión
Descripción: Permitir a Admisión consultar el estado de las especialidades activas y sus UPS correspondientes para informar correctamente a los pacientes presenciales.
Trazabilidad: CU21
Prioridad: Baja

---

## RF30 Visualizar Agenda Diaria
Actor: Médico
Descripción: Permitir al médico visualizar la lista cronológica de pacientes asignados a sus citas del día, incluyendo el estado de cada paciente (ej. "Listo Atención", "En Triaje").
Prioridad: Alta

## RF31 Búsqueda Rápida de Pacientes
Actor: Admisión
Descripción: Proveer un buscador global en el portal de admisión que permita filtrar y localizar pacientes mediante DNI o Número de Historia Clínica.
Prioridad: Media

## RF32 Validaciones Visuales Clínicas
Actor: Enfermería
Descripción: El sistema debe validar en tiempo real los valores numéricos introducidos en los campos de triaje (Tensión Arterial, Temperatura, Peso, etc.) y mostrar alertas visuales (ej. bordes rojos) si los valores exceden los rangos lógicos esperados.
Prioridad: Media

---

# REQUISITOS NO FUNCIONALES

## RNF01 Rendimiento

Tiempo de respuesta ≤ 3 segundos.

---

## RNF02 Seguridad

Autenticación obligatoria.

---

## RNF03 Disponibilidad

Operación durante horario laboral.

---

## RNF04 Usabilidad

Interfaz simple.

---

## RNF05 Escalabilidad

Permitir agregar especialidades.

---

## RNF06 Compatibilidad

Compatible con navegador.

---

## RNF07 Trazabilidad

Registrar cambios

## RNF08 Experiencia de Usuario Ágil e Inmersiva
El sistema debe reducir las recargas de página completas durante flujos críticos (ej. Reserva de citas mediante Wizard de pasos, visualización de detalles clínicos en Modales u Offcanvas).

## RNF09 Accesibilidad de Impresión
Las vistas diseñadas como documentos formales (Ej. Ticket Electrónico de cita) deben estar optimizadas nativamente para la impresión (media query de print) garantizando que elementos no esenciales de la UI (navegación, botones, sombras) sean ocultados.


## RNF10 Interfaz Reactiva (Kanban)
El tablero Kanban de Enfermería debe actualizar los estados de las tarjetas de manera fluida, sin refrescar toda la página, utilizando peticiones asíncronas para mantener el contexto visual operativo.

## RNF11 Seguridad en Delegación de Sesiones
El mecanismo de "Cambio de Sesión" hacia un dependiente debe validar la integridad del vínculo (`ResponsableId`) a nivel de base de datos antes de generar el nuevo token de autenticación, asegurando que un usuario no suplante a nadie ajeno a su núcleo familiar.

---

# CRITERIOS DE ACEPTACIÓN

El sistema deberá permitir:

✔ Registrar citas.

✔ Compartir disponibilidad.

✔ Registrar triaje.

✔ Visualizar estados.

✔ Generar ticket.

✔ Ejecutar flujo completo.


HISTORIAS DE USUARIO (SpecDD & BDD)
Esta sección traduce la totalidad de los Casos de Uso y Reglas de Negocio en especificaciones ejecutables organizadas por Épicas. La priorización sigue estrictamente el modelo MoSCoW para definir el MVP.

ÉPICA 1: Gestión de Acceso y Usuarios


HU01 - Inicio de Sesión

Como Usuario (Paciente o Personal), quiero ingresar mis credenciales para acceder a las funciones correspondientes a mi rol.

Prioridad: Must Have

Trazabilidad: CU02, RF02

Criterios de Aceptación:

El sistema debe denegar el acceso si la cuenta no ha sido habilitada administrativamente (RN01).

Tras la validación exitosa, el sistema debe redireccionar al usuario al panel correspondiente a su rol.

HU02 - Gestión de Dependientes Menores

Como Responsable del menor, quiero registrar y administrar pacientes menores para poder reservar y visualizar el historial de sus citas.

Prioridad: Must Have

Trazabilidad: CU03, RF04

Criterios de Aceptación:

Solo un usuario identificado como adulto responsable puede administrar reservas de dependientes menores (RN03).

El sistema debe confirmar si el dependiente ya se encuentra registrado para evitar duplicidad.

HU03 - Consultar y Actualizar Perfil

Como Paciente, quiero visualizar y actualizar mi información para mantener mis datos de contacto al día.

Prioridad: Should Have

Trazabilidad: RF05, RF06

Criterios de Aceptación:

El sistema debe permitir modificar únicamente el número de celular y la contraseña (RN02).

El sistema debe bloquear la edición del DNI, Nombre y Fecha de nacimiento (RN02).

HU04 - Gestionar Usuarios Internos

Como Administrador, quiero administrar las cuentas de Admisión y Enfermería para controlar el acceso operativo al sistema.

Prioridad: Should Have

Trazabilidad: CU18, RF22

Criterios de Aceptación:

El administrador debe poder buscar, editar y guardar la configuración de acceso del personal.

ÉPICA 2: Configuración y Disponibilidad Operativa

HU05 - Configuración de Duración de Atención

Como Administrador, quiero configurar la duración de atención por especialidad para que el sistema calcule correctamente la generación de cupos.

Prioridad: Must Have

Trazabilidad: CU19, RF23

Criterios de Aceptación:

Cada especialidad debe permitir una duración distinta (ej. 20 minutos) (RN29).

La modificación de la duración debe impactar automáticamente en la futura generación de tarjetas de disponibilidad.

HU06 - Gestionar UPS y Especialidades

Como Administrador, quiero crear y configurar UPS y Especialidades para estructurar la oferta de servicios de la posta.

Prioridad: Must Have

Trazabilidad: RF13, RF14

Criterios de Aceptación:

Las UPS configuradas deben mantenerse de uso estrictamente interno y no ser visibles para los pacientes (RN07).

HU07 - Configuración de Programación Operativa

Como Administrador, quiero definir las jornadas de los médicos para establecer la oferta base de turnos.

Prioridad: Must Have

Trazabilidad: CU10, RF15A

Criterios de Aceptación:

La programación debe incluir especialidad, médico, turno, duración y cupos totales (RN17).

El sistema debe restringir cualquier intento de modificar una atención o programación que ya haya iniciado (RN09).

HU08 - Habilitación de Disponibilidad

Como Admisión, quiero habilitar la programación existente para abrir los cupos al público web y presencial.

Prioridad: Must Have

Trazabilidad: CU11, RF15

Criterios de Aceptación:

Admisión no debe poder crear nuevos horarios fuera de la planificación del administrador (RN06).

Al confirmar la apertura, la disponibilidad debe generarse automáticamente dividiendo el turno según la duración configurada (RN08).

HU09 - Ajustar Disponibilidad Futura

Como Admisión, quiero modificar los cupos por rotación o ausencia para mantener la oferta real actualizada.

Prioridad: Should Have

Trazabilidad: CU12, RF16

Criterios de Aceptación:

El ajuste operativo debe limitarse única y estrictamente a jornadas futuras (RN18).

HU10 - Gestionar Sobrecupos

Como Admisión, quiero registrar excepciones de disponibilidad para atender casos fuera del stock regular.

Prioridad: Could Have

Trazabilidad: CU20, RF24

Criterios de Aceptación:

La disponibilidad extraordinaria generada (sobrecupo) debe permanecer totalmente oculta para los pacientes en la plataforma web (RN16).

ÉPICA 3: Consulta y Reserva de Citas
HU11 - Consultar Especialidades

Como Paciente, quiero visualizar la lista de especialidades para iniciar mi proceso de reserva.

Prioridad: Must Have

Trazabilidad: CU04, RF07

Criterios de Aceptación:

El sistema debe mostrar únicamente las especialidades con disponibilidad, ocultando las jerarquías internas de UPS (RN07).

HU12 - Consultar Disponibilidad

Como Paciente, quiero consultar los horarios y médicos por especialidad para elegir mi turno ideal.

Prioridad: Must Have

Trazabilidad: CU05, RF08

Criterios de Aceptación:

El paciente debe visualizar los turnos segmentados únicamente entre Mañana (08:00–13:30) y Tarde (15:00–19:00) (RN27, RN28).

HU13 - Reserva de Cita Web

Como Paciente, quiero seleccionar un horario y confirmar mi reserva para asegurar mi atención.

Prioridad: Must Have

Trazabilidad: CU06, RF09

Criterios de Aceptación:

La reserva confirmada debe descontar inmediatamente un cupo del stock compartido (RN04).

El sistema debe bloquear mediante validación la duplicidad de reservas activas para el mismo paciente y horario (RN31).

HU14 - Registro de Cita Presencial

Como Admisión, quiero registrar reservas físicas para atender a los pacientes en ventanilla.

Prioridad: Must Have

Trazabilidad: CU07, RF10

Criterios de Aceptación:

La reserva presencial debe consumir cupos exactamente del mismo stock que utiliza la web (RN04).

HU15 - Generación de Ticket

Como Sistema, quiero emitir un comprobante automático al finalizar una reserva para garantizar la formalidad del proceso.

Prioridad: Must Have

Trazabilidad: CU09, RF12

Criterios de Aceptación:

Toda reserva confirmada (web o presencial) debe generar obligatoriamente un ticket (RN12).

El ticket debe contener Especialidad, Médico, Hora y Fecha.

HU16 - Cancelación de Cita

Como Paciente, quiero cancelar mi reserva activa para liberar mi cupo.

Prioridad: Should Have

Trazabilidad: CU08, RF11

Criterios de Aceptación:

La cancelación solo debe permitirse si se solicita antes de iniciar el periodo de triaje (20 minutos antes del turno) (RN13, RN36).

La cancelación exitosa debe devolver inmediatamente la disponibilidad al stock (RN14).

ÉPICA 4: Triaje y Trazabilidad

HU17 - Registro de Triaje

Como Enfermería, quiero registrar la evaluación inicial del paciente para habilitarlo para atención.

Prioridad: Must Have

Trazabilidad: CU13, RF17

Criterios de Aceptación:

El registro debe ser exclusivo del rol de enfermería (RN20).

Solo se permite ingresar peso, talla, temperatura, presión y observación (sin historia clínica) (RN22, RN23).

El paciente debe tener una cita activa y el registro debe cambiar el estado a "En triaje".

HU18 - Actualizar Estado de Atención

Como Enfermería, quiero actualizar los estados de la cita para mantener la trazabilidad operativa.

Prioridad: Must Have

Trazabilidad: CU14, RF18

Criterios de Aceptación:

Las transiciones deben respetar el flujo estricto: Pendiente → En triaje → Listo atención → No asistió (RN21).

Cada transición debe generar un registro de auditoría en la base de datos (RN30).

HU19 - Consultar Trazabilidad

Como Paciente, quiero consultar el estado de mi cita para hacer seguimiento a mi atención.

Prioridad: Should Have

Trazabilidad: CU15, RF19

Criterios de Aceptación:

El paciente debe visualizar el estado actualizado en tiempo real sin requerir intervención administrativa.

ÉPICA 5: Avisos Operativos (MVP Complementario)
HU20 - Registrar Aviso de Atención Inmediata

Como Paciente, quiero enviar un aviso indicando mi intención de asistir de inmediato para alertar a enfermería.

Prioridad: Could Have

Trazabilidad: CU16, RF20

Criterios de Aceptación:

El aviso debe registrarse sin generar una reserva médica real ni alterar el stock (RN24).

El aviso no debe otorgar prioridad automática sobre el orden de citas (RN25).

HU21 - Visualizar Panel de Avisos

Como Enfermería, quiero visualizar un panel con las notificaciones de los pacientes para estar preparado ante llegadas inminentes.

Prioridad: Could Have

Trazabilidad: CU17, RF21

Criterios de Aceptación:

El panel debe ser visible exclusiva y únicamente por el personal de enfermería (RN26).

Los avisos deben poder transicionar entre estados: Pendiente, Visualizado y Cerrado.

**HU22- Reserva de Cita Web**
* **Como** Paciente, **quiero** seleccionar un horario y confirmar mi reserva **para** asegurar mi atención.
* **Prioridad:** Must Have
* **Trazabilidad:** CU06, RF09
* **Criterios de Aceptación:**
    * La reserva confirmada debe descontar inmediatamente un cupo del stock compartido (RN04).
    * El sistema debe bloquear mediante validación la duplicidad de reservas activas para el mismo paciente y horario (RN31).
    * El sistema solo debe permitir reservar citas para el mismo día si es de Lunes a Viernes, o para el Lunes si la operación se realiza un día Sábado. Las reservas en Domingo están bloqueadas (RN37).
*HU23 - Visualización de Agenda Diaria**
* **Como** Médico, **quiero** visualizar mi agenda diaria en formato cronológico **para** saber qué pacientes tengo programados y su estado actual.
* **Prioridad:** Must Have
* **Trazabilidad:** RF26
* **Criterios de Aceptación:**
    * El médico solo debe ver a los pacientes asignados a sus propios turnos.
    * Los pacientes deben mostrar de forma clara si ya completaron el triaje y están listos para la consulta.

**HU24- Alertas de Rangos Clínicos en Triaje**
* **Como** Profesional de Enfermería, **quiero** que el formulario de triaje me alerte si introduzco valores ilógicos **para** evitar errores de digitación en los signos vitales.
* **Prioridad:** Should Have
* **Trazabilidad:** RF28
* **Criterios de Aceptación:**
    * Los campos numéricos deben indicar un error visual inmediato si el valor introducido es biológicamente improbable (Ej. Temperatura > 45°C).

**HU25 - Búsqueda Ágil en Recepción**
* **Como** Personal de Admisión, **quiero** disponer de un buscador rápido en mi panel principal **para** encontrar pacientes por su DNI o Historia Clínica y agilizar su atención.
* **Prioridad:** Should Have
* **Trazabilidad:** RF27
* **Criterios de Aceptación:**
    * La búsqueda debe ser accesible sin navegar por menús profundos.
    * Los resultados deben coincidir parcialmente con el número de historia clínica o exactamente con el DNI.

---

ÉPICA 6: Integración y Trazabilidad Global

**HU26 - Tablero Kanban para Enfermería**
* **Como** Enfermera, **quiero** gestionar a los pacientes del día mediante un tablero Kanban **para** visualizar de manera rápida quién falta por triaje y quién está listo para atención médica.
* **Prioridad:** Must Have
* **Trazabilidad:** CU17, RF29

**HU27 - Registro Integral de Citas (Presencial y Web)**
* **Como** Admisión, **quiero** consultar un listado consolidado de citas de todas las modalidades **para** gestionar la agenda global de la posta y resolver conflictos diarios.
* **Prioridad:** Must Have
* **Trazabilidad:** CU19, RF31

**HU28 - Delegación de Cuenta (Mi Familia)**
* **Como** Tutor, **quiero** ingresar al perfil de mi hijo directamente desde mi cuenta **para** reservarle citas médicas sin requerir credenciales separadas.
* **Prioridad:** Must Have
* **Trazabilidad:** CU22, RF30

**HU29 - Localización Ágil de Historia Clínica**
* **Como** Admisión, **quiero** ingresar el DNI de un paciente y ver su Número de Historia Clínica **para** buscar físicamente su expediente en el archivo de forma rápida.
* **Prioridad:** Should Have
* **Trazabilidad:** CU20, RF32

---



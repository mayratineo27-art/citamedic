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

# MÓDULO 1 — AUTENTICACIÓN

## RF01 Solicitar credenciales

Actor:
Paciente

Descripción:
El paciente podrá solicitar acceso al sistema.

Reglas:

- Debe existir registro administrativo.
- Se verifica identidad.
- Se genera usuario.

Prioridad:
Alta

---

## RF02 Iniciar sesión

Actor:
Todos

Descripción:
Permitir acceso según rol.

Prioridad:
Alta

---

## RF03 Recuperar acceso

Actor:
Paciente

Descripción:

Permitir restablecer contraseña mediante validación del número celular registrado.

Incluye:

- Cambio de contraseña.
- Cambio de número.


Flujo:

DNI

↓

Validación celular

↓

Nueva contraseña

Prioridad:
Alta

---

## RF04 Gestionar menores

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

## RF05 Consultar perfil

Actor:
Paciente

Descripción:

Visualizar:

- Datos personales.
- Estado SIS.
- Posta asociada.

Prioridad:
Media

---

## RF06 Actualizar datos permitidos

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

## RF-25: Validación y Autocompletado de Datos de Identidad (DNI)**
* **Descripción:** Al momento de registrar un nuevo actor (Paciente, Médico, etc.), el sistema debe requerir el ingreso del DNI (8 dígitos). El sistema ejecutará una consulta asíncrona para verificar si el DNI existe en la base de datos local. De no existir, consultará un servicio de validación de identidad (Simulado/Mock de RENIEC).
* **Restricción de Integridad:** Los campos "Nombres" y "Apellidos" deben mantenerse en estado de solo lectura (readonly) para el usuario digitador. Estos se autocompletarán únicamente con los datos devueltos por el servicio de validación para evitar duplicidad y errores tipográficos.

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

Registrar cambios.

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
HU01 - Solicitud de Credenciales

Como Paciente, quiero solicitar acceso al sistema para poder gestionar mis citas médicas de forma virtual.

Prioridad: Must Have

Trazabilidad: CU01, RF01

Criterios de Aceptación:

El paciente debe existir en el registro administrativo previo para enviar la solicitud.

La cuenta no se habilita automáticamente; el sistema debe requerir validación explícita por parte de Admisión (RN01).

HU02 - Inicio de Sesión

Como Usuario (Paciente o Personal), quiero ingresar mis credenciales para acceder a las funciones correspondientes a mi rol.

Prioridad: Must Have

Trazabilidad: CU02, RF02

Criterios de Aceptación:

El sistema debe denegar el acceso si la cuenta no ha sido habilitada administrativamente (RN01).

Tras la validación exitosa, el sistema debe redireccionar al usuario al panel correspondiente a su rol.

HU03 - Recuperación de Acceso

Como Paciente, quiero restablecer mi contraseña para recuperar el acceso a mi cuenta si la he olvidado.

Prioridad: Must Have

Trazabilidad: RF03, RN02-A

Criterios de Aceptación:

El sistema debe exigir la validación conjunta del DNI y el número celular registrado (RN02-A).

HU04 - Gestión de Dependientes Menores

Como Responsable del menor, quiero registrar y administrar pacientes menores para poder reservar y visualizar el historial de sus citas.

Prioridad: Must Have

Trazabilidad: CU03, RF04

Criterios de Aceptación:

Solo un usuario identificado como adulto responsable puede administrar reservas de dependientes menores (RN03).

El sistema debe confirmar si el dependiente ya se encuentra registrado para evitar duplicidad.

HU05 - Consultar y Actualizar Perfil

Como Paciente, quiero visualizar y actualizar mi información para mantener mis datos de contacto al día.

Prioridad: Should Have

Trazabilidad: RF05, RF06

Criterios de Aceptación:

El sistema debe permitir modificar únicamente el número de celular y la contraseña (RN02).

El sistema debe bloquear la edición del DNI, Nombre y Fecha de nacimiento (RN02).

HU06 - Gestionar Usuarios Internos

Como Administrador, quiero administrar las cuentas de Admisión y Enfermería para controlar el acceso operativo al sistema.

Prioridad: Should Have

Trazabilidad: CU18, RF22

Criterios de Aceptación:

El administrador debe poder buscar, editar y guardar la configuración de acceso del personal.

ÉPICA 2: Configuración y Disponibilidad Operativa
HU07 - Configuración de Duración de Atención

Como Administrador, quiero configurar la duración de atención por especialidad para que el sistema calcule correctamente la generación de cupos.

Prioridad: Must Have

Trazabilidad: CU19, RF23

Criterios de Aceptación:

Cada especialidad debe permitir una duración distinta (ej. 20 minutos) (RN29).

La modificación de la duración debe impactar automáticamente en la futura generación de tarjetas de disponibilidad.

HU08 - Gestionar UPS y Especialidades

Como Administrador, quiero crear y configurar UPS y Especialidades para estructurar la oferta de servicios de la posta.

Prioridad: Must Have

Trazabilidad: RF13, RF14

Criterios de Aceptación:

Las UPS configuradas deben mantenerse de uso estrictamente interno y no ser visibles para los pacientes (RN07).

HU09 - Configuración de Programación Operativa

Como Administrador, quiero definir las jornadas de los médicos para establecer la oferta base de turnos.

Prioridad: Must Have

Trazabilidad: CU10, RF15A

Criterios de Aceptación:

La programación debe incluir especialidad, médico, turno, duración y cupos totales (RN17).

El sistema debe restringir cualquier intento de modificar una atención o programación que ya haya iniciado (RN09).

HU10 - Habilitación de Disponibilidad

Como Admisión, quiero habilitar la programación existente para abrir los cupos al público web y presencial.

Prioridad: Must Have

Trazabilidad: CU11, RF15

Criterios de Aceptación:

Admisión no debe poder crear nuevos horarios fuera de la planificación del administrador (RN06).

Al confirmar la apertura, la disponibilidad debe generarse automáticamente dividiendo el turno según la duración configurada (RN08).

HU11 - Ajustar Disponibilidad Futura

Como Admisión, quiero modificar los cupos por rotación o ausencia para mantener la oferta real actualizada.

Prioridad: Should Have

Trazabilidad: CU12, RF16

Criterios de Aceptación:

El ajuste operativo debe limitarse única y estrictamente a jornadas futuras (RN18).

HU12 - Gestionar Sobrecupos

Como Admisión, quiero registrar excepciones de disponibilidad para atender casos fuera del stock regular.

Prioridad: Could Have

Trazabilidad: CU20, RF24

Criterios de Aceptación:

La disponibilidad extraordinaria generada (sobrecupo) debe permanecer totalmente oculta para los pacientes en la plataforma web (RN16).

ÉPICA 3: Consulta y Reserva de Citas
HU13 - Consultar Especialidades

Como Paciente, quiero visualizar la lista de especialidades para iniciar mi proceso de reserva.

Prioridad: Must Have

Trazabilidad: CU04, RF07

Criterios de Aceptación:

El sistema debe mostrar únicamente las especialidades con disponibilidad, ocultando las jerarquías internas de UPS (RN07).

HU14 - Consultar Disponibilidad

Como Paciente, quiero consultar los horarios y médicos por especialidad para elegir mi turno ideal.

Prioridad: Must Have

Trazabilidad: CU05, RF08

Criterios de Aceptación:

El paciente debe visualizar los turnos segmentados únicamente entre Mañana (08:00–13:30) y Tarde (15:00–19:00) (RN27, RN28).

HU15 - Reserva de Cita Web

Como Paciente, quiero seleccionar un horario y confirmar mi reserva para asegurar mi atención.

Prioridad: Must Have

Trazabilidad: CU06, RF09

Criterios de Aceptación:

La reserva confirmada debe descontar inmediatamente un cupo del stock compartido (RN04).

El sistema debe bloquear mediante validación la duplicidad de reservas activas para el mismo paciente y horario (RN31).

HU16 - Registro de Cita Presencial

Como Admisión, quiero registrar reservas físicas para atender a los pacientes en ventanilla.

Prioridad: Must Have

Trazabilidad: CU07, RF10

Criterios de Aceptación:

La reserva presencial debe consumir cupos exactamente del mismo stock que utiliza la web (RN04).

HU17 - Generación de Ticket

Como Sistema, quiero emitir un comprobante automático al finalizar una reserva para garantizar la formalidad del proceso.

Prioridad: Must Have

Trazabilidad: CU09, RF12

Criterios de Aceptación:

Toda reserva confirmada (web o presencial) debe generar obligatoriamente un ticket (RN12).

El ticket debe contener Especialidad, Médico, Hora y Fecha.

HU18 - Cancelación de Cita

Como Paciente, quiero cancelar mi reserva activa para liberar mi cupo.

Prioridad: Should Have

Trazabilidad: CU08, RF11

Criterios de Aceptación:

La cancelación solo debe permitirse si se solicita antes de iniciar el periodo de triaje (20 minutos antes del turno) (RN13, RN36).

La cancelación exitosa debe devolver inmediatamente la disponibilidad al stock (RN14).

ÉPICA 4: Triaje y Trazabilidad
HU19 - Registro de Triaje

Como Enfermería, quiero registrar la evaluación inicial del paciente para habilitarlo para atención.

Prioridad: Must Have

Trazabilidad: CU13, RF17

Criterios de Aceptación:

El registro debe ser exclusivo del rol de enfermería (RN20).

Solo se permite ingresar peso, talla, temperatura, presión y observación (sin historia clínica) (RN22, RN23).

El paciente debe tener una cita activa y el registro debe cambiar el estado a "En triaje".

HU20 - Actualizar Estado de Atención

Como Enfermería, quiero actualizar los estados de la cita para mantener la trazabilidad operativa.

Prioridad: Must Have

Trazabilidad: CU14, RF18

Criterios de Aceptación:

Las transiciones deben respetar el flujo estricto: Pendiente → En triaje → Listo atención → No asistió (RN21).

Cada transición debe generar un registro de auditoría en la base de datos (RN30).

HU21 - Consultar Trazabilidad

Como Paciente, quiero consultar el estado de mi cita para hacer seguimiento a mi atención.

Prioridad: Should Have

Trazabilidad: CU15, RF19

Criterios de Aceptación:

El paciente debe visualizar el estado actualizado en tiempo real sin requerir intervención administrativa.

ÉPICA 5: Avisos Operativos (MVP Complementario)
HU22 - Registrar Aviso de Atención Inmediata

Como Paciente, quiero enviar un aviso indicando mi intención de asistir de inmediato para alertar a enfermería.

Prioridad: Could Have

Trazabilidad: CU16, RF20

Criterios de Aceptación:

El aviso debe registrarse sin generar una reserva médica real ni alterar el stock (RN24).

El aviso no debe otorgar prioridad automática sobre el orden de citas (RN25).

HU23 - Visualizar Panel de Avisos

Como Enfermería, quiero visualizar un panel con las notificaciones de los pacientes para estar preparado ante llegadas inminentes.

Prioridad: Could Have

Trazabilidad: CU17, RF21

Criterios de Aceptación:

El panel debe ser visible exclusiva y únicamente por el personal de enfermería (RN26).

Los avisos deben poder transicionar entre estados: Pendiente, Visualizado y Cerrado.

**HU24- Reserva de Cita Web**
* **Como** Paciente, **quiero** seleccionar un horario y confirmar mi reserva **para** asegurar mi atención.
* **Prioridad:** Must Have
* **Trazabilidad:** CU06, RF09
* **Criterios de Aceptación:**
    * La reserva confirmada debe descontar inmediatamente un cupo del stock compartido (RN04).
    * El sistema debe bloquear mediante validación la duplicidad de reservas activas para el mismo paciente y horario (RN31).
    * El sistema solo debe permitir reservar citas para el mismo día si es de Lunes a Viernes, o para el Lunes si la operación se realiza un día Sábado. Las reservas en Domingo están bloqueadas (RN37).
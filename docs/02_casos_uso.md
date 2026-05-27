# CASOS DE USO

Proyecto:
Sistema Web de Gestión de Citas Médicas – Posta de Salud

Versión:
1.0

---

# ACTORES

PAC → Paciente  
RESP → Responsable del menor  
ADM → Admisión  
ENF → Enfermería  
ADMIN → Administrador

---

# DIAGRAMA GENERAL (TEXTUAL)

PAC / RESP
↓

Autenticación

↓

Consultar Especialidades

↓

Consultar Disponibilidad

↓

Reservar

↓

Generar Ticket

↓3

Llegada

↓

Triaje

↓

Atención

---



CU01 Iniciar sesión

## Actor:

Usuario

## Actores especializados:

Paciente
Administrador
Admisión
Enfermería

## Objetivo:

Ingresar según rol.

## Precondiciones: 

- Usuario registrado.
- Cuenta activa.
- Credenciales existentes.

## Flujo principal:

1. Abrir pantalla Login.
2. Ingresar DNI.
3. Ingresar contraseña.
4. Seleccionar Ingresar.
5. Sistema valida credenciales.
6. Sistema identifica rol.
7. Sistema redirige al panel correspondiente.

## Flujos alternativos

A1 Credenciales incorrectas
5A.Sistema rechaza acceso.

Mostrar mensaje.
A2 Cuenta inactiva
5B.Mostrar restricción.

A3 Contraseña olvidada
3A.Seleccionar recuperar acceso.

## Postcondiciones:

Sesión iniciada.

## Reglas asociadas

RN01 Acceso controlado

RN02 Gestión de cuenta

RN03 Recuperación acceso

## Requisitos asociados

RF01 Autenticación

RF02 Gestión acceso

## Historia de usuario asociada
HU01

Como usuario

quiero iniciar sesión

para acceder al sistema.

## Vista asociada
Login

## Prueba asociada

CP01

Validar acceso.

##### MÓDULO PACIENTES


## CU04 — Visualizar perfil

## Objetivo

Permitir al paciente consultar su información registrada y actualizar datos permitidos.

## Actor principal

Paciente

## Precondiciones

El paciente inició sesión.
La cuenta se encuentra habilitada.
El paciente posee información registrada.

## Flujo principal
El paciente accede al módulo Perfil.
El sistema recupera información registrada.
El sistema muestra:
Nombre.
DNI.
Número de historia clínica.
Dirección.
Teléfono.
Establecimiento asignado.
El paciente modifica información permitida.
El sistema valida cambios.
El sistema guarda actualización.
## Flujos alternativos

FA01 — Datos inválidos.

El sistema rechaza actualización de número de celular.

## Postcondiciones
Información del perfil visualizada.
Opcionalmente se actualiza:
Número celular.
Contraseña.

## Reglas asociadas

RN02 Gestión de cuenta
RN02-A Recuperación de acceso
RN35 Continuidad

## CU05 — Consultar disponibilidad

## Objetivo

Permitir al paciente consultar horarios disponibles para una especialidad.

## Actor principal

Paciente

## Precondiciones

El paciente inició sesión.
Existe disponibilidad publicada para la jornada.

## Flujo principal

El paciente accede al módulo Disponibilidad.
El sistema muestra listado de especialidades.
El paciente selecciona especialidad.
El sistema recupera disponibilidad.
El sistema muestra tarjetas de atención.
El paciente visualiza:
Horario
Médico asignado
Cupos disponibles
El paciente selecciona una tarjeta (opcional).

## Flujos alternativos

FA01 — Sin disponibilidad

El sistema informa que no existen cupos disponibles.

FA02 — Jornada cerrada

El sistema informa que la reserva no se encuentra habilitada.

## Postcondiciones

Disponibilidad visualizada.
El paciente podrá continuar con el registro de cita.

## Reglas de negocio asociadas

RN05 Publicación operativa

RN07 Especialidades visibles

RN08 Generación automática

RN10 Orden de reserva

RN17 Configuración administrativa

RN27 Turnos

RN28 Horario operativo

RN29 Duración configurable

RN37 Ventana estricta de reserva


## CU06 — Registrar cita médica

## Objetivo

Permitir reservar una cita médica disponible.

## Actor principal

Paciente

## Precondiciones
El paciente inició sesión.
Existe disponibilidad publicada.
El paciente no posee reserva activa para mismo horario.


## Flujo principal
El paciente accede al registro.
El sistema muestra especialidades.
El paciente selecciona especialidad.
El sistema muestra los turnos, mañana y tarde.
El usuario selecciona  turno.
El sistema muestra las tarjetas disponibles asignadas al turno.(Las tarjetas contienen el horario por ejemplo: 9:00 - 9:15 y abajo Dr. Luis Quispe Mendoza, todo en la misma tarjeta)
El paciente selecciona horario con el médico que atenderá.
El sistema muestra un resumen de la reserva.
El paciente confirma.
El sistema registra reserva.
El sistema genera ticket.
## Flujos alternativos

FA01 — Sin disponibilidad.

El sistema informa cupos agotados.

FA02 — El usuario intenta reservar una cita en otra especialidad.

Reserva rechaza y muestra un mensaje aclarativo, indicando que solo puede reservar uno por ese día.

FA03 - El usuario ingresa el día sábado a registrar una cita y el sistema muestra un mensaje indicando que puede realizar la reserva para ese día y el día lunes.
Flujo:
1. El usuario ingresa el día sábado a registrar una cita.
2. El sistema muestra un mensaje (indicando que puede realizar la reserva para ese día y el día lunes) y las tarjetas para seleccionar sábado o lunes.
3. El usuario selecciona el día sábado o lunes para registrar una cita.
4. El sistema muestra las tarjetas disponibles asignadas al turno.(Las tarjetas contienen el horario por ejemplo: 9:00 - 9:15 y abajo Dr. Luis Quispe Mendoza, todo en la misma tarjeta) y el paciente selecciona horario con el médico que atenderá. 
5. El sistema muestra un resumen de la reserva.
6. El paciente confirma.
7. El sistema registra reserva.
8. El sistema genera ticket.

## Postcondiciones

Cita registrada.
Ticket generado.
Disponibilidad actualizada.
Estado asistencia:

## Reglas asociadas

RN04 Stock compartido
RN07 Especialidades visibles
RN10 Orden de reserva
RN11 Reserva de cita
RN12 Ticket obligatorio
RN31 Integridad
RN37 Ventana estricta de reserva

## CU07 — Visualizar historial de citas

##  Objetivo

Permitir consultar citas realizadas.

## Actor principal

Paciente

## Precondiciones

El paciente inició sesión.

## Flujo principal

El paciente abre Historial.
El sistema recupera citas.
El sistema muestra:
Fecha.
Especialidad.
Médico.
Estado asistencia.
Estado atención.

## Flujos alternativos

FA01 — Sin historial.

Mostrar lista vacía.

## Reglas asociadas

RN30 Seguimiento
RN38 Estados de asistencia

## Postcondiciones

Historial visualizado.

## CU08 — Visualizar triajes registrados

## Objetivo

Consultar registros operativos asociados a citas.

## Actor principal

Paciente

## Precondiciones

El paciente inició sesión.

Existe al menos una cita con triaje.

## Flujo principal
El paciente abre Mis Triajes.
El sistema obtiene citas.
El sistema recupera triajes asociados.
El sistema muestra:
Fecha.
Peso.
Talla.
Temperatura.
Presión.
Observación.
## Flujos alternativos

FA01 — Sin triajes.

Mostrar mensaje informativo.

## Postcondiciones

Triajes visualizados.

## Reglas asociadas

RN22 Registro básico
RN23 Historial limitado
RN30 Seguimiento

## CU09 — Cancelar cita médica

## Objetivo

Cancelar una cita reservada.

## Actor principal

Paciente

## Precondiciones

El paciente inició sesión.
Existe cita activa.
La hora actual se encuentra dentro del periodo permitido.

## Flujo principal
El sistema actualiza la vista y luego de haberse reservado la cita cuando ingresas a resrvar cita te aparecen dos opciones : detalle de cita y cancelar cita.
El paciente hace clic en cancelar cita.
El sistema verifica que no se haya iniciado triaje.
El sistema muestra ventana de confirmación.
El paciente confirma cancelación.
El sistema libera cupo.
El sistema actualiza estado.

## Flujos alternativos

FA01 — Triaje iniciado.

Cancelar deshabilitado y muestra el mensaje en letras plomas de por qué ya no se puede cancelar.

## Postcondiciones

Cita cancelada.
Cupo liberado.
Estado actualizado:
Cancelada

## Reglas asociadas

RN13 Restricción de cancelación
RN14 Liberación de cupo
RN36 Inicio de triaje
RN38 Estados de asistencia

## CU10 — Visualizar detalle de cita

## Objetivo

Consultar información detallada.

## Actor principal

Paciente

## Precondiciones

El paciente inició sesión.
Existe cita registrada.

## Flujo principal

El paciente selecciona el botón de reservar cita.
El sistema muestra dos opciones : "detalle de cita" y "cancelar cita".
El paciente selecciona la opción "detalle de cita".
El sistema muestra el ticket.
Especialidad.
Horario.
Médico.
Estado.
Número de atención.

## Flujos alternativos

FA01 — Cita inexistente.

Mostrar mensaje.

## Postcondiciones

Detalle visualizado y descargar en pdf.

## Reglas asociadas

RN12 Ticket obligatorio
RN30 Seguimiento


## CU11 — Visualizar y descargar ticket

## Objetivo

Permitir visualizar y descargar el ticket de una cita registrada.

## Precondiciones:

El paciente inició sesión.
Existe cita registrada.

## Flujo principal

1 Seleccionar cita
2 Visualizar ticket o Descargar PDF

## RN asociada:

RN12 Ticket obligatorio


## CU12 - Registrar aviso de atención inmediata

### Objetivo
Permitir al paciente registrar y enviar un aviso de atención inmediata para alertar al personal de enfermería sobre una necesidad urgente.

### Actor principal
Paciente

### Precondiciones
El paciente inició sesión.

### Flujo principal
1. El paciente accede al módulo o pestaña "Atención Inmediata".
2. El paciente ingresa el motivo o malestar de su solicitud (máximo 300 caracteres).
3. El paciente hace clic en "Enviar Solicitud a Enfermería".
4. El sistema valida que el motivo no esté vacío y no supere los 300 caracteres.
5. El sistema registra el aviso en estado `Pendiente`.
6. El sistema muestra un mensaje de éxito indicando que el aviso ha sido registrado y que enfermería ha sido notificada.

### Flujos alternativos
FA01 — Datos de motivo inválidos:
El paciente intenta enviar un motivo vacío o que excede el límite de 300 caracteres.
El sistema muestra un mensaje de alerta y no permite el registro.

### Postcondiciones
- Aviso de atención inmediata registrado en estado `Pendiente`.
- El aviso se refleja automáticamente en la vista de solicitudes de atención del personal de enfermería.

### Reglas asociadas
- RN24 Aviso informativo
- RN25 Sin prioridad automática
- RN26 Visualización restringida

### Requisitos funcionales asociados
- RF20 Registrar aviso de atención inmediata
- RF21 Visualizar avisos


##### MÓDULO ENFERMERÍA

---

## CU13 — Buscar paciente por DNI

### Actor principal

Enfermera

### Objetivo

Recuperar automáticamente información del paciente para registrar el triaje.

### Precondiciones

- Enfermera autenticada.
- Paciente con cita activa.

### Flujo principal

1. Abrir módulo Nuevo Triaje.
2. Ingresar DNI.
3. Seleccionar Buscar.
4. El sistema recupera:

   - Nombre
   - Apellidos
   - Especialidad
   - Médico
   - Hora de atención

5. El formulario se completa automáticamente.

### Flujos alternativos

#### Paciente no encontrado

Mostrar mensaje.

#### Paciente sin cita activa

Mostrar restricción.

### Postcondiciones

- Formulario precargado.

### Reglas de negocio asociadas

RN20 Registro exclusivo
RN23 Historial limitado
RN39 Cierre de jornada
RN30 Seguimiento
RN40 Precarga por DNI

### Requisitos funcionales asociados

- RF17 Buscar paciente.

### Historia de usuario asociada

- HU13 Buscar paciente por DNI.

---

## CU14 — Registrar nuevo triaje

### Actor principal

Enfermera

### Objetivo

Registrar información previa a la atención médica.

### Precondiciones

- Paciente identificado.
- Cita activa.

### Flujo principal

1. Sistema recupera datos del paciente.
2. Completa:
   - Peso
   - Talla
   - Presión arterial
   - Temperatura
   - Observaciones
10. Selecciona Guardar.
11. Sistema registra el triaje.
12. Sistema actualiza el estado de la cita.

## Postcondiciones
Triaje registrado correctamente.
Estado de asistencia actualizado a:
Asistió
Estado de atención actualizado a:
En triaje

### Reglas de negocio asociadas

RN19 Triaje obligatorio
RN20 Registro exclusivo
RN22 Registro básico
RN30 Seguimiento
RN36 Inicio de triaje
RN38 Estados de asistencia
RN23 Historial limitado
RN39 Cierre de jornada
RN40 Precarga por DNI

### Requisitos funcionales asociados

- RF18 Registrar triaje.

### Historia de usuario asociada

- HU14 Registrar triaje.

---

##  CU15 — Visualizar solicitudes de atención inmediata

### Actor principal

Enfermera

### Objetivo

Visualizar solicitudes enviadas por pacientes.

### Precondiciones

- Enfermera autenticada.

### Flujo principal

1. Abrir panel de solicitudes.

2. El sistema muestra:

   - Paciente
   - Hora
   - Motivo
   - Estado

3. Seleccionar solicitud.

4. Revisar detalle.

### Estados permitidos

```
Pendiente
↓
Visualizado
↓
Cerrado
```

### Postcondiciones

- Solicitud revisada.

### Reglas de negocio asociadas

RN30 Seguimiento
RN32 Auditoría
RN39 Cierre de jornada
RN26 Visualización restringida

### Requisitos funcionales asociados

- RF20 Visualizar solicitudes.

### Historia de usuario asociada

- HU15 Gestionar avisos.

---

## CU16 — Actualizar estado de solicitud

### Actor principal

Enfermera

### Objetivo

Registrar seguimiento de solicitudes.

### Precondiciones

- Solicitud existente.

### Flujo principal

1. Seleccionar solicitud.
2. Actualizar estado.
3. Guardar cambios.

### Estados permitidos

- Visualizado
- Cerrado

### Postcondiciones

- Estado actualizado.

### Reglas de negocio asociadas

RN24 Aviso informativo
RN25 Sin prioridad automática
RN26 Visualización restringida
RN33 Emergencias
RN32 Auditoría

### Requisitos funcionales asociados

- RF21 Actualizar estado solicitud.

### Historia de usuario asociada

- HU16 Actualizar estado solicitud.




# CU17 — Gestionar tablero Kanban de citas del día

## Actor principal
Enfermera

## Objetivo
Visualizar a todos los pacientes del día organizados en un tablero Kanban según su estado de triaje y atención, permitiendo avanzar su flujo con un solo clic.

## Precondiciones
- Enfermera autenticada.
- Existen citas programadas para la fecha actual.

## Flujo principal
1. La enfermera ingresa al Panel de Enfermería.
2. El sistema renderiza el tablero Kanban compuesto por 4 columnas:
   - **Falta Triaje:** Citas en estado *Pendiente*.
   - **En Triaje:** Citas en estado *En Triaje*.
   - **Listo Atención:** Citas en estado *Listo para Atención*.
   - **Finalizados:** Citas en estado *No Asistió* o *Cancelada*.
3. En la columna **Falta Triaje**, el sistema muestra las tarjetas de los pacientes y un botón para "Registrar Triaje".
4. En la columna **En Triaje**, el sistema muestra las tarjetas con botones de acción rápida: "Listo Atención" y "No Asistió".
5. En la columna **Listo Atención**, el sistema muestra las tarjetas de los pacientes triados con un resumen de sus signos vitales (Peso, Talla, Temperatura, Presión arterial y observaciones).
6. En la columna **Finalizados**, el sistema muestra el histórico de citas canceladas o marcadas como inasistencia.
7. La enfermera puede presionar los botones de acción rápida para mover las tarjetas entre las columnas.
8. El sistema actualiza el estado de la cita, registra el cambio en el historial de trazabilidad y refresca el tablero visual.

## Postcondiciones
- El tablero Kanban refleja el estado real y el flujo de los pacientes en la posta de salud.
- Se mantiene el orden operativo y de atención de los pacientes.

## Reglas de negocio asociadas
- RN19 Triaje obligatorio
- RN21 Estados permitidos
- RN30 Seguimiento
- RN38 Estados de asistencia

## Requisitos funcionales asociados
- RF18 Actualizar estado de atención
- RNF07 Trazabilidad


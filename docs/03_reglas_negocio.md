# REGLAS DE NEGOCIO

Proyecto:
Sistema Web de Gestión de Citas Médicas – Posta de Salud

Versión:
1.0

---

# OBJETIVO

Definir restricciones operativas que gobiernan el funcionamiento del sistema.

Las reglas de negocio prevalecen sobre decisiones técnicas.

---

# GESTIÓN DE ACCESO

## RN01 Acceso controlado

El paciente no podrá registrarse libremente.

El acceso deberá ser habilitado administrativamente.

---

## RN02 Gestión de cuenta

El paciente podrá actualizar únicamente:

Permitido:

- Número celular
- Contraseña

No permitido:

- DNI
- Nombre
- Fecha de nacimiento

---

## RN02-A Recuperación de acceso

La recuperación de contraseña requerirá:

- DNI
- Validación del número celular registrado

## RN03 Gestión de menores

Solo un adulto responsable podrá administrar reservas de pacientes menores.

---

# DISPONIBILIDAD

## RN04 Stock compartido

Toda reserva realizada desde:

- Web
- Atención presencial

deberá descontar disponibilidad del mismo conjunto de cupos.

---

## RN05 Publicación operativa

La disponibilidad deberá ser habilitada previamente a la jornada.

---

## RN06 Restricción de publicación

Admisión no podrá crear programación médica.

Solo podrá habilitar disponibilidad previamente configurada.

---

## RN07 Especialidades visibles

Los pacientes visualizarán únicamente:

- Especialidades
- Turnos
- Disponibilidad

No visualizarán:

- UPS
- Configuración interna

---

## RN08 Generación automática

La disponibilidad podrá generarse automáticamente según:

- Duración
- Médico
- Turno
- Cupos

---

## RN09 Modificación futura

Las modificaciones solo afectarán jornadas futuras.

No modificar atención iniciada.

---

# RESERVAS

## RN10 Orden de reserva

La reserva será confirmada según disponibilidad al finalizar el proceso.

---

## RN11 Reserva de cita

La selección seguirá el orden:

Especialidad

↓

Turno

↓

Disponibilidad

↓

Médico

↓

Confirmación

---

## RN12 Ticket obligatorio

Toda reserva confirmada generará ticket.

---

## RN13 Restricción de cancelación

La cancelación de citas estará permitida únicamente antes del inicio del periodo de triaje correspondiente.

---

## RN14 Liberación de cupo

Toda cancelación deberá devolver disponibilidad.

---

## RN15 Restricción de sobrecupo

Los sobrecupos deberán aprobarse manualmente.

---

## RN16 Sobrecupo oculto

La disponibilidad extraordinaria no será visible para pacientes.

---

# PROGRAMACIÓN OPERATIVA

## RN17 Configuración administrativa

La programación será definida administrativamente.

Incluye:

- Especialidad
- Médico
- Turno
- Cupos
- Duración

---

## RN18 Rotación operativa

Las variaciones del personal afectarán únicamente programación futura.

---

# TRIAJE

## RN19 Triaje obligatorio

Todo paciente con cita deberá pasar por triaje antes de atención.

---

## RN20 Registro exclusivo

Solo enfermería podrá registrar triaje.

---

## RN21 Estados permitidos

Estados válidos:

Pendiente

↓

En triaje

↓

Listo atención

↓

No asistió

---

## RN22 Registro básico

El triaje registrará únicamente:

- Peso
- Talla
- Temperatura
- Presión
- Observación

---

## RN23 Historial limitado

El sistema no almacenará historia clínica.

Solo registros operativos.

---

# AVISOS DE ATENCIÓN

## RN24 Aviso informativo

El aviso de atención inmediata no genera reserva.

---

## RN25 Sin prioridad automática

La notificación no altera el orden de atención.

---

## RN26 Visualización restringida

Los avisos solo serán visibles para enfermería.

---

# HORARIOS

## RN27 Turnos

Turnos disponibles:

- Mañana
- Tarde

---

## RN28 Horario operativo

Turno mañana:

08:00–13:30

Turno tarde:

15:00–19:00

---

## RN29 Duración configurable

Cada especialidad podrá tener duración distinta.

Ejemplo:

Medicina:
20–25 minutos

---

# TRAZABILIDAD

## RN30 Seguimiento

Toda cita deberá registrar cambios de estado.

---

## RN31 Integridad

No se permitirá duplicidad de reservas activas para mismo paciente y horario.

---

## RN32 Auditoría

Las acciones administrativas deberán quedar registradas.

---

# ALCANCE

## RN33 Emergencias

El sistema no sustituye atención de emergencia.

---

## RN34 Historia clínica

La historia clínica física permanecerá como fuente oficial.

---

## RN35 Continuidad

El sistema complementa el proceso presencial.

## RN36 Inicio de triaje

El proceso de triaje iniciará veinte (20) minutos antes del inicio del turno correspondiente.

Turno mañana:

08:00 atención

07:40 inicio triaje

Turno tarde:

15:00 atención

14:40 inicio triaje

## RN37 Ventana estricta de reserva
La fecha de la reserva web está estrictamente condicionada por el día de la solicitud:
- **Lunes a Viernes:** El sistema solo permitirá reservar cupos correspondientes a la fecha actual (mismo día).
- **Sábado:** El sistema solo permitirá reservar cupos correspondientes al día sábado y Lunes de la siguiente semana (fecha actual + 2 días).
- **Domingo:** El sistema no habilitará la reserva web.

## RN38 Estados de asistencia

Reservadacu
Asistió
No asistió
Cancelada

## RN39 — Cierre de jornada

Descripción:

Las citas que finalizaron el turno y no registraron triaje
podrán marcarse como No asistió.

## RN40 Precarga por DNI

La búsqueda por DNI recuperará datos existentes.
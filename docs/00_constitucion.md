# CONSTITUCIÓN DEL PROYECTO

## Información general

Proyecto:
Sistema Web de Gestión de Citas Médicas – Posta de Salud

Versión:
1.0

Metodología:
Spec-Driven Development (SDD)

Arquitectura:
MVC

Tecnologías:

- ASP.NET MVC (.NET)
- SQL Server
- Entity Framework Core
- Visual Studio

---

# PROPÓSITO

Desarrollar un sistema web que apoye la gestión de citas médicas ambulatorias de una posta pública mediante digitalización parcial del proceso de reserva, control de disponibilidad y trazabilidad del paciente.

El sistema busca complementar el proceso presencial sin reemplazar el modelo operativo existente.

---

# CONTEXTO DEL PROBLEMA

Actualmente la atención presenta características operativas como:

- Alta demanda de citas.
- Formación presencial temprana.
- Disponibilidad limitada.
- Dependencia administrativa manual.
- Historia clínica física.
- Falta de visualización anticipada de disponibilidad.
- Pérdida de cupos por inasistencia.

En algunos casos las citas se agotan durante las primeras horas de atención.

---

# OBJETIVO GENERAL

Desarrollar un sistema de gestión de citas médicas que permita mejorar el acceso a disponibilidad y mantener trazabilidad básica del proceso de atención.

---

# OBJETIVOS ESPECÍFICOS

OE01  
Permitir reserva de citas médicas mediante plataforma web.

OE02  
Centralizar disponibilidad de atención presencial y virtual.

OE03  
Permitir trazabilidad del estado de atención.

OE04  
Registrar información básica de triaje.

OE05  
Disminuir pérdida de cupos.

OE06  
Permitir administración controlada de horarios.

---

# ALCANCE

## Incluye

- Solicitud y administración de credenciales.
- Gestión de pacientes.
- Reserva de citas.
- Consulta de disponibilidad.
- Gestión de especialidades.
- Gestión de UPS.
- Generación de ticket.
- Registro de triaje.
- Seguimiento de estado.
- Aviso de atención inmediata.
- Gestión de menores mediante responsable.
- Registro presencial de citas por admisión.
- Publicación de disponibilidad para jornadas posteriores.
- Ajuste de disponibilidad por rotación operativa del personal.

---

## No incluye

- Historia clínica electrónica.
- Gestión médica clínica.
- Recetas.
- Farmacia.
- Referencias.
- Gestión de emergencias.
- Gestión de turnos del personal.
- Hospitalización.

---

# ACTORES

## Paciente

Solicita citas y consulta disponibilidad.

---

## Responsable del menor

Gestiona reservas para menores.

---

## Admisión

Gestiona disponibilidad.

Registra citas presenciales.

Publica disponibilidad.

Ajusta horarios según disponibilidad operativa.

Administra acceso de pacientes.

---

## Enfermería

Registra triaje.

Visualiza avisos de atención inmediata.

---

## Administrador

Configura estructura operativa.

---

# PRINCIPIOS DE DISEÑO

## Continuidad operativa

El sistema no reemplaza atención presencial.

---

## Disponibilidad compartida

Las reservas presenciales y web utilizarán el mismo stock.

---

## Trazabilidad

Cada cambio de estado deberá quedar registrado.

---

## Simplicidad

Priorizar implementación funcional.

---

# DECISIONES FUNCIONALES

## Reserva

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

## Gestión de acceso

El paciente solicitará credenciales.

Admisión habilitará el acceso.

---

## Menores

El adulto responsable podrá gestionar reservas.

---

## Disponibilidad

Las especialidades serán visibles para pacientes.

Las UPS serán internas.

---

## Triaje

Registrado únicamente por enfermería.

---

## Atención inmediata

Solo registra aviso.

No garantiza atención.

---

## Sobrecupo

Permitido únicamente mediante aprobación operativa.

---

## Gestión operativa

Admisión podrá registrar citas presenciales.

Las reservas presenciales y web compartirán disponibilidad.

La disponibilidad podrá publicarse anticipadamente para jornadas futuras según planificación operativa.

---

# MÓDULOS

M01 Autenticación

M02 Gestión de Pacientes

M03 Gestión de Citas

M04 Gestión de Disponibilidad

M05 Triaje

M06 Administración

M07 Seguimiento

M08 Aviso de Atención

---

# CRITERIOS DE ÉXITO

El sistema será considerado exitoso si permite:

- Registrar reservas.
- Visualizar disponibilidad.
- Mantener cupos consistentes.
- Registrar triaje.
- Generar trazabilidad.
- Ejecutar flujo completo.

---

# RESTRICCIONES

Tiempo limitado.

Implementación MVP.

Adaptación al proceso actual.

No desplazar atención presencial.
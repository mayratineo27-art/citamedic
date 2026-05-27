# Mapa de Funcionalidades y Cumplimiento de Requisitos

Este documento presenta el mapeo completo de los requisitos funcionales (RF), casos de uso (CU) y reglas de negocio (RN) del sistema **PostaCitasWeb**, contrastando el estado de implementación actual y las tareas necesarias para lograr el cumplimiento del 100% de las especificaciones del diseño de software dirigido por especificaciones (SpecDD).

---

## 1. Mapeo General de Módulos del Sistema

| Módulo | Casos de Uso (CU) | Requisitos Funcionales (RF) | Reglas de Negocio Asociadas (RN) | Estado de Implementación |
| :--- | :--- | :--- | :--- | :--- |
| **M1: Autenticación e Identidad** | CU01, CU02, CU03, CU18 | RF01, RF02, RF03, RF04, RF-25 | RN01, RN02, RN02-A, RN03 | **Completo / Implementado** (Cuentas activas, roles y DNI local) |
| **M2: Pacientes (Padrón)** | CU18 | RF05, RF06 | RN02, RN03 | **Completo / Implementado** (Dashboard Pacientes + Búsqueda DNI local) |
| **M3: Directorio Médico** | CU18 | RF22 | - | **Completo / Implementado** (Directorio médico con Edición y CMP único) |
| **M4: UPS y Especialidades** | CU19 | RF13, RF14, RF23 | RN07, RN29 | **Completo / Implementado** (Mosaico de UPS y Especialidades con duración) |
| **M5: Programación Operativa** | CU10, CU11, CU12 | RF15, RF15A, RF16 | RN05, RN06, RN08, RN09, RN17, RN18, RN27, RN28 | **Parcial** (Lógica de creación y habilitación/ajuste de disponibilidad) |
| **M6: Gestión de Citas y Tickets** | CU06, CU07, CU08, CU09, CU20 | RF07, RF08, RF09, RF10, RF11, RF12, RF24 | RN04, RN10, RN11, RN12, RN13, RN14, RN15, RN16, RN31, RN37 | **Por Implementar** (Flujo de reserva activa/presencial y generación de Ticket) |
| **M7: Triaje y Trazabilidad** | CU13, CU14, CU15 | RF17, RF18, RF19 | RN19, RN20, RN21, RN22, RN23, RN30, RN34, RN36 | **Por Implementar** (Flujo de ingreso de peso/talla por Enfermería) |
| **M8: Avisos de Atención** | CU16, CU17 | RF20, RF21 | RN24, RN25, RN26 | **Por Implementar** (Avisos de atención inmediata para Enfermería) |

---

## 2. Detalle de Requisitos y Reglas por Módulo

### Épica 1: Autenticación, Usuarios e Identidad
* **Requisitos y Casos de Uso:** RF01, RF02, RF03, RF04, RF-25, CU01, CU02, CU03, CU18.
* **Reglas de Negocio:**
  - **RN01 (Acceso controlado):** Habilitación administrativa por Admisión (`Activo = false` inicial).
  - **RN02 (Gestión de cuenta):** El paciente solo edita celular y contraseña. DNI, nombres y fecha de nacimiento son de solo lectura.
  - **RN02-A:** Recuperación mediante DNI + celular coincidente.
  - **RN03 (Menores):** Un adulto responsable administra reservas del menor.
* **Mapeo Técnico:**
  - `SeedData.cs` inicializa los roles de Paciente, Admisión, Enfermería, Médico y Administrador con DNI real y hash estático de BCrypt.
  - Formulario de Registro de Usuarios en `Usuarios.cshtml` autocompleta con datos de pacientes si el DNI existe en el sistema local, de lo contrario abre edición manual.

### Épica 2: Configuración, UPS y Especialidades
* **Requisitos y Casos de Uso:** RF13, RF14, RF23, CU19.
* **Reglas de Negocio:**
  - **RN07 (Especialidades visibles):** El paciente ve Especialidades y Turnos, pero no la UPS interna.
  - **RN29 (Duración configurable):** Especialidades con duración parametrizable (ej. 20 min).
* **Mapeo Técnico:**
  - Capas de persistencia y controladores completos para UPS, Especialidades y Médicos.

### Épica 3: Programación Operativa y Disponibilidad
* **Requisitos y Casos de Uso:** RF15, RF15A, RF16, CU10, CU11, CU12.
* **Reglas de Negocio:**
  - **RN05, RN06, RN08, RN09, RN17, RN18.**
  - **RN27, RN28 (Turnos y Horarios):** Mañana (08:00–13:30) y Tarde (15:00–19:00).
* **Mapeo Técnico:**
  - El administrador crea la programación base y el personal de Admisión publica/deshabilita la disponibilidad. Se requiere implementar el cálculo de slots en base a la duración y rango del turno al habilitar.

### Épica 4: Reserva de Citas, Tickets y Sobrecupos (Pendiente de Lógica)
* **Requisitos y Casos de Uso:** RF07, RF08, RF09, RF10, RF11, RF12, RF24, CU06, CU07, CU08, CU09, CU20.
* **Reglas de Negocio:**
  - **RN04 (Stock compartido):** Reservas web y presenciales consumen los mismos cupos.
  - **RN12 (Ticket obligatorio):** Cita confirmada crea una transacción que inserta Cita y Ticket en un solo bloque.
  - **RN13 (Restricción de cancelación):** Cancelar únicamente antes de triaje (20 min antes del turno: antes de 07:40 para mañana y 14:40 para tarde).
  - **RN15, RN16 (Sobrecupos):** Aprobados manualmente por Admisión y ocultos para pacientes.
  - **RN31 (Integridad):** No permitir duplicación de citas activas del mismo paciente en el mismo horario.
  - **RN37 (Ventana estricta):** Reservas solo para el mismo día (lunes a viernes), sábado (para el lunes) y domingo inactivo.

### Épica 5: Triaje, Trazabilidad y Avisos de Atención (Pendiente de Lógica)
* **Requisitos y Casos de Uso:** RF17, RF18, RF19, RF20, RF21, CU13, CU14, CU15, CU16, CU17.
* **Reglas de Negocio:**
  - **RN19 (Triaje obligatorio):** Triar antes de atención.
  - **RN20 (Registro exclusivo):** Solo Enfermería registra triaje.
  - **RN21, RN22 (Registro básico):** Estados válidos (*Pendiente*, *En triaje*, *Listo atención*, *No asistió*). Datos limitados (peso, talla, temperatura, presión y observación).
  - **RN23, RN34 (No Historia Clínica):** Se prohíbe explícitamente almacenar datos de diagnóstico o tratamiento médico.
  - **RN24, RN25, RN26 (Avisos):** Avisos informativos enviados por pacientes virtuales a la Enfermería física sin alterar el orden.
  - **RN30, RN32 (Auditoría):** Tabla de logs `HistorialEstadoCita`.

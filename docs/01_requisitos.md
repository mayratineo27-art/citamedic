# REQUISITOS DEL SISTEMA

**Proyecto:** Sistema Web de Gestión de Citas Médicas – Posta de Salud  
**Versión:** 1.0

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

| Campo | Valor |
|-------|-------|
| **Actor** | Todos |
| **Descripción** | Permitir acceso según rol |
| **Prioridad** | Alta |

---

## RF03 Gestionar menores

| Campo | Valor |
|-------|-------|
| **Actor** | Responsable del menor |
| **Descripción** | Permitir administrar pacientes menores |
| **Acciones** | • Registrar dependiente<br>• Reservar<br>• Visualizar historial de citas |
| **Prioridad** | Alta |

---

## RF04 Consultar perfil

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente |
| **Descripción** | Permitir que el paciente consulte su perfil desde su dashboard |
| **Visualizar** | • Nombres y Apellidos<br>• DNI<br>• Nro. Historia Clínica<br>• Seguro de Salud SIS<br>• Establecimiento asignado<br>• Dirección domiciliaria |
| **Prioridad** | Media |

---

## RF05 Actualizar datos permitidos

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente |
| **Descripción** | Modificar información del perfil |
| **Modificar** | • Número celular<br>• Contraseña |
| **Restricción** | No modificar: DNI, Nombre, Fecha de nacimiento |
| **Prioridad** | Media |

---

# MÓDULO 2 — PACIENTES

*Los requisitos RF04 y RF05 pertenecen a este módulo*

---

# MÓDULO 3 — CITAS

## RF07 Consultar especialidades

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente |
| **Descripción** | Visualizar especialidades disponibles |
| **Restricción** | No visualizar UPS |
| **Prioridad** | Alta |

---

## RF08 Consultar disponibilidad

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente |
| **Descripción** | Consultar cupos por especialidad, turno, médico y horario |
| **Consultar por** | • Especialidad<br>• Turno<br>• Médico<br>• Horario |
| **Prioridad** | Alta |

---

## RF09 Reservar cita

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente |
| **Descripción** | Reservar mediante flujo guiado |
| **Flujo** | Especialidad → Turno → Tarjeta disponible → Confirmar |
| **Reglas** | • Descontar cupo<br>• Generar ticket |
| **Prioridad** | Alta |

---

## RF10 Registrar cita presencial

| Campo | Valor |
|-------|-------|
| **Actor** | Admisión |
| **Descripción** | Registrar reservas realizadas presencialmente |
| **Reglas** | • Compartir stock con web |
| **Prioridad** | Alta |

---

## RF11 Cancelar cita

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente |
| **Descripción** | Cancelar reserva |
| **Reglas** | • Liberar cupo<br>• Solo antes del inicio del triaje |
| **Prioridad** | Alta |

---

## RF12 Generar ticket

| Campo | Valor |
|-------|-------|
| **Actor** | Sistema |
| **Descripción** | Emitir comprobante |
| **Datos** | • Especialidad<br>• Médico<br>• Hora<br>• Fecha |
| **Prioridad** | Alta |

---

# MÓDULO 4 — DISPONIBILIDAD

## RF13 Gestionar UPS

| Campo | Valor |
|-------|-------|
| **Actor** | Administrador |
| **Descripción** | Gestionar UPS |
| **Acciones** | • Crear<br>• Editar<br>• Desactivar |
| **Prioridad** | Alta |

---

## RF14 Gestionar especialidades

| Campo | Valor |
|-------|-------|
| **Actor** | Administrador |
| **Descripción** | Configurar especialidades |
| **Configurar** | • Nombre<br>• UPS<br>• Duración atención |
| **Prioridad** | Alta |

---

## RF15 Habilitar disponibilidad

| Campo | Valor |
|-------|-------|
| **Actor** | Admisión |
| **Descripción** | Habilitar la disponibilidad previamente configurada para la jornada correspondiente |
| **Incluye** | • Abrir cupos<br>• Asociar médico disponible<br>• Confirmar publicación |
| **Restricción** | No podrá crear horarios fuera de la planificación establecida |
| **Prioridad** | Alta |

---

## RF15A Configurar programación operativa

| Campo | Valor |
|-------|-------|
| **Actor** | Administrador |
| **Descripción** | Configurar programación futura |
| **Incluye** | • Especialidad<br>• Médico<br>• Turno<br>• Duración<br>• Cupos |
| **Prioridad** | Alta |

---

## RF16 Ajustar disponibilidad

| Campo | Valor |
|-------|-------|
| **Actor** | Admisión |
| **Descripción** | Modificar disponibilidad futura |
| **Motivos** | • Rotación<br>• Ausencia<br>• Reprogramación |
| **Restricción** | No modificar atención iniciada |
| **Prioridad** | Alta |

---

# MÓDULO 5 — TRIAJE

## RF17 Registrar triaje

| Campo | Valor |
|-------|-------|
| **Actor** | Enfermería |
| **Descripción** | Registrar signos vitales y observación |
| **Registrar** | • Peso<br>• Talla<br>• Temperatura<br>• Presión<br>• Observación |
| **Prioridad** | Alta |

---

## RF18 Actualizar estado de atención

| Campo | Valor |
|-------|-------|
| **Actor** | Enfermería |
| **Estados** | • Pendiente<br>• En triaje<br>• Listo atención<br>• No asistió |
| **Prioridad** | Alta |

---

## RF19 Consultar trazabilidad

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente |
| **Descripción** | Visualizar estados de la cita |
| **Prioridad** | Media |

---

# MÓDULO 6 — AVISO DE ATENCIÓN

## RF20 Registrar aviso de atención inmediata

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente |
| **Descripción** | Enviar aviso operativo |
| **Restricción** | No genera cita |
| **Prioridad** | Baja |

---

## RF21 Visualizar avisos

| Campo | Valor |
|-------|-------|
| **Actor** | Enfermería |
| **Descripción** | Visualizar panel de avisos |
| **Estados** | • Pendiente<br>• Visualizado<br>• Cerrado |
| **Prioridad** | Baja |

---

# MÓDULO 7 — ADMINISTRACIÓN

## RF22 Gestionar usuarios

| Campo | Valor |
|-------|-------|
| **Actor** | Administrador |
| **Descripción** | Administrar usuarios del sistema |
| **Administrar** | • Pacientes<br>• Enfermería<br>• Admisión |
| **Prioridad** | Media |

---

## RF23 Configurar duración de atención

| Campo | Valor |
|-------|-------|
| **Actor** | Administrador |
| **Descripción** | Configurar duración de atención por especialidad |
| **Ejemplo** | Medicina: 20 minutos |
| **Prioridad** | Alta |

---

## RF24 Gestionar sobrecupos

| Campo | Valor |
|-------|-------|
| **Actor** | Admisión |
| **Descripción** | Registrar excepción de disponibilidad |
| **Restricción** | No visible al paciente |
| **Prioridad** | Baja |

---

# MÓDULO 8 — DERIVADOS

## RF25 Tablero Kanban de Atención

| Campo | Valor |
|-------|-------|
| **Actor** | Enfermería |
| **Descripción** | Vista Kanban con estados: "Falta Triaje", "En Triaje", "Listo Atención", "Finalizados" para mover pacientes rápidamente |
| **Prioridad** | Alta |

---

## RF27 Visualización Global y Filtrado de Citas

| Campo | Valor |
|-------|-------|
| **Actor** | Admisión |
| **Descripción** | Listar todas las citas (web y presencial) con filtrado en tiempo real por DNI |
| **Prioridad** | Alta |

---

## RF28 Consulta de Historia Clínica por DNI

| Campo | Valor |
|-------|-------|
| **Actor** | Admisión |
| **Descripción** | Búsqueda para obtener el Identificador de Historia Clínica (Ej. HC-XXXX) a partir del DNI |
| **Prioridad** | Media |

---

## RF29 Vista Administrativa de Oferta en Tiempo Real

| Campo | Valor |
|-------|-------|
| **Actor** | Admisión |
| **Descripción** | Consultar estado de especialidades activas y sus UPS para informar a pacientes presenciales |
| **Prioridad** | Baja |

---

## RF30 Visualizar Agenda Diaria

| Campo | Valor |
|-------|-------|
| **Actor** | Médico |
| **Descripción** | Visualizar lista cronológica de pacientes asignados a sus citas del día con estado de cada paciente |
| **Prioridad** | Alta |

---

## RF31 Búsqueda Rápida de Pacientes

| Campo | Valor |
|-------|-------|
| **Actor** | Admisión |
| **Descripción** | Buscador global en portal de admisión para filtrar pacientes por DNI o Número de Historia Clínica |
| **Prioridad** | Media |

---

## RF32 Validaciones Visuales Clínicas

| Campo | Valor |
|-------|-------|
| **Actor** | Enfermería |
| **Descripción** | Validar en tiempo real valores numéricos de triaje y mostrar alertas visuales (bordes rojos) si exceden rangos lógicos |
| **Prioridad** | Media |

---

## RF36 Gestión de Sesión Delegada (Mi Familia)

| Campo | Valor |
|-------|-------|
| **Actor** | Paciente (Responsable) |
| **Descripción** | Permitir al tutor cambiar su sesión a la de un dependiente menor registrado, operando en nombre de este, y regresar con un clic |
| **Prioridad** | Alta |

---

# REQUISITOS NO FUNCIONALES

## RNF01 Rendimiento

| Campo | Valor |
|-------|-------|
| **Requisito** | Tiempo de respuesta ≤ 3 segundos |

---

## RNF02 Seguridad

| Campo | Valor |
|-------|-------|
| **Requisito** | Autenticación obligatoria |

---

## RNF03 Disponibilidad

| Campo | Valor |
|-------|-------|
| **Requisito** | Operación durante horario laboral |

---

## RNF04 Usabilidad

| Campo | Valor |
|-------|-------|
| **Requisito** | Interfaz simple |

---

## RNF05 Escalabilidad

| Campo | Valor |
|-------|-------|
| **Requisito** | Permitir agregar especialidades |

---

## RNF06 Compatibilidad

| Campo | Valor |
|-------|-------|
| **Requisito** | Compatible con navegador |

---

## RNF07 Trazabilidad

| Campo | Valor |
|-------|-------|
| **Requisito** | Registrar cambios |

---

## RNF08 Experiencia de Usuario Ágil e Inmersiva

| Campo | Valor |
|-------|-------|
| **Requisito** | Reducir recargas de página completas durante flujos críticos (Wizard de pasos, Modales, Offcanvas) |

---

## RNF09 Accesibilidad de Impresión

| Campo | Valor |
|-------|-------|
| **Requisito** | Vistas formales (Ticket Electrónico) optimizadas nativamente para impresión, ocultando UI no esencial |

---

## RNF10 Interfaz Reactiva (Kanban)

| Campo | Valor |
|-------|-------|
| **Requisito** | Tablero Kanban actualiza estados fluidamente sin refrescar página, usando peticiones asíncronas |

---

## RNF11 Seguridad en Delegación de Sesiones

| Campo | Valor |
|-------|-------|
| **Requisito** | Validar integridad del vínculo (`ResponsableId`) a nivel de base de datos antes de generar nuevo token de autenticación |

---

# CRITERIOS DE ACEPTACIÓN

El sistema deberá permitir:

✔ Registrar citas  
✔ Compartir disponibilidad  
✔ Registrar triaje  
✔ Visualizar estados  
✔ Generar ticket  
✔ Ejecutar flujo completo

---

# HISTORIAS DE USUARIO (SpecDD & BDD)

Esta sección traduce la totalidad de los Casos de Uso y Reglas de Negocio en especificaciones ejecutables organizadas por Épicas. La priorización sigue estrictamente el modelo MoSCoW para definir el MVP.

---

## ÉPICA 1: Gestión de Acceso y Usuarios

### HU01 - Inicio de Sesión

| Campo | Valor |
|-------|-------|
| **Como** | Usuario (Paciente o Personal) |
| **Quiero** | Ingresar mis credenciales |
| **Para** | Acceder a las funciones correspondientes a mi rol |
| **Prioridad** | Must Have |
| **Reglas** | RN01: Denegar acceso si cuenta no habilitada administratoriamente |
| **Criterios de Aceptación** | • Tras validación exitosa, redirigir al panel correspondiente al rol |

---

### HU02 - Gestión de Dependientes Menores

| Campo | Valor |
|-------|-------|
| **Como** | Responsable del menor |
| **Quiero** | Registrar y administrar pacientes menores |
| **Para** | Reservar y visualizar el historial de sus citas |
| **Prioridad** | Must Have |
| **Reglas** | RN03: Solo adulto responsable puede administrar reservas de dependientes menores |
| **Criterios de Aceptación** | • El sistema confirma si el dependiente ya está registrado para evitar duplicidad |

---

### HU03 - Consultar y Actualizar Perfil

| Campo | Valor |
|-------|-------|
| **Como** | Paciente |
| **Quiero** | Visualizar y actualizar mi información |
| **Para** | Mantener mis datos de contacto al día |
| **Prioridad** | Should Have |
| **Reglas** | RN02: Solo modificar celular y contraseña; bloquear edición de DNI, Nombre, Fecha de nacimiento |
| **Criterios de Aceptación** | • El sistema permite modificar únicamente número de celular y contraseña |

---

### HU04 - Gestionar Usuarios Internos

| Campo | Valor |
|-------|-------|
| **Como** | Administrador |
| **Quiero** | Administrar cuentas de Admisión y Enfermería |
| **Para** | Controlar el acceso operativo al sistema |
| **Prioridad** | Should Have |
| **Criterios de Aceptación** | • El administrador puede buscar, editar y guardar configuración de acceso del personal |

---

## ÉPICA 2: Configuración y Disponibilidad Operativa

### HU05 - Configuración de Duración de Atención

| Campo | Valor |
|-------|-------|
| **Como** | Administrador |
| **Quiero** | Configurar duración de atención por especialidad |
| **Para** | Que el sistema calcule correctamente la generación de cupos |
| **Prioridad** | Must Have |
| **Reglas** | RN29: Cada especialidad permite duración distinta (ej. 20 minutos) |
| **Criterios de Aceptación** | • La modificación impacta automáticamente en futura generación de tarjetas de disponibilidad |

---

### HU06 - Gestionar UPS y Especialidades

| Campo | Valor |
|-------|-------|
| **Como** | Administrador |
| **Quiero** | Crear y configurar UPS y Especialidades |
| **Para** | Estructurar la oferta de servicios de la posta |
| **Prioridad** | Must Have |
| **Reglas** | RN07: UPS de uso estrictamente interno, no visibles para pacientes |
| **Criterios de Aceptación** | • Las UPS configuradas permanecen ocultas a pacientes |

---

### HU07 - Configuración de Programación Operativa

| Campo | Valor |
|-------|-------|
| **Como** | Administrador |
| **Quiero** | Definir jornadas de los médicos |
| **Para** | Establecer la oferta base de turnos |
| **Prioridad** | Must Have |
| **Reglas** | RN17: Programación incluye especialidad, médico, turno, duración y cupos totales<br>RN09: Restringir modificación de atención/programación ya iniciada |
| **Criterios de Aceptación** | • El sistema restringe intento de modificar atención ya iniciada |

---

### HU08 - Habilitación de Disponibilidad

| Campo | Valor |
|-------|-------|
| **Como** | Admisión |
| **Quiero** | Habilitar programación existente |
| **Para** | Abrir cupos al público web y presencial |
| **Prioridad** | Must Have |
| **Reglas** | RN06: Admisión no puede crear horarios fuera de planificación del administrador<br>RN08: Disponibilidad genera automáticamente dividiendo turno según duración configurada |
| **Criterios de Aceptación** | • Al confirmar apertura, disponibilidad se genera automáticamente |

---

### HU09 - Ajustar Disponibilidad Futura

| Campo | Valor |
|-------|-------|
| **Como** | Admisión |
| **Quiero** | Modificar cupos por rotación o ausencia |
| **Para** | Mantener oferta real actualizada |
| **Prioridad** | Should Have |
| **Reglas** | RN18: Ajuste limitado única y estrictamente a jornadas futuras |
| **Criterios de Aceptación** | • El ajuste operativo solo aplica a jornadas futuras |

---

### HU10 - Gestionar Sobrecupos

| Campo | Valor |
|-------|-------|
| **Como** | Admisión |
| **Quiero** | Registrar excepciones de disponibilidad |
| **Para** | Atender casos fuera del stock regular |
| **Prioridad** | Could Have |
| **Reglas** | RN16: Sobrecupo permanece totalmente oculto para pacientes en plataforma web |
| **Criterios de Aceptación** | • Disponibilidad extraordinaria oculta a pacientes |

---

## ÉPICA 3: Consulta y Reserva de Citas

### HU11 - Consultar Especialidades

| Campo | Valor |
|-------|-------|
| **Como** | Paciente |
| **Quiero** | Visualizar lista de especialidades |
| **Para** | Iniciar proceso de reserva |
| **Prioridad** | Must Have |
| **Reglas** | RN07: Mostrar solo especialidades con disponibilidad, ocultando jerarquías internas de UPS |
| **Criterios de Aceptación** | • El sistema muestra únicamente especialidades con disponibilidad |

---

### HU12 - Consultar Disponibilidad

| Campo | Valor |
|-------|-------|
| **Como** | Paciente |
| **Quiero** | Consultar horarios y médicos por especialidad |
| **Para** | Elegir mi turno ideal |
| **Prioridad** | Must Have |
| **Reglas** | RN27, RN28: Turnos segmentados entre Mañana (08:00–13:30) y Tarde (15:00–19:00) |
| **Criterios de Aceptación** | • Paciente visualiza turnos segmentados Mañana/Tarde |

---

### HU13 - Reserva de Cita Web

| Campo | Valor |
|-------|-------|
| **Como** | Paciente |
| **Quiero** | Seleccionar horario y confirmar reserva |
| **Para** | Asegurar mi atención |
| **Prioridad** | Must Have |
| **Reglas** | RN04: Reserva confirmed desconta inmediatamente cupo del stock compartido<br>RN31: Bloquear duplicidad de reservas activas para mismo paciente y horario<br>RN37: Solo reservar mismo día Lunes–Viernes, o Lunes si es Sábado; bloqueado Domingo |
| **Criterios de Aceptación** | • Reserva confirma y desconta cupo inmediatamente<br>• Sistema bloquea duplicidad de reservas activas |

---

### HU14 - Registro de Cita Presencial

| Campo | Valor |
|-------|-------|
| **Como** | Admisión |
| **Quiero** | Registrar reservas físicas |
| **Para** | Atender pacientes en ventanilla |
| **Prioridad** | Must Have |
| **Reglas** | RN04: Reserva presencial consume cupos del mismo stock que web |
| **Criterios de Aceptación** | • Reserva presencial consume cupos del mismo stock compartido |

---

### HU15 - Generación de Ticket

| Campo | Valor |
|-------|-------|
| **Como** | Sistema |
| **Quiero** | Emitir comprobante automático al finalizar reserva |
| **Para** | Garantizar formalidad del proceso |
| **Prioridad** | Must Have |
| **Reglas** | RN12: Toda reserva confirmada (web o presencial) genera obligatoriamente ticket |
| **Criterios de Aceptación** | • Ticket contiene: Especialidad, Médico, Hora, Fecha |

---

### HU16 - Cancelación de Cita

| Campo | Valor |
|-------|-------|
| **Como** | Paciente |
| **Quiero** | Cancelar reserva activa |
| **Para** | Liberar mi cupo |
| **Prioridad** | Should Have |
| **Reglas** | RN13, RN36: Cancelación solo antes de iniciar triaje (20 min antes del turno)<br>RN14: Cancelación exitosa devuelve disponibilidad al stock inmediatamente |
| **Criterios de Aceptación** | • Cancelación solo antes de triaje<br>• Disponibilidad regresada inmediatamente al stock |

---

## ÉPICA 4: Triaje y Trazabilidad

### HU17 - Registro de Triaje

| Campo | Valor |
|-------|-------|
| **Como** | Enfermería |
| **Quiero** | Registrar evaluación inicial del paciente |
| **Para** | Habilitarlo para atención |
| **Prioridad** | Must Have |
| **Reglas** | RN20: Registro exclusivo del rol de enfermería<br>RN22, RN23: Solo peso, talla, temperatura, presión y observación (sin historia clínica) |
| **Criterios de Aceptación** | • Paciente debe tener cita activa<br>• Registro cambia estado a "En triaje" |

---

### HU18 - Actualizar Estado de Atención

| Campo | Valor |
|-------|-------|
| **Como** | Enfermería |
| **Quiero** | Actualizar estados de la cita |
| **Para** | Mantener trazabilidad operativa |
| **Prioridad** | Must Have |
| **Reglas** | RN21: Flujo estricto: Pendiente → En triaje → Listo atención → No asistió<br>RN30: Cada transición genera registro de auditoría en base de datos |
| **Criterios de Aceptación** | • Transiciones respetan flujo estricto<br>• Cada transición genera registro de auditoría |

---

### HU19 - Consultar Trazabilidad

| Campo | Valor |
|-------|-------|
| **Como** | Paciente |
| **Quiero** | Consultar estado de mi cita |
| **Para** | Hacer seguimiento a mi atención |
| **Prioridad** | Should Have |
| **Criterios de Aceptación** | • Paciente visualiza estado actualizado en tiempo real sin intervención administrativa |

---

## ÉPICA 5: Avisos Operativos (MVP Complementario)

### HU20 - Registrar Aviso de Atención Inmediata

| Campo | Valor |
|-------|-------|
| **Como** | Paciente |
| **Quiero** | Enviar aviso indicando intención de asistir de inmediato |
| **Para** | Alertar a enfermería |
| **Prioridad** | Could Have |
| **Reglas** | RN24: Aviso se registra sin generar reserva médica real ni alterar stock<br>RN25: Aviso no otorga prioridad automática sobre orden de citas |
| **Criterios de Aceptación** | • Aviso no genera reserva real ni altera stock |

---

### HU21 - Visualizar Panel de Avisos

| Campo | Valor |
|-------|-------|
| **Como** | Enfermería |
| **Quiero** | Visualizar panel con notificaciones de pacientes |
| **Para** | Estar preparado ante llegadas inminentes |
| **Prioridad** | Could Have |
| **Reglas** | RN26: Panel visible exclusiva y únicamente por personal de enfermería |
| **Criterios de Aceptación** | • Avisos transicionan entre estados: Pendiente, Visualizado, Cerrado |

---

### HU24 - Alertas de Rangos Clínicos en Triaje

| Campo | Valor |
|-------|-------|
| **Como** | Profesional de Enfermería |
| **Quiero** | Que formulario de triaje alerte si introduzco valores ilógicos |
| **Para** | Evitar errores de digitación en signos vitales |
| **Prioridad** | Should Have |
| **Reglas** | RN32: Campos numéricos indican error visual inmediato si valor es biológicamente improbable (Ej. Temperatura > 45°C) |
| **Criterios de Aceptación** | • Error visual inmediato si valor excede rangos lógicos |

---

### HU25 - Búsqueda Ágil en Recepción

| Campo | Valor |
|-------|-------|
| **Como** | Personal de Admisión |
| **Quiero** | Disponer de buscador rápido en panel principal |
| **Para** | Encontrar pacientes por DNI o Historia Clínica y agilizar atención |
| **Prioridad** | Should Have |
| **Reglas** | RN31: Búsqueda accesible sin navegar menús profundos |
| **Criterios de Aceptación** | • Resultados coinciden parcialmente con Historia Clínica o exactamente con DNI |

---

## ÉPICA 6: Integración y Trazabilidad Global

### HU26 - Tablero Kanban para Enfermería

| Campo | Valor |
|-------|-------|
| **Como** | Enfermera |
| **Quiero** | Gestionar pacientes del día mediante tablero Kanban |
| **Para** | Visualizar rápidamente quién falta por triaje y quién está listo para atención |
| **Prioridad** | Must Have |
| **Reglas** | RN25: Tablero Kanban con estados "Falta Triaje", "En Triaje", "Listo Atención", "Finalizados" |

---

### HU27 - Registro Integral de Citas (Presencial y Web)

| Campo | Valor |
|-------|-------|
| **Como** | Admisión |
| **Quiero** | Consultar listado consolidado de citas de todas modalidades |
| **Para** | Gestionar agenda global de la posta y resolver conflictos diarios |
| **Prioridad** | Must Have |
| **Reglas** | RN27: Listar todas citas (web y presencial) con filtrado en tiempo real por DNI |

---

### HU28 - Delegación de Cuenta (Mi Familia)

| Campo | Valor |
|-------|-------|
| **Como** | Tutor |
| **Quiero** | Ingresar al perfil de mi hijo directamente desde mi cuenta |
| **Para** | Reservarle citas médicas sin requerir credenciales separadas |
| **Prioridad** | Must Have |
| **Reglas** | RN36: Cambio de sesión valida integridad del vínculo `ResponsableId` a nivel de base de datos |

---

### HU29 - Localización Ágil de Historia Clínica

| Campo | Valor |
|-------|-------|
| **Como** | Admisión |
| **Quiero** | Ingresar DNI de paciente y ver Número de Historia Clínica |
| **Para** | Buscar físicamente expediente en archivo de forma rápida |
| **Prioridad** | Should Have |
| **Reglas** | RN28: Búsqueda para obtener Identificador de Historia Clínica (Ej. HC-XXXX) a partir del DNI |
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



# CU02 Iniciar sesión

Actor:
Todos

Objetivo:

Ingresar según rol.

Flujo:

1. Ingresar credenciales.
2. Validar.
3. Redireccionar.

Postcondición:

Sesión iniciada.

---

# CU03 Gestionar dependientes

Actor:
RESP

Objetivo:

Administrar menores.

Flujo:

1. Seleccionar menor.
2. Registrar relación.
3. Confirmar.

Alternativo:

Dependiente ya registrado.

Postcondición:

Menor asociado.

---

# CU04 Consultar especialidades

Actor:
PAC

Objetivo:

Visualizar oferta.

Precondición:

Sesión iniciada.

Flujo:

1. Abrir citas.
2. Sistema muestra especialidades.

Restricción:

UPS no visibles.

Postcondición:

Especialidad seleccionada.

---

# CU05 Consultar disponibilidad

Actor:
PAC

Objetivo:

Consultar cupos.

Flujo:

1. Elegir especialidad.
2. Elegir turno.

Sistema muestra:

- Horarios
- Médico
- Cupos

Postcondición:

Disponibilidad visible.

---

# CU06 Reservar cita

Actor:
PAC

Objetivo:

Reservar atención.

Precondición:

Disponibilidad existente.

Flujo principal:

1. Seleccionar especialidad.
2. Seleccionar turno.
3. Visualizar tarjetas.
4. Seleccionar horario.
5. Confirmar.
6. Generar ticket.
7. Descontar cupo.

Flujo alternativo:

6A Cupo agotado.

Postcondición:

Reserva creada.

---

# CU07 Registrar cita presencial

Actor:
ADM

Objetivo:

Registrar reservas físicas.

Precondiciones:

Disponibilidad habilitada.

Flujo:

1. Buscar paciente.
2. Seleccionar especialidad.
3. Seleccionar horario.
4. Confirmar.

Sistema:

- Descuenta cupo.

Postcondición:

Reserva registrada.

---

# CU08 Cancelar cita

Actor:
PAC

Objetivo:

Liberar cupo.

Precondiciones:

Cita activa.

Flujo:

1. Seleccionar cancelar.
2. Confirmar.
3. Liberar cupo.

Restricción:

Solo antes del inicio del triaje.

Postcondición:

Disponibilidad actualizada.

---

# CU09 Generar ticket

Actor:
Sistema

Objetivo:

Emitir comprobante.

Contenido:

- Número de atención
- Especialidad
- Médico
- Hora
- Fecha

Postcondición:

Ticket generado.

---

# CU10 Configurar programación operativa

Actor:
ADMIN

Objetivo:

Configurar jornadas.

Flujo:

1. Seleccionar especialidad.
2. Asociar médico.
3. Configurar turno.
4. Configurar duración.
5. Configurar cupos.

Restricción:

No modificar atención iniciada.

Postcondición:

Programación disponible.

---

# CU11 Habilitar disponibilidad

Actor:
ADM

Objetivo:

Abrir cupos.

Precondiciones:

Programación existente.

Flujo:

1. Consultar programación.
2. Confirmar apertura.
3. Publicar disponibilidad.

Restricción:

No crear horarios nuevos.

Postcondición:

Disponibilidad habilitada.

---

# CU12 Ajustar disponibilidad

Actor:
ADM

Objetivo:

Actualizar cupos futuros.

Motivos:

- Rotación
- Ausencia
- Reprogramación

Restricción:

Solo jornadas futuras.

Postcondición:

Disponibilidad actualizada.

---

# CU13 Registrar triaje

Actor:
ENF

Objetivo:

Registrar evaluación inicial.

Precondición:

Paciente presente.

Flujo:

1. Buscar cita.
2. Registrar:

- Peso
- Talla
- Temperatura
- Presión

3. Guardar.

Sistema:

Estado → En triaje

Postcondición:

Paciente preparado.

---

# CU14 Actualizar estado

Actor:
ENF

Objetivo:

Mantener trazabilidad.

Estados:

Pendiente

↓

En triaje

↓

Listo atención

↓

No asistió

Postcondición:

Estado actualizado.

---

# CU15 Consultar trazabilidad

Actor:
PAC

Objetivo:

Consultar avance.

Flujo:

1. Abrir cita.
2. Ver estado.

Postcondición:

Seguimiento disponible.

---

# CU16 Registrar aviso de atención inmediata

Actor:
PAC

Objetivo:

Comunicar intención de asistir.

Flujo:

1. Abrir aviso.
2. Registrar motivo.
3. Confirmar.

Sistema:

Notifica enfermería.

Restricción:

No genera cita.

Postcondición:

Aviso creado.

---

# CU17 Visualizar avisos

Actor:
ENF

Objetivo:

Visualizar panel.

Estados:

Pendiente

Visualizado

Cerrado

Postcondición:

Aviso actualizado.

---

# CU18: Gestionar Usuarios y Personal
Actor Principal: Administrador (ADMIN)

Objetivo: Registrar, buscar, editar y administrar el acceso y roles de los distintos actores del sistema (Pacientes, Médicos, Enfermeras, Personal de Admisión).

Precondiciones: 1. El Administrador ha iniciado sesión correctamente en el sistema.
2. El navegador se encuentra en la ruta base de administración (/Admin/Index).

Flujo Principal (Escenario de Éxito)
El Administrador selecciona la opción "Gestión de Usuarios" en el panel principal.

El sistema renderiza la interfaz dinámica con la lista de usuarios existentes y el formulario de registro/edición.

El Administrador introduce el DNI (8 dígitos) del usuario a registrar o modificar y hace clic en el botón "Buscar".

El sistema realiza una consulta asíncrona en segundo plano (Fetch/AJAX):

Verifica que el DNI no exista previamente en la base de datos local.

Al no encontrarlo localmente, invoca al servicio de validación de identidad externo (Mock RENIEC).

El sistema recibe la estructura de datos JSON, autocompleta automáticamente los campos "Nombres" y "Apellidos" en el formulario y los bloquea con el atributo de integridad readonly.

El Administrador asigna el Rol del usuario mediante un menú desplegable (Paciente, Medico, Enfermeria, Admision) y completa los campos de contacto o credenciales (Correo, Contraseña).

El Administrador hace clic en "Guardar".

El sistema ejecuta las validaciones (Client-side y Server-side), procesa el cifrado de la contraseña (Hashing), persiste el registro en la base de datos y actualiza la tabla dinámica de forma asíncrona.

---

# CU19 Configurar duración atención

Actor:
ADMIN

Objetivo:

Generar horarios.

Ejemplo:

Medicina

↓

20 minutos

Sistema:

Genera tarjetas.

Postcondición:

Disponibilidad calculada.

---

# CU20 Gestionar sobrecupo

Actor:
ADM

Objetivo:

Registrar excepción.

Precondición:

Sin disponibilidad.

Flujo:

1. Solicitar excepción.
2. Confirmar.
3. Registrar.

Restricción:

No visible paciente.

Postcondición:

Sobrecupo registrado.

---

# RELACIONES

CU06
<<include>>
CU05

CU06
<<include>>
CU09

CU13
<<include>>
CU14

CU11
<<include>>
CU10
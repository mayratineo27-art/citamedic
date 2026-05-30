# Historial de Prompts (Spec-Driven Development)

Este documento recopila todas las instrucciones y prompts enviados por el usuario para la construcción iterativa del sistema bajo el enfoque SpecDD.

### Prompt

> corre el sistema

---

### Prompt

> credenciales de paciente

---

### Prompt

> credenciales para pacientes

---

### Prompt

> revisa si está habilitada la vista de paciente

---

### Prompt

> Actúa como auditor de software. Basándote en nuestra sesión, genera un archivo Bitacora_Prompts_SDLC.md que organice todos los prompts que hemos utilizado desde el análisis de requerimientos hasta la corrección de errores, clasificándolos por fase del ciclo de vida del software. Incluye el objetivo de cada prompt y el resultado técnico obtenido.

---

### Prompt

> revisa todos los archivos del proyecto en especial los casos de uso y evalúasi para cada actor están cubiertas las funcionalidades que debería tener, reporta todo ello como un informe de cobertura

---

### Prompt

> algún actor tiene habilitada su vista con las funciones necesarias?

---

### Prompt

> Actúa como desarrollador Senior ASP.NET Core MVC.

PROBLEMA: En PostaCitasWeb, las acciones HabilitarProgramacion,
DeshabilitarProgramacion y AjustarProgramacion están dentro de
AdminController con [Authorize(Roles = "Administrador")].
Según RN06, CU11 y CU12, estas acciones pertenecen al actor
Admisión, NO al Administrador. El personal de Admisión no puede
usar su propio módulo.

TAREA: Mueve esas tres acciones a AdmisionController.cs con
[Authorize(Roles = "Admision")]. Si AdmisionController ya existe
(tiene CU07 registrar cita presencial), agrégalas ahí.
Si no existe, créalo completo.

Entrega:
1. AdmisionController.cs completo con las acciones existentes
   (CU07) + las tres acciones movidas (CU11, CU12).
2. Los cambios mínimos en AdminController.cs (solo eliminación
   de esas tres acciones).
3. Las líneas a cambiar en las vistas que invocan esas acciones
   (rutas de formularios o links que apuntaban a /Admin/...,
   ahora deben apuntar a /Admision/...).

No cambies la lógica interna de los métodos ni los servicios.
Solo mueve y reasigna el rol.

---

### Prompt

> revisa el artefacto #03_reglas_negocio.md y ayúdame a completar las reglas de necio en el artecto 02_casos_uso.md para él módulo de enfermera con los casos de uso 12 13 14 15

---

### Prompt

> Estoy documentando un proyecto académico llamado:

Sistema Web de Gestión de Citas Médicas – Posta de Salud.

Necesito que identifiques y redactes los CASOS DE USO del MÓDULO ADMINISTRADOR comenzando desde el número CU16 en adelante.

IMPORTANTE:
No inventes funcionalidades.
Debes ser extremadamente conservador y documentar únicamente funcionalidades que realmente existan en la vista del administrador o que estén efectivamente implementadas en el código.

OBJETIVO:
Generar casos de uso coherentes con el sistema ya desarrollado y mantener trazabilidad con reglas de negocio y requisitos.

REGLAS DE GENERACIÓN:

1. Analiza únicamente:
- Funciones visibles en la interfaz.
- Navegación disponible.
- Formularios existentes.
- Botones implementados.
- Tablas.
- Acciones realmente disponibles.

NO PROPONGAS:
- futuras mejoras
- módulos ideales
- funcionalidades hospitalarias completas
- procesos no implementados

2. Mantener numeración consecutiva desde:

CU16

3. Mantener EXACTAMENTE este formato:

# CUXX — Nombre del caso de uso

## Objetivo

(Descripción breve)

## Actor principal

Administrador

## Precondiciones

(Lista)

## Postcondiciones

(Lista)

## Flujo principal

(Pasos numerados)

## Flujos alternativos

FA01 —
FA02 —

## Reglas de negocio asociadas

(Listar únicamente RN existentes)

4. Mantener consistencia con módulos anteriores:
- Login NO se vuelve a documentar.
- Recuperar contraseña NO se vuelve a documentar.
- Solo documentar acciones propias del administrador.

5. Validar coherencia con estas reglas ya existentes:
- RN05 Publicación operativa
- RN06 Restricción de publicación
- RN08 Generación automática
- RN09 Modificación futura
- RN17 Configuración administrativa
- RN18 Rotación operativa
- RN30 Seguimiento
- RN31 Integridad
- RN32 Auditoría
- RN35 Continuidad

6. Si una funcionalidad visible NO merece ser un caso de uso independiente (por ejemplo abrir modal, generar ticket interno, filtrar tabla, exportar temporalmente), clasificarla como:
"Paso interno del sistema"
y NO generar CU.

7. Detectar duplicidades:
Si una acción ya pertenece a Admisión o Enfermería NO crearla nuevamente.

8. Entregar al final:

## MATRIZ RESUMEN

| CU | Función implementada | Estado |
|----|----------------------|--------|

Estado:
Implementado
Parcial
No identificado

9. Si detectas funciones visibles que no tienen suficientes datos para documentarse:
crear sección:

## Pendientes de validación

y explicar qué falta.

10. Priorizar documentación de:
- Gestión de disponibilidad
- Gestión de especialidades
- Gestión de médicos
- Gestión de programación
- Gestión de usuarios
- Gestión de configuración
- Gestión de auditoría

pero SOLO si existen realmente.

Salida esperada:
Casos de uso completos listos para copiar en markdown.

---

### Prompt

> ejecuta el sistema

---

### Prompt

> implementa ## CU17 — Gestionar Perfil de Usuario en la vista del administrador en gestionar usuarios sin romper funcionalidades

---

### Prompt

> soluciona

---

### Prompt

> dame las credenciales para acceder a la vista paciente

---

### Prompt

> requiero de paciente

---

### Prompt

> Revisa el artefacto de casos de uso e implementa el módulo de pacientes

---

### Prompt

> implementa el ## CU03 — Consultar Programación Referencial de Atención

---

### Prompt

> de acuerdo

---

### Prompt

> implmenta 07_Especificaciones_UI

---

### Prompt

> continúa

---

### Prompt

> porque el formulario para reservar se ve así?, soluciona

---

### Prompt

> Actúa como un desarrollador Frontend Senior experto en ASP.NET Core y Bootstrap. Tengo un problema de visualización en la vista de reserva de citas: los elementos, como la caja de '¡Cita reservada!', aparecen superpuestos y desordenados, rompiendo la estructura de la página.

Tareas a realizar:

Auditoría de Layout: Revisa el archivo .cshtml de la vista de reserva y asegúrate de que todos los elementos estén contenidos en una estructura de rejilla (container > row > col) correcta de Bootstrap.

Corrección de CSS: Identifica si hay elementos con position: absolute o fixed que estén causando que el mensaje de éxito tape el resto del contenido. Ajusta el CSS para que los elementos fluyan naturalmente o usa clases de utilidad (d-none o d-block) para ocultar/mostrar la confirmación solo cuando sea necesario.

Consistencia Visual: Aplica el sistema de diseño 'Clinic Calm' (Azul #1D2D50 y Verde #2AAE8A) que definimos, asegurándote de que el espaciado (padding/margin) sea uniforme y no se amontone el contenido.

Respeto a la funcionalidad: No cambies la lógica del controlador, solo corrige el HTML y el CSS para que la interfaz sea profesional, limpia y responsiva.

Entregable: Proporcióname el código corregido de la vista y, si añadiste algún estilo personalizado, indícame en qué archivo CSS debo pegarlo."

---

### Prompt

> compila

---

### Prompt

> en programación operativa cuando selecciono especialidad no me muestra el catálogo de especialidades que sí existen conecta esa ruta para que muestre, además hay error en la solicitud al servidor cuando intento deshabilitar en las programaciones registradas

---

### Prompt

> ese prototipo quiero que se aplique para e frontend de paciente como puedes observar tiene inicio y luego irían las funcionalidades aplicadas a mi sistema, quiero ese toque moderno, no rompas funcionalidades solo cambia el frontend

---

### Prompt

> continúa

---

### Prompt

> documenta y actualiza los parámetros empleados para tener esa vista en el artefacto 07 especificaciones ui

---

### Prompt

> NECESITO QUE EL USUARIO ADMINISTRADOR AL LA HORA DE REGISTRAR TENGA RECURSOS VISUALES MUY SIMILARES COMO HOLA JUAN PERO ADMIN, LUEGO LAS TARJETAS, ADEMÁS LAS TARJETAS QUE S E VEAN MÁ MODERNAS Y CON LA FUNTE QUE TIENE PACIENTE, NO ROMPAS NINGUNA FUNCIONALIDAD

---

### Prompt

> AHORA EL DE ENFERMERA Y DE ADMISIÓN

---

### Prompt

> MEJORA LA VISTA Y ALINEA CON LA DE EL DASHBOARD EN CUANTO A COLORES PERO HE OBSERVADO QUE CUANDO PROGRAMO EN ADMINISTRADOR TURNOS PARA LA TARDE O MAÑANA CUANDO INTENTO SACAR COMO PACIENTE NO ME SLE HORARIOS PROGRAMADOS ESTÁ VACÍO

---

### Prompt

> CONITNÚA

---

### Prompt

> NOOO SI EL ADMINISTRADOR CONFIGURA DE LUNES A VIERNES AL PACIENTE LE SALE PARA ESOSS DÍAS EN CAMBIO SI ES SABADO SE PODRÁ PARA SABADO Y LUNES ESA ES LA LOGICA Y ME SIGUEN SIN APARECER LAS TARJETAS CON LOS HORARIOS, REVISA EL CASO DE USO ## CU06 — Registrar cita médica

---

### Prompt

> SALE ESTE ERROR

---

### Prompt

> ACTUALIZA PARA TODOS NO SOLO PARA ODONTOLOGÍA, EN TODAS LAS ESPECIALIDADES DEBERÍA GENERAR LAS TARJETAS TMBIÉN  ADEMÁS CUANDO EL USUARIO GUARDA SU CITA EL SISTEMA DEBERÍA MOSTRAR UN MENSAJE DE CONFIRMACIÓN DE RESERVA Y QUE S QUIERE VER SU TICKET SE DESPLACE PARA ABAJO

---

### Prompt

> REVISA EN MIS RN DEBERÍA HABER EL BOTÓN DE NUEVA RESERVA PARA QUE TE DES CUENTA QUE EL PACIENTE SOLO PUEDE SACAR UNA CITA POR EL DÍA YA SEA DE LUNES O VIERNES Y SÁBADO SOLO PARA UNA ESPECIALIDAD Y UN DÍA MISMO Y REVISA EL CASO DE USO 11 E IMPLMÉNTALO REVISANDO REGLAS DE NEGOCIO

---

### Prompt

> NECESITO QUE ACTUALICES EL ARTEFACTO 07 ESPECIFICACIONES UI Y COLOQUES LOS REQUERIMIENTOS PARA LAS VISTAS DEA ADMINISTRADOR Y TODO LO QUE HAS MODIFICADO

---

### Prompt

> Y GENERA UN MD EN DOCS QUE GUARDE TODOS LOS PROMPTS QUE HE EMPLEADO HASTA AHORA PARA LA CREACIÓN DEL SISTEMA BAJO EL ENFOQUE SPEC DRIVN DEVELOPMENT

---



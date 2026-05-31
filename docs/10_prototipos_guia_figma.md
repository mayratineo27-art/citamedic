# 10 — Guía de Prototipos para Figma/Balsamiq
## Sistema Web de Gestión de Citas Médicas — PostaCitasWeb — Iteración 2

**Versión:** 1.0  
**Fecha:** 2026  
**Sistema de diseño de referencia:** Clinic Calm Premium (ver `09_Especificaciones_UI.md`)  
**Paleta principal:** Navy `#1D2D50` · Teal `#2AAE8A` · Fondo `#f0f5f4`

---

## Índice de Pantallas

| ID | Pantalla | Actor | Requisito |
|----|----------|-------|-----------|
| P-01 | Login | Todos | RF01 |
| P-02 | Activar cuenta / Recuperar contraseña | Paciente | RF_ACT |
| P-03 | Dashboard Paciente — Inicio | Paciente | RF04, RF07 |
| P-04 | Wizard Reserva — Paso 1: Especialidad | Paciente | RF09 |
| P-05 | Wizard Reserva — Paso 2: Turno | Paciente | RF08, RF09 |
| P-06 | Wizard Reserva — Paso 3: Slot | Paciente | RF08, RF09 |
| P-07 | Wizard Reserva — Paso 4: Confirmación | Paciente | RF09 |
| P-08 | Mis Citas + Ticket | Paciente | RF19, RF12 |
| P-09 | Perfil del Paciente | Paciente | RF04, RF05 |
| P-10 | Sesión Delegada — Selector de Dependiente | Tutor | RF36 |
| P-11 | Dashboard Admisión | Admisión | RF27, RF31 |
| P-12 | Registro Cita Presencial | Admisión | RF10 |
| P-13 | Habilitar Programación Operativa | Admisión | RF15 |
| P-14 | Reset de Contraseña desde Ventanilla | Admisión | RF_RST |
| P-15 | Dashboard Administrador | Administrador | RF13–RF22 |
| P-16 | Gestión de Programaciones Operativas | Administrador | RF15A |
| P-17 | Gestión de Vínculos Tutor–Dependiente | Administrador | RF_TD-A |

---

## P-01 — Login

**Ruta:** `/Auth/Login`  
**Actor:** Todos los roles

### Elementos de la pantalla

| Zona | Elemento | Detalle |
|------|----------|---------|
| Centro | Logo CITAMEDIC | Ícono `fa-plus` en cuadrado teal `#2AAE8A`, 64px, border-radius 18px |
| Centro | Título | "Bienvenido" — Outfit 700, 2rem, navy |
| Centro | Subtítulo | "Posta Los Licenciados" — muted, 0.9rem |
| Centro | Campo DNI | Input con ícono `fa-id-card`, placeholder "Ingresa tu DNI" |
| Centro | Campo Contraseña | Input password con ícono `fa-lock`, toggle mostrar/ocultar |
| Centro | Botón principal | "Ingresar" — fondo teal, texto blanco, border-radius 12px, ancho completo |
| Inferior | Enlace 1 | "Activar cuenta" — texto teal, centrado, `fa-user-plus` |
| Inferior | Enlace 2 | "¿Olvidaste tu contraseña?" — texto muted, centrado, `fa-key` |
| Inferior | Separador | Línea divisoria entre botón y enlaces |

### Estados a prototipar

- **Estado normal** — formulario vacío
- **Estado error** — mensaje "Credenciales inválidas" en banner rojo bajo el formulario
- **Estado cuenta inactiva** — mensaje "Cuenta no habilitada. Contacte a Admisión"

### Flujo de navegación

```
Login ──[Ingresar OK, rol Paciente]──► Dashboard Paciente (P-03)
     ──[Ingresar OK, rol Admisión]──► Dashboard Admisión (P-11)
     ──[Ingresar OK, rol Admin]────► Dashboard Administrador (P-15)
     ──[Activar cuenta]────────────► P-02 (mismo formulario)
     ──[¿Olvidaste tu contraseña?]─► P-02 (mismo formulario)
```

---

## P-02 — Activar Cuenta / Recuperar Contraseña

**Ruta:** `/Auth/ActivarCuenta`  
**Actor:** Paciente  
**Nota:** El mismo formulario sirve para activación y recuperación. El sistema detecta el flujo automáticamente.

### Paso 1 — Verificación de identidad

| Zona | Elemento | Detalle |
|------|----------|---------|
| Superior | Título | "Verificar identidad" — Outfit 700, 1.75rem |
| Superior | Subtítulo | "Ingresa tus datos tal como están registrados en la posta" |
| Formulario | Campo DNI | Input, requerido, 8 dígitos |
| Formulario | Campo Fecha de nacimiento | Date picker |
| Formulario | Campo Nombres completos | Input texto |
| Inferior | Botón | "Verificar" — teal, ancho completo |
| Inferior | Enlace | "← Volver al inicio de sesión" |

### Paso 2a — Definir contraseña (activación, `Activo = false`)

| Zona | Elemento | Detalle |
|------|----------|---------|
| Superior | Banner informativo | "¡Bienvenido! Tu cuenta está lista para activarse." — fondo teal-light |
| Formulario | Campo Nueva contraseña | Input password con indicador de fortaleza |
| Formulario | Campo Confirmar contraseña | Input password |
| Inferior | Botón | "Activar cuenta" — teal, ancho completo |

### Paso 2b — Definir nueva contraseña (recuperación, `Activo = true`)

| Zona | Elemento | Detalle |
|------|----------|---------|
| Superior | Banner informativo | "Identidad verificada. Define tu nueva contraseña." — fondo teal-light |
| Formulario | Campo Nueva contraseña | Input password con indicador de fortaleza |
| Formulario | Campo Confirmar contraseña | Input password |
| Inferior | Botón | "Cambiar contraseña" — teal, ancho completo |

### Estados de error

- Datos no encontrados: banner rojo "Datos no encontrados en el padrón de la posta"
- Menor de edad: banner rojo "Los menores de edad no pueden activar cuenta directamente"

---

## P-03 — Dashboard Paciente — Inicio

**Ruta:** `/Paciente/Index`  
**Actor:** Paciente  
**Layout:** Sidebar izquierdo (290px) + área principal

### Sidebar

| Elemento | Detalle |
|----------|---------|
| Logo CITAMEDIC | Ícono teal + texto "CITAMEDIC / Portal Salud" |
| Tarjeta de perfil | Avatar circular + nombre del paciente + badge rol |
| Menú (8 ítems) | Inicio, Mis Citas, Especialidades, Médicos y Horarios, Reservar Cita, Triajes, Mi Perfil, Notificaciones |
| Botón logout | "Cerrar sesión" — rojo al hover |

### Área principal — Tab Inicio

| Zona | Elemento | Detalle |
|------|----------|---------|
| Header | Saludo dinámico | "Buenos días, [Nombre] 👋" — Outfit 400, 2.35rem |
| Header | Status pill | "Todo en orden" con ícono latido |
| Hero card | Próxima cita | Gradiente verde oscuro, datos de cita o CTA "Reservar" |
| Accesos directos | 3 tarjetas | Reservar Cita, Mis Citas, Mi Perfil |
| Columna lateral | Widget Mi Familia | Lista de dependientes vinculados |
| Columna lateral | Widget Último Pulso | BPM del último triaje |

### Variante: Sesión Delegada activa

- Banner amarillo en la parte superior: "Estás gestionando la cuenta de **[Nombre Dependiente]**"
- Botón "Volver a mi perfil" visible en el banner
- Nombre en sidebar muestra el dependiente, no el tutor

---

## P-04 — Wizard Reserva — Paso 1: Especialidad

**Ruta:** `/Citas/Reservar` (paso 1)  
**Actor:** Paciente

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Superior | Indicador de pasos | 4 pasos: Especialidad (activo) → Turno → Slot → Confirmación |
| Contenido | Título | "¿Qué especialidad necesitas?" |
| Contenido | Grid de tarjetas | Una tarjeta por especialidad activa con disponibilidad |
| Tarjeta | Ícono especialidad | Ícono médico representativo, fondo teal-light |
| Tarjeta | Nombre especialidad | Texto bold, navy |
| Tarjeta | Disponibilidad | Badge verde "X cupos disponibles" |
| Estado bloqueado | Domingo | Banner rojo "Las reservas web no están disponibles los domingos" |
| Estado vacío | Sin disponibilidad | Mensaje "No hay especialidades con disponibilidad para hoy" |

---

## P-05 — Wizard Reserva — Paso 2: Turno

**Ruta:** `/Citas/Reservar` (paso 2)  
**Actor:** Paciente

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Superior | Indicador de pasos | Paso 2 activo |
| Superior | Breadcrumb | Especialidad seleccionada (con opción de volver) |
| Contenido | Título | "Selecciona el turno" |
| Tarjeta Mañana | Horario | "08:00 – 13:30" — ícono sol |
| Tarjeta Tarde | Horario | "15:00 – 19:00" — ícono luna |
| Tarjeta sin disp. | Estado | Badge gris "Sin disponibilidad" — tarjeta deshabilitada |

---

## P-06 — Wizard Reserva — Paso 3: Slot

**Ruta:** `/Citas/Reservar` (paso 3)  
**Actor:** Paciente

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Superior | Indicador de pasos | Paso 3 activo |
| Superior | Breadcrumb | Especialidad + Turno seleccionados |
| Contenido | Título | "Elige tu horario" |
| Agrupador | Fecha | "📅 Lunes, 1 de junio" — encabezado de grupo |
| Tarjeta slot disponible | Hora | "08:00 – 08:20" — borde teal, badge verde "Disponible" |
| Tarjeta slot agotado | Hora | "08:20 – 08:40" — gris, badge "Agotado", cursor not-allowed |
| Nota | Sábado | Muestra slots del sábado Y del próximo lunes en grupos separados |

---

## P-07 — Wizard Reserva — Paso 4: Confirmación

**Ruta:** `/Citas/Reservar` (paso 4)  
**Actor:** Paciente

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Superior | Indicador de pasos | Paso 4 activo |
| Resumen | Tarjeta resumen | Especialidad, médico, fecha, hora, turno — solo lectura |
| Inferior | Botón confirmar | "Confirmar reserva" — teal, prominente |
| Inferior | Enlace | "← Cambiar horario" |
| Estado éxito | Alert | Banner verde "¡Cita reservada exitosamente!" + código de ticket |
| Estado error cupo | Alert | Banner rojo "El cupo seleccionado ya no está disponible. Por favor elija otro horario" |
| Estado error duplicado | Alert | Banner rojo "Ya tiene una cita activa para esta fecha" |

---

## P-08 — Mis Citas + Ticket

**Ruta:** `/Paciente/Index` (tab Mis Citas)  
**Actor:** Paciente

### Tabla de citas

| Columna | Detalle |
|---------|---------|
| Especialidad | Texto |
| Médico | Texto |
| Fecha | Formato DD/MM/YYYY |
| Turno | Badge Mañana/Tarde |
| Hora | HH:MM |
| Estado | Badge coloreado (ver sistema de badges) |
| Acciones | Botón "Ver ticket" + botón "Cancelar" (solo si aplica) |

### Modal Ticket

| Elemento | Detalle |
|----------|---------|
| Código | Monospace extragrande, `TKT-YYYYMMDD-NNNN` |
| Datos | Paciente, especialidad, médico, fecha, hora, turno |
| Botón | "Imprimir / Guardar PDF" |

### Modal Confirmación Cancelación

| Elemento | Detalle |
|----------|---------|
| Ícono | Triángulo advertencia, fondo rojo claro |
| Mensaje | "¿Estás seguro de cancelar esta cita?" |
| Botones | "Mantener cita" (outline) + "Sí, Cancelar" (danger) |
| Error fuera de ventana | Banner "El plazo para cancelar esta cita ha vencido (turno Mañana: hasta 07:40 / turno Tarde: hasta 14:40)" |

---

## P-09 — Perfil del Paciente

**Ruta:** `/Paciente/Index` (tab Mi Perfil)  
**Actor:** Paciente

### Sección datos personales (solo lectura)

| Campo | Detalle |
|-------|---------|
| Nombres completos | Solo lectura, con ícono candado |
| DNI | Solo lectura, con ícono candado |
| Fecha de nacimiento | Solo lectura, con ícono candado |
| N° Historia Clínica | Solo lectura, formato `HC-XXXXXX` |
| Estado SIS | Badge activo/inactivo |

### Sección editable

| Campo | Detalle |
|-------|---------|
| Número de celular | Input editable, validación 9–15 dígitos |
| Nueva contraseña | Input password |
| Confirmar contraseña | Input password |
| Botón | "Guardar cambios" — teal |

---

## P-10 — Sesión Delegada — Selector de Dependiente

**Ruta:** Modal o vista dentro del Dashboard Paciente  
**Actor:** Tutor

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Título | "Gestionar cuenta de un dependiente" | |
| Lista | Tarjetas de dependientes vinculados | Nombre, edad, badge "Menor" |
| Tarjeta | Botón | "Gestionar" — teal |
| Error | Sin vínculo | "No tiene vínculo registrado con este paciente" |
| Error | Dependiente intenta delegar | "Esta opción no está disponible para menores de edad" |

---

## P-11 — Dashboard Admisión

**Ruta:** `/Admision/Index`  
**Actor:** Admisión

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Header | Buscador rápido | Input DNI prominente en la cabecera, 8 dígitos |
| Resultado búsqueda | Tarjeta paciente | Nombre, DNI, HC, estado cuenta + acciones directas |
| Acciones directas | Botones | "Registrar cita", "Cancelar cita activa", "Resetear contraseña" |
| Panel citas del día | Tabla | Todas las citas del día ordenadas por turno y hora |
| Filtro tabla | Input DNI | Filtro en tiempo real |
| Columnas tabla | Datos | Paciente, DNI, Especialidad, Médico, Turno, Hora, Estado, Origen |

---

## P-12 — Registro Cita Presencial

**Ruta:** `/Admision/RegistrarCita`  
**Actor:** Admisión

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Campo DNI | Input + botón buscar | Precarga datos del paciente automáticamente |
| Datos paciente | Solo lectura | Nombre, HC — precargados |
| Selección | Especialidad, Turno, Slot | Mismos controles que el wizard web |
| Advertencia duplicado | Banner amarillo | "Este paciente ya tiene una cita activa para esta fecha. ¿Confirmar de todas formas?" |
| Sobrecupo | Checkbox | "Registrar como sobrecupo" — visible solo si cupos = 0 |
| Botón | "Registrar cita" — teal | |

---

## P-13 — Habilitar Programación Operativa

**Ruta:** `/Admision/Programaciones`  
**Actor:** Admisión

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Tabla | Programaciones pendientes | Solo las con `Habilitada = false` y fecha ≥ hoy |
| Columnas | Datos | Especialidad, Médico, Turno, Fecha, Cupos, Estado |
| Acción | Botón "Habilitar" | Verde, por fila |
| Confirmación | Modal | "¿Habilitar esta programación? Se expondrán X cupos al público." |
| Nota | Sin slots | "Se generarán los slots automáticamente al habilitar" |

---

## P-14 — Reset de Contraseña desde Ventanilla

**Ruta:** Modal dentro del Dashboard Admisión  
**Actor:** Admisión

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Campo | DNI del paciente | Input, precargado si viene de búsqueda rápida |
| Botón | "Generar contraseña temporal" — teal | |
| Resultado | Contraseña temporal | Mostrada en pantalla en caja destacada, fuente monospace |
| Instrucción | Texto | "Comunique esta contraseña al paciente presencialmente" |
| Error | DNI no encontrado | "Paciente no encontrado" |

---

## P-15 — Dashboard Administrador

**Ruta:** `/Admin/Index`  
**Actor:** Administrador

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Widgets métricas | 4 tarjetas | Total pacientes, médicos activos, programaciones del mes, usuarios internos |
| Menú lateral | Secciones | UPS, Especialidades, Médicos, Programaciones, Usuarios, Vínculos TD |
| Acceso rápido | Botones | "Nueva programación", "Nuevo médico", "Nuevo usuario" |

---

## P-16 — Gestión de Programaciones Operativas

**Ruta:** `/Admin/Programaciones`  
**Actor:** Administrador

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Tabla | Listado | Especialidad, Médico, Turno, Fecha, Cupos, Estado (Borrador/Habilitada) |
| Formulario creación | 2 columnas | Especialidad, Médico, Turno, Fecha, Cupos Total, Duración minutos |
| Validación | Fecha | No permite fechas pasadas o de hoy |
| Error duplicado | Banner | "Ya existe una programación para esta combinación" |
| Slots generados | Vista previa | Tabla de slots que se generarán (calculado en tiempo real) |

---

## P-17 — Gestión de Vínculos Tutor–Dependiente

**Ruta:** `/Admin/VinculosTD`  
**Actor:** Administrador

### Elementos

| Zona | Elemento | Detalle |
|------|----------|---------|
| Formulario | DNI Tutor + DNI Dependiente | Inputs con búsqueda y precarga de nombres |
| Validación | Menor de edad | Badge "Menor" visible al precargar el dependiente |
| Error | Vínculo existente | "El vínculo ya existe" |
| Tabla | Vínculos activos | Tutor, Dependiente, Fecha registro, Acción eliminar |

---

## Flujo de Navegación Global

```
Login (P-01)
├── Paciente ──► Dashboard (P-03)
│                ├── Reservar ──► P-04 → P-05 → P-06 → P-07
│                ├── Mis Citas ──► P-08
│                ├── Perfil ──► P-09
│                └── Sesión Delegada ──► P-10 → (vuelve a P-03 con contexto dependiente)
│
├── Admisión ──► Dashboard (P-11)
│                ├── Registrar cita ──► P-12
│                ├── Habilitar programación ──► P-13
│                └── Reset contraseña ──► P-14
│
└── Administrador ──► Dashboard (P-15)
                     ├── Programaciones ──► P-16
                     └── Vínculos TD ──► P-17
```

---

## Notas para Figma

1. **Componentes reutilizables a crear:** Sidebar, Tarjeta Premium, Badge Estado, Botón Teal, Input con ícono, Indicador de pasos (wizard), Modal genérico.
2. **Fuentes a instalar en Figma:** Outfit + Plus Jakarta Sans (disponibles en Google Fonts plugin).
3. **Paleta de colores:** Crear estilos de color con los valores de `09_Especificaciones_UI.md` sección 1.1.
4. **Flujos a conectar:** Usar Prototype mode para conectar las pantallas según el diagrama de navegación global.
5. **Variantes de estado:** Para cada pantalla con múltiples estados (error, éxito, vacío), crear variantes del componente.

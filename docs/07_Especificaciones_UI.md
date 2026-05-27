# 07 — Especificaciones UI: Portal del Paciente (CITAMEDIC)

**Versión:** 1.1  
**Fecha:** 2026-05-27  
**Vista implementada:** `Views/Paciente/Index.cshtml`  
**Sistema de diseño:** Clinic Calm — CITAMEDIC Portal Salud

---

## 1. Sistema de Diseño Global

### 1.1 Variables CSS (`:root`)

Todas las variables se declaran en el bloque `:root` del archivo `Index.cshtml` y son consumidas por todos los componentes de la vista.

| Variable | Valor | Uso |
|---|---|---|
| `--navy` | `#1D2D50` | Color primario de texto y fondos de encabezados |
| `--teal` | `#2AAE8A` | Color de acción, íconos activos y botones primarios |
| `--teal-light` | `#e8f7f3` | Fondo hover del sidebar y accesos directos |
| `--bg-main` | `#f0f5f4` | Fondo general de la página (área de contenido) |
| `--bg-sidebar` | `#ffffff` | Fondo del sidebar |
| `--text-main` | `#1D2D50` | Color de texto principal |
| `--text-muted` | `#64748b` | Texto secundario y etiquetas |
| `--border` | `#e2e8f0` | Bordes de tarjetas, separadores y inputs |
| `--radius` | `24px` | Radio estándar de bordes para tarjetas |
| `--shadow-sm` | `0 4px 6px -1px rgba(29,45,80,0.02), 0 2px 4px -2px rgba(29,45,80,0.02)` | Sombra sutil para tarjetas base |
| `--shadow-md` | `0 10px 15px -3px rgba(29,45,80,0.04), 0 4px 6px -4px rgba(29,45,80,0.04)` | Sombra media para hover |
| `--shadow-lg` | `0 20px 25px -5px rgba(29,45,80,0.06), 0 10px 10px -5px rgba(29,45,80,0.03)` | Sombra grande (modales) |
| `--green-hero-start` | `#0a6b59` | Inicio del gradiente del Hero card |
| `--green-hero-end` | `#044136` | Fin del gradiente del Hero card |

### 1.2 Tipografía

| Elemento | Fuente | Peso | Tamaño |
|---|---|---|---|
| Body general | `'Plus Jakarta Sans', 'Inter', sans-serif` | 400–700 | Base |
| Títulos y headings | `'Outfit', sans-serif` | 400–800 | Variable |
| Marca CITAMEDIC | `'Outfit', sans-serif` | 800 | `1.15rem` |
| Subtítulo marca | `'Outfit', sans-serif` | 700 | `0.65rem` |
| Welcome title | `'Outfit', sans-serif` | 400 | `2.35rem` |
| Card title premium | `'Outfit', sans-serif` | 700 | `1.15rem` |
| Hero card h3 | `'Outfit', sans-serif` | 700 | `1.75rem` |
| Widget BPM | `'Outfit', sans-serif` | 800 | `1.8rem` |

**Fuentes externas cargadas (Google Fonts):**
```html
<link href="https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700;800&family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap" rel="stylesheet">
```

**Íconos:**
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
```

---

## 2. Layout General

### 2.1 Estructura Principal

```
.portal-wrapper (display: flex, min-height: 100vh)
├── aside.portal-sidebar     (width: 290px, sticky, height: 100vh)
└── main.portal-main         (flex-grow: 1, padding: 2.5rem 3rem)
```

### 2.2 Sidebar (`.portal-sidebar`)

| Propiedad | Valor |
|---|---|
| Ancho | `290px` |
| Fondo | `#ffffff` |
| Borde derecho | `1px solid var(--border)` |
| Padding | `1.75rem 1.5rem` |
| Posición | `sticky`, `top: 0`, `height: 100vh` |
| z-index | `100` |
| Transición | `all 0.3s ease` |

#### Secciones del Sidebar (orden de aparición):

1. **`.sidebar-brand-mock`** — Logo + nombre CITAMEDIC / Portal Salud
2. **`.profile-card-mock`** — Tarjeta de perfil activo con nombre del paciente
3. **`.menu-section-header`** — Etiqueta de sección "Menú Principal"
4. **`.sidebar-menu`** — Lista de navegación (8 ítems)
5. **`.sidebar-footer`** — Botón de Cerrar Sesión

#### Logo de Marca (`.brand-logo-mock`)

| Propiedad | Valor |
|---|---|
| Tamaño | `46px × 46px` |
| Fondo | `var(--teal)` = `#2AAE8A` |
| Border-radius | `14px` |
| Ícono | `fa-solid fa-plus` (FontAwesome 6) |
| Color ícono | `white` |

#### Tarjeta de Perfil Activo (`.profile-card-mock`)

| Propiedad | Valor |
|---|---|
| Fondo | `#ffffff` |
| Borde | `1px solid var(--border)` |
| Border-radius | `18px` |
| Padding | `0.85rem 1rem` |
| Margin-bottom | `1.75rem` |
| Sombra | `var(--shadow-sm)` |
| Avatar size | `38px × 38px`, `border-radius: 50%` |
| Avatar fondo | `#f8fafc` |
| Avatar color ícono | `var(--teal)` |
| Avatar ícono | `fa-regular fa-user` |
| Nombre max-width | `140px`, `text-overflow: ellipsis` |

#### Items del Menú (`.menu-link`)

| Estado | Propiedad | Valor |
|---|---|---|
| Normal | `border-radius` | `9999px` (pill) |
| Normal | `padding` | `0.75rem 1.25rem` |
| Normal | `font-size` | `0.925rem` |
| Normal | `font-weight` | `600` |
| Normal | `color` | `var(--navy)` |
| Hover | `color` | `var(--teal)` |
| Hover | `background` | `var(--teal-light)` |
| Activo | `background` | `linear-gradient(135deg, #0a6b59 0%, #044136 100%)` |
| Activo | `color` | `white` |
| Activo | `font-weight` | `700` |
| Activo | `box-shadow` | `0 6px 16px rgba(10, 107, 89, 0.2)` |

#### Ítems del menú (8 opciones):

| Ícono | Etiqueta | Acción |
|---|---|---|
| `fa-house` | Inicio | `switchTab('tab-inicio', this)` |
| `fa-calendar-check` | Mis Citas | `switchTab('tab-mis-citas', this)` |
| `fa-stethoscope` | Especialidades | `switchTab('tab-especialidades', this)` |
| `fa-user-doctor` | Médicos y Horarios | `switchTab('tab-medicos', this)` |
| `fa-calendar-plus` | Reservar Cita | `asp-controller="Citas" asp-action="Reservar"` |
| `fa-heart-pulse` | Triajes | `switchTab('tab-triajes', this)` |
| `fa-user-gear` | Mi Perfil | `switchTab('tab-perfil', this)` |
| `fa-bell` | Notificaciones | `switchTab('tab-notificaciones', this)` |

#### Botón Logout (`.logout-btn-mock`)

| Propiedad | Valor |
|---|---|
| `border-radius` | `9999px` |
| `padding` | `0.75rem 1.25rem` |
| `color` normal | `var(--text-muted)` |
| `color` hover | `#ef4444` |
| `background` hover | `#fee2e2` |
| Acción | `POST` a `/Auth/Logout` con AntiForgeryToken |

### 2.3 Área Principal (`.portal-main`)

| Propiedad | Valor |
|---|---|
| `padding` | `2.5rem 3rem` |
| `max-width` | `calc(100% - 290px)` |
| `overflow-y` | `auto` |

---

## 3. Header de Bienvenida

### Componente `.welcome-header`

| Propiedad | Valor |
|---|---|
| `margin-bottom` | `2.25rem` |

### Título (`.welcome-title`)

| Propiedad | Valor |
|---|---|
| Familia | `'Outfit', sans-serif` |
| `font-weight` | `400` |
| `font-size` | `2.35rem` |
| `color` | `var(--navy)` |
| `margin-bottom` | `0.25rem` |
| Nombre del paciente | `<span style="color: #0a846e; font-weight: 700;">` |

**Comportamiento JS dinámico:**
```javascript
// Saludo según hora del día
06:00–11:59 → "Buenos días, [Nombre] 👋"
12:00–18:59 → "Buenas tardes, [Nombre] 👋"
19:00–05:59 → "Buenas noches, [Nombre] 👋"
// Actualizado via: document.getElementById('welcome-greeting').innerHTML
```

### Status Pill (`.status-pill`)

| Propiedad | Valor |
|---|---|
| `background-color` | `#ffffff` |
| `border` | `1px solid var(--border)` |
| `border-radius` | `9999px` |
| `padding` | `0.5rem 1.25rem` |
| `box-shadow` | `var(--shadow-sm)` |
| Ícono | `fa-solid fa-heart-pulse` con `.heartbeat-anim` |
| Texto | "Todo en orden" |

---

## 4. Tab Panels

### Sistema de Tabs Dinámicos

Los paneles se muestran/ocultan con JavaScript puro (sin jQuery ni Bootstrap Tabs). La función `switchTab(tabId, buttonElement)`:
1. Remueve `.active` de todos los `.tab-panel-custom`
2. Remueve `.active` de todos los `.menu-link`
3. Añade `.active` al panel e item del sidebar seleccionado

```css
.tab-panel-custom {
    display: none;
    animation: slideUp 0.35s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}
.tab-panel-custom.active { display: block; }

@keyframes slideUp {
    from { opacity: 0; transform: translateY(12px); }
    to   { opacity: 1; transform: translateY(0); }
}
```

### Paneles disponibles (8 tabs):

| ID | Contenido |
|---|---|
| `tab-inicio` | Hero card + accesos directos + widgets laterales |
| `tab-mis-citas` | Tabla completa de citas del paciente |
| `tab-especialidades` | Catálogo de especialidades activas |
| `tab-medicos` | Tabla referencial de médicos y horarios (RN07) |
| `tab-historial` | Citas finalizadas / canceladas / no asistió |
| `tab-triajes` | Signos vitales registrados por enfermería |
| `tab-perfil` | Ficha de afiliación + formulario de seguridad |
| `tab-notificaciones` | Buzón de notificaciones |

---

## 5. Tab Inicio — Componentes

### 5.1 Hero Card — Próxima Cita (`.card-next-appointment`)

| Propiedad | Valor |
|---|---|
| `background` | `linear-gradient(135deg, #0a6b59 0%, #044136 100%)` |
| `border-radius` | `var(--radius)` = `24px` |
| `box-shadow` | `0 10px 25px rgba(10, 107, 89, 0.12)` |
| `padding` | `p-5` (Bootstrap = `3rem`) |
| `position` | `relative`, `overflow: hidden` |
| Ícono decorativo | `fa-regular fa-calendar-check`, `14rem`, `rgba(255,255,255,0.04)`, `position: absolute; right: -20px; bottom: -40px; rotate(-10deg)` |

#### Estado: Con cita programada
- Badge de estado: `badge-premium-status` con `EstadoCita`
- Título: "Tienes una cita programada" — `1.75rem`, `font-weight: 700`
- Grid de 4 columnas (`col-md-3`): Especialidad, Médico, Fecha, Hora
- Botones: `.btn-premium-pill` (Ver ticket) + link cancelar (si aplica por RN36)

#### Estado: Sin cita programada
- Badge: "PRÓXIMA CITA" con `fa-regular fa-clock`
- Título: "No tienes citas programadas para hoy." — `1.75rem`
- Subtítulo: texto motivacional, `max-width: 500px`
- CTA: `.btn-premium-pill` → `/Citas/Reservar`

#### Badge de Estado (`.badge-premium-status`)

| Propiedad | Valor |
|---|---|
| `padding` | `0.45rem 0.9rem` |
| `border-radius` | `9999px` |
| `font-size` | `0.725rem` |
| `font-weight` | `700` |
| `background` | `rgba(255, 255, 255, 0.15)` |
| `border` | `1px solid rgba(255, 255, 255, 0.12)` |
| `letter-spacing` | `0.5px` |

#### Botón Pill (`.btn-premium-pill`)

| Propiedad | Valor |
|---|---|
| `background-color` | `white` |
| `color` | `var(--green-hero-start)` = `#0a6b59` |
| `border-radius` | `9999px` |
| `padding` | `0.75rem 1.75rem` |
| `font-weight` | `700` |
| `font-size` | `0.925rem` |
| Hover `background` | `#f2faf8` |
| Hover `transform` | `translateY(-1px)` |

### 5.2 Accesos Directos (`.shortcut-card-mock`)

Layout: `row g-3` con 3 columnas `col-md-4`

| Propiedad | Valor |
|---|---|
| `border-radius` | `20px` |
| `padding` | `1.25rem` |
| `box-shadow` | `var(--shadow-sm)` |
| Hover `transform` | `translateY(-3px)` |
| Hover `border-color` | `var(--teal)` |

| Acceso | Ícono | Color icono | Fondo icono | Destino |
|---|---|---|---|---|
| Reservar Cita | `fa-calendar-plus` | `var(--teal)` | `var(--teal-light)` | `/Citas/Reservar` |
| Mis Citas | `fa-calendar-check` | `#4f46e5` | `#e0e7ff` (indigo) | `switchTab('tab-mis-citas')` |
| Mi Perfil | `fa-user-gear` | `#d97706` | `#fffbeb` (amber) | `switchTab('tab-perfil')` |

Tamaño del ícono: `48px × 48px`, `border-radius: 14px`

### 5.3 Widget "Mi Familia" (Columna lateral)

| Propiedad | Valor |
|---|---|
| `border-radius` | `var(--radius)` |
| `min-height` | `112px` |
| Layout | `display: flex; flex-direction: column; justify-content: space-between` |
| Título ícono | `fa-solid fa-users`, color `var(--teal)` |
| Badge "VER TODOS" | fondo `#e0e7ff`, color `#4f46e5`, `font-size: 0.65rem` |
| Botón agregar | `.add-family-btn`, `36px × 36px`, dashed border `#cbd5e1` |

### 5.4 Widget "Último Pulso" (Columna lateral)

| Propiedad | Valor |
|---|---|
| `border-radius` | `var(--radius)` |
| `min-height` | `112px` |
| Layout | `display: flex; align-items: center; justify-content: space-between` |
| Avatar ícono | `fa-solid fa-heart-pulse`, `48px × 48px`, fondo `#fee2e2`, color `#ef4444` |
| Animación ícono | `.heartbeat-anim` — escala 1 → 1.15 → 1 cada 1.2s |
| Valor | `72 BPM`, `1.8rem`, `font-family: Outfit`, `font-weight: 800` |
| Badge estado | `Normal`, color `text-success` |

---

## 6. Componentes de Tarjetas Generales

### `.card-premium` (tarjeta base)

| Propiedad | Valor |
|---|---|
| `background` | `white` |
| `border-radius` | `var(--radius)` = `24px` |
| `border` | `1px solid var(--border)` |
| `box-shadow` | `var(--shadow-sm)` |
| `margin-bottom` | `1.5rem` |
| `overflow` | `hidden` |

### `.card-header-premium`

| Propiedad | Valor |
|---|---|
| `padding` | `1.25rem 1.75rem` |
| `border-bottom` | `1px solid var(--border)` |
| Título font-size | `1.15rem`, `font-family: Outfit`, `font-weight: 700` |

### `.card-body-premium`

| Propiedad | Valor |
|---|---|
| `padding` | `1.75rem` |

---

## 7. Botones del Sistema

| Clase | Uso | Fondo | Color | Border-radius |
|---|---|---|---|---|
| `.btn-premium` | Acción primaria (teal) | `var(--teal)` = `#2AAE8A` | `white` | `12px` |
| `.btn-premium-light` | Acción secundaria sobre hero (translúcido) | `rgba(255,255,255,0.15)` | `white` | `12px` |
| `.btn-premium-outline` | Acción neutral / vista ticket | `white` | `var(--text-main)` | `12px` |
| `.btn-premium-danger` | Cancelación | `#ef4444` | `white` | `12px` |
| `.btn-premium-pill` | CTA principal sobre hero card | `white` | `#0a6b59` | `9999px` |

Todos los botones comparten:
- `padding: 0.65rem 1.25rem` (excepto pill: `0.75rem 1.75rem`)
- `font-weight: 600` (pill: `700`)
- `transition: all 0.2s ease`
- Hover: `transform: translateY(-1px)` + sombra de color

---

## 8. Badges de Estado de Cita

Usados en tablas y tarjetas para indicar el estado de la cita médica.

| Clase | Estado | Fondo | Color texto |
|---|---|---|---|
| `.badge-dot-Pendiente` | Pendiente | `#dbeafe` | `#1e40af` (azul) |
| `.badge-dot-EnTriaje` | En Triaje | `#fef3c7` | `#92400e` (ámbar) |
| `.badge-dot-ListoAtencion` | Listo Atención | `#d1fae5` | `#065f46` (verde) |
| `.badge-dot-NoAsistio` | No Asistió | `#fee2e2` | `#991b1b` (rojo) |
| `.badge-dot-Cancelada` | Cancelada | `#f1f5f9` | `#475569` (slate) |

Propiedades comunes: `border-radius: 9999px`, `font-size: 0.725rem`, `font-weight: 700`, `padding: 0.3rem 0.65rem`

---

## 9. Tabla Premium (`.table-premium`)

| Elemento | Propiedad | Valor |
|---|---|---|
| `th` | `color` | `var(--navy)` |
| `th` | `font-size` | `0.75rem` |
| `th` | `font-weight` | `700` |
| `th` | `text-transform` | `uppercase` |
| `th` | `background-color` | `#f8fafc` |
| `th` | `border-bottom` | `1.5px solid var(--border)` |
| `th` | `padding` | `1rem 1.5rem` |
| `td` | `color` | `#334155` |
| `td` | `font-size` | `0.925rem` |
| `td` | `border-bottom` | `1px solid var(--border)` |
| `td` | `padding` | `1.25rem 1.5rem` |

---

## 10. Grilla de Perfil (`.profile-info-grid`)

| Propiedad | Valor |
|---|---|
| `display` | `grid` |
| `grid-template-columns` | `repeat(auto-fit, minmax(240px, 1fr))` |
| `gap` | `1.5rem` |

Cada ítem (`.info-item`):
- `background: #f8fafc`, `padding: 1.25rem`, `border-radius: 12px`, `border: 1px solid var(--border)`
- Etiqueta (`.info-label`): `0.725rem`, uppercase, `var(--text-muted)`
- Valor (`.info-value`): `font-weight: 700`, `var(--navy)`, `1.05rem`

---

## 11. Animaciones

### `heartbeat` — Latido del corazón
```css
@keyframes heartbeat {
    0%  { transform: scale(1); }
    14% { transform: scale(1.15); }
    28% { transform: scale(1); }
    42% { transform: scale(1.15); }
    70% { transform: scale(1); }
}
/* Duración: 1.2s, infinite */
```

### `slideUp` — Entrada de paneles
```css
@keyframes slideUp {
    from { opacity: 0; transform: translateY(12px); }
    to   { opacity: 1; transform: translateY(0); }
}
/* Duración: 0.35s, cubic-bezier(0.16, 1, 0.3, 1) */
```

### `pulse-ring` — Pulso del status indicator
```css
@keyframes pulse-ring {
    0%   { transform: scale(0.5); opacity: 1; }
    100% { transform: scale(2);   opacity: 0; }
}
/* Duración: 1.5s, infinite, ease-out */
```

> **Nota técnica Razor:** En vistas `.cshtml`, los `@keyframes` y `@media` deben escribirse como `@@keyframes` y `@@media` para evitar que el compilador Razor los interprete como directivas C# (error CS0103).

---

## 12. Responsive (Breakpoint ≤ 991.98px)

| Elemento | Desktop | Mobile/Tablet |
|---|---|---|
| `.portal-wrapper` | `flex-direction: row` | `flex-direction: column` |
| `.portal-sidebar` | `width: 290px`, sticky, `height: 100vh` | `width: 100%`, `height: auto`, `position: relative` |
| `.sidebar-menu` | `flex-direction: column`, `gap: 0.45rem` | `flex-direction: row`, `flex-wrap: wrap`, `gap: 0.3rem` |
| `.menu-link` | `width: 100%`, `padding: 0.75rem 1.25rem` | `width: auto`, `padding: 0.6rem 1rem`, `font-size: 0.85rem` |
| `.portal-main` | `padding: 2.5rem 3rem` | `padding: 1.75rem 1.5rem`, `max-width: 100%` |

---

## 13. Modales

### Modal Ticket (`.modal#modalTicket`)
- `modal-dialog-centered`
- `border-radius: 16px`
- Muestra: código de ticket, paciente, especialidad, médico, fecha, hora, estado
- Botón PDF: link dinámico a `/Citas/Ticket/{citaId}`

### Modal Confirmación Cancelación (`.modal#modalCancelar`)
- `modal-sm`, centrado
- Ícono de advertencia `fa-triangle-exclamation`, fondo `#fee2e2`
- 2 botones: "Mantener cita" (outline) / "Sí, Cancelar" (danger)
- Lógica: AJAX POST a `/Paciente/CancelarCita` con AntiForgeryToken

---

## 14. Seguridad — AntiForgery Token

Todas las peticiones AJAX (cancelación, actualización de perfil) leen el token del formulario oculto:

```html
<form id="antiForgeryForm" style="display:none;">
    @Html.AntiForgeryToken()
</form>
```

```javascript
const token = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]').value;
// Enviado en header: 'RequestVerificationToken': token
```

---

## 15. Reglas de Negocio Aplicadas en la Vista

| Regla | Implementación en UI |
|---|---|
| **RN07** | Tab "Médicos y Horarios" no muestra UPS ni cupos internos, solo datos referenciales |
| **RN33 / RN35** | Disclaimer de emergencias visible en la pestaña Inicio |
| **RN34** | Número de HC generado como `HC-{PacienteId:D6}` en tab Perfil |
| **RN36** | Botón "Cancelar" solo visible si `EstadoCita == Pendiente` y no se superó el límite horario (07:40 mañana / 14:40 tarde) |
| **RN38** | Tab Historial: estado "Asistió / Faltó / Cancelada" derivado de `EstadoCita` |

---

## 16. Portal del Administrador (`Views/Admin/Index.cshtml`)

### 16.1 Layout Administrativo
El panel del administrador sigue la misma estética visual (Clinic Calm Premium) pero con un enfoque en la gestión de datos.
- **`.card-admin`**: Tarjeta con borde superior coloreado (border-top-color) según el tipo de métrica (Pacientes = Teal, Médicos = Índigo, Programaciones = Naranja).
- **Widgets de Métricas**: Ubicados en la cabecera, muestran conteos rápidos con iconos grandes e indicativos visuales.

### 16.2 Gestión de Programaciones Operativas
- **Listado y Filtros**: Tabla interactiva con estado visual de "Habilitado" vs "Borrador".
- **Formulario de Creación**: 
  - Layout en 2 columnas.
  - Inputs con estilo premium (`border-radius: 12px`, padding interior amplio, shadow al hacer focus).
  - Almacena campos clave: Especialidad, Médico, Fecha, Turno, Duración y Total de Cupos.
  - Al guardar, auto-genera los sub-registros (Slots) en base a la configuración y redirige actualizando la UI.

---

## 17. Flujo de Reserva de Citas (`Views/Citas/Reservar.cshtml`)

### 17.1 Interfaz de Pasos (Wizard)
El paciente completa su reserva en pasos progresivos sin recargar la página:
1. **Especialidad** (Tarjetas con iconos grandes).
2. **Turno** (Mañana/Tarde).
3. **Selección de Horarios (Slots)**.

### 17.2 Tarjetas de Horario (Slots)
- **Agrupamiento Dinámico por Fecha**: Los horarios se muestran agrupados bajo un título visual elegante (Ej: `📅 Sábado, 27 de Mayo`).
- **Estados Visuales**:
  - **Disponible**: Borde `var(--teal)`, fondo blanco, badge verde claro.
  - **Agotado / Ocupado**: Clase `.disabled`, texto grisáceo, badge `#e2e8f0`. Cursor `not-allowed`.

### 17.3 Confirmación Premium
- **Alerta Flotante**: Tras confirmar la cita, el sistema **no oculta** el flujo anterior (para preservar contexto). Aparece un `.alert-success` en la parte superior con un icono `fa-check-circle` y colores institucionales (`--teal`).
- **Auto-scroll Smooth**: La interfaz sube automáticamente a la alerta de éxito (`window.scrollTo(0)`).
- **Eliminación de "Nueva Reserva"**: Por restricción estricta (1 cita por día, FA02), se eliminó la posibilidad de reservar en bucle; se redirige directamente al historial (`/Paciente/Index`) o al inicio.

---

## 18. Ficha Electrónica (Ticket) (`Views/Citas/Ticket.cshtml`)

### 18.1 Diseño de Impresión (PDF)
El ticket electrónico simula un recibo médico físico premium:
- **Contenedor Principal**: `.modern-card` con `border-top` prominente.
- **Código Hash**: Estilo `monospace` extragrande, letter-spacing amplio.
- **Datos Estructurados**: Grid de dos columnas con bordes inferiores divisorios ligeros.
- **`@@media print`**: CSS dedicado que oculta botones, márgenes, navegadores y sombras (`box-shadow: none !important;`) garantizando una impresión limpia en hoja A4/A5 o guardado como PDF nativo (CU11).

---

## 19. Portal de Admisión (`Views/Admision/Index.cshtml`)

### 19.1 Diseño Funcional (Recepción)
- **Dashboard Orientado a Tareas**: Destaca atajos para "Habilitar Programaciones", "Registro Presencial" y "Consulta de Pacientes".
- **Sistema de Color**: Emplea tonos que transmiten orden (índigo o gris cálido) diferenciándolo de los colores clínicos (verde/teal).
- **Tablas de Trabajo**: Diseño optimizado para visualizar grandes volúmenes de citas (fuente ligeramente más compacta, botones de acción en línea y badges de estado actualizados en tiempo real).
- **Formularios de Búsqueda**: Inputs anchos en la cabecera para buscar rápidamente pacientes por DNI o número de historia clínica.

---

## 20. Portal de Enfermería (`Views/Enfermeria/Index.cshtml`)

### 20.1 Registro Clínico Rápido (Triaje)
- **Interfaz "Libre de Distracciones"**: Minimiza los elementos decorativos para priorizar los campos clínicos (Tensión Arterial, Temperatura, Peso, etc.).
- **Indicadores Visuales**:
  - Campos numéricos con validación visual (borde rojo si se excede el rango lógico).
  - Alertas estilo banner (warning) cuando un triaje ya ha sido registrado (Prevención de duplicidad RN19).
- **Lista de Citas (Cola de Pacientes)**: Paneles colapsables o tabla destacando las citas cuyo estado es `Pendiente`, usando badges parpadeantes o botones prominentes de "Registrar Triaje".

---

## 21. Portal del Médico (`Views/Medico/Index.cshtml` / `Views/Home/Index.cshtml`)

### 21.1 Gestión de Consultas Diarias
- **Vista de Agenda (Time-blocking)**: Visualización en formato de lista cronológica destacando los horarios y los pacientes citados.
- **Estados Paciente**: Identificación rápida (Badge) indicando si el paciente ya pasó por Triaje y está `ListoAtencion`.
- **Modales de Detalles Clínicos**: Al hacer clic en un paciente, un modal lateral (offcanvas) o centrado revela la información del ticket, motivo y signos vitales registrados en el triaje sin perder el contexto de la lista.
- **Acciones Rápidas**: Botones limpios (`.btn-premium-outline`) para "Llamar Paciente", "Ver Historial" o "Atender", integrados armónicamente al esquema Clinic Calm Premium.

# Especificación de Interfaz de Usuario (UI/UX) - Dashboard de Pacientes

Este documento define el sistema de diseño visual adoptado para el Dashboard de Pacientes, combinando una estética profesional de salud con altos estándares de usabilidad.

---

## 1. Sistema de Diseño Visual

### Paleta de Colores
* **Azul Profesional (Color Primario):** `#1E3A8A` (Deep Blue) y `#2563EB` (Royal Blue). Transmite confianza, autoridad y seriedad médica. Usado en encabezados, botones principales y métricas clave.
* **Verde Sanación (Color Secundario / Acierto):** `#10B981` (Emerald Green) y `#059669` (Medium Emerald). Representa salud, bienestar y recuperación. Usado para indicadores de estado activo, seguros (SIS) y botones de éxito.
* **Fondo de Interfaz:** `#F8FAFC` (Slate Gray muy claro) que evita la fatiga visual provocada por el blanco puro.
* **Texto Principal:** `#0F172A` (Slate Dark) para contraste y legibilidad óptima.

### Tipografía
* Se utiliza la fuente sans-serif del sistema moderno (`Segoe UI`, `Roboto` o `Outfit`), con tamaños y grosores tipográficos definidos por jerarquía visual:
  - Títulos Principales: `fs-2` o `fs-3`, `fw-bold`
  - Subtítulos / Resúmenes: `fs-5` o `fs-6`, `text-muted`
  - Datos de Tablas: `fw-medium`

---

## 2. Estructura y Distribución del Dashboard (Responsive Layout)

El Dashboard de Pacientes se organiza en una arquitectura limpia de tres niveles:

1. **Tarjetas de Resumen Ejecutivo (Métricas)**:
   - Tarjeta 1: Total de Pacientes Registrados (Azul Profesional).
   - Tarjeta 2: Pacientes con Seguro SIS (Verde Sanación).
   - Tarjeta 3: Pacientes Menores de Edad (Celeste/Cian).

2. **Panel de Búsqueda y Filtros**:
   - Barra de búsqueda interactiva por DNI (8 dígitos) para filtrado rápido.
   - Filtros de estado y SIS.

3. **Directorio y Listado de Pacientes**:
   - Tabla responsiva con bordes redondeados y sombras difuminadas (`box-shadow`), estrictamente sin bordes sólidos agresivos.
   - Acciones integradas por paciente (Editar, Ver Citas, Estado).

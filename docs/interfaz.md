## Especificación de Interfaz de Usuario (UI/UX)

Para garantizar la usabilidad y reducir la fatiga visual del personal médico y administrativo, el sistema adopta una interfaz basada en el diseño por componentes (Tiles/Mosaicos) con los siguientes lineamientos:

**1. Paleta de Colores Corporativa:**
* **Color Primario (Acciones/Botones):** Teal/Cian Oscuro (`#008B8B`). Transmite limpieza y salud.
* **Color de Fondo General:** Degradado suave de Cian Claro (`#E0F7FA`) a Blanco, evitando el blanco puro absoluto para reducir el contraste agresivo.
* **Texto:** Gris oscuro (`#333333`) para legibilidad.

**2. Lineamientos de Estilo de Componentes:**
* **Tipografía:** Sistema Sans-serif moderno (`Segoe UI`, `Roboto` o `Poppins`).
* **Tarjetas y Mosaicos (Cards):** Estrictamente sin bordes sólidos (`border: none`). Se utilizarán esquinas pronunciadamente redondeadas (`border-radius: 16px`) y sombras difuminadas (`box-shadow`) para separar los elementos del fondo mediante elevación (Z-index visual).
* **Interactividad:** Los módulos principales se presentarán como áreas clicables amplias con retroalimentación visual al pasar el cursor (Hover effect), priorizando el espaciado (padding) para evitar la saturación de información.
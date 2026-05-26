# Panel de Administrador - Documentación de Cambios

## 📋 Resumen
Se ha transformado la página de Panel del Administrador (Admin/Index.cshtml) de una vista estática con solo descripciones a un **panel funcional completo** con formularios interactivos para crear, editar y desactivar entidades en la base de datos.

## 🎯 Funcionalidades Implementadas

### 1. **Gestión de UPS (Unidades Prestadoras de Salud)**
- ✅ **Crear UPS**: Formulario para ingresar nombre de la UPS
- ✅ **Listar UPS**: Tabla con todas las UPS registradas
- ✅ **Editar UPS**: Modal para actualizar nombre
- ✅ **Desactivar UPS**: Marcar como inactiva (sin eliminar de BD)

### 2. **Gestión de Especialidades**
- ✅ **Crear Especialidad**: Formulario con campos:
  - Nombre de la especialidad
  - UPS asociada (dropdown dinámico)
  - Duración en minutos
- ✅ **Listar Especialidades**: Tabla con información completa
- ✅ **Editar Especialidad**: Modal para actualizar datos
- ✅ **Desactivar Especialidad**: Marcar como inactiva

### 3. **Características Generales**
- ✅ Mensajes de éxito al crear/actualizar/desactivar
- ✅ Validación de datos en formularios
- ✅ Confirmación de eliminación (desactivación)
- ✅ Tablas responsivas con estado visual (badges)
- ✅ Integración completa con Base de Datos

## 📁 Archivos Modificados

### **Controllers/AdminController.cs**
```
Cambios:
- Inyección de dependencias para UPS y Especialidad repositories
- Método Index(): Carga datos de BD y los pasa a la vista
- CreateUPS(): POST para crear nueva UPS
- CreateEspecialidad(): POST para crear especialidad
- UpdateUPS(): POST para actualizar UPS
- UpdateEspecialidad(): POST para actualizar especialidad
- DeleteUPS(): POST para desactivar UPS
- DeleteEspecialidad(): POST para desactivar especialidad
```

### **Views/Admin/Index.cshtml**
```
Cambios:
- Actualización del modelo a AdminDashboardViewModel
- Formulario de creación de UPS con validación
- Formulario de creación de Especialidad con dropdown dinámico
- Tablas de datos con botones de acción (Editar, Desactivar)
- Inclusión de parciales para modales
- Scripts para cargar datos en los modales
```

### **Models/ViewModels/**
```
Nuevos archivos:
- CreateUPSViewModel.cs: Modelo para crear UPS
- CreateEspecialidadViewModel.cs: Modelo para crear Especialidad

Modificado:
- AdminDashboardViewModel.cs: Agregadas colecciones de UPS y Especialidades
```

### **Views/Admin/**
```
Nuevos archivos:
- _EditUPSModal.cshtml: Modal para editar UPS
- _EditEspecialidadModal.cshtml: Modal para editar Especialidad
```

## 🔄 Flujo de Datos

### Crear UPS
1. Usuario rellena formulario → POST a CreateUPS()
2. Validación de datos
3. Inserción en BD
4. Redirect a Index con mensaje de éxito
5. Tabla se actualiza con la nueva UPS

### Crear Especialidad
1. Usuario selecciona UPS en dropdown → POST a CreateEspecialidad()
2. Validación (nombre, UPS, duración)
3. Inserción en BD
4. Redirect a Index
5. Tabla se actualiza

### Editar Entidad
1. Click en botón "Editar" → Modal se abre
2. Script carga datos en los campos
3. Usuario modifica datos → POST a UpdateUPS/UpdateEspecialidad()
4. Actualización en BD
5. Redirect a Index

### Desactivar Entidad
1. Click en "Desactivar" → Confirmación
2. POST a DeleteUPS/DeleteEspecialidad()
3. Marca Activa = false en BD
4. Redirect a Index

## 🛡️ Validaciones

### UPS
- Nombre requerido (3-100 caracteres)
- Confirmación antes de desactivar

### Especialidad
- Nombre requerido (3-100 caracteres)
- UPS requerida (debe seleccionar una)
- Duración: 1-480 minutos
- Confirmación antes de desactivar

## 📊 Base de Datos

No se han hecho cambios en la estructura de BD. Se utilizan las tablas existentes:
- `UPS`: Tabla de unidades prestadoras
- `Especialidades`: Tabla de especialidades

## 🚀 Cómo Usar

1. **Crear UPS**:
   - Ir a sección "UPS"
   - Ingresar nombre en el formulario
   - Click en "Crear UPS"

2. **Crear Especialidad**:
   - Ir a sección "Especialidades"
   - Rellenar nombre, seleccionar UPS, ingresar duración
   - Click en "Crear Especialidad"

3. **Editar**:
   - Click en botón "Editar" en la tabla
   - Modificar datos en el modal
   - Click en "Guardar cambios"

4. **Desactivar**:
   - Click en "Desactivar"
   - Confirmar en el diálogo
   - La entidad se marca como inactiva

## ✅ Testing

Para verificar que todo funciona:

```bash
dotnet build
dotnet run
```

Luego:
1. Navegar a `/Admin`
2. Crear una nueva UPS
3. Crear una especialidad asociada a esa UPS
4. Editar ambas
5. Desactivar para confirmar que cambian de estado

## 📝 Notas Importantes

- Las entidades **no se eliminan** sino que se marcan como inactivas (`Activa = false`)
- Los dropdowns de UPS solo muestran UPS **activas**
- Los formularios tienen validación client y server-side
- Los mensajes de éxito aparecen usando `TempData`
- El controlador debe estar autenticado como "Administrador"

## 🔒 Seguridad

- Controlador protegido con `[Authorize(Roles = "Administrador")]`
- Validación de entrada en servidor
- Confirmaciones antes de operaciones destructivas (desactivación)

## 🎨 UI/UX

- Interfaz moderna con Bootstrap
- Tablas responsivas
- Modales Bootstrap para edición
- Badges para estados (Activa/Inactiva)
- Feedback visual con alertas de éxito

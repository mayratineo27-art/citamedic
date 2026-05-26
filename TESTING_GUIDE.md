# Guía de Prueba - Panel de Administrador

## 🧪 Pruebas Funcionales

### Requisitos
- Estar autenticado como usuario con rol **Administrador**
- Tener acceso a la base de datos SQL Server
- La URL debe ser: `https://localhost:XXXX/Admin`

---

## ✅ Test 1: Crear UPS

**Objetivo**: Verificar que se puede crear una nueva UPS y aparece en la tabla

**Pasos**:
1. En la sección "UPS", ingresar nombre: "Urgencias"
2. Click en "Crear UPS"
3. Verificar:
   - ✓ Aparece mensaje de éxito
   - ✓ La nueva UPS aparece en la tabla
   - ✓ Estado es "Activa"
   - ✓ Se puede ver el botón "Editar" y "Desactivar"

**Resultado esperado**: UPS creada correctamente en BD

---

## ✅ Test 2: Crear Especialidad

**Objetivo**: Verificar creación de especialidad con relación a UPS

**Pasos**:
1. En la sección "Especialidades", rellenar:
   - Nombre: "Cardiología"
   - UPS: "Urgencias" (la que creamos)
   - Duración: "30" minutos
2. Click en "Crear Especialidad"
3. Verificar:
   - ✓ Aparece mensaje de éxito
   - ✓ La especialidad aparece en tabla con UPS correcta
   - ✓ La duración se ve como "30" minutos

**Resultado esperado**: Especialidad creada con relación correcta a UPS

---

## ✅ Test 3: Validación de Campos

**Objetivo**: Verificar que no permite datos inválidos

**Pasos para UPS**:
1. Intentar crear UPS con nombre vacío → No debe permitir envío
2. Intentar crear UPS con nombre de 1 carácter → Validación debe activarse

**Pasos para Especialidad**:
1. Dejar especialidad vacía → No debe permitir
2. No seleccionar UPS → Error de validación
3. Ingresar duración > 480 → Error de validación
4. Ingresar duración = 0 → Error de validación

**Resultado esperado**: Todas las validaciones funcionan correctamente

---

## ✅ Test 4: Editar UPS

**Objetivo**: Verificar que se puede editar el nombre de una UPS

**Pasos**:
1. En tabla de UPS, click en botón "Editar" de una UPS
2. Se abre modal con datos precargados
3. Cambiar nombre de "Urgencias" a "Urgencias Cardiology"
4. Click en "Guardar cambios"
5. Verificar:
   - ✓ Modal se cierra
   - ✓ Mensaje de éxito aparece
   - ✓ Tabla se actualiza con nuevo nombre
   - ✓ BD tiene el nuevo valor

**Resultado esperado**: Edición exitosa

---

## ✅ Test 5: Editar Especialidad

**Objetivo**: Verificar que se puede editar especialidad completa

**Pasos**:
1. En tabla de Especialidades, click en "Editar"
2. Modal se abre con datos precargados
3. Cambiar:
   - Nombre: "Cardiología Pediátrica"
   - Duración: "40" minutos
4. Click en "Guardar cambios"
5. Verificar actualización en tabla

**Resultado esperado**: Edición correcta con persistencia en BD

---

## ✅ Test 6: Desactivar UPS

**Objetivo**: Verificar que desactivar cambia el estado

**Pasos**:
1. Click en "Desactivar" en una UPS
2. Confirmar en diálogo de JavaScript
3. Verificar:
   - ✓ Mensaje de éxito
   - ✓ Badge cambia a rojo "Inactiva"
   - ✓ Desaparece el botón "Editar"/"Desactivar"
   - ✓ Ya no aparece en dropdowns de especialidades

**Resultado esperado**: UPS marcada como inactiva

---

## ✅ Test 7: Desactivar Especialidad

**Objetivo**: Verificar desactivación de especialidad

**Pasos**:
1. Click en "Desactivar" en una especialidad
2. Confirmar
3. Verificar:
   - ✓ Badge es rojo "Inactiva"
   - ✓ Sin botones de acción

**Resultado esperado**: Especialidad inactiva

---

## ✅ Test 8: Dropdown de UPS Dinámico

**Objetivo**: Verificar que el dropdown solo muestra UPS activas

**Pasos**:
1. Crear una UPS "Test"
2. Ir a crear especialidad → Debe aparecer en dropdown
3. Desactivar "Test"
4. Refresh la página o ir a crear otra especialidad
5. Verificar que "Test" NO aparece en el dropdown

**Resultado esperado**: Dropdown dinámico filtra UPS activas

---

## ✅ Test 9: Persistencia en BD

**Objetivo**: Verificar que datos se guardan realmente en BD

**Pasos**:
1. Crear UPS y Especialidad
2. Cerrar la aplicación completamente
3. Reiniciar la aplicación
4. Ir a Admin
5. Verificar que datos siguen ahí

**Resultado esperado**: Datos persisten en BD

---

## ✅ Test 10: Autorización

**Objetivo**: Verificar que solo administradores acceden

**Pasos**:
1. Si está logueado, logout
2. Ir a `/Admin` sin estar autenticado
3. Debe redirigir a Login
4. Loguearse como usuario NO administrador
5. Ir a `/Admin` → Debe negar acceso

**Resultado esperado**: Solo Administradores acceden

---

## 🐛 Checklist de Validación

- [ ] UPS se crea exitosamente
- [ ] Especialidades se crean con UPS correcta
- [ ] Ediciones se guardan en BD
- [ ] Desactivaciones funcionan
- [ ] Validaciones evitan datos inválidos
- [ ] Mensajes de éxito aparecen
- [ ] Tablas se actualizan sin refresh manual
- [ ] Dropdowns muestran datos correctos
- [ ] Datos persisten después de reinicio
- [ ] Autorización funciona correctamente

---

## 📋 Tabla de Estado de Pruebas

| Test | Resultado | Notas |
|------|-----------|-------|
| Crear UPS | ✓/✗ | |
| Crear Especialidad | ✓/✗ | |
| Validaciones | ✓/✗ | |
| Editar UPS | ✓/✗ | |
| Editar Especialidad | ✓/✗ | |
| Desactivar UPS | ✓/✗ | |
| Desactivar Especialidad | ✓/✗ | |
| Dropdown Dinámico | ✓/✗ | |
| Persistencia BD | ✓/✗ | |
| Autorización | ✓/✗ | |

---

## 🔍 Debugging

Si hay problemas, verificar:

1. **Base de datos**: `select * from UPS` y `select * from Especialidades`
2. **Logs de aplicación**: Verificar errores en consola
3. **Network**: F12 → Network → Verificar llamadas POST
4. **Validación**: Verificar ModelState en servidor
5. **Autorización**: Verificar rol del usuario autenticado

---

## 📞 Contacto

Cualquier problema reportar con:
- Pasos para reproducir
- Mensaje de error exacto
- Estado de BD
- Rol del usuario

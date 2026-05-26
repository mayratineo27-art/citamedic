# 🚀 Guía de Instalación y Ejecución

## 📋 Requisitos Previos

- ✅ .NET 10 SDK instalado
- ✅ SQL Server (local o remoto)
- ✅ Visual Studio 2026 Community (o superior)
- ✅ Git (para clonar si es necesario)

---

## 🔧 Instalación

### 1. Preparar la Base de Datos

```bash
# Ubicarse en la carpeta del proyecto
cd C:\Users\MAYRATM\source\repos\PostaCitasWeb

# Ejecutar migraciones (si no están aplicadas)
dotnet ef database update
```

**Nota**: Las tablas UPS y Especialidades deben existir en la BD.

### 2. Restaurar Dependencias

```bash
# Restaurar paquetes NuGet
dotnet restore
```

### 3. Compilar Solución

```bash
# Compilar proyecto
dotnet build

# Resultado esperado:
# ✅ Compilación correcta
# ✅ Sin errores CS
```

---

## ▶️ Ejecución

### Opción 1: Desde Visual Studio

```
1. Abrir PostaCitasWeb.sln en Visual Studio
2. Click derecho en solución → Rebuild
3. Presionar F5 o Click en ▶ Play
4. Se abrirá en browser en https://localhost:7XXX
```

### Opción 2: Desde PowerShell

```powershell
# En la carpeta del proyecto
cd C:\Users\MAYRATM\source\repos\PostaCitasWeb

# Ejecutar
dotnet run

# Resultado esperado:
# Now listening on: https://localhost:7XXX
# Now listening on: http://localhost:5XXX
```

---

## 🔐 Autenticación

### 1. Login

```
URL: https://localhost:XXXX/Auth/Login

Credenciales de prueba:
- Usuario: admin@admin.com
- Contraseña: Admin123!
```

### 2. Verificar Rol

Una vez logueado, verificar que el usuario tenga rol **"Administrador"**

```sql
-- En SQL Server
SELECT u.*, r.Nombre as Rol
FROM Usuarios u
JOIN Roles r ON u.RolId = r.RolId
WHERE u.Email = 'admin@admin.com'
```

---

## 📍 Acceder al Panel de Administrador

### Una vez autenticado:

```
URL Directa: https://localhost:XXXX/Admin

O navegar desde menú:
1. Login exitoso
2. Dashboard
3. Admin → Panel del Administrador
```

---

## ✅ Verificar Funcionamiento

### Test 1: Crear UPS

```
1. Ir a /Admin
2. Scroll a sección "UPS"
3. Ingresar nombre: "Test UPS"
4. Click "Crear UPS"

Resultado esperado:
✅ Mensaje "UPS creada exitosamente"
✅ Aparece en tabla
✅ Estado "Activa"
```

### Test 2: Crear Especialidad

```
1. Scroll a sección "Especialidades"
2. Ingresar:
   - Nombre: "Test Especialidad"
   - UPS: "Test UPS" (la que acabamos de crear)
   - Duración: 30 minutos
3. Click "Crear Especialidad"

Resultado esperado:
✅ Aparece en tabla
✅ Muestra UPS correcta
✅ Muestra duración 30
```

### Test 3: Editar

```
1. En tabla de UPS, click "Editar"
2. Modal se abre
3. Cambiar nombre a "Test UPS Editado"
4. Click "Guardar cambios"

Resultado esperado:
✅ Modal se cierra
✅ Tabla se actualiza
✅ Nombre nuevo visible
```

### Test 4: Desactivar

```
1. Click "Desactivar" en una entrada
2. Confirmar en diálogo

Resultado esperado:
✅ Badge cambia a rojo "Inactiva"
✅ Desaparecen botones de acción
✅ Ya no aparece en dropdowns
```

---

## 🐛 Solución de Problemas

### Error: "Base de datos no encontrada"

```bash
# Solución: Crear migraciones
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Error: "Usuario no tiene rol Administrador"

```sql
-- Asignar rol en BD
UPDATE Usuarios 
SET RolId = (SELECT RolId FROM Roles WHERE Nombre = 'Administrador')
WHERE Email = 'admin@admin.com'
```

### Error: "Puerto ocupado"

```powershell
# Cambiar puerto en launchSettings.json
# O usar:
dotnet run --urls "https://localhost:8000;http://localhost:5000"
```

### Error: "Tablas no existen en BD"

```bash
# Ejecutar todas las migraciones pendientes
dotnet ef database update

# O recrear BD
dotnet ef database drop
dotnet ef database update
```

---

## 📊 Verificación de Base de Datos

### Consultas útiles

```sql
-- Ver todas las UPS
SELECT * FROM UPS WHERE Activa = 1

-- Ver especialidades activas
SELECT e.*, u.Nombre as UPSNombre 
FROM Especialidades e
JOIN UPS u ON e.UPSId = u.UPSId
WHERE e.Activa = 1

-- Ver usuario admin
SELECT * FROM Usuarios WHERE Email = 'admin@admin.com'

-- Ver roles
SELECT * FROM Roles
```

---

## 🔄 Flujo Típico de Uso

```
1. Iniciar aplicación
2. Login con credentials admin
3. Navegar a /Admin
4. Panel carga datos de BD
5. Crear UPS → Insert en BD
6. Crear Especialidad → Insert en BD
7. Editar entidad → Update en BD
8. Desactivar → Update Activa = false
9. Refresh página → Carga datos nuevamente
```

---

## 📱 Pruebas en Diferentes Navegadores

```
Soportados:
✅ Chrome 90+
✅ Firefox 88+
✅ Edge 90+
✅ Safari 14+

Requerimientos:
- JavaScript habilitado
- Cookies habilitadas
- Bootstrap 5 soportado
```

---

## 🔍 Inspeccionar Red (F12)

### Para verificar requests:

```
F12 → Network tab → Filtrar XHR

Esperado ver:
POST /Admin/CreateUPS → 302 Redirect
POST /Admin/CreateEspecialidad → 302 Redirect
POST /Admin/UpdateUPS → 302 Redirect
POST /Admin/DeleteUPS → 302 Redirect
```

---

## 📈 Monitoreo

### Logs de aplicación

```bash
# En Visual Studio
- View → Output
- Seleccionar "ASP.NET Core Server"
- Ver logs en tiempo real
```

### Ejemplos de logs esperados

```
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
	  Entity Framework Core initialized 'AppDbContext'

info: Microsoft.AspNetCore.Authorization.DefaultAuthorizationService[1]
	  Authorization was successful for user 'admin@admin.com'

info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[2]
	  Executing action method AdminController.CreateUPS
```

---

## 🎯 Checklist Pre-Producción

- [ ] Base de datos creada y poblada
- [ ] Migraciones ejecutadas
- [ ] Usuario admin con rol Administrador creado
- [ ] Aplicación compila sin errores
- [ ] Panel Admin accesible en /Admin
- [ ] Crear UPS funciona
- [ ] Crear Especialidad funciona
- [ ] Editar funciona
- [ ] Desactivar funciona
- [ ] Validaciones funcionan
- [ ] Mensajes de éxito aparecen
- [ ] BD se actualiza correctamente
- [ ] Autorización funciona (no admin = deniega)

---

## 🚀 Deploy en Producción

### Preparación

```bash
# Build en release mode
dotnet build -c Release

# Publicar
dotnet publish -c Release -o ./publish
```

### Configuración

```json
// appsettings.Production.json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=PRODUCTION_SERVER;Database=PostaCitasWeb;..."
  }
}
```

---

## 📞 Soporte

Para reportar problemas:

1. **Incluir**:
   - Pasos exactos para reproducir
   - Mensaje de error completo
   - Versión de .NET
   - Navegador utilizado

2. **Archivos adjuntos**:
   - Screenshot del error
   - Logs de aplicación
   - BD dump si aplica

---

## ✅ Resumen

```
Instalación: ✅ dotnet restore + dotnet build
Ejecución: ✅ dotnet run o F5 en VS
Verificación: ✅ Tests funcionales pasando
Uso: ✅ Panel Admin completamente operativo
```

**¡Listo para usar! 🎉**

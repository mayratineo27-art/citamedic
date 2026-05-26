# 🏗️ Arquitectura de Implementación - Panel de Administrador

## 📊 Diagrama de Flujo General

```
┌─────────────────────────────────────────────────────────────────┐
│                    USUARIO ADMINISTRADOR                         │
│              (Autenticado con rol Administrador)                │
└─────────────────────────┬───────────────────────────────────────┘
						  │
						  ▼
				   ┌──────────────┐
				   │  Views/Admin │
				   │  Index.cshtml│
				   └──────┬───────┘
						  │
		 ┌────────────────┼────────────────┐
		 │                │                │
		 ▼                ▼                ▼
	┌────────┐      ┌──────────┐    ┌──────────┐
	│ Form   │      │  Tablas  │    │ Modales  │
	│ Crear  │      │ Dinámicas│    │ Editar   │
	│UPS     │      │          │    │          │
	│Esp.    │      │UPS       │    │UPS       │
	│        │      │Esp.      │    │Esp.      │
	└────┬───┘      └─────┬────┘    └────┬─────┘
		 │                │              │
		 └────────────────┼──────────────┘
						  │
						  ▼
				  ┌────────────────┐
				  │  AdminController│
				  │  6 métodos POST │
				  └────────┬────────┘
						  │
		┌─────────────────┼─────────────────┐
		│                 │                 │
		▼                 ▼                 ▼
   ┌─────────┐       ┌──────────┐      ┌─────────┐
   │ Crear   │       │ Actualizar│      │ Desactivar
   │ Insert  │       │ Update    │      │ Soft Delete
   └────┬────┘       └─────┬─────┘      └────┬─────┘
		│                  │                  │
		└──────────────────┼──────────────────┘
						  │
						  ▼
				   ┌────────────────┐
				   │   DbContext    │
				   │  AppDbContext  │
				   └────────┬───────┘
						  │
						  ▼
				  ┌──────────────────┐
				  │  SQL Server BD   │
				  │                  │
				  │  ├─ UPS          │
				  │  └─ Especialidad │
				  └──────────────────┘
```

---

## 🔄 Flujo de Creación de UPS

```
┌──────────────┐
│ Usuario      │
│ Completa:    │
│ - Nombre: XX │
└────┬─────────┘
	 │
	 ▼
┌────────────────────────────┐
│ Validación Client-Side      │
│ - JavaScript Form Validation│
│ - Bootstrap Validation      │
└────────┬───────────────────┘
		 │ ✓ Válido
		 ▼
	┌────────┐
	│ POST   │
	│        │
	└───┬────┘
		│
		▼
┌─────────────────────────────┐
│ AdminController.CreateUPS() │
│ - ModelState.IsValid?       │
│ - Crear objeto UPS          │
└────────┬────────────────────┘
		 │
		 ▼
┌─────────────────────────────┐
│ _upsRepository.AddAsync()   │
│ Insertar en contexto        │
└────────┬────────────────────┘
		 │
		 ▼
┌──────────────────────────┐
│ SaveChangesAsync()       │
│ INSERT INTO UPS ...      │
└────────┬─────────────────┘
		 │
		 ▼
┌─────────────────────────────┐
│ Redirect Index              │
│ TempData["SuccessMessage"]  │
└────────┬────────────────────┘
		 │
		 ▼
┌──────────────────────┐
│ Index() recarga BD   │
│ Tabla actualiza      │
│ Mostrar mensaje éxito│
└──────────────────────┘
```

---

## 📁 Estructura de Archivos Impactados

```
PostaCitasWeb/
│
├── Controllers/
│   └── AdminController.cs ✏️ (MODIFICADO)
│       ├── Index()
│       ├── CreateUPS() ✨
│       ├── CreateEspecialidad() ✨
│       ├── UpdateUPS() ✨
│       ├── UpdateEspecialidad() ✨
│       ├── DeleteUPS() ✨
│       └── DeleteEspecialidad() ✨
│
├── Models/ViewModels/
│   ├── AdminDashboardViewModel.cs ✏️ (MODIFICADO)
│   ├── CreateUPSViewModel.cs ✨ (NUEVO)
│   └── CreateEspecialidadViewModel.cs ✨ (NUEVO)
│
├── Views/Admin/
│   ├── Index.cshtml ✏️ (COMPLETAMENTE REDISEÑADO)
│   │   ├── Formulario Create UPS
│   │   ├── Formulario Create Especialidad
│   │   ├── Tabla UPS
│   │   ├── Tabla Especialidades
│   │   ├── Modales de Edición
│   │   └── Scripts
│   │
│   ├── _EditUPSModal.cshtml ✨ (NUEVO)
│   │   └── Modal Bootstrap
│   │
│   └── _EditEspecialidadModal.cshtml ✨ (NUEVO)
│       └── Modal Bootstrap
│
├── Entities/
│   ├── UPS.cs (SIN CAMBIOS)
│   └── Especialidad.cs (SIN CAMBIOS)
│
└── Data/
	└── AppDbContext.cs (SIN CAMBIOS)
```

---

## 🔗 Relación de Entidades

```
┌─────────────┐      1:N      ┌──────────────────┐
│    UPS      │◄─────────────►│  Especialidad    │
├─────────────┤               ├──────────────────┤
│ UPSId (PK)  │               │ EspecialidadId   │
│ Nombre      │               │ Nombre           │
│ Activa      │               │ UPSId (FK) ──────┘
│             │               │ DuracionMinutos  │
│             │               │ Activa           │
└─────────────┘               └──────────────────┘
```

---

## 🎯 Validación de Datos - Cascada

```
Entrada Usuario
	 │
	 ▼
┌─────────────────────────────┐
│ Validación JavaScript       │
│ - Required                  │
│ - MinLength/MaxLength       │
│ - Pattern                   │
└────────┬────────────────────┘
		 │ Pasa ✓
		 ▼
┌─────────────────────────────┐
│ POST a Servidor             │
│ HTTP Request                │
└────────┬────────────────────┘
		 │
		 ▼
┌──────────────────────────────┐
│ Validación Data Annotations  │
│ [Required]                   │
│ [StringLength]               │
│ [Range]                      │
└────────┬─────────────────────┘
		 │ ModelState.IsValid?
		 ├─→ No: Redirect Index
		 │
		 └─→ Sí:
			 ▼
		┌──────────────┐
		│ Procesar en  │
		│ Controller   │
		│ Guardar BD   │
		└──────────────┘
```

---

## 💾 Ciclo de Vida de una Especialidad

```
1. CREACIÓN
   Formulario → CreateEspecialidad() → AddAsync() → SaveChangesAsync()

   ┌───────┐       ┌────────────┐      ┌────────┐      ┌────────┐
   │Entrada│──────►│Controller  │─────►│Repo    │─────►│  BD    │
   └───────┘       │Valida      │      │Insert  │      │Insert  │
				   │Crea Obj    │      │        │      │        │
				   └────────────┘      └────────┘      └────────┘

2. LECTURA
   Index() → GetAllAsync() → WHERE Activa = true → Mostrar tabla

   ┌───────┐       ┌────────────┐      ┌────────┐      ┌────────┐
   │  BD   │◄──────│Repo        │◄─────│Index() │◄─────│ Vista  │
   │SELECT │       │GetAll()    │      │Load    │      │Request │
   └───────┘       └────────────┘      └────────┘      └────────┘

3. ACTUALIZACIÓN
   Modal → UpdateEspecialidad() → Update() → SaveChangesAsync()

   ┌────────┐       ┌────────────┐      ┌────────┐      ┌────────┐
   │ Modal  │──────►│Controller  │─────►│Repo    │─────►│  BD    │
   │ Datos  │       │Valida      │      │Update  │      │UPDATE  │
   └────────┘       │Modifica Obj│      │        │      │        │
					└────────────┘      └────────┘      └────────┘

4. DESACTIVACIÓN (Soft Delete)
   Click Desactivar → DeleteEspecialidad() → Update(Activa=false)

   ┌────────┐       ┌────────────┐      ┌────────┐      ┌────────┐
   │Confirm │──────►│Controller  │─────►│Repo    │─────►│  BD    │
   │Delete  │       │Set Activa  │      │Update  │      │Activa= │
   │        │       │= false     │      │        │      │false   │
   └────────┘       └────────────┘      └────────┘      └────────┘
```

---

## 📡 Endpoints Implementados

```
GET  /Admin
	 ├─ Carga datos de BD
	 ├─ Muestra formularios
	 ├─ Muestra tablas
	 └─ Retorna Index.cshtml

POST /Admin/CreateUPS
	 ├─ Recibe CreateUPSViewModel
	 ├─ Valida datos
	 ├─ Inserta en BD
	 └─ Redirect /Admin + TempData

POST /Admin/CreateEspecialidad
	 ├─ Recibe CreateEspecialidadViewModel
	 ├─ Valida datos
	 ├─ Inserta en BD
	 └─ Redirect /Admin + TempData

POST /Admin/UpdateUPS
	 ├─ Recibe upsId, nombre
	 ├─ Busca en BD
	 ├─ Actualiza
	 └─ Redirect /Admin

POST /Admin/UpdateEspecialidad
	 ├─ Recibe especialidadId, nombre, upsId, duracion
	 ├─ Busca en BD
	 ├─ Actualiza todos los campos
	 └─ Redirect /Admin

POST /Admin/DeleteUPS
	 ├─ Recibe id
	 ├─ Busca en BD
	 ├─ Set Activa = false
	 └─ Redirect /Admin

POST /Admin/DeleteEspecialidad
	 ├─ Recibe id
	 ├─ Busca en BD
	 ├─ Set Activa = false
	 └─ Redirect /Admin
```

---

## 🎨 Componentes UI Utilizados

```
Bootstrap 5
├── Formularios
│   ├── form-control
│   ├── form-select
│   └── form-label
│
├── Botones
│   ├── btn-primary (Crear)
│   ├── btn-warning (Editar)
│   ├── btn-danger (Desactivar)
│   └── btn-secondary (Cancelar)
│
├── Tablas
│   ├── table
│   ├── table-sm
│   ├── table-hover
│   └── table-light
│
├── Badges
│   ├── bg-success (Activa)
│   └── bg-danger (Inactiva)
│
├── Modales
│   ├── modal-header
│   ├── modal-body
│   ├── modal-footer
│   └── modal-dialog
│
└── Alertas
	├── alert-success
	├── alert-dismissible
	└── fade show
```

---

## 🔐 Autorización y Seguridad

```
┌─────────────────────────────┐
│ HTTP Request a /Admin       │
└────────────┬────────────────┘
			 │
			 ▼
┌──────────────────────────────┐
│ [Authorize(Roles = "Admin")] │
├──────────────────────────────┤
│ ¿Usuario autenticado? ─────┐ │
│ ¿Rol = "Administrador"? ──┐ │
└────────────┬────────────────┘
			 │
	  ┌──────┴──────┐
	  │ No         │ Sí
	  ▼             ▼
   Redirect    ┌──────────────┐
   /Auth/Login │ Acceso OK    │
			  │ Index()      │
			  │ Mostrar Panel│
			  └──────────────┘
```

---

## ✨ Características Transversales

```
Autenticación y Autorización
├─ [Authorize(Roles = "Administrador")]
├─ Redirige a Login si no auth
└─ Deniega acceso si rol incorrecto

Validación de Datos
├─ Data Annotations (Server)
├─ JavaScript/HTML5 (Client)
└─ Bootstrap Validation UI

Manejo de Errores
├─ ModelState.IsValid
├─ Try-Catch (Opcional)
└─ TempData para mensajes

Persistencia
├─ EF Core DbContext
├─ Repository Pattern
└─ SaveChangesAsync()

UI/UX
├─ Bootstrap 5 Responsive
├─ Modales para acciones
├─ Tablas dinámicas
└─ Mensajes de feedback
```

---

## 📊 Resumen Visual de Implementación

```
Líneas de Código:
├─ Controllers: +105
├─ Views: +180
├─ ViewModels: +15
└─ Total: ~300+

Métodos Nuevos:
├─ CreateUPS: 1
├─ CreateEspecialidad: 1
├─ UpdateUPS: 1
├─ UpdateEspecialidad: 1
├─ DeleteUPS: 1
└─ DeleteEspecialidad: 1
   Total: 6 métodos

Componentes Nuevos:
├─ ViewModels: 2
├─ Partials: 2
├─ Modales: 2
└─ Total: 6 archivos

Validaciones:
├─ UPS: 3 validaciones
├─ Especialidad: 5 validaciones
└─ Total: 8+ reglas

Operaciones BD:
├─ INSERT: 2 tipos
├─ UPDATE: 2 tipos
├─ SELECT: 2 tipos
└─ SOFT DELETE: 2 tipos
```

---

**Diagrama completo de la arquitectura implementada** ✅

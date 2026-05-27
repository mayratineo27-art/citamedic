# Bitácora de Prompts y Auditoría Técnica - SDLC

**Proyecto:** Sistema Web de Gestión de Citas Médicas – Posta Los Licenciados  
**Versión:** 1.0  
**Enfoque:** Spec-Driven Development (SpecDD)  
**Rol del Agente:** Auditor de Software & Desarrollador Senior

Este documento sirve como registro oficial y bitácora técnica de auditoría de los prompts, objetivos y resultados obtenidos durante la sesión de trabajo, estructurados de acuerdo a las fases del ciclo de vida del desarrollo de software (SDLC).

---

## 📂 1. Análisis de Requerimientos y Diseño (Requirements & Design Analysis)

### Prompt / Solicitud del Usuario
> **"credenciales de paciente"** y **"credenciales para pacientes"**

* **Objetivo:**
  Identificar las cuentas y claves de prueba generadas por la base de datos semilla para simular el rol de paciente.
* **Resultados Técnicos:**
  * **Inspección del Contexto de Datos:** Se auditó [AppDbContext.cs](file:///c:/Users/MAYRATM/source/repos/PostaCitasWeb/Data/AppDbContext.cs) y [SeedData.cs](file:///c:/Users/MAYRATM/source/repos/PostaCitasWeb/Data/SeedData.cs), encontrando 11 pacientes semilla registrados.
  * **Análisis de Hashing:** Se detectó que el hash por defecto en la base de datos es `$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82`.
  * **Análisis de Flujo de Validación:** Se inspeccionó el método `EsPasswordValida` en [AuthService.cs](file:///c:/Users/MAYRATM/source/repos/PostaCitasWeb/Services/AuthService.cs), descubriendo que permite texto plano en base de datos si la clave no empieza con `$2` (lo cual aplica al usuario de prueba rápido con DNI `44444444` y clave `Paciente123!`).

---

## 🛠️ 2. Implementación y Desarrollo (Implementation & Construction)

### Prompt / Solicitud del Usuario
> **"revisa si está habilitada la vista de paciente"**

* **Objetivo:**
  Verificar la correcta implementación y cableado del módulo de paciente en la arquitectura ASP.NET MVC estricta.
* **Resultados Técnicos:**
  * **Auditoría de Controladores:** Se verificó la existencia y directivas de autorización de [PacienteController.cs](file:///c:/Users/MAYRATM/source/repos/PostaCitasWeb/Controllers/PacienteController.cs), el cual está restringido correctamente mediante `[Authorize(Roles = "Paciente")]`.
  * **Auditoría de Vistas:** Se inspeccionó la vista Razor [Index.cshtml](file:///c:/Users/MAYRATM/source/repos/PostaCitasWeb/Views/Paciente/Index.cshtml) en el módulo `Views/Paciente/`, validando la presentación del dashboard de citas y del modelo `PacienteDashboardViewModel`.
  * **Verificación de Flujos de Reserva:** Se auditaron las llamadas en [CitasController.cs](file:///c:/Users/MAYRATM/source/repos/PostaCitasWeb/Controllers/CitasController.cs) (`Reservar`, `GetEspecialidades`, `GetSlots`, `Confirmar` y `Ticket`) asegurando el cumplimiento de la **RN07** (Privacidad de UPS frente al paciente).

---

## 🧪 3. Pruebas y Aseguramiento de Calidad (Testing & Quality Assurance)

### Prompt / Solicitud del Usuario
> **"corre el sistema"**

* **Objetivo:**
  Compilar y levantar el servidor web de desarrollo de ASP.NET Core MVC local de forma segura en segundo plano.
* **Resultados Técnicos:**
  * **Compilación (Build):** Se ejecutó `dotnet build` sobre el directorio del proyecto obteniendo compilación exitosa (0 errores, 0 advertencias).
  * **Despliegue Local (Execution):** Se ejecutó `dotnet run` levantando el servidor de desarrollo en segundo plano de manera estable en los puertos:
    * **HTTPS:** [https://localhost:7110](https://localhost:7110)
    * **HTTP:** [http://localhost:5242](http://localhost:5242)
  * **Descifrado de Hash Semilla:** Se ejecutó un script en consola mediante un proyecto temporal `scratch` para verificar las firmas criptográficas de BCrypt contra el hash semilla. Se descubrió con éxito que el hash desencripta a la contraseña global de prueba: **`Paciente123!`**.

---

## 🩹 4. Corrección de Errores e Integración (Debugging & Hotfixing)

### Prompt / Solicitud Técnica (Acción Automática del Agente)
> **"Conflictos de compilación por subcarpetas y bloqueo de recursos"**

* **Objetivo:**
  Resolver el conflicto de compilación `CS0260` (falta de modificador parcial en la clase `Program` debido a la presencia de dos archivos `Program.cs` en la estructura de directorios del proyecto principal) y evitar bloqueos de archivos ejecutables (`PostaCitasWeb.exe` en ejecución).
* **Resultados Técnicos:**
  * **Aislamiento de Compilación:** Se modificó temporalmente el archivo [PostaCitasWeb.csproj](file:///c:/Users/MAYRATM/source/repos/PostaCitasWeb/PostaCitasWeb.csproj) para agregar la carpeta de pruebas a la regla de exclusión `<DefaultItemExcludes>` mediante `scratch\**`.
  * **Aislamiento de Dependencias:** Se modificó [scratch.csproj](file:///c:/Users/MAYRATM/source/repos/PostaCitasWeb/scratch/scratch.csproj) cambiando la referencia del proyecto `<ProjectReference>` por un `<PackageReference>` directo de NuGet (`BCrypt.Net-Next`), eliminando la necesidad de compilar el binario web bloqueado en memoria por el host activo.
  * **Limpieza de Entorno (Sanitization):** Una vez descifrada la contraseña, se eliminó de forma segura la carpeta temporal `scratch` y se restauró el archivo de configuración `PostaCitasWeb.csproj` al estado de producción.

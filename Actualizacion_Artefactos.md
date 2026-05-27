# Actualización de Artefactos - Gestión de Maestros y Transacciones

Este documento presenta el resumen ejecutivo y técnico de los cambios realizados en el proyecto **Sistema Web de Gestión de Citas Médicas – Posta Los Licenciados** para migrar de una simulación externa a una lógica real de **Gestión de Maestros y Transacciones** orientada a las entidades `Paciente` y `Medico`.

---

## 1. Resumen Ejecutivo de Cambios

Se han implementado y verificado las siguientes modificaciones estructurales, funcionales y de seguridad en el sistema:

1. **Gestión de Pacientes (Lógica de Búsqueda Local)**:
   - Se estableció la entidad `Paciente` (con DNI, Nombres, Apellido Paterno, Apellido Materno y otras dependencias) vinculada 1:1 con `Usuario`.
   - Se configuró la clase `SeedData.cs` inyectando **10 pacientes y usuarios de prueba** con DNIs reales para simular el padrón local de la posta en la base de datos.
   - Para resolver la advertencia de cambios pendientes no deterministas en EF Core (`PendingModelChangesWarning`), reemplazamos la encriptación de contraseña dinámica de BCrypt por un hash estático precalculado (`$2a$11$pAeGrUlfoJBDW4N/CYvpGuTp8EKk5l8xBZSd1OR7hhjA87SlsPP82`), lo que permitió agregar la migración `AddSeedPatients` y actualizar la base de datos de manera estable.
   - Se refactorizó la acción `/Admin/BuscarDni` en el controlador `AdminController.cs` para realizar la búsqueda asíncrona directamente contra la base de datos local a través de `_pacienteRepository.GetAllAsync()`.
   - Si el DNI existe en el sistema local, el frontend (`Usuarios.cshtml`) autocompleta el formulario y bloquea los campos en modo `readonly`. Si no existe, permite el ingreso manual de los datos desbloqueando los campos.

2. **Gestión y Edición de Médicos**:
   - Se implementó la capacidad de **Edición** para el Personal Médico mediante la creación de la acción `UpdateMedico` en `AdminController.cs` y el modal parcial `_EditMedicoModal.cshtml`.
   - Se agregaron validaciones en el controlador para controlar e impedir la duplicación del número de colegiatura (CMP) al registrar o modificar médicos, arrojando alertas amigables en lugar de excepciones de base de datos directas.
   - Se integraron bloques de alertas de Bootstrap (`SuccessMessage` y `ErrorMessage`) en `Medicos.cshtml`, `UPS.cshtml` y `Especialidades.cshtml` para brindar una retroalimentación visual al usuario en caso de éxito o error de validación.

3. **Corrección de Errores y Seguridad (Guardrails Técnicos)**:
   - Se corrigieron los errores de compilación `CS0266` en todos los métodos `[HttpGet]` del `AdminController` aplicando la proyección `.Select(...)` explícita y forzando la evaluación inmediata con `.ToList()`.
   - Se securizaron todos los formularios dentro de `Views/Admin/` asegurando el uso del método `POST` y la inserción directa de `@Html.AntiForgeryToken()` para resolver el error *405 Method Not Allowed* y evitar ataques CSRF.
   - Se solucionó un problema recursivo en la compilación de MSBuild en `PostaCitasWeb.csproj` agregando la directiva `<DefaultItemExcludes>` para la carpeta del proyecto de pruebas `PostaCitasWeb.Tests`.

4. **Documentación del Proyecto (SDD)**:
   - Se actualizó el **Diagrama Entidad-Relación (DER / Diagrama de Clases)** en `docs/04_modelo_dominio.md` (Mermaid) incluyendo el campo `ApellidoMaterno` a la entidad `Medico` y la relación exacta de `Paciente`.
   - Se ajustó el **Caso de Uso CU18: Gestionar Usuarios y Personal** en `docs/02_casos_uso.md` para documentar la búsqueda local y manual en lugar de un servicio externo (Mock RENIEC).

---

## 2. Detalle Técnico de Componentes Modificados

### A. Base de Datos e Inyección de Semilla (`SeedData.cs` y `AppDbContext.cs`)
- **`SeedData.cs`**:
  Contiene los métodos estáticos `GetSeedUsuarios()` y `GetSeedPacientes()`. Registra 10 identidades con DNI real, contraseñas hasheadas fijas con BCrypt y roles correspondientes para poblar la base de datos de manera determinista.
- **`AppDbContext.cs`**:
  Registra la colección estática de datos semilla mediante `HasData(...)` en el método `OnModelCreating`, garantizando que al ejecutar la migración `AddSeedPatients`, el padrón inicial de pacientes esté disponible localmente.

### B. Controlador Administrativo (`AdminController.cs`)
- Inyección de `IBaseRepository<Paciente>` para consultas directas sobre la tabla local.
- Refactorización de la acción de búsqueda de DNI.
- Incorporación de la acción `UpdateMedico`:
  ```csharp
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> UpdateMedico(int medicoId, string nombres, string apellidoPaterno, string apellidoMaterno, string cmp, bool activo)
  {
      var medico = await _medicoRepository.GetByIdAsync(medicoId);
      if (medico != null)
      {
          var medicosExistentes = await _medicoRepository.GetAllAsync();
          if (medicosExistentes.Any(m => m.CMP == cmp && m.MedicoId != medicoId))
          {
              TempData["ErrorMessage"] = $"El número de colegiatura (CMP) '{cmp}' ya está registrado por otro médico.";
              return RedirectToAction(nameof(Medicos));
          }
          medico.Nombres = nombres;
          medico.ApellidoPaterno = apellidoPaterno;
          medico.ApellidoMaterno = apellidoMaterno ?? "";
          medico.CMP = cmp;
          medico.Activo = activo;
          _medicoRepository.Update(medico);
          await _medicoRepository.SaveChangesAsync();
          TempData["SuccessMessage"] = "Médico actualizado exitosamente.";
      }
      return RedirectToAction(nameof(Medicos));
  }
  ```

### C. Vistas y Seguridad en Forms (`Views/Admin/`)
- Todos los formularios usan ahora estrictamente `method="post"` y `@Html.AntiForgeryToken()`.
- JavaScript de `Usuarios.cshtml` gestiona el estado dinámico (`readonly`) de los nombres e interactividad del formulario según el valor de `existe` de forma asíncrona.
- `Medicos.cshtml` integra el modal parcial `_EditMedicoModal.cshtml` y el script `loadMedicoData` para poblar dinámicamente los campos al hacer clic en **Editar**.

---

## 3. Verificación de Compilación y Pruebas

- **Compilación del Proyecto**:
  Se ejecutó `dotnet build` obteniendo **Compilación Correcta** sin errores (`0 Errores`, `0 Warnings` relacionados con el cambio).
- **Ejecución de Pruebas**:
  Se corrió la suite de pruebas unitarias (`dotnet test PostaCitasWeb.Tests/PostaCitasWeb.Tests.csproj`):
  ```bash
  Correctas! - Con error:     0, Superado:     2, Omitido:     0, Total:     2
  ```
  Todas las pruebas pasaron satisfactoriamente, asegurando la estabilidad lógica del núcleo del sistema.

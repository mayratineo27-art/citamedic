## 🚀 GUÍA RÁPIDA DE COMPILACIÓN Y VERIFICACIÓN

### 1️⃣ COMPILACIÓN
Desde PowerShell en la raíz del proyecto:

```powershell
dotnet build
```

**Resultado esperado:** "Build succeeded" ✅

---

### 2️⃣ VERIFICACIÓN DE MIGRACIONES
Si necesitas resetear la BD:

```powershell
# Eliminar la migración actual
dotnet ef migrations remove --force

# Crear una nueva
dotnet ef migrations add InitialCreate --output-dir Data/Migrations

# Aplicar a la BD
dotnet ef database update
```

---

### 3️⃣ INYECCIÓN DE DEPENDENCIAS (Ya configurada)
Los siguientes servicios ya están registrados en `Program.cs`:

```csharp
// Repositorios
builder.Services.AddScoped<IBaseRepository<T>>(typeof(BaseRepository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICitaService, CitaService>();
```

**Para usar en controllers:**
```csharp
public class MiController : Controller
{
	private readonly IAuthService _authService;

	public MiController(IAuthService authService)
	{
		_authService = authService;
	}
}
```

---

### 4️⃣ EJEMPLOS DE USO

#### Autenticación
```csharp
var resultado = await _authService.LoginAsync("usuario", "contraseña");
if (resultado.Success)
{
	// Usuario autenticado
	var usuarioId = resultado.UsuarioId;
}
```

#### Reservar Cita
```csharp
var resultado = await _citaService.ReserveCitaAsync(
	pacienteId: 1,
	slotId: 5,
	origen: OrigenReserva.Web
);

if (resultado.Success)
{
	Console.WriteLine($"Ticket: {resultado.TicketCodigo}");
}
```

#### Registrar Triaje
```csharp
var resultado = await _citaService.RegistrarTriajeAsync(
	citaId: 1,
	enfermeriaUsuarioId: 3,
	peso: 75.5m,
	talla: 170m,
	temperatura: 37.2m,
	presionSistolica: 120,
	presionDiastolica: 80,
	observacion: "Sin observaciones"
);
```

#### Obtener Citas de un Paciente
```csharp
var citas = await _citaService.GetCitasByPacienteAsync(pacienteId: 1);
foreach (var cita in citas)
{
	Console.WriteLine($"Cita {cita.CitaId}: {cita.EstadoCita}");
}
```

---

### 5️⃣ ESTRUCTURA DE CARPETAS KEY
| Carpeta | Propósito |
|---------|-----------|
| `Entities/` | Modelos del dominio (13 clases) |
| `Data/` | DbContext, Configuraciones, Migraciones |
| `Repositories/` | Acceso a datos (4 interfaces, 4 implementaciones) |
| `Services/` | Lógica de negocio (2 servicios implementados) |

---

### 6️⃣ PENDIENTE: RN37 (Generación de Sobrecupos)
Ubicación: `Services/CitaService.cs` línea ~200

```csharp
public async Task<bool> GenerarSobrecuposAsync(int slotId)
{
	// TODO: Implementar lógica de generación de sobrecupos según demanda
	// - Analizar demanda del slot
	// - Crear nuevos SlotDisponible con EsSobrecupo = true
	// - Retornar true si se generaron, false si no fue necesario
	await Task.CompletedTask;
	return false;
}
```

---

### 7️⃣ SIGUIENTE SPRINT: CONTROLADORES (S3)
Para implementar `AuthController` y `TriajeController`:

1. Inyectar `IAuthService` en `AuthController`
2. Crear acción `Login` que llame a `_authService.LoginAsync()`
3. Inyectar `ICitaService` en `TriajeController`
4. Implementar `RegistrarTriaje` que valide roles (solo Enfermería)

---

### 📞 ÚTILES
- **DB Query:** Ver tablas creadas en SQL Server (LocalDB)
- **BCrypt:** `BC.HashPassword()` y `BC.Verify()` listos para usar
- **DateOnly/TimeOnly:** Tipos nativos de .NET 10 ✨

¡Todo listo para continuar! 🎉

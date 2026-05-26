Stack Tecnológico y Estándares
1. Stack Técnico Definido
Lenguaje: C#

Framework Web: ASP.NET Core MVC

ORM: Entity Framework Core (SQL Server)

Base de Datos: SQL Server (LocalDB integrado en Visual Studio para desarrollo)

Frontend: Razor Views (.cshtml), HTML5, CSS3, JavaScript (Vanilla + jQuery Validation)

Testing: xUnit, Moq, FluentAssertions, Respawn

Despliegue (DevOps): Plataformas PaaS (como Render o Railway) y servicios de bases de datos relacionales gestionadas.

2. Entorno de Desarrollo (Visual Studio)
El desarrollo se ejecuta íntegramente a través de Visual Studio, utilizando la Consola del Administrador de Paquetes (Package Manager Console - PMC) para la gestión del ORM y dependencias.

Paquetes Base Instalados (Core & BD):

PowerShell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
Paquetes Adicionales a instalar (Fase de Pruebas en PostaCitasWeb.Tests):

PowerShell
Install-Package Moq
Install-Package FluentAssertions
Install-Package Respawn
3. Estándares de Codificación y Arquitectura
Para mantener la integridad del sistema bajo un modelo MVC limpio y un desarrollo guiado por especificaciones (SpecDD), se deben respetar las siguientes directrices:

Clean Code y Separación de Responsabilidades: * Los Controladores son delgados; su única función es recibir la petición HTTP, delegar la ejecución a un servicio y retornar una Vista o Redirección. Ninguna regla de negocio (como la RN37 de fechas) debe programarse aquí.

Los Servicios contienen el 100% de la lógica de negocio y validaciones de dominio.

Validación de Datos (KISS): * Uso intensivo de Data Annotations ([Required], [StringLength], [Range]) directamente en los ViewModels y Models.

Habilitar validación en el cliente mediante Unobtrusive JavaScript (_ValidationScriptsPartial) y validación obligatoria en el backend con if (!ModelState.IsValid).

Programación Asíncrona: * Implementación estricta del patrón asíncrono (async / await / Task<T>) en todas las operaciones de I/O, acceso a datos con EF Core y llamadas a servicios.

Todos los métodos asíncronos deben llevar el sufijo Async (ej. ReservarAsync).

Inyección de Dependencias (DI): * Registro centralizado en Program.cs.

Uso exclusivo del ciclo de vida Scoped para repositorios y servicios, garantizando que cada petición HTTP maneje su propia instancia y transacción de base de datos.

SpecDD como Fuente de la Verdad: * El código de producción se escribe para satisfacer los criterios de aceptación documentados. Si una prueba unitaria especificada en 07_pruebas.md falla, el código debe ser refactorizado hasta que pase. Las pruebas mandan sobre la implementación.
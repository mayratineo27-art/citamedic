Product Backlog y Sprints (SpecDD)
Sprint 1: Core de Dominio y Persistencia (Épica 2 y Base)
Objetivo: Establecer las entidades, el DbContext y probar la creación de la estructura de la posta médica en la base de datos.

[x] 1.1 Inicializar la solución en Visual Studio, definir la estructura de carpetas (Models, Data, Repositories, Services) e instalar los paquetes de EF Core.

[x] 1.2 Codificar Entidades de Dominio (Usuario, Paciente, UPS, Especialidad, Medico).

[x] 1.3 Codificar Entidades Operativas (ProgramacionOperativa, SlotDisponible, Cita, Ticket, Triaje, HistorialEstadoCita, AvisoAtencionInmediata).

[x] 1.4 Configurar AppDbContext y aplicar configuraciones Fluent API (DeleteBehavior.Restrict y constraints de las reglas de negocio).

[x] 1.5 Configurar la cadena de conexión en appsettings.json apuntando a SQL Server EXPRESS.

[x] 1.6 Ejecutar la migración inicial en la Consola del Administrador de Paquetes: Add-Migration InitialCreate.

[x] 1.7 Modificar la migración generada para agregar manualmente el índice único condicional para la RN31 (UX_Citas_PacienteSlotActiva).

[x] 1.8 Implementar datos semilla (SeedData) en el DbContext (Usuario Administrador, UPS y Especialidades).

[x] 1.9 Aplicar la migración a la base de datos: Update-Database.

Sprint 2: Lógica de Negocio y SpecDD (Épicas 2 y 3)
Objetivo: Desarrollar los servicios e implementar las pruebas unitarias para asegurar las reglas de negocio antes de exponer la web.

[x] 2.1 Configurar el proyecto de pruebas (PostaCitasWeb.Tests) e instalar Mocking frameworks (Moq, FluentAssertions).

[x] 2.2 Escribir pruebas unitarias ([U-01] a [U-04]) e implementar AuthService y PacienteService.

[ ] 2.3 Escribir pruebas unitarias ([U-05] a [U-08]) e implementar DisponibilidadService (generación automática de slots).

[x] 2.4 Escribir pruebas unitarias ([U-09] a [U-14] incluyendo [U-11B]) e implementar CitaService. Atención especial aquí: Implementar la lógica condicional de la RN37 (restricción de fechas mismo día o sábado para lunes).

[x] 2.5 Implementar repositorios (CitaRepository, SlotRepository, etc.) referenciados por los servicios.

[x ] 2.6 Validar que el 100% de las pruebas unitarias pasen en el Explorador de Pruebas.

Sprint 3: Controladores, Vistas y Seguridad (Épicas 1 y 3)
Objetivo: Exponer la lógica probada a través de interfaces web para los pacientes y admisión.

[x] 3.1 Configurar Cookie Authentication y Autorización por Roles en Program.cs.

[x] 3.2 Implementar AuthController y vistas Razor asociadas (Login, Solicitud de acceso, Recuperación).

[x] 3.3 Implementar DisponibilidadController y vistas para Admisión (habilitar programación).

[x] 3.4 Implementar CitasController y vista de consulta de especialidades/slots disponibles (filtrando sobrecupos).

[x] 3.5 Implementar flujo de Reserva Web (POST) y generación de la vista del Ticket.

[x] 3.6 Implementar funcionalidad de registro presencial exclusivo para rol Admisión en CitasController.

[x] 3.7 Integrar validaciones de frontend (Data Annotations y jQuery Validation) en los formularios Razor.

Sprint 4: Triaje, Trazabilidad y MVP Complementario (Épicas 4 y 5)
Objetivo: Cerrar el flujo operativo en la clínica y habilitar seguimiento.

[ ] 4.1 Escribir pruebas ([U-15], [U-16]) e implementar TriajeService.

[ ] 4.2 Implementar TriajeController y formulario de evaluación inicial exclusivo para Enfermería.

[ ] 4.3 Desarrollar vista de seguimiento de estados (trazabilidad) para el paciente (HistorialEstadoCita).

[ ] 4.4 Implementar AvisosController y AvisoService para el envío de notificaciones de atención inmediata.

[ ] 4.5 Construir el Panel de Avisos restringido para el rol de Enfermería.

Sprint 5: Despliegue en la Nube (DevOps)
Objetivo: Preparar la aplicación para producción y alojarla en plataformas PaaS.

[ ] 5.1 Externalizar cadenas de conexión usando variables de entorno o User Secrets.

[ ] 5.2 Provisionar la base de datos relacional remota (instancia SQL Server en nube o ajuste a un proveedor compatible).

[ ] 5.3 Desplegar la aplicación web (PostaCitasWeb) en plataformas de hosting (Render, Railway, o Azure App Service).

[ ] 5.4 Ejecutar las migraciones en la base de datos de producción.

[ ] 5.5 Realizar validación final (Smoke Testing) en el entorno productivo.
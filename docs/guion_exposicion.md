# Guión de Exposición: Aspectos Técnicos del Sistema de Gestión de Citas Médicas

---

## 1. Introducción (Apertura)
"Buenos días con todos. Hoy les presento la arquitectura y los detalles técnicos detrás de nuestro Sistema Web de Gestión de Citas Médicas para la Posta de Salud. Más que un simple CRUD, este sistema ha sido desarrollado bajo el enfoque **Spec-Driven Development (SpecDD)**, lo que garantiza que cada línea de código responda de manera estricta a las reglas de negocio y requisitos validados en nuestra documentación."

---

## 2. Arquitectura y Stack Tecnológico
"En el corazón del sistema, utilizamos el framework **.NET 10 con C# 12**, implementando el patrón arquitectónico **MVC (Modelo-Vista-Controlador) estricto**. 

*   **Capa de Datos:** Utilizamos **Entity Framework Core** conectado a **SQL Server**. Centralizamos la persistencia mediante repositorios, aislando las consultas a la base de datos.
*   **Capa de Servicios:** Toda la inteligencia y reglas de negocio del sistema viven aquí. Los Controladores son extremadamente delgados y limpios; su única responsabilidad es recibir peticiones HTTP, orquestar llamadas asíncronas a los *Services* y retornar la vista correspondiente.
*   **Frontend:** Construimos la interfaz utilizando **Razor Views (.cshtml)** impulsadas por **Vanilla CSS** bajo nuestro propio sistema de diseño enfocado en la usabilidad ('Clinic Calm'). Hemos evitado recargas de página innecesarias empleando modales y controles dinámicos para ofrecer una experiencia de usuario ágil."

---

## 3. Seguridad y Concurrencia
"Dado que manejamos datos de salud, la seguridad y la fiabilidad de las transacciones son pilares críticos:
*   La autenticación está construida nativamente con **Cookie Authentication** de ASP.NET Core, utilizando `Claims` (reclamos) para separar herméticamente las sesiones por roles (Pacientes, Enfermería, Admisión, Médicos y Administradores).
*   Protegemos todas las transacciones contra ataques CSRF (Falsificación de Petición en Sitios Cruzados) utilizando **Anti-Forgery Tokens**.
*   A nivel transaccional, el stock de cupos médicos es **compartido en tiempo real** entre el portal web y ventanilla presencial, evitando cualquier riesgo de duplicidad de reservas."

---

## 4. Innovaciones Técnicas del Flujo Operativo
"Técnicamente, resolvimos problemas reales de la posta con patrones eficientes:
1.  **Delegación de Sesión (Mi Familia):** A nivel backend, validamos el campo `ResponsableId`. Esto nos permite cambiar el contexto de seguridad en caliente, logrando que un tutor opere a nombre de un menor sin necesidad de re-autenticación constante.
2.  **Tablero Kanban de Trazabilidad:** Desarrollamos un tablero visual para Enfermería que registra la transición de estados en tiempo real (Falta Triaje, En Triaje, Listo Atención). Cada movimiento genera auditoría asíncrona en la base de datos."

---

## 5. Aseguramiento de Calidad (Test Automation)
"Para asegurar un código mantenible a largo plazo, integramos una suite automatizada robusta en nuestra capa de Testing:
*   Usamos **xUnit** como marco de pruebas principal.
*   Implementamos **Moq** para simular nuestros repositorios. Esto aísla la lógica del servicio, permitiendo pruebas unitarias veloces sin depender de una base de datos real.
*   Inyectamos **FluentAssertions** para que las validaciones de los resultados sean semánticas, legibles y descriptivas."

---

## 6. Cierre
"En resumen, hemos diseñado un sistema de alta cohesión y bajo acoplamiento. Es escalable, rápido gracias a peticiones asíncronas, seguro por diseño y probado exhaustivamente. El resultado es una plataforma que moderniza los flujos de la posta sin perder robustez técnica. Muchas gracias."

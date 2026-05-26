# MATRIZ DE TRAZABILIDAD DE VISTAS POR ACTOR

Proyecto: Sistema Web de Gestión de Citas Médicas – Posta de Salud  
Versión: 1.0  
Propósito: Trazar qué vistas Razor están permitidas o no permitidas para cada actor según los artefactos del SDD.

---

## Matriz Actor → Evidencia documental → Vista permitida / no permitida

| Actor | Evidencia documental | Vista permitida | Vista no permitida |
|---|---|---|---|
| Paciente | `docs/00_constitucion.md` (Actor: Paciente), `docs/01_requisitos.md` (RF05, RF06, RF07, RF08, RF09, RF11, RF12, RF19, RF20), `docs/02_casos_uso.md` (CU01, CU02, CU04, CU05, CU06, CU08, CU09, CU15, CU16) | `Views/Paciente/Index.cshtml`, `Views/Citas/Index.cshtml`, `Views/Citas/Create.cshtml`, `Views/Citas/Details.cshtml`, `Views/Citas/Edit.cshtml` | Vista propia de Médico, Enfermería, Admisión, Administrador |
| Responsable del menor | `docs/00_constitucion.md` (Actor: Responsable del menor), `docs/01_requisitos.md` (RF04), `docs/02_casos_uso.md` (CU03) | Misma experiencia que Paciente para reservar y consultar citas de dependientes, reutilizando vistas de citas | Dashboard independiente no documentado |
| Admisión | `docs/00_constitucion.md` (Actor: Admisión), `docs/01_requisitos.md` (RF10, RF15, RF15A, RF16, RF24, RF22), `docs/02_casos_uso.md` (CU07, CU10, CU11, CU12, CU18, CU19, CU20) | `Views/Admision/Index.cshtml`, `Views/Citas/Create.cshtml`, `Views/Citas/Index.cshtml` | Vista propia de Médico, Enfermería, Paciente, Administrador |
| Enfermería | `docs/00_constitucion.md` (Actor: Enfermería), `docs/01_requisitos.md` (RF17, RF18, RF21), `docs/02_casos_uso.md` (CU13, CU14, CU17) | `Views/Enfermeria/Index.cshtml`, `Views/Triaje/Create.cshtml` | Vista propia de Médico, Admisión, Paciente, Administrador |
| Administrador | `docs/00_constitucion.md` (Actor: Administrador), `docs/01_requisitos.md` (RF13, RF14, RF15A, RF22, RF23), `docs/02_casos_uso.md` (CU10, CU18, CU19) | `Views/Admin/Index.cshtml` | Vista propia de Médico, Enfermería, Admisión, Paciente |
| Médico | `docs/04_modelo_dominio.md` (Entidad `Medico` y relaciones con `ProgramacionOperativa`), no aparece como actor en `docs/00_constitucion.md`, `docs/01_requisitos.md` ni `docs/02_casos_uso.md` | No aplica vista propia; solo como dato relacionado en vistas de otros actores cuando la cita/programación lo requiera | `Views/Medico/Index.cshtml` y cualquier dashboard propio de Médico |

---

## Conclusión

Según los artefactos, **Médico no es un actor funcional del sistema** y por tanto **no debe existir una vista principal propia para Médico** (incluida `Views/Medico/Index.cshtml`). Sí debe mantenerse como entidad de dominio para ser mostrada como información asociada en los flujos de Paciente, Admisión y Administración cuando corresponda.

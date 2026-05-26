# 📅 PROGRAMACIÓN OPERATIVA - Guía Completa

## 🎯 ¿Qué es la Programación Operativa?

La **Programación Operativa** es el plan maestro que define **cuándo, quién y cómo** se atienden los pacientes en la posta.

Es como el **horario de trabajo** de los médicos, pero con detalles operativos específicos.

---

## 📊 Estructura de Programación Operativa

```
┌─────────────────────────────────────────────────────┐
│         PROGRAMACIÓN OPERATIVA                      │
├─────────────────────────────────────────────────────┤
│ • Especialidad: MEDICINA GENERAL                    │
│ • Médico: Dr. Juan Pérez (CMP: 12345)              │
│ • Fecha: 2025-01-20                                 │
│ • Turno: MAÑANA (9:00 - 13:00)                     │
│ • Duración por cita: 20 minutos                     │
│ • Cupos disponibles: 12                             │
│ • Estado: NO HABILITADA (Habilitada = false)       │
└─────────────────────────────────────────────────────┘
```

### **Componentes Clave:**

| Campo | Descripción | Ejemplo |
|-------|-------------|---------|
| **Especialidad** | Tipo de servicio médico | Medicina General, Odontología, Pediatría |
| **Médico** | Profesional que atiende | Dr. Juan Pérez |
| **Fecha** | Día de la consulta | 2025-01-20 |
| **Turno** | Jornada laboral | Mañana (9-13) o Tarde (14-18) |
| **Duración** | Tiempo por paciente (min) | 20 minutos |
| **Cupos** | Cantidad de pacientes | 12 = 12 pacientes máximo |
| **Habilitada** | ¿Se puede reservar? | ❌ NO / ✅ SÍ |

---

## 🔄 Flujo de Vida de una Programación

```
PASO 1: CREAR PROGRAMACIÓN
↓
Admin configura:
- Especialidad
- Médico
- Fecha/Turno
- Cupos y duración
- Estado: Habilitada = FALSE (por defecto)
↓

PASO 2: REVISAR Y VALIDAR
↓
Admin revisa que todo sea correcto
↓

PASO 3: HABILITAR DISPONIBILIDAD (CU11)
↓
Admin hace clic en "Habilitar"
- Cambio: Habilitada = FALSE → TRUE
- La programación está LISTA para que pacientes reserven
↓

PASO 4: PACIENTES PUEDEN RESERVAR
↓
- El sistema muestra la programación al paciente
- El paciente puede reservar citas
- Los cupos disminuyen (12 → 11 → 10...)
↓

PASO 5: AJUSTAR SI ES NECESARIO (CU12)
↓
Si hay rotación o el médico falta:
- Admin puede DESABILITAR o cambiar cupos
- Restricción: Solo programaciones FUTURAS
↓

PASO 6: FINALIZACIÓN
↓
Cuando pasa la fecha → automáticamente irrelevante
```

---

## 🖱️ Botones en la UI y sus Significados

### **Estado: Habilitada = FALSE (Rojo - Deshabilitada)**
```
┌──────────────────────────────┐
│ Medicina General | Mañana    │
│ Dr. Pérez       | 12 cupos   │
│ 2025-01-20      | ❌ DESHABILITADA
├──────────────────────────────┤
│ [Editar] [Habilitar] [Info]  │ ← Botones disponibles
└──────────────────────────────┘
```

**¿Qué significa?**
- La programación está configurada
- PERO los pacientes NO la ven
- NO se pueden hacer reservas

**¿Qué hacer?**
- Haz clic en **"Habilitar"** → Habilitada = TRUE

---

### **Estado: Habilitada = TRUE (Verde - Habilitada)**
```
┌──────────────────────────────┐
│ Medicina General | Mañana    │
│ Dr. Pérez       | 12 cupos   │
│ 2025-01-20      | ✅ HABILITADA
├──────────────────────────────┤
│ [Editar] [Desabilitar] [Info]│ ← Botones disponibles
└──────────────────────────────┘
```

**¿Qué significa?**
- Los pacientes VEN esta programación
- Los pacientes PUEDEN reservar
- Los cupos están activos

**¿Qué hacer?**
- Nada, ya está funcionando
- O haz clic en **"Desabilitar"** si necesitas suspender

---

## 📋 Comparación: Crear vs Habilitar

| Acción | Quién | Cuándo | Resultado |
|--------|------|--------|-----------|
| **Crear Programación** | ADMIN | Planifica para el mes | Habilitada = FALSE |
| **Habilitar Disponibilidad** | ADM/ADMIN | Día previo o mañana | Habilitada = TRUE |
| **Desabilitar** | ADMIN | Médico ausente | Habilitada = FALSE |
| **Ajustar Cupos** | ADM | Rotación necesaria | CuposTotal se actualiza |

---

## 💡 Ejemplos Prácticos

### **Ejemplo 1: Planificación Semanal**

**Lunes (Admin planifica para toda la semana):**
```
Crea 5 programaciones:
- Martes 9:00 - Dr. Pérez (Medicina) - 12 cupos - Habilitada = FALSE
- Martes 14:00 - Dra. López (Pediatría) - 8 cupos - Habilitada = FALSE
- Miércoles 9:00 - Dr. Pérez (Medicina) - 12 cupos - Habilitada = FALSE
- Jueves 14:00 - Dr. García (Odontología) - 6 cupos - Habilitada = FALSE
- Viernes 9:00 - Dra. López (Pediatría) - 8 cupos - Habilitada = FALSE
```

**Martes 8:00 (Admin habilita antes de abrir):**
```
Habilita la programación:
- Martes 9:00 - Dr. Pérez → Habilitada = TRUE ✅
- Martes 14:00 - Dra. López → Habilitada = TRUE ✅
```

**Martes 9:00 (Pacientes ven y reservan):**
```
Paciente 1 reserva Martes 9:00 → Cupos: 12 → 11
Paciente 2 reserva Martes 9:20 → Cupos: 11 → 10
Paciente 3 reserva Martes 9:40 → Cupos: 10 → 9
...
```

---

### **Ejemplo 2: Médico Enfermo**

**Miércoles 7:00 AM:**
```
Doctor X avisa que está enfermo
Admin entra a "Programación Operativa"
Busca: Miércoles 9:00 - Dr. X
Haz clic en [Desabilitar]
→ Habilitada = FALSE
→ Los pacientes ya NO ven esa programación
```

---

### **Ejemplo 3: Ajustar Cupos por Demanda**

**Jueves 10:00 AM:**
```
Admin ve que faltan cupos para Viernes
Busca: Viernes 14:00 - Dra. López (Pediatría)
Haz clic en [Editar]
Cambia cupos: 8 → 12 (aumenta)
Guarda
→ Ahora hay 12 cupos disponibles para Viernes
```

---

## 🚀 Flujo de Usuario (Paso a Paso)

### **CREAR una Programación Operativa:**
```
1. Ir a Panel Admin → "Programación Operativa"
2. Llenar formulario:
   - Especialidad: [Seleccionar]
   - Médico: [Seleccionar]
   - Fecha: [2025-01-20]
   - Turno: [Mañana ▼]
   - Cupos: [12]
   - Duración: [20 min]
3. Clic en [Crear Programación]
4. Estado: ❌ DESHABILITADA (automático)
```

### **HABILITAR una Programación (CU11):**
```
1. Ir a Panel Admin → "Programación Operativa"
2. Buscar la programación que quieres habilitar
3. Estado actual: ❌ DESHABILITADA
4. Clic en botón [Habilitar]
5. Confirmación: "¿Deseas publicar esta disponibilidad?"
6. Clic en [Sí]
7. Estado cambia: ✅ HABILITADA
   → Pacientes ya la ven
   → Ya se pueden hacer reservas
```

### **DESABILITAR una Programación (Emergencia):**
```
1. Ir a Panel Admin → "Programación Operativa"
2. Buscar la programación deshabilitada
3. Estado actual: ✅ HABILITADA
4. Clic en botón [Desabilitar]
5. Confirmación: "¿Deseas suspender esta disponibilidad?"
6. Clic en [Sí]
7. Estado cambia: ❌ DESHABILITADA
   → Pacientes ya NO la ven
   → NO se pueden hacer más reservas
```

### **AJUSTAR una Programación (CU12):**
```
1. Ir a Panel Admin → "Programación Operativa"
2. Buscar la programación a ajustar
3. Clic en [Editar]
4. Cambiar:
   - Cupos: 12 → 15 (aumentar por demanda)
   - O mantener cupos pero cambiar duración
5. Clic en [Guardar]
6. Cambios aplicados
```

---

## 🔐 Restricciones Importantes

### ❌ **NO se puede hacer:**

| Restricción | Razón | Solución |
|-----------|-------|----------|
| Modificar una programación **en ejecución** | Afecta atención actual | Crear nueva programación para mañana |
| Habilitar programaciones **pasadas** | Ya pasó la fecha | Solo programaciones futuras |
| Cambiar cupos a 0 directamente | Confunde al sistema | Usar [Desabilitar] |
| Reservar sin habilitar primero | Pacientes no la ven | [Habilitar] antes |

---

## 📌 Resumen Rápido

| Necesito... | Hago esto | Resultado |
|-----------|----------|-----------|
| **Planificar médicos** | Crear Programación | Habilitada = FALSE |
| **Abrir citas para reserva** | [Habilitar] | Habilitada = TRUE |
| **Cerrar citas de emergencia** | [Desabilitar] | Habilitada = FALSE |
| **Aumentar/bajar cupos** | [Editar] → cambiar cupos | Cupos actualizados |
| **Ver mis programaciones** | Panel → Programación Operativa | Lista completa |

---

## 🎓 Conclusión

**Programación Operativa = Calendario de atención**

```
ADMIN planifica → Crea programaciones (No habilitadas)
	 ↓
ADMIN valida → Revisa que todo esté correcto
	 ↓
ADMIN habilita → Abre para reservas (Habilitada = TRUE)
	 ↓
PACIENTES ven → Reservan citas
	 ↓
SISTEMA descuenta → Cupos disminuyen
	 ↓
Si hay problema → ADMIN desabilita (Habilitada = FALSE)
```

**Recuerda:** Una programación NO habilitada es **invisible** para los pacientes.

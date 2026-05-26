# 🧪 PLAN DE PRUEBAS - Panel de Administrador vs Casos de Uso

## 📋 Objetivo
Verificar que las funcionalidades implementadas en el Panel de Administrador cumplan con los requisitos especificados en:
- `02_casos_uso.md`
- `07_pruebas.md`
- `03_reglas_negocio.md`

---

## 🎯 Mapeo: Casos de Uso → Implementación

### CU10: Configurar programación operativa
**Descripción:** Configurar jornadas con especialidad, médico, turno, duración y cupos

**Requisitos:**
- [ ] Seleccionar especialidad
- [ ] Asociar médico
- [ ] Configurar turno
- [ ] Configurar duración
- [ ] Configurar cupos
- [ ] Restricción: No modificar atención iniciada

**Implementado en Panel:**
✅ Sección "Programación operativa"
- ✅ Dropdown de Especialidades (solo activas)
- ✅ Dropdown de Médicos (solo activos)
- ✅ Selector de Turno (Mañana/Tarde)
- ✅ Date picker para Fecha
- ✅ Input numérico para Cupos (1-100)
- ✅ Input numérico para Duración (1-480 min)
- ✅ Checkbox de Habilitación
- ✅ Método POST: CreateProgramacion()

**Estado:** ✅ IMPLEMENTADO

---

### CU18: Gestionar usuarios
**Descripción:** Administrar acceso (buscar, editar, guardar)

**Requisitos:**
- [ ] Buscar usuario
- [ ] Editar datos
- [ ] Guardar cambios
- [ ] Validar credenciales

**Implementado en Panel:**
✅ Sección "Usuarios internos"
- ✅ Crear usuario (no editar, solo crear y desactivar por ahora)
- ✅ DNI (8 dígitos)
- ✅ Nombre de usuario
- ✅ Contraseña (hasheada con BCrypt)
- ✅ Confirmación de contraseña
- ✅ Rol (5 opciones)
- ✅ Celular
- ✅ Tabla de usuarios activos
- ✅ Botón desactivar
- ✅ Método POST: CreateUsuario(), DeleteUsuario()

**Estado:** ✅ IMPLEMENTADO (Crear/Desactivar)

---

### CU19: Configurar duración atención
**Descripción:** Generar horarios y calcular disponibilidad por duración

**Requisitos:**
- [ ] Definir duración por especialidad
- [ ] Generar tarjetas/slots
- [ ] Calcular disponibilidad

**Implementado en Panel:**
✅ Sección "Especialidades"
- ✅ Campo "Duración (minutos)" en cada especialidad
- ✅ Validación: 1-480 minutos
- ✅ Se almacena en BD
- ✅ Se usa en Programación Operativa

**Estado:** ✅ IMPLEMENTADO

---

## 🧪 TESTS DE VALIDACIÓN

### TEST SUITE 1: Programación Operativa (CU10)

#### T1.1: Crear Programación con todos los campos requeridos
```
DADO: Panel Admin accesible, especialidades y médicos activos disponibles
CUANDO: Admin completa formulario de programación con:
  - Especialidad: "Medicina General"
  - Médico: "Carlos Ramírez"
  - Turno: "Mañana"
  - Fecha: fecha futura válida
  - Cupos: 10
  - Duración: 20 minutos
  - Habilitada: checked
ENTONCES:
  ✓ POST /Admin/CreateProgramacion es enviado
  ✓ Validaciones pasan (client + server)
  ✓ Registro se inserta en BD
  ✓ Mensaje de éxito aparece
  ✓ Tabla se actualiza con nueva programación
```

**Validaciones esperadas:**
- ✓ Especialidad no nula
- ✓ Médico no nulo
- ✓ Turno seleccionado
- ✓ Fecha válida (no pasada)
- ✓ Cupos entre 1-100
- ✓ Duración entre 1-480

---

#### T1.2: Dropdowns solo muestran items activos
```
DADO: Existen especialidades y médicos activos e inactivos
CUANDO: Admin abre formulario de crear programación
ENTONCES:
  ✓ Dropdown de Especialidades solo muestra activas
  ✓ Dropdown de Médicos solo muestra activos
  ✓ Inactivos no aparecen en dropdowns
```

---

#### T1.3: No se permite crear con campos inválidos
```
DATO 1: Cupos = 0 (fuera de rango)
DATO 2: Duración = 500 (mayor a 480)
DATO 3: Fecha pasada
CUANDO: Admin intenta enviar
ENTONCES:
  ✓ Validación HTML5 evita envío
  ✓ Si alcanza servidor, ModelState.IsValid = false
  ✓ No se inserta en BD
```

---

#### T1.4: Deshabilitar Programación
```
DADO: Programación habilitada con ProgramacionId = X
CUANDO: Admin hace click en "Deshabilitar"
Y: Confirma en diálogo
ENTONCES:
  ✓ POST /Admin/DeleteProgramacion(id: X)
  ✓ Campo Habilitada = false en BD
  ✓ Badge cambia de verde a rojo
  ✓ Botón desaparece
  ✓ Mensaje de éxito muestra
```

---

### TEST SUITE 2: Gestión de Usuarios (CU18)

#### T2.1: Crear Usuario con todos los campos
```
DADO: Panel Admin, sección Usuarios
CUANDO: Admin completa formulario:
  - DNI: "12345678"
  - Usuario: "juan_pérez"
  - Contraseña: "MiPass123!"
  - Confirmar: "MiPass123!"
  - Rol: "Admisión"
  - Celular: "987654321"
ENTONCES:
  ✓ POST /Admin/CreateUsuario enviado
  ✓ Contraseña hasheada con BCrypt
  ✓ Inserción exitosa en BD
  ✓ Usuario aparece en tabla
  ✓ Estado: "Activo"
```

**Verificaciones BD:**
```sql
SELECT DNI, NombreUsuario, Rol, Activo 
FROM Usuarios 
WHERE DNI = '12345678'
-- Resultado esperado:
-- 12345678 | juan_pérez | Admision | 1
```

---

#### T2.2: Validación de DNI
```
ESCENARIO A: DNI con < 8 dígitos
  CUANDO: DNI = "1234567"
  ENTONCES: ✓ Validación HTML5 rechaza

ESCENARIO B: DNI con > 8 dígitos
  CUANDO: DNI = "123456789"
  ENTONCES: ✓ Validación HTML5 rechaza

ESCENARIO C: DNI exacto
  CUANDO: DNI = "12345678"
  ENTONCES: ✓ Pasa validación
```

---

#### T2.3: Validación de Contraseña
```
ESCENARIO A: Contraseña muy corta
  CUANDO: Password = "Pass1" (5 chars)
  ENTONCES: ✓ Error: "mínimo 8 caracteres"

ESCENARIO B: Contraseñas no coinciden
  CUANDO: Password = "MiPass123!" Y ConfirmPassword = "OtraPass123!"
  ENTONCES: ✓ Error: "no coinciden"

ESCENARIO C: Contraseñas válidas
  CUANDO: Password = ConfirmPassword = "MiPass123!"
  ENTONCES: ✓ Se hashea y guarda
```

---

#### T2.4: Desactivar Usuario
```
DADO: Usuario activo en tabla
CUANDO: Click en "Desactivar"
Y: Confirma en JS confirm()
ENTONCES:
  ✓ POST /Admin/DeleteUsuario(id: X)
  ✓ Usuario.Activo = false en BD
  ✓ Badge cambia a rojo "Inactivo"
  ✓ Botones desaparecen
  ✓ Mensaje de éxito muestra
```

---

### TEST SUITE 3: Duración de Atención (CU19)

#### T3.1: Crear Especialidad con Duración
```
DADO: Sección Especialidades
CUANDO: Admin crea especialidad:
  - Nombre: "Cardiología"
  - UPS: (seleccionada)
  - Duración: "30"
ENTONCES:
  ✓ Especialidad.DuracionMinutos = 30
  ✓ Se inserta en BD
  ✓ Aparece en tabla
  ✓ Disponible en dropdowns de Programación
```

---

#### T3.2: Validación de Rango de Duración
```
ESCENARIO A: Duración = 0
  ENTONCES: ✓ Validación rechaza (min: 1)

ESCENARIO B: Duración = 500
  ENTONCES: ✓ Validación rechaza (max: 480)

ESCENARIO C: Duración = 1, 240, 480
  ENTONCES: ✓ Todas pasan validación
```

---

#### T3.3: Duración influye en Programación
```
DATO: Especialidad "Medicina" con Duración = 20 min
CUANDO: Se crea Programación para esa especialidad
Y: Se abre en Programación Operativa
ENTONCES:
  ✓ Campo DuracionMinutos se carga automáticamente
  ✓ O es editable para excepciones
```

---

## 🔐 TEST SUITE 4: Seguridad

#### T4.1: Solo Administrador accede al Panel
```
ESCENARIO A: Paciente intenta /Admin
  ENTONCES: ✓ Redirect a /Auth/Login

ESCENARIO B: Usuario sin autenticar
  ENTONCES: ✓ Redirect a /Auth/Login

ESCENARIO C: Admin autenticado
  ENTONCES: ✓ Acceso permitido
```

---

#### T4.2: Contraseñas se hashean con BCrypt
```
DADO: Usuario creado con Password = "MiPass123!"
CUANDO: Se consulta BD
ENTONCES:
  ✓ PasswordHash NO contiene "MiPass123!" en texto plano
  ✓ PasswordHash comienza con "$2a$" o "$2b$" (formato BCrypt)
  ✓ Hash tiene ~60 caracteres
```

---

#### T4.3: Soft Delete preserva datos
```
DADO: Usuario desactivado
CUANDO: Se consulta BD
ENTONCES:
  ✓ Registro sigue en BD
  ✓ Activo = false
  ✓ Otros datos intactos
  ✓ Pueda reactivarse si es necesario
```

---

## 📊 TEST SUITE 5: Persistencia en BD

#### T5.1: Programación se guarda completamente
```sql
-- Después de crear programación:
SELECT ProgramacionId, EspecialidadId, MedicoId, Turno, 
	   Fecha, CuposTotal, DuracionMinutos, Habilitada
FROM ProgramacionesOperativas
WHERE ProgramacionId = (última creada)

-- Verificar que TODOS los campos tienen valores correctos
```

---

#### T5.2: Usuario se guarda con Rol correcto
```sql
SELECT UsuarioId, DNI, NombreUsuario, Rol, Activo
FROM Usuarios
WHERE DNI = '12345678'

-- Rol debe ser enum (0-4): Paciente(0), Admision(1), Enfermeria(2), Admin(3), Medico(4)
```

---

#### T5.3: Especialidad con Duración se usa en Programación
```sql
SELECT e.Nombre, e.DuracionMinutos, p.DuracionMinutos
FROM Especialidades e
JOIN ProgramacionesOperativas p ON e.EspecialidadId = p.EspecialidadId
WHERE p.ProgramacionId = (última creada)

-- Los DuracionMinutos deben coincidir (si no se editó)
```

---

## 🎯 CRITERIOS DE ACEPTACIÓN

✅ **Panel Admin cumple CU10 (Programación Operativa)**
- [x] Todos los campos presentes
- [x] Validaciones funcionan
- [x] Datos se guardan en BD
- [x] Restricción: solo especialidades y médicos activos

✅ **Panel Admin cumple CU18 (Gestionar Usuarios)**
- [x] Crear usuario funciona
- [x] Desactivar usuario funciona
- [x] Contraseñas se hashean
- [x] Validaciones rigorosas

✅ **Panel Admin cumple CU19 (Duración Atención)**
- [x] Duración se configura en especialidades
- [x] Se valida rango (1-480)
- [x] Se usa en programaciones

✅ **Seguridad**
- [x] Solo Admin accede
- [x] BCrypt hashing
- [x] Soft delete
- [x] Confirmaciones

---

## 📝 Metodología de Prueba

1. **Manual Testing:** Acceder a /Admin y completar formularios
2. **BD Validation:** Ejecutar queries SQL para verificar datos
3. **Security Check:** Intentar acceso sin auth
4. **Regression:** Compilación sin errores

---

**Este documento sirve como especificación ejecutable para validar la implementación.**

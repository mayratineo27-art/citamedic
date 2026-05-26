╔═══════════════════════════════════════════════════════════════════════════════╗
║                                                                               ║
║                    🎯 COMANDO ÚNICO DE COMPILACIÓN                            ║
║                                                                               ║
╚═══════════════════════════════════════════════════════════════════════════════╝

COPIA Y EJECUTA EN POWERSHELL (desde la raíz del proyecto):

┌───────────────────────────────────────────────────────────────────────────────┐
│                                                                               │
│  dotnet build && dotnet ef database update                                   │
│                                                                               │
└───────────────────────────────────────────────────────────────────────────────┘

DESGLOSE:
─────────
1. dotnet build
   └─ Compila el proyecto (verifica sintaxis y referencias)

2. && (conector AND)
   └─ Ejecuta el siguiente comando si el anterior fue exitoso

3. dotnet ef database update
   └─ Aplica migraciones a la BD (crea/actualiza tablas)


RESULTADO ESPERADO:
───────────────────
✅ Build succeeded                                    (1-2 seg)
✅ Done.                                              (1-2 seg)
✅ Done. Applying migration '20260524225754_Ini...'  (2-3 seg)


TIEMPO TOTAL: ~5-10 segundos


═══════════════════════════════════════════════════════════════════════════════

ALTERNATIVAS:

1. Solo compilar (sin BD):
   $ dotnet build

2. Solo actualizar BD (asume ya compiló):
   $ dotnet ef database update

3. Si necesitas resetear la BD:
   $ dotnet ef database drop --force
   $ dotnet ef database update

4. Ver estado de migraciones:
   $ dotnet ef migrations list

═══════════════════════════════════════════════════════════════════════════════

SI ALGO FALLA:

❌ "Unable to connect to database"
   └─ Verifica la cadena de conexión en appsettings.json

❌ "Build failed"
   └─ Ejecuta: dotnet restore
   └─ Luego: dotnet build

❌ "Package not found"
   └─ Ejecuta: dotnet restore

❌ "EF Core tools not installed"
   └─ Ejecuta: dotnet tool install --global dotnet-ef

═══════════════════════════════════════════════════════════════════════════════

VERIFICACIÓN FINAL:

1. Confirma compilación:
   $ dotnet build
   ✅ Build succeeded

2. Confirma BD:
   $ dotnet ef migrations list
   ✅ 20260524225754_InitialCreate  (Applied)

3. Inicia la aplicación (opcional):
   $ dotnet run
   ✅ Now listening on: https://localhost:5001/

═══════════════════════════════════════════════════════════════════════════════

¡LISTO! El proyecto está completamente configurado y compilado. ✨

Próximo paso: Implementar Controladores en Sprint 3

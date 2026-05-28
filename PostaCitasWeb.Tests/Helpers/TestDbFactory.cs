using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PostaCitasWeb.Data;
using Respawn;
using System;
using System.Threading.Tasks;

namespace PostaCitasWeb.Tests.Helpers
{
    /// <summary>
    /// Factory para crear y gestionar instancias de base de datos para pruebas de integración.
    /// Usa Respawn para limpiar la base de datos entre pruebas.
    /// </summary>
    public class TestDbFactory
    {
        private static readonly string _connectionString;
        private static readonly IConfiguration _configuration;
        private static Respawner _respawner;

        static TestDbFactory()
        {
            // Cargar configuración desde appsettings.Test.json si existe, sino usar el default
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
            _connectionString = _configuration.GetConnectionString("TestConnection") 
                ?? _configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Crea una nueva instancia de AppDbContext para pruebas.
        /// </summary>
        public static async Task<AppDbContext> CreateContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_connectionString)
                .EnableSensitiveDataLogging() // Para ver valores en logs de pruebas
                .Options;

            var context = new AppDbContext(options);

            // Asegurar que la base de datos existe
            await context.Database.EnsureCreatedAsync();

            return context;
        }

        /// <summary>
        /// Inicializa Respawn para limpiar la base de datos entre pruebas.
        /// Debe llamarse una vez al inicio de la suite de pruebas.
        /// </summary>
        public static async Task InitializeRespawnerAsync()
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                TablesToIgnore = new[] { new Respawn.Graph.Table("__EFMigrationsHistory") },
                WithReseed = true
            });
        }

        /// <summary>
        /// Resetea la base de datos a su estado inicial usando Respawn.
        /// Debe llamarse antes de cada prueba de integración.
        /// </summary>
        public static async Task ResetDatabaseAsync()
        {
            if (_respawner == null)
            {
                await InitializeRespawnerAsync();
            }

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);
        }

        /// <summary>
        /// Elimina la base de datos de pruebas completa.
        /// Útil para limpieza al final de todas las pruebas.
        /// </summary>
        public static async Task DropDatabaseAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            await using var context = new AppDbContext(options);
            await context.Database.EnsureDeletedAsync();
        }

        /// <summary>
        /// Obtiene la cadena de conexión actual.
        /// </summary>
        public static string GetConnectionString() => _connectionString;
    }
}

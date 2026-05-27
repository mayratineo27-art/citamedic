using Microsoft.Extensions.Configuration;
using System;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Implementación que lee una fecha/hora simulada desde la configuración para facilitar pruebas manuales.
    /// </summary>
    public class SimulatedDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTime _simulatedTime;

        public SimulatedDateTimeProvider(IConfiguration configuration)
        {
            var simTimeStr = configuration["SimulatedTime"];
            if (!string.IsNullOrEmpty(simTimeStr) && DateTime.TryParse(simTimeStr, out var parsedTime))
            {
                _simulatedTime = parsedTime;
            }
            else
            {
                // Si no se encuentra en configuración, usar 2026-05-27 07:15:00 como hora por defecto
                _simulatedTime = new DateTime(2026, 5, 27, 7, 15, 0);
            }
        }

        public DateTime Now => _simulatedTime;
        public DateTime UtcNow => _simulatedTime.ToUniversalTime();
        public DateOnly Today => DateOnly.FromDateTime(_simulatedTime);
    }
}

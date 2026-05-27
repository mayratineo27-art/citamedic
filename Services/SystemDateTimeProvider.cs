using System;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Implementación que utiliza el reloj del sistema de producción.
    /// </summary>
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
    }
}

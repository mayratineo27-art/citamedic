using System;

namespace PostaCitasWeb.Services
{
    /// <summary>
    /// Proveedor de fecha y hora para facilitar la testabilidad y simulación de tiempo.
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateOnly Today { get; }
    }
}

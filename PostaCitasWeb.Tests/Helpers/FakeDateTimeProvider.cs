using System;
using PostaCitasWeb.Services;

namespace PostaCitasWeb.Tests.Helpers
{
    /// <summary>
    /// Implementación de IDateTimeProvider para pruebas que permite controlar el tiempo.
    /// Esencial para probar reglas de negocio que dependen de ventanas horarias y días de la semana.
    /// </summary>
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        private DateTime _now;
        private DateTime _utcNow;

        public FakeDateTimeProvider()
        {
            _now = DateTime.Now;
            _utcNow = DateTime.UtcNow;
        }

        public DateTime Now
        {
            get => _now;
            set => _now = value;
        }

        public DateTime UtcNow
        {
            get => _utcNow;
            set => _utcNow = value;
        }

        public DateOnly Today => DateOnly.FromDateTime(_now);

        /// <summary>
        /// Establece la fecha y hora actual (local).
        /// </summary>
        public void SetNow(DateTime dateTime)
        {
            _now = dateTime;
        }

        /// <summary>
        /// Establece la fecha y hora actual (UTC).
        /// </summary>
        public void SetUtcNow(DateTime dateTime)
        {
            _utcNow = dateTime;
        }

        /// <summary>
        /// Establece solo la fecha (la hora se establece a mediodía).
        /// </summary>
        public void SetToday(DateOnly date)
        {
            _now = date.ToDateTime(new TimeOnly(12, 0, 0));
            _utcNow = _now.ToUniversalTime();
        }

        /// <summary>
        /// Avanza el tiempo una cantidad específica.
        /// Útil para probar expiraciones y ventanas de tiempo.
        /// </summary>
        public void AdvanceTime(TimeSpan timeSpan)
        {
            _now = _now.Add(timeSpan);
            _utcNow = _utcNow.Add(timeSpan);
        }

        /// <summary>
        /// Reinicia al tiempo actual del sistema.
        /// </summary>
        public void ResetToSystemTime()
        {
            _now = DateTime.Now;
            _utcNow = DateTime.UtcNow;
        }
    }
}

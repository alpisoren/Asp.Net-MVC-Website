using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// ASP.NET Core'un hata ve kritik günlük kayıtlarını (ör. yakalanmayan istisnalar) Trace'e, oradan da
    /// GunlukDinleyici ile App_Data/Logs altındaki günlük dosyalara yazar.
    /// </summary>
    public sealed class TraceHataGunlukSaglayici : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new Kaydedici(categoryName);
        }

        public void Dispose()
        {
        }

        private sealed class Kaydedici : ILogger
        {
            private readonly string kategori;

            public Kaydedici(string kategori)
            {
                this.kategori = kategori;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= LogLevel.Error;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }
                Trace.TraceError("[{0:u}] {1}: {2}{3}", DateTime.UtcNow, kategori, formatter(state, exception),
                    exception == null ? "" : Environment.NewLine + exception);
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// Projenin kendi CSS/JS dosyalarına sürüm parametresi ekler: /Content/x.css?v=8DD1A2B3C4.
    /// Dosya değişince adres değişir, tarayıcılar eski kopyayı önbellekten kullanmaz.
    /// </summary>
    public static class VarlikHelper
    {
        private static readonly ConcurrentDictionary<string, string> Surumler = new ConcurrentDictionary<string, string>();

        /// <param name="yol">"~/Content/..." biçiminde site içi yol.</param>
        public static string Url(string yol)
        {
            var adres = VirtualPathUtility.ToAbsolute(yol);
            var surum = Surumler.GetOrAdd(yol, Hesapla);
            return surum == null ? adres : adres + "?v=" + surum;
        }

        private static string Hesapla(string yol)
        {
            var dosya = HostingEnvironment.MapPath(yol);
            if (dosya == null || !File.Exists(dosya))
            {
                return null;
            }
            // Uygulama yeniden başlayınca (yayın) önbellek boşalır; çalışırken dosya değişirse izleyici sıfırlar.
            IzleyiciKur(Path.GetDirectoryName(dosya));
            return File.GetLastWriteTimeUtc(dosya).Ticks.ToString("x");
        }

        private static readonly ConcurrentDictionary<string, FileSystemWatcher> Izleyiciler = new ConcurrentDictionary<string, FileSystemWatcher>();

        private static void IzleyiciKur(string klasor)
        {
            Izleyiciler.GetOrAdd(klasor, k =>
            {
                try
                {
                    var w = new FileSystemWatcher(k) { IncludeSubdirectories = false, EnableRaisingEvents = true };
                    FileSystemEventHandler temizle = (s, e) => Surumler.Clear();
                    w.Changed += temizle;
                    w.Created += temizle;
                    w.Renamed += (s, e) => Surumler.Clear();
                    return w;
                }
                catch (Exception)
                {
                    return null; // izlenemiyorsa sürüm yeniden başlatmada güncellenir
                }
            });
        }
    }
}

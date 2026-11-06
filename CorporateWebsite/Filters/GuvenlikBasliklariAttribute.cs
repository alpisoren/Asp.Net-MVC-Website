using CorporateWebsite.Controllers;
using CorporateWebsite.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CorporateWebsite.Filters
{
    /// <summary>
    /// Content-Security-Policy başlığını ekler (Program.cs'te global filtre).
    ///  - Ziyaretçi sitesi (Home, Hata): inline script'e izin verilmez; XSS'e karşı asıl koruma budur.
    ///    Google Analytics yalnızca appsettings → Site:GoogleAnalyticsId doluysa ve yalnızca gereken Google adresleri için açılır.
    ///  - Yönetim paneli: aynı şekilde inline script yasak (Jodit editörü inline olay özniteliği kullanmaz).
    ///    Editör ve tablolar satır içi stil kullandığı için yalnızca style-src'de 'unsafe-inline' var.
    /// Yazı tipleri sitenin kendi sunucusundan gelir (wwwroot/Content/SiteLayout/fonts). Diğer başlıklar Program.cs'te.
    /// </summary>
    public class GuvenlikBasliklariAttribute : ResultFilterAttribute
    {
        private const string GoogleAnalyticsBetik = " https://www.googletagmanager.com";
        private const string GoogleAnalyticsBaglanti = " https://*.google-analytics.com https://*.analytics.google.com https://*.googletagmanager.com";

        public static string SitePolitikasi(bool analitik)
        {
            return "default-src 'self'; " +
                   "script-src 'self'" + (analitik ? GoogleAnalyticsBetik : "") + "; " +
                   "style-src 'self' 'unsafe-inline'; " +
                   "font-src 'self' data:; " +
                   "img-src 'self' data: https:; " +
                   "frame-src https://www.google.com; " +
                   "connect-src 'self'" + (analitik ? GoogleAnalyticsBaglanti : "") + "; " +
                   "object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'self'";
        }

        public const string PanelPolitikasi =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "font-src 'self' data:; " +
            "img-src 'self' data: blob: https:; " +
            "connect-src 'self'; " +
            "object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'self'";

        /// <summary>appsettings → Site:GoogleAnalyticsId (G-XXXXXXXXXX). Boşsa analitik kapalı.</summary>
        public static string AnalitikKimligi
        {
            get { return AnalitikKimligiDogrula(Ayarlar.Site.GoogleAnalyticsId); }
        }

        /// <summary>Yalnızca GA4 biçimi (G-XXXXXXXX) kabul edilir: değer HTML'e ve CSP'ye yazıldığı için doğrulanır.</summary>
        public static string AnalitikKimligiDogrula(string ham)
        {
            var id = (ham ?? "").Trim();
            return System.Text.RegularExpressions.Regex.IsMatch(id, "^G-[A-Z0-9]{4,20}$") ? id : null;
        }

        /// <summary>
        /// Yalnızca geliştirmede: Visual Studio'nun Browser Link ve Hot Reload betikleri localhost'taki başka bir porta
        /// HTTP ve WebSocket ile bağlanır. Canlıda bu betikler eklenmez, politika değişmez.
        /// </summary>
        public const string GelistirmeBaglantilari = " http://localhost:* https://localhost:* ws://localhost:* wss://localhost:*";

        public static string GelistirmeyeUyarla(string politika)
        {
            return politika.Replace("connect-src 'self'", "connect-src 'self'" + GelistirmeBaglantilari);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var siteMi = filterContext.Controller is HomeController || filterContext.Controller is HataController;
            var politika = siteMi ? SitePolitikasi(AnalitikKimligi != null) : PanelPolitikasi;

            var ortam = filterContext.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            if (ortam != null && ortam.IsDevelopment())
            {
                politika = GelistirmeyeUyarla(politika);
            }

            filterContext.HttpContext.Response.Headers["Content-Security-Policy"] = politika;
        }
    }
}

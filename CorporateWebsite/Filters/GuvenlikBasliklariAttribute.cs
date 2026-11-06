using CorporateWebsite.Controllers;
using System.Configuration;
using System.Web.Mvc;

namespace CorporateWebsite.Filters
{
    /// <summary>
    /// Content-Security-Policy başlığını ekler.
    ///  - Ziyaretçi sitesi (Home, Hata): inline script'e izin verilmez; XSS'e karşı asıl koruma budur.
    ///    Google Analytics yalnızca Web.config'te GoogleAnalyticsId doluysa ve yalnızca gereken Google adresleri için açılır.
    ///  - Yönetim paneli: aynı şekilde inline script yasak (Jodit editörü inline olay özniteliği kullanmaz).
    ///    Editör ve tablolar satır içi stil kullandığı için yalnızca style-src'de 'unsafe-inline' var.
    /// Yazı tipleri sitenin kendi sunucusundan gelir (Content/SiteLayout/fonts). Diğer güvenlik başlıkları Web.config'de.
    /// </summary>
    public class GuvenlikBasliklariAttribute : ActionFilterAttribute
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

        /// <summary>Web.config → GoogleAnalyticsId (G-XXXXXXXXXX). Boşsa analitik kapalı.</summary>
        public static string AnalitikKimligi
        {
            get { return AnalitikKimligiDogrula(ConfigurationManager.AppSettings["GoogleAnalyticsId"]); }
        }

        /// <summary>Yalnızca GA4 biçimi (G-XXXXXXXX) kabul edilir: değer HTML'e ve CSP'ye yazıldığı için doğrulanır.</summary>
        public static string AnalitikKimligiDogrula(string ham)
        {
            var id = (ham ?? "").Trim();
            return System.Text.RegularExpressions.Regex.IsMatch(id, "^G-[A-Z0-9]{4,20}$") ? id : null;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Child action'lar (partial) ana isteğin başlıklarını paylaşır; ikinci kez ekleme.
            if (filterContext.IsChildAction)
            {
                return;
            }

            var siteMi = filterContext.Controller is HomeController || filterContext.Controller is HataController;
            var response = filterContext.HttpContext.Response;
            response.Headers.Remove("Content-Security-Policy");
            response.AddHeader("Content-Security-Policy", siteMi ? SitePolitikasi(AnalitikKimligi != null) : PanelPolitikasi);
        }
    }
}

using CorporateWebsite.Controllers;
using System.Web.Mvc;

namespace CorporateWebsite.Filters
{
    /// <summary>
    /// Content-Security-Policy başlığını ekler.
    ///  - Ziyaretçi sitesi (HomeController): inline script'e izin verilmez; XSS'e karşı asıl koruma budur.
    ///  - Yönetim paneli: CKEditor 4 araç çubuğu inline olay öznitelikleri (onclick) kullandığı için
    ///    'unsafe-inline' script gerekir. Panel zaten oturum gerektirir ve içerik HtmlTemizleyici'den geçer.
    /// Diğer güvenlik başlıkları (X-Frame-Options vb.) Web.config'de tanımlıdır.
    /// </summary>
    public class GuvenlikBasliklariAttribute : ActionFilterAttribute
    {
        public const string SitePolitikasi =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
            "font-src 'self' data: https://fonts.gstatic.com; " +
            "img-src 'self' data: https:; " +
            "frame-src https://www.google.com; " +
            "connect-src 'self'; " +
            "object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'self'";

        public const string PanelPolitikasi =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline'; " +
            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
            "font-src 'self' data: https://fonts.gstatic.com; " +
            "img-src 'self' data: blob: https:; " +
            "connect-src 'self'; " +
            "object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'self'";

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Child action'lar (partial) ana isteğin başlıklarını paylaşır; ikinci kez ekleme.
            if (filterContext.IsChildAction)
            {
                return;
            }

            var siteMi = filterContext.Controller is HomeController;
            var response = filterContext.HttpContext.Response;
            response.Headers.Remove("Content-Security-Policy");
            response.AddHeader("Content-Security-Policy", siteMi ? SitePolitikasi : PanelPolitikasi);
        }
    }
}

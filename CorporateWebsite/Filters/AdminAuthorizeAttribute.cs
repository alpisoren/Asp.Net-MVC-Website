using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CorporateWebsite.Filters
{
    /// <summary>
    /// Yönetim paneli yetki kontrolü.
    ///  - Oturum cookie'si (Forms Authentication) yoksa giriş sayfasına yönlendirir (dönüş adresiyle).
    ///  - Cookie'deki yönetici veritabanında hâlâ var mı ve yetkisi ne, her istekte doğrular.
    ///  - SadeceAdmin = true ise yalnızca Yetki değeri "Admin" olanlar erişebilir; diğerleri 403 alır.
    /// Action üzerinde [AllowAnonymous] varsa kontrol atlanır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public const string AdminYetkisi = AktifYonetici.AdminYetkisi;

        public bool SadeceAdmin { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Aynı istekte (child action'lar dahil) veritabanına ikinci kez gitme
            var yonetici = YoneticiOturumu.Aktif(httpContext);
            if (yonetici == null)
            {
                var id = YoneticiOturumu.KimlikId(httpContext.User);
                if (id == null)
                {
                    return false;
                }
                yonetici = YoneticiBul(id.Value);
                if (yonetici == null)
                {
                    return false; // hesap silinmiş
                }
                YoneticiOturumu.Ata(httpContext, yonetici);
            }

            return !SadeceAdmin || yonetici.AdminMi;
        }

        /// <summary>Yöneticiyi veritabanından okur. Testlerde ezilebilir.</summary>
        protected virtual AktifYonetici YoneticiBul(int adminId)
        {
            using (var db = new KurumsalDBContext())
            {
                return db.Admin.AsNoTracking()
                    .Where(a => a.AdminId == adminId)
                    .Select(a => new AktifYonetici { Id = a.AdminId, Eposta = a.Eposta, Yetki = a.Yetki })
                    .FirstOrDefault();
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var context = filterContext.HttpContext;

            if (YoneticiOturumu.Aktif(context) != null)
            {
                // Giriş yapılmış fakat bu sayfa için yetkisi yok.
                filterContext.Result = new HttpStatusCodeResult(403, "Bu işlem için yetkiniz yok.");
                return;
            }

            // Oturum yok, süresi dolmuş ya da hesap silinmiş: cookie'yi temizle ve girişe yönlendir.
            if (context.User != null && context.User.Identity.IsAuthenticated)
            {
                YoneticiOturumu.CikisYap();
            }

            var degerler = new RouteValueDictionary { { "controller", "Admin" }, { "action", "Login" } };
            if (context.Request != null && string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)
                && !filterContext.IsChildAction)
            {
                degerler["returnUrl"] = context.Request.RawUrl;
            }
            filterContext.Result = new RedirectToRouteResult(degerler);
        }
    }
}

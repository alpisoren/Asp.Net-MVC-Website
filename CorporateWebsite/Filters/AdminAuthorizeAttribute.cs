using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CorporateWebsite.Filters
{
    /// <summary>
    /// Yönetim paneli için oturum kontrolü. Oturum yoksa giriş sayfasına yönlendirir.
    /// SadeceAdmin = true ise yalnızca Yetki değeri "Admin" olan kullanıcılar erişebilir.
    /// Action üzerinde [AllowAnonymous] varsa kontrol atlanır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public const string AdminYetkisi = "Admin";

        public bool SadeceAdmin { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var session = httpContext.Session;
            if (session == null || session["adminid"] == null)
            {
                return false;
            }

            if (SadeceAdmin)
            {
                var yetki = session["yetki"] as string;
                return string.Equals(yetki, AdminYetkisi, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            bool girisYapilmis = session != null && session["adminid"] != null;

            if (girisYapilmis)
            {
                // Giriş yapılmış fakat yetkisi yetersiz.
                filterContext.Result = new HttpStatusCodeResult(403, "Bu işlem için yetkiniz yok.");
                return;
            }

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Admin" },
                { "action", "Login" }
            });
        }
    }
}

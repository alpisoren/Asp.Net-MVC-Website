using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CorporateWebsite.Filters
{
    /// <summary>
    /// Yönetim paneli yetki kontrolü.
    ///  - Oturum çerezi yoksa giriş sayfasına yönlendirir (GET isteklerinde dönüş adresiyle).
    ///  - Çerezdeki yönetici veritabanında hâlâ var mı ve yetkisi ne, her istekte doğrular.
    ///  - SadeceAdmin = true ise yalnızca Yetki değeri "Admin" olanlar erişebilir; diğerleri 403 alır.
    /// Aksiyonda [AllowAnonymous] varsa kontrol atlanır. Sınıfta ve aksiyonda birlikte kullanılabilir (ikisi de uygulanır).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AdminAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public const string AdminYetkisi = AktifYonetici.AdminYetkisi;
        public const string GirisAdresi = "/yonetimpaneli/giris";

        public bool SadeceAdmin { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
            {
                return;
            }

            var http = context.HttpContext;

            // Aynı istekte veritabanına ikinci kez gitme (sınıf + aksiyon özniteliği birlikte çalışabilir).
            var yonetici = YoneticiOturumu.Aktif(http);
            if (yonetici == null)
            {
                var id = YoneticiOturumu.KimlikId(http.User);
                yonetici = id == null ? null : YoneticiBul(id.Value);
                if (yonetici == null)
                {
                    // Oturum yok, süresi dolmuş ya da hesap silinmiş: çerezi temizle ve girişe yönlendir.
                    if (http.User?.Identity?.IsAuthenticated == true)
                    {
                        await YoneticiOturumu.CikisYap(http);
                    }
                    context.Result = new RedirectResult(GirisYonlendirmesi(http.Request));
                    return;
                }
                YoneticiOturumu.Ata(http, yonetici);
            }

            if (SadeceAdmin && !yonetici.AdminMi)
            {
                // Giriş yapılmış fakat bu sayfa için yetkisi yok.
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }

        private static string GirisYonlendirmesi(HttpRequest istek)
        {
            if (!HttpMethods.IsGet(istek.Method))
            {
                return GirisAdresi;
            }
            var donus = istek.PathBase + istek.Path + istek.QueryString;
            return GirisAdresi + "?returnUrl=" + Uri.EscapeDataString(donus);
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
    }
}

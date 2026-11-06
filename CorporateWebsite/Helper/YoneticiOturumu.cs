using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CorporateWebsite.Helper
{
    /// <summary>İstek boyunca kullanılan, veritabanından doğrulanmış aktif yönetici bilgisi.</summary>
    public class AktifYonetici
    {
        public const string AdminYetkisi = "Admin";

        public int Id { get; set; }
        public string Eposta { get; set; }
        public string Yetki { get; set; }

        public bool AdminMi
        {
            get { return string.Equals(Yetki, AdminYetkisi, StringComparison.Ordinal); }
        }
    }

    /// <summary>
    /// Çerez tabanlı yönetici oturumu (ASP.NET Core cookie authentication, çerez adı ".KurumsalPanel").
    /// Çerez şifreli ve HttpOnly'dir; içinde yalnızca yönetici kimliği (AdminId) taşınır.
    /// Yetki her istekte veritabanından okunur (bkz. AdminAuthorizeAttribute), böylece
    /// yetki değişikliği ya da silinen hesap anında etkili olur.
    /// </summary>
    public static class YoneticiOturumu
    {
        public static readonly TimeSpan OturumSuresi = TimeSpan.FromMinutes(120);
        public static readonly TimeSpan HatirlaSuresi = TimeSpan.FromDays(14);
        public const string KimlikTalebi = "kurumsal:adminId";
        private const string ItemAnahtari = "KurumsalWeb.AktifYonetici";

        public static Task GirisYap(HttpContext context, int adminId, string eposta, bool beniHatirla)
        {
            var kimlik = new ClaimsIdentity(new[]
            {
                new Claim(KimlikTalebi, adminId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, eposta ?? "")
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            var ozellikler = new AuthenticationProperties
            {
                // "Beni hatırla": kalıcı çerez, 14 gün. Aksi halde tarayıcı kapanınca biter, 120 dk hareketsizlikte düşer.
                IsPersistent = beniHatirla,
                ExpiresUtc = beniHatirla ? DateTimeOffset.UtcNow.Add(HatirlaSuresi) : (DateTimeOffset?)null,
                AllowRefresh = true
            };
            return context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(kimlik), ozellikler);
        }

        public static Task CikisYap(HttpContext context)
        {
            return context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>Çerezdeki yönetici kimliği; oturum yoksa ya da talep bozuksa null.</summary>
        public static int? KimlikId(ClaimsPrincipal kullanici)
        {
            if (kullanici?.Identity == null || !kullanici.Identity.IsAuthenticated)
            {
                return null;
            }
            var deger = kullanici.FindFirst(KimlikTalebi)?.Value;
            return int.TryParse(deger, NumberStyles.None, CultureInfo.InvariantCulture, out var id) && id > 0 ? id : (int?)null;
        }

        /// <summary>Bu istekte doğrulanmış aktif yönetici (yetki filtresi çalıştıktan sonra dolar).</summary>
        public static AktifYonetici Aktif(HttpContext context)
        {
            return context?.Items[ItemAnahtari] as AktifYonetici;
        }

        internal static void Ata(HttpContext context, AktifYonetici yonetici)
        {
            context.Items[ItemAnahtari] = yonetici;
        }
    }
}

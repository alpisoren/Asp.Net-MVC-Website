using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

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
    /// Cookie tabanlı yönetici oturumu (Forms Authentication).
    /// Cookie şifreli ve HttpOnly'dir; içinde yalnızca yönetici kimliği (AdminId) taşınır.
    /// Yetki her istekte veritabanından okunur (bkz. AdminAuthorizeAttribute), böylece
    /// yetki değişikliği ya da silinen hesap anında etkili olur.
    /// Uygulama yeniden başlasa da oturum düşmez (sunucu belleğindeki Session'a bağlı değil).
    /// </summary>
    public static class YoneticiOturumu
    {
        public static readonly TimeSpan HatirlaSuresi = TimeSpan.FromDays(14);
        private const string ItemAnahtari = "KurumsalWeb.AktifYonetici";

        public static void GirisYap(HttpResponseBase response, int adminId, string eposta, bool beniHatirla)
        {
            var simdi = DateTime.Now;
            var sure = beniHatirla ? HatirlaSuresi : FormsAuthentication.Timeout;
            var bilet = new FormsAuthenticationTicket(1, eposta ?? "", simdi, simdi.Add(sure), beniHatirla,
                adminId.ToString(System.Globalization.CultureInfo.InvariantCulture), FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(bilet))
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath,
                SameSite = SameSiteMode.Lax
            };
            if (beniHatirla)
            {
                cookie.Expires = bilet.Expiration;
            }
            response.Cookies.Add(cookie);
        }

        public static void CikisYap()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>Cookie'deki yönetici kimliği; oturum yoksa ya da bilet bozuksa null.</summary>
        public static int? KimlikId(IPrincipal kullanici)
        {
            var kimlik = kullanici?.Identity as FormsIdentity;
            if (kimlik == null || !kimlik.IsAuthenticated || kimlik.Ticket == null)
            {
                return null;
            }
            int id;
            return int.TryParse(kimlik.Ticket.UserData, out id) && id > 0 ? id : (int?)null;
        }

        /// <summary>Bu istekte doğrulanmış aktif yönetici (yetki filtresi çalıştıktan sonra dolar).</summary>
        public static AktifYonetici Aktif(HttpContextBase context)
        {
            return context?.Items[ItemAnahtari] as AktifYonetici;
        }

        internal static void Ata(HttpContextBase context, AktifYonetici yonetici)
        {
            context.Items[ItemAnahtari] = yonetici;
        }
    }
}

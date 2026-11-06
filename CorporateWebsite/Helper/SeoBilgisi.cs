using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Script.Serialization;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// Bir sayfanın arama motoru ve sosyal paylaşım bilgileri. Controller ViewBag.Seo'ya koyar,
    /// _SiteLayout içindeki _SeoEtiketleri kısmi görünümü meta / Open Graph / JSON-LD olarak basar.
    /// Boş bırakılan alanlar site kimliğinden (Kimlik tablosu) tamamlanır.
    /// </summary>
    public class SeoBilgisi
    {
        /// <summary>Meta açıklama (en fazla ~160 karakter önerilir).</summary>
        public string Aciklama { get; set; }

        /// <summary>Kalıcı adres: site içi yol ("/Hakkimizda") ya da mutlak adres. Boşsa canonical basılmaz.</summary>
        public string Canonical { get; set; }

        /// <summary>Örn. "noindex, follow" (arama sonuçları, hata sayfaları).</summary>
        public string Robots { get; set; }

        /// <summary>Open Graph türü: "website" ya da "article".</summary>
        public string Tur { get; set; } = "website";

        /// <summary>Paylaşım görseli: site içi yol ya da mutlak adres.</summary>
        public string Resim { get; set; }

        public DateTime? YayinTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public string Yazar { get; set; }
        public IList<string> Etiketler { get; set; } = new List<string>();

        /// <summary>Sayfaya özel schema.org nesneleri (JSON-LD olarak basılır).</summary>
        public IList<object> YapisalVeri { get; } = new List<object>();
    }

    public static class SeoAdres
    {
        /// <summary>
        /// Site içi yolu mutlak adrese çevirir. Web.config → appSettings → SiteAdresi doluysa o kullanılır
        /// (ör. https://www.alanadi.com.tr); boşsa isteğin kendi adresi. Ters vekil (proxy) arkasında ayarı doldurun.
        /// </summary>
        public static string Mutlak(HttpRequestBase istek, string yol)
        {
            return Mutlak(istek, yol, ConfigurationManager.AppSettings["SiteAdresi"]);
        }

        public static string Mutlak(HttpRequestBase istek, string yol, string siteAdresi)
        {
            if (string.IsNullOrEmpty(yol))
            {
                return null;
            }
            if (yol.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || yol.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return yol;
            }
            if (yol.StartsWith("~"))
            {
                yol = yol.Substring(1);
            }
            if (!yol.StartsWith("/"))
            {
                yol = "/" + yol;
            }
            return Kok(istek, siteAdresi) + yol;
        }

        /// <summary>Şema + alan adı, sonda eğik çizgi olmadan.</summary>
        public static string Kok(HttpRequestBase istek)
        {
            return Kok(istek, ConfigurationManager.AppSettings["SiteAdresi"]);
        }

        public static string Kok(HttpRequestBase istek, string siteAdresi)
        {
            if (!string.IsNullOrWhiteSpace(siteAdresi))
            {
                return siteAdresi.Trim().TrimEnd('/');
            }
            return istek.Url.GetLeftPart(UriPartial.Authority);
        }
    }

    public static class JsonLd
    {
        /// <summary>
        /// schema.org nesnesini &lt;script type="application/ld+json"&gt; içine güvenle konacak JSON'a çevirir.
        /// "&lt;" kaçışlanır; içerikteki "&lt;/script&gt;" bloğu kapatamaz.
        /// </summary>
        public static IHtmlString Yaz(object veri)
        {
            var json = new JavaScriptSerializer().Serialize(veri)
                .Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
            return new HtmlString(json);
        }
    }
}

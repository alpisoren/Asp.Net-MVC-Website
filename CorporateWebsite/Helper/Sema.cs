using CorporateWebsite.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateWebsite.Helper
{
    /// <summary>schema.org nesneleri (JSON-LD). Google'ın zengin sonuçlar için okuduğu alanlar doldurulur.</summary>
    public static class Sema
    {
        private static Dictionary<string, object> Yeni(string tur)
        {
            return new Dictionary<string, object> { { "@context", "https://schema.org" }, { "@type", tur } };
        }

        private static void Ekle(Dictionary<string, object> d, string anahtar, object deger)
        {
            var metin = deger as string;
            if (deger == null || (metin != null && metin.Trim().Length == 0))
            {
                return;
            }
            d[anahtar] = deger;
        }

        /// <summary>ISO 8601, saat dilimi farkıyla (ör. 2026-11-06T10:27:41+03:00).</summary>
        public static string Tarih(DateTime t)
        {
            var fark = TimeZoneInfo.Local.GetUtcOffset(t);
            return t.ToString("yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
                + (fark < TimeSpan.Zero ? "-" : "+") + fark.ToString("hh\\:mm");
        }

        public static Dictionary<string, object> Kurum(HttpRequestBase istek, Kimlik kimlik, Iletisim iletisim)
        {
            var d = Yeni("Organization");
            d["@id"] = SeoAdres.Kok(istek) + "/#kurum";
            Ekle(d, "name", kimlik?.Unvan ?? kimlik?.Title);
            d["url"] = SeoAdres.Kok(istek) + "/";
            Ekle(d, "description", kimlik?.Description);
            if (!string.IsNullOrEmpty(kimlik?.LogoUrl))
            {
                d["logo"] = SeoAdres.Mutlak(istek, kimlik.LogoUrl);
            }
            if (iletisim != null)
            {
                Ekle(d, "email", iletisim.Eposta);
                Ekle(d, "telephone", iletisim.Telefon);
                if (!string.IsNullOrWhiteSpace(iletisim.Adres))
                {
                    d["address"] = new Dictionary<string, object>
                    {
                        { "@type", "PostalAddress" }, { "streetAddress", iletisim.Adres }, { "addressCountry", "TR" }
                    };
                }
                var profiller = new List<string>();
                if (!string.IsNullOrWhiteSpace(iletisim.Facebook)) profiller.Add("https://www.facebook.com/" + iletisim.Facebook.Trim());
                if (!string.IsNullOrWhiteSpace(iletisim.Twitter)) profiller.Add("https://twitter.com/" + iletisim.Twitter.Trim());
                if (!string.IsNullOrWhiteSpace(iletisim.Instagram)) profiller.Add("https://www.instagram.com/" + iletisim.Instagram.Trim());
                if (profiller.Count > 0)
                {
                    d["sameAs"] = profiller;
                }
            }
            return d;
        }

        /// <summary>Site ve site içi arama kutusu (Google "sitelinks searchbox").</summary>
        public static Dictionary<string, object> WebSitesi(HttpRequestBase istek, string ad)
        {
            var kok = SeoAdres.Kok(istek);
            var d = Yeni("WebSite");
            d["@id"] = kok + "/#site";
            Ekle(d, "name", ad);
            d["url"] = kok + "/";
            d["inLanguage"] = "tr-TR";
            d["publisher"] = new Dictionary<string, object> { { "@id", kok + "/#kurum" } };
            d["potentialAction"] = new Dictionary<string, object>
            {
                { "@type", "SearchAction" },
                { "target", new Dictionary<string, object> { { "@type", "EntryPoint" }, { "urlTemplate", kok + "/BlogPost?q={search_term_string}" } } },
                { "query-input", "required name=search_term_string" }
            };
            return d;
        }

        public static Dictionary<string, object> BlogYazisi(HttpRequestBase istek, Blog blog, string canonical, string aciklama, Kimlik kimlik)
        {
            var kok = SeoAdres.Kok(istek);
            var d = Yeni("BlogPosting");
            d["mainEntityOfPage"] = new Dictionary<string, object> { { "@type", "WebPage" }, { "@id", canonical } };
            d["headline"] = blog.Baslik.Length > 110 ? blog.Baslik.Substring(0, 110) : blog.Baslik;
            Ekle(d, "description", aciklama);
            d["datePublished"] = Tarih(blog.YayinTarihi);
            d["dateModified"] = Tarih(blog.GuncellemeTarihi ?? blog.YayinTarihi);
            d["inLanguage"] = "tr-TR";
            d["url"] = canonical;
            if (!string.IsNullOrEmpty(blog.ResimURL))
            {
                d["image"] = new[] { SeoAdres.Mutlak(istek, blog.ResimURL) };
            }
            d["author"] = string.IsNullOrWhiteSpace(blog.Yazar)
                ? (object)new Dictionary<string, object> { { "@type", "Organization" }, { "@id", kok + "/#kurum" }, { "name", kimlik?.Unvan ?? kimlik?.Title }, { "url", kok + "/" } }
                : new Dictionary<string, object> { { "@type", "Person" }, { "name", blog.Yazar } };
            var yayinci = new Dictionary<string, object> { { "@type", "Organization" }, { "@id", kok + "/#kurum" }, { "name", kimlik?.Unvan ?? kimlik?.Title } };
            if (!string.IsNullOrEmpty(kimlik?.LogoUrl))
            {
                yayinci["logo"] = new Dictionary<string, object> { { "@type", "ImageObject" }, { "url", SeoAdres.Mutlak(istek, kimlik.LogoUrl) } };
            }
            d["publisher"] = yayinci;
            if (blog.Kategori != null)
            {
                d["articleSection"] = blog.Kategori.KategoriAd;
            }
            var etiketler = EtiketHelper.Ayir(blog.Etiketler);
            if (etiketler.Count > 0)
            {
                d["keywords"] = string.Join(", ", etiketler);
            }
            return d;
        }

        /// <summary>Konum (breadcrumb) listesi: (ad, site içi yol) çiftleri; son öğenin yolu boş olabilir.</summary>
        public static Dictionary<string, object> Kirinti(HttpRequestBase istek, IEnumerable<Tuple<string, string>> ogeler, string sonAdres)
        {
            var d = Yeni("BreadcrumbList");
            var liste = ogeler.ToList();
            d["itemListElement"] = liste.Select((o, i) =>
            {
                var oge = new Dictionary<string, object> { { "@type", "ListItem" }, { "position", i + 1 }, { "name", o.Item1 } };
                var adres = string.IsNullOrEmpty(o.Item2) ? sonAdres : SeoAdres.Mutlak(istek, o.Item2);
                if (adres != null)
                {
                    oge["item"] = adres;
                }
                return oge;
            }).ToList();
            return d;
        }
    }
}

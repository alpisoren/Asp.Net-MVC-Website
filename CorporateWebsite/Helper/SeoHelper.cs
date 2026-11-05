using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CorporateWebsite.Helper
{
    public class SeoHelper
    {
        /// <summary>Blog yazısının kalıcı adresi: /BlogPost/{slug}-{id}</summary>
        public static string BlogUrl(string baslik, int id)
        {
            return "/BlogPost/" + Slug(baslik, "yazi") + "-" + id;
        }

        /// <summary>Kategori sayfasının kalıcı adresi: /BlogPost/{slug}/{id}</summary>
        public static string KategoriUrl(string kategoriAd, int id)
        {
            return "/BlogPost/" + Slug(kategoriAd, "kategori") + "/" + id;
        }

        /// <summary>Boş sonuç üretebilecek başlıklar için (ör. sadece noktalama) yedek değerli slug.</summary>
        public static string Slug(string metin, string yedek)
        {
            var slug = ToSeoUrl(metin);
            return string.IsNullOrEmpty(slug) ? yedek : slug;
        }

        public static string ToSeoUrl(string IncomingText)
        {
            if (string.IsNullOrEmpty(IncomingText))
            {
                return "";
            }
            IncomingText = IncomingText.Replace("ş", "s");
            IncomingText = IncomingText.Replace("Ş", "s");
            IncomingText = IncomingText.Replace("İ", "i");
            IncomingText = IncomingText.Replace("I", "i");
            IncomingText = IncomingText.Replace("ı", "i");
            IncomingText = IncomingText.Replace("ö", "o");
            IncomingText = IncomingText.Replace("Ö", "o");
            IncomingText = IncomingText.Replace("ü", "u");
            IncomingText = IncomingText.Replace("Ü", "u");
            IncomingText = IncomingText.Replace("Ç", "c");
            IncomingText = IncomingText.Replace("ç", "c");
            IncomingText = IncomingText.Replace("ğ", "g");
            IncomingText = IncomingText.Replace("Ğ", "g");
            IncomingText = IncomingText.Replace(" ", "-");
            IncomingText = IncomingText.Replace("---", "-");
            IncomingText = IncomingText.Replace("?", "");
            IncomingText = IncomingText.Replace("/", "");
            IncomingText = IncomingText.Replace(".", "");
            IncomingText = IncomingText.Replace("'", "");
            IncomingText = IncomingText.Replace("#", "");
            IncomingText = IncomingText.Replace("%", "");
            IncomingText = IncomingText.Replace("&", "");
            IncomingText = IncomingText.Replace("*", "");
            IncomingText = IncomingText.Replace("!", "");
            IncomingText = IncomingText.Replace("@", "");
            IncomingText = IncomingText.Replace("+", "");

            IncomingText = IncomingText.ToLower();
            IncomingText = IncomingText.Trim();

            // tüm harfleri küçült
            string encodedUrl = (IncomingText ?? "").ToLower();

            // & ile " " yer değiştirme
            encodedUrl = Regex.Replace(encodedUrl, @"&+", "and");

            // " " karakterlerini silme
            encodedUrl = encodedUrl.Replace("'", "");

            // geçersiz karakterleri sil
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // tekrar edenleri sil
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // karakterlerin arasına tire koy
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }
    }
}
using Ganss.Xss;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// CKEditor ile girilen zengin metni (blog, hizmet, hakkımızda) güvenli hale getirir:
    /// script, olay öznitelikleri (onclick vb.), javascript: bağlantıları ve tehlikeli CSS temizlenir;
    /// başlık, paragraf, liste, tablo, resim, bağlantı gibi biçimlendirmeler korunur.
    /// </summary>
    public static class HtmlTemizleyici
    {
        // HtmlSanitizer yapılandırıldıktan sonra Sanitize çağrıları iş parçacığı güvenlidir.
        private static readonly HtmlSanitizer Temizleyici = Olustur();

        private static HtmlSanitizer Olustur()
        {
            var t = new HtmlSanitizer();
            t.AllowedAttributes.Add("class");
            t.AllowedAttributes.Add("target");
            return t;
        }

        public static string Temizle(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }
            return Temizleyici.Sanitize(html);
        }
    }
}

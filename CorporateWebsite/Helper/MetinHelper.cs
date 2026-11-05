using System.Text.RegularExpressions;
using System.Web;

namespace CorporateWebsite.Helper
{
    public static class MetinHelper
    {
        private static readonly Regex EtiketDesen = new Regex("<[^>]*>", RegexOptions.Compiled);
        private static readonly Regex BoslukDesen = new Regex(@"\s+", RegexOptions.Compiled);

        /// <summary>
        /// HTML içerikten etiketleri atıp düz metin özeti üretir. Sonuç view'da
        /// normal @ ile (encode edilerek) yazılmalıdır; Html.Raw gerekmez.
        /// </summary>
        public static string Ozet(string html, int uzunluk = 200)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }
            var metin = HttpUtility.HtmlDecode(EtiketDesen.Replace(html, " "));
            metin = BoslukDesen.Replace(metin, " ").Trim();
            if (metin.Length <= uzunluk)
            {
                return metin;
            }
            var kesim = metin.LastIndexOf(' ', uzunluk);
            return metin.Substring(0, kesim > uzunluk / 2 ? kesim : uzunluk) + "…";
        }

        /// <summary>Baştaki/sondaki boşlukları atar; boş ya da yalnızca boşluksa null döner.</summary>
        public static string Kirp(string metin)
        {
            if (metin == null)
            {
                return null;
            }
            metin = metin.Trim();
            return metin.Length == 0 ? null : metin;
        }
    }
}

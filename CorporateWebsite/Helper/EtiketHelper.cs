using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CorporateWebsite.Helper
{
    /// <summary>Blog etiketleri tek bir metin alanında virgülle ayrılmış olarak tutulur ("seo, web tasarım").</summary>
    public static class EtiketHelper
    {
        public const int MaksimumEtiket = 10;
        public const int MaksimumUzunluk = 30;

        private static readonly CultureInfo Tr = new CultureInfo("tr-TR");

        /// <summary>Kayıtlı metni etiket listesine ayırır (boşlar atlanır).</summary>
        public static List<string> Ayir(string etiketler)
        {
            if (string.IsNullOrWhiteSpace(etiketler))
            {
                return new List<string>();
            }
            return etiketler.Split(',')
                .Select(e => e.Trim())
                .Where(e => e.Length > 0)
                .ToList();
        }

        /// <summary>
        /// Formdan gelen metni düzenler: boşlukları sadeleştirir, küçük harfe çevirir (Türkçe kurallarıyla),
        /// tekrarları atar, en fazla 10 etiket ve etiket başına 30 karakter. Etiket yoksa null döner.
        /// </summary>
        public static string Duzenle(string girdi)
        {
            var liste = Ayir(girdi)
                .Select(e => string.Join(" ", e.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).ToLower(Tr))
                .Select(e => e.Length > MaksimumUzunluk ? e.Substring(0, MaksimumUzunluk).Trim() : e)
                .Where(e => e.Length > 0)
                .Distinct()
                .Take(MaksimumEtiket)
                .ToList();
            return liste.Count == 0 ? null : string.Join(", ", liste);
        }

        /// <summary>Etiket sayfasının adresi: /BlogPost/etiket/{etiket}</summary>
        public static string Url(string etiket)
        {
            return "/BlogPost/etiket/" + Uri.EscapeDataString(etiket);
        }
    }
}

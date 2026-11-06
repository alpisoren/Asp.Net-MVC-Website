using System;
using System.Net.Mail;
using System.Runtime.Caching;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// Herkese açık formlar (yorum, iletişim) ve giriş için IP bazlı, bellek içi istek sınırlayıcı.
    /// Uygulama yeniden başlayınca sayaçlar sıfırlanır ve sunucular arasında paylaşılmaz; bu yüzden
    /// kayıtlı yönetici hesaplarının kilidi ayrıca veritabanında tutulur (Admin.KilitBitis, YoneticiServisi.GirisYap).
    /// </summary>
    public static class SpamKorumasi
    {
        private static readonly MemoryCache Onbellek = new MemoryCache("SpamKorumasi");
        private static readonly object Kilit = new object();

        private class Sayac
        {
            public int Adet;
        }

        /// <summary>Verilen süre içinde aynı IP'den en fazla "limit" isteğe izin verir.</summary>
        public static bool IzinVer(string islem, string ip, int limit, TimeSpan sure)
        {
            var anahtar = Anahtar(islem, ip);
            lock (Kilit)
            {
                var sayac = Onbellek.Get(anahtar) as Sayac;
                if (sayac == null)
                {
                    Onbellek.Add(anahtar, new Sayac { Adet = 1 }, DateTimeOffset.UtcNow.Add(sure));
                    return true;
                }
                if (sayac.Adet >= limit)
                {
                    return false;
                }
                sayac.Adet++;
                return true;
            }
        }

        /// <summary>Başarısız deneme sayısı limite ulaştı mı? (sayacı artırmaz)</summary>
        public static bool SinirAsildi(string islem, string ip, int limit)
        {
            var sayac = Onbellek.Get(Anahtar(islem, ip)) as Sayac;
            return sayac != null && sayac.Adet >= limit;
        }

        /// <summary>Başarısız denemeyi sayar; sayaç ilk denemeden itibaren "sure" kadar yaşar.</summary>
        public static void BasarisizDenemeEkle(string islem, string ip, TimeSpan sure)
        {
            var anahtar = Anahtar(islem, ip);
            lock (Kilit)
            {
                var sayac = Onbellek.Get(anahtar) as Sayac;
                if (sayac == null)
                {
                    Onbellek.Add(anahtar, new Sayac { Adet = 1 }, DateTimeOffset.UtcNow.Add(sure));
                }
                else
                {
                    sayac.Adet++;
                }
            }
        }

        public static void Sifirla(string islem, string ip)
        {
            Onbellek.Remove(Anahtar(islem, ip));
        }

        private static string Anahtar(string islem, string ip)
        {
            return islem + ":" + (ip ?? "bilinmiyor");
        }

        public static bool GecerliEposta(string eposta)
        {
            if (string.IsNullOrWhiteSpace(eposta))
            {
                return false;
            }
            try
            {
                var adres = new MailAddress(eposta.Trim());
                return adres.Address == eposta.Trim();
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class SpamKorumasiTests
    {
        // Her test kendi benzersiz IP'sini kullanır; paylaşılan bellek önbelleği testleri etkilemesin.
        private static string YeniIp()
        {
            return "test-" + Guid.NewGuid().ToString("N");
        }

        [TestMethod]
        public void IzinVer_limite_kadar_izin_verir_sonra_engeller()
        {
            var ip = YeniIp();
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(SpamKorumasi.IzinVer("yorum", ip, 3, TimeSpan.FromMinutes(1)), "deneme " + (i + 1));
            }
            Assert.IsFalse(SpamKorumasi.IzinVer("yorum", ip, 3, TimeSpan.FromMinutes(1)));
        }

        [TestMethod]
        public void IzinVer_farkli_IP_ve_islemleri_ayri_sayar()
        {
            var ip1 = YeniIp();
            var ip2 = YeniIp();
            Assert.IsTrue(SpamKorumasi.IzinVer("iletisim", ip1, 1, TimeSpan.FromMinutes(1)));
            Assert.IsFalse(SpamKorumasi.IzinVer("iletisim", ip1, 1, TimeSpan.FromMinutes(1)));
            Assert.IsTrue(SpamKorumasi.IzinVer("iletisim", ip2, 1, TimeSpan.FromMinutes(1)));
            Assert.IsTrue(SpamKorumasi.IzinVer("yorum", ip1, 1, TimeSpan.FromMinutes(1)));
        }

        [TestMethod]
        public void BasarisizDeneme_sayaci_limit_ve_sifirlama()
        {
            var ip = YeniIp();
            Assert.IsFalse(SpamKorumasi.SinirAsildi("giris", ip, 2));

            SpamKorumasi.BasarisizDenemeEkle("giris", ip, TimeSpan.FromMinutes(1));
            Assert.IsFalse(SpamKorumasi.SinirAsildi("giris", ip, 2));

            SpamKorumasi.BasarisizDenemeEkle("giris", ip, TimeSpan.FromMinutes(1));
            Assert.IsTrue(SpamKorumasi.SinirAsildi("giris", ip, 2));

            SpamKorumasi.Sifirla("giris", ip);
            Assert.IsFalse(SpamKorumasi.SinirAsildi("giris", ip, 2));
        }

        [TestMethod]
        public void GecerliEposta_formatlari_dogru_degerlendirir()
        {
            Assert.IsTrue(SpamKorumasi.GecerliEposta("ad.soyad@ornek.com"));
            Assert.IsTrue(SpamKorumasi.GecerliEposta("  ad@ornek.com "));
            Assert.IsFalse(SpamKorumasi.GecerliEposta("ad@"));
            Assert.IsFalse(SpamKorumasi.GecerliEposta("Ad Soyad <ad@ornek.com>"), "Görünen ad içeren adres kabul edilmemeli");
            Assert.IsFalse(SpamKorumasi.GecerliEposta(""));
            Assert.IsFalse(SpamKorumasi.GecerliEposta(null));
        }
    }
}

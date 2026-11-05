using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class SeoHelperTests
    {
        [TestMethod]
        public void ToSeoUrl_Turkce_karakterleri_donusturur_ve_kucuk_harf_yapar()
        {
            // Kesme işareti ve nokta silinir (tireye çevrilmez); mevcut blog adresleri bu davranışa dayanıyor.
            Assert.AreEqual("istanbulda-guzel-bir-gun", SeoHelper.ToSeoUrl("İstanbul'da Güzel Bir Gün"));
            Assert.AreEqual("cicek-sogan-ozel-isik", SeoHelper.ToSeoUrl("Çiçek Soğan Özel Işık"));
        }

        [TestMethod]
        public void ToSeoUrl_ozel_karakterleri_ve_tekrarlanan_tireleri_temizler()
        {
            Assert.AreEqual("aspnet-mvc-nedir", SeoHelper.ToSeoUrl("  ASP.NET   MVC -- nedir?!  "));
            Assert.AreEqual("c-ve-c", SeoHelper.ToSeoUrl("C# ve C++"));
        }

        [TestMethod]
        public void ToSeoUrl_null_ve_bos_icin_bos_doner()
        {
            Assert.AreEqual("", SeoHelper.ToSeoUrl(null));
            Assert.AreEqual("", SeoHelper.ToSeoUrl(""));
        }

        [TestMethod]
        public void BlogUrl_ve_KategoriUrl_kalici_adres_uretir()
        {
            Assert.AreEqual("/BlogPost/yeni-hizmetimiz-12", SeoHelper.BlogUrl("Yeni Hizmetimiz", 12));
            Assert.AreEqual("/BlogPost/teknoloji/3", SeoHelper.KategoriUrl("Teknoloji", 3));
        }

        [TestMethod]
        public void BlogUrl_slug_bos_kalirsa_yedek_kullanir()
        {
            Assert.AreEqual("/BlogPost/yazi-7", SeoHelper.BlogUrl("???", 7));
            Assert.AreEqual("/BlogPost/kategori/2", SeoHelper.KategoriUrl(null, 2));
        }
    }
}

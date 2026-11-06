using CorporateWebsite.Models.ViewModel;
using CorporateWebsite.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Web.Mvc;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class ServisKurallariTests
    {
        [TestMethod]
        public void SifreKontrol_kurallari()
        {
            Assert.IsNotNull(YoneticiServisi.SifreKontrol("kisa", "kisa"), "8 karakterden kısa");
            Assert.IsNotNull(YoneticiServisi.SifreKontrol(null, null));
            Assert.IsNotNull(YoneticiServisi.SifreKontrol(new string('a', 101), new string('a', 101)), "100 karakterden uzun");
            Assert.AreEqual("Şifreler eşleşmiyor.", YoneticiServisi.SifreKontrol("12345678", "12345679"));
            Assert.IsNull(YoneticiServisi.SifreKontrol("12345678", "12345678"));
        }

        [TestMethod]
        public void Yetkiler_yalnizca_tanimli_degerleri_kabul_eder()
        {
            Assert.IsTrue(Yetkiler.Gecerli("Admin"));
            Assert.IsTrue(Yetkiler.Gecerli("Editor"));
            Assert.IsFalse(Yetkiler.Gecerli("admin"), "Büyük/küçük harf duyarlı olmalı");
            Assert.IsFalse(Yetkiler.Gecerli("Admin "));
            Assert.IsFalse(Yetkiler.Gecerli(null));
        }

        [TestMethod]
        public void Yetkiler_secim_listesi_secili_degeri_isaretler()
        {
            var liste = Yetkiler.SecimListesi("Editor");
            Assert.AreEqual(2, liste.Count());
            Assert.AreEqual("Editor", liste.Single(x => x.Selected).Value);
        }

        [TestMethod]
        public void IslemSonucu_hatalari_ModelStatee_aktarir()
        {
            var sonuc = IslemSonucu.Hata("Eposta", "Kayıtlı").HataEkle(null, "Genel hata");
            var ms = new ModelStateDictionary();

            sonuc.ModelStateAktar(ms);

            Assert.IsFalse(sonuc.Basarili);
            Assert.AreEqual("Kayıtlı", ms["Eposta"].Errors.Single().ErrorMessage);
            Assert.AreEqual("Genel hata", ms[""].Errors.Single().ErrorMessage);
            Assert.IsTrue(IslemSonucu.Tamam().Basarili);
        }
    }
}

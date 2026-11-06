using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Models.ViewModel;
using CorporateWebsite.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CorporateWebsite.Tests
{
    /// <summary>Servislerin iş kuralları, gerçek (geçici) bir SQL Server veritabanına karşı.</summary>
    [TestClass]
    [TestCategory("Veritabani")]
    public class ServisVeritabaniTests
    {
        private KurumsalDBContext db;

        [ClassInitialize]
        public static void Baslat(TestContext _)
        {
            VeritabaniTestOrtami.Hazirla();
        }

        [TestInitialize]
        public void Once()
        {
            db = VeritabaniTestOrtami.YeniBaglam();
        }

        [TestCleanup]
        public void Sonra()
        {
            db.Dispose();
        }

        private static string Benzersiz(string on)
        {
            return on + " " + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        private Blog YaziEkle(string baslik, DateTime tarih, string etiketler = null, int? kategoriId = null)
        {
            var b = new Blog { Baslik = baslik, Icerik = "<p>" + baslik + "</p>", YayinTarihi = tarih, Etiketler = etiketler, KategoriId = kategoriId };
            db.Blog.Add(b);
            db.SaveChanges();
            return b;
        }

        // ---------------------------------------------------------------- Kategori

        [TestMethod]
        public void Kategori_ayni_adla_ikinci_kez_eklenemez()
        {
            var servis = new KategoriServisi(db);
            var ad = Benzersiz("Kategori");
            Assert.IsTrue(servis.Olustur(new Kategori { KategoriAd = ad }).Basarili);

            var ikinci = servis.Olustur(new Kategori { KategoriAd = "  " + ad + " " });
            Assert.IsFalse(ikinci.Basarili);
            Assert.AreEqual("KategoriAd", ikinci.Hatalar.Single().Key);
        }

        [TestMethod]
        public void Kategori_silinince_yazilari_silinmez_kategorisiz_kalir()
        {
            var kategori = new Kategori { KategoriAd = Benzersiz("Silinecek") };
            db.Kategori.Add(kategori);
            db.SaveChanges();
            var yazi = YaziEkle(Benzersiz("Yazı"), DateTime.Now.AddDays(-1), kategoriId: kategori.KategoriId);

            Assert.AreEqual(kategori.KategoriAd, new KategoriServisi(db).Sil(kategori.KategoriId));

            using (var taze = VeritabaniTestOrtami.YeniBaglam())
            {
                var kalan = taze.Blog.Single(x => x.BlogId == yazi.BlogId);
                Assert.IsNull(kalan.KategoriId);
            }
            Assert.IsNull(new KategoriServisi(db).Sil(int.MaxValue), "Olmayan kayıt null döner");
        }

        // ---------------------------------------------------------------- Blog

        [TestMethod]
        public void Blog_ileri_tarihli_yazi_sitede_gorunmez()
        {
            var eski = YaziEkle(Benzersiz("Yayında"), DateTime.Now.AddMinutes(-5));
            var planli = YaziEkle(Benzersiz("Planlı"), DateTime.Now.AddDays(3));
            var servis = new BlogServisi(db, null);

            var idler = servis.Yayindakiler().Select(x => x.BlogId).ToList();
            CollectionAssert.Contains(idler, eski.BlogId);
            CollectionAssert.DoesNotContain(idler, planli.BlogId);
        }

        [TestMethod]
        public void Blog_etiket_tam_eslesir_alt_metni_eslemez()
        {
            var etiket = "etk" + Guid.NewGuid().ToString("N").Substring(0, 6);
            var tek = YaziEkle(Benzersiz("Tek"), DateTime.Now.AddMinutes(-1), etiket);
            var bas = YaziEkle(Benzersiz("Baş"), DateTime.Now.AddMinutes(-1), etiket + ", web");
            var orta = YaziEkle(Benzersiz("Orta"), DateTime.Now.AddMinutes(-1), "web, " + etiket + ", seo");
            var son = YaziEkle(Benzersiz("Son"), DateTime.Now.AddMinutes(-1), "web, " + etiket);
            var benzer = YaziEkle(Benzersiz("Benzer"), DateTime.Now.AddMinutes(-1), etiket + " araçları");

            var idler = new BlogServisi(db, null).EtiketliYazilar(etiket.ToUpperInvariant()).Select(x => x.BlogId).ToList();
            CollectionAssert.AreEquivalent(new[] { tek.BlogId, bas.BlogId, orta.BlogId, son.BlogId }, idler);
            CollectionAssert.DoesNotContain(idler, benzer.BlogId);
        }

        [TestMethod]
        public void Blog_arama_baslik_icerik_ve_etiketlerde_yapilir()
        {
            var kelime = "aranan" + Guid.NewGuid().ToString("N").Substring(0, 6);
            var baslikta = YaziEkle("Başlıkta " + kelime, DateTime.Now.AddMinutes(-1));
            var etikette = YaziEkle(Benzersiz("Etikette"), DateTime.Now.AddMinutes(-1), "web, " + kelime);
            var planli = YaziEkle("Planlı " + kelime, DateTime.Now.AddDays(1));

            var idler = new BlogServisi(db, null).Ara(kelime).Select(x => x.BlogId).ToList();
            CollectionAssert.AreEquivalent(new[] { baslikta.BlogId, etikette.BlogId }, idler, "Planlı yazı aramada da çıkmamalı");
        }

        [TestMethod]
        public void Blog_olusturulurken_etiketler_sadelesir_ve_tarih_atanir()
        {
            var baslik = Benzersiz("Yeni yazı");
            var sonuc = new BlogServisi(db, null).Olustur(new BlogFormModeli
            {
                Baslik = baslik,
                Icerik = "<p>x</p><script>alert(1)</script>",
                YayinTarihi = DateTime.Today,
                Yazar = "  ",
                Etiketler = "Web Tasarım,  SEO , seo,,İzmir"
            }, null);
            Assert.IsTrue(sonuc.Basarili);

            var b = db.Blog.Single(x => x.Baslik == baslik);
            Assert.AreEqual("web tasarım, seo, izmir", b.Etiketler);
            Assert.IsNull(b.Yazar, "Boş yazar null kaydedilir");
            Assert.IsFalse(b.Icerik.Contains("<script"), "İçerik temizlenir");
            Assert.IsTrue((DateTime.Now - b.YayinTarihi).Duration() < TimeSpan.FromMinutes(1), "Bugün seçilince şu anki saat kullanılır");
        }

        [TestMethod]
        public void Blog_guncellemede_gun_degismezse_saat_korunur()
        {
            var ilk = new DateTime(2025, 3, 10, 14, 25, 0);
            var b = YaziEkle(Benzersiz("Saatli"), ilk);
            var servis = new BlogServisi(db, null);
            var model = servis.FormModeliGetir(b.BlogId);
            model.Baslik += " (düzenlendi)";

            Assert.IsTrue(servis.Guncelle(b.BlogId, model, null).Basarili);
            using (var taze = VeritabaniTestOrtami.YeniBaglam())
            {
                var guncel = taze.Blog.Single(x => x.BlogId == b.BlogId);
                Assert.AreEqual(ilk, guncel.YayinTarihi);
                Assert.IsNotNull(guncel.GuncellemeTarihi);
            }
        }

        // ---------------------------------------------------------------- Yorum

        [TestMethod]
        public void Yorum_onayi_tersine_cevrilir_ve_bekleyenler_once_listelenir()
        {
            var yazi = YaziEkle(Benzersiz("Yorumlu"), DateTime.Now.AddMinutes(-1));
            var y = new Yorum { AdSoyad = "Deneme", Icerik = "Merhaba", BlogId = yazi.BlogId, Onayli = true };
            db.Yorum.Add(y);
            db.SaveChanges();
            var servis = new YorumServisi(db);

            Assert.AreEqual(false, servis.OnayDegistir(y.YorumId));
            Assert.AreEqual(false, servis.Listele().First().Onayli, "Onay bekleyenler listenin başında");
            Assert.AreEqual(true, servis.OnayDegistir(y.YorumId));
            Assert.IsNull(servis.OnayDegistir(int.MaxValue));
            Assert.IsFalse(servis.Sil(int.MaxValue));
            Assert.IsTrue(servis.Sil(y.YorumId));
        }

        // ---------------------------------------------------------------- Ayarlar

        [TestMethod]
        public void Hakkimizda_metni_temizlenerek_kaydedilir()
        {
            var h = new Hakkimizda { Aciklama = "<p>eski</p>" };
            db.Hakkimizda.Add(h);
            db.SaveChanges();

            Assert.IsTrue(new AyarServisi(db, null).HakkimizdaGuncelle(h.HakkimizdaId, "<p onclick=\"x()\">yeni</p><script>alert(1)</script>"));
            using (var taze = VeritabaniTestOrtami.YeniBaglam())
            {
                var kayit = taze.Hakkimizda.Single(x => x.HakkimizdaId == h.HakkimizdaId).Aciklama;
                Assert.AreEqual("<p>yeni</p>", kayit);
            }
            Assert.IsFalse(new AyarServisi(db, null).HakkimizdaGuncelle(int.MaxValue, "x"));
        }

        [TestMethod]
        public void Iletisim_bos_alanlar_null_dolu_alanlar_kirpilmis_kaydedilir()
        {
            var servis = new AyarServisi(db, null);
            var i = new Iletisim { Adres = "eski" };
            db.Iletisim.Add(i);
            db.SaveChanges();

            Assert.IsTrue(servis.IletisimGuncelle(i.IletisimId, new Iletisim { Adres = "  Yeni adres  ", Telefon = "   ", Instagram = "kurumsal" }));
            using (var taze = VeritabaniTestOrtami.YeniBaglam())
            {
                var k = taze.Iletisim.Single(x => x.IletisimId == i.IletisimId);
                Assert.AreEqual("Yeni adres", k.Adres);
                Assert.IsNull(k.Telefon);
                Assert.AreEqual("kurumsal", k.Instagram);
            }
        }

        // ---------------------------------------------------------------- Yönetici

        [TestMethod]
        public void Yonetici_kendini_ve_son_Admini_silemez()
        {
            // Bu test kendi verisini kullanır: Admin tablosunda tek Admin bırakılır.
            db.Database.ExecuteSqlCommand("DELETE FROM Admin");
            var admin = new Admin { Eposta = Benzersiz("a").Replace(" ", "") + "@test.local", Sifre = SifreHelper.Hashle("Gecici-Sifre-123"), Yetki = Yetkiler.Admin };
            var editor = new Admin { Eposta = Benzersiz("e").Replace(" ", "") + "@test.local", Sifre = SifreHelper.Hashle("Gecici-Sifre-123"), Yetki = Yetkiler.Editor };
            db.Admin.Add(admin);
            db.Admin.Add(editor);
            db.SaveChanges();
            var servis = new YoneticiServisi(db);

            Assert.IsFalse(servis.Sil(admin.AdminId, admin.AdminId).Basarili, "Kendi hesabını silemez");
            Assert.IsFalse(servis.Sil(admin.AdminId, editor.AdminId).Basarili, "Son Admin silinemez");
            Assert.IsTrue(servis.Sil(editor.AdminId, admin.AdminId).Basarili);
        }
    }
}

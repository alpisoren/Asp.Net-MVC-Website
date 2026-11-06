using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System.IO;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class ResimHelperTests
    {
        private string kok;

        [TestInitialize]
        public void Hazirla()
        {
            kok = GeciciKlasor.Olustur();
            Directory.CreateDirectory(Path.Combine(kok, "Uploads", "Blog"));
            ResimHelper.KokKlasor = kok;
        }

        [TestCleanup]
        public void Temizle()
        {
            Directory.Delete(kok, true);
        }

        private static byte[] Resim(int genislik, int yukseklik, SKEncodedImageFormat bicim)
        {
            using (var bmp = new SKBitmap(genislik, yukseklik))
            {
                bmp.Erase(new SKColor(30, 120, 80));
                using (var img = SKImage.FromBitmap(bmp))
                using (var veri = img.Encode(bicim, 90))
                {
                    return veri.ToArray();
                }
            }
        }

        [TestMethod]
        public void Kaydet_dosya_yoksa_hata_doner()
        {
            Assert.IsNull(ResimHelper.Kaydet(null, "Blog", 10, 10, out var hata));
            Assert.IsNotNull(hata);
        }

        [TestMethod]
        public void Kaydet_izinsiz_uzantiyi_reddeder()
        {
            foreach (var ad in new[] { "kabuk.aspx", "resim.png.config", "betik.svg", "dosya" })
            {
                Assert.IsNull(ResimHelper.Kaydet(Sahte.Dosya(ad, new byte[] { 1, 2, 3 }), "Blog", 10, 10, out var hata), ad);
                StringAssert.Contains(hata, ".jpg");
            }
        }

        [TestMethod]
        public void Kaydet_4MB_ustu_dosyayi_reddeder()
        {
            var dosya = Sahte.Dosya("buyuk.jpg", new byte[] { 1 }, 4 * 1024 * 1024 + 1);
            Assert.IsNull(ResimHelper.Kaydet(dosya, "Blog", 10, 10, out var hata));
            StringAssert.Contains(hata, "4 MB");
        }

        [TestMethod]
        public void Kaydet_resim_olmayan_icerigi_reddeder()
        {
            var dosya = Sahte.Dosya("sahte.jpg", System.Text.Encoding.UTF8.GetBytes("<% Response.Write(1) %>"));
            Assert.IsNull(ResimHelper.Kaydet(dosya, "Blog", 10, 10, out var hata));
            Assert.AreEqual("Geçerli bir resim dosyası değil.", hata);
        }

        [TestMethod]
        public void Kaydet_buyuk_resmi_oranini_koruyarak_kutuya_sigdirir()
        {
            var url = ResimHelper.Kaydet(Sahte.Dosya("kapak.jpg", Resim(1200, 900, SKEncodedImageFormat.Jpeg)), "Blog", 600, 400, out var hata);

            Assert.IsNull(hata);
            StringAssert.StartsWith(url, "/Uploads/Blog/");
            var yol = Path.Combine(kok, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            using (var kayitli = SKBitmap.Decode(yol))
            {
                Assert.AreEqual(533, kayitli.Width, "1200x900, 600x400 kutusuna sığarken yükseklik sınırlar");
                Assert.AreEqual(400, kayitli.Height);
            }
        }

        [TestMethod]
        public void Kaydet_kucuk_resmi_buyutmez_ve_png_olarak_saklar()
        {
            var url = ResimHelper.Kaydet(Sahte.Dosya("logo.png", Resim(120, 80, SKEncodedImageFormat.Png)), "Kimlik", 300, 300, out var hata);

            Assert.IsNull(hata);
            StringAssert.EndsWith(url, ".png");
            var yol = Path.Combine(kok, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            using (var kayitli = SKBitmap.Decode(yol))
            {
                Assert.AreEqual(120, kayitli.Width);
                Assert.AreEqual(80, kayitli.Height);
            }
        }

        [TestMethod]
        public void Sil_Uploads_altindaki_dosyayi_siler()
        {
            var yol = Path.Combine(kok, "Uploads", "Blog", "a.jpg");
            File.WriteAllText(yol, "x");

            ResimHelper.Sil("/Uploads/Blog/a.jpg");

            Assert.IsFalse(File.Exists(yol));
        }

        [TestMethod]
        public void Sil_Uploads_disini_gosteren_yollari_silmez()
        {
            var ayar = Path.Combine(kok, "appsettings.json");
            File.WriteAllText(ayar, "x");

            ResimHelper.Sil("/Uploads/../appsettings.json");
            ResimHelper.Sil("/appsettings.json");
            ResimHelper.Sil(null);
            ResimHelper.Sil("");

            Assert.IsTrue(File.Exists(ayar));
        }
    }
}

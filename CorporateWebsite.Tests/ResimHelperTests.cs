using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class ResimHelperTests
    {
        private string kok;
        private SahteServer server;

        [TestInitialize]
        public void Hazirla()
        {
            kok = GeciciKlasor.Olustur();
            Directory.CreateDirectory(Path.Combine(kok, "Uploads", "Blog"));
            server = new SahteServer(kok);
        }

        [TestCleanup]
        public void Temizle()
        {
            Directory.Delete(kok, true);
        }

        [TestMethod]
        public void Kaydet_dosya_yoksa_hata_doner()
        {
            string hata;
            Assert.IsNull(ResimHelper.Kaydet(null, "Blog", 10, 10, out hata));
            Assert.IsNotNull(hata);
        }

        [TestMethod]
        public void Kaydet_izinsiz_uzantiyi_reddeder()
        {
            string hata;
            foreach (var ad in new[] { "kabuk.aspx", "resim.png.config", "betik.svg", "dosya" })
            {
                Assert.IsNull(ResimHelper.Kaydet(new SahteDosya(ad, new byte[] { 1, 2, 3 }), "Blog", 10, 10, out hata), ad);
                StringAssert.Contains(hata, ".jpg");
            }
        }

        [TestMethod]
        public void Kaydet_4MB_ustu_dosyayi_reddeder()
        {
            string hata;
            var dosya = new SahteDosya("buyuk.jpg", new byte[] { 1 }, 4 * 1024 * 1024 + 1);
            Assert.IsNull(ResimHelper.Kaydet(dosya, "Blog", 10, 10, out hata));
            StringAssert.Contains(hata, "4 MB");
        }

        [TestMethod]
        public void Kaydet_resim_olmayan_icerigi_reddeder()
        {
            string hata;
            var dosya = new SahteDosya("sahte.jpg", System.Text.Encoding.UTF8.GetBytes("<% Response.Write(1) %>"));
            Assert.IsNull(ResimHelper.Kaydet(dosya, "Blog", 10, 10, out hata));
            Assert.AreEqual("Geçerli bir resim dosyası değil.", hata);
        }

        [TestMethod]
        public void Sil_Uploads_altindaki_dosyayi_siler()
        {
            var yol = Path.Combine(kok, "Uploads", "Blog", "a.jpg");
            File.WriteAllText(yol, "x");

            ResimHelper.Sil(server, "/Uploads/Blog/a.jpg");

            Assert.IsFalse(File.Exists(yol));
        }

        [TestMethod]
        public void Sil_Uploads_disini_gosteren_yollari_silmez()
        {
            var webConfig = Path.Combine(kok, "Web.config");
            File.WriteAllText(webConfig, "x");

            ResimHelper.Sil(server, "/Uploads/../Web.config");
            ResimHelper.Sil(server, "/Web.config");
            ResimHelper.Sil(server, null);
            ResimHelper.Sil(server, "");

            Assert.IsTrue(File.Exists(webConfig));
        }
    }
}

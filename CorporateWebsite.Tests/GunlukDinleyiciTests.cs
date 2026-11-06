using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class GunlukDinleyiciTests
    {
        private string klasor;

        [TestInitialize]
        public void Once()
        {
            klasor = Path.Combine(Path.GetTempPath(), "gunluk-test-" + Guid.NewGuid().ToString("N"));
        }

        [TestCleanup]
        public void Sonra()
        {
            if (Directory.Exists(klasor))
            {
                Directory.Delete(klasor, true);
            }
        }

        [TestMethod]
        public void Her_gun_ayri_dosyaya_yazar()
        {
            var simdi = new DateTime(2026, 11, 6, 23, 59, 0);
            var d = new GunlukDinleyici(klasor, saat: () => simdi);
            d.WriteLine("birinci");
            simdi = simdi.AddMinutes(2);
            d.WriteLine("ikinci");

            Assert.AreEqual("birinci" + Environment.NewLine, File.ReadAllText(Path.Combine(klasor, "hatalar-2026-11-06.log")));
            Assert.AreEqual("ikinci" + Environment.NewLine, File.ReadAllText(Path.Combine(klasor, "hatalar-2026-11-07.log")));
        }

        [TestMethod]
        public void Boyut_asilinca_yeni_parcaya_gecer()
        {
            var d = new GunlukDinleyici(klasor, maksimumBoyut: 10, saat: () => new DateTime(2026, 1, 2));
            d.Write("0123456789");   // 10 bayt: sınıra ulaştı
            d.Write("abc");

            Assert.AreEqual("0123456789", File.ReadAllText(Path.Combine(klasor, "hatalar-2026-01-02.log")));
            Assert.AreEqual("abc", File.ReadAllText(Path.Combine(klasor, "hatalar-2026-01-02-2.log")));
        }

        [TestMethod]
        public void Saklama_suresinden_eski_dosyalar_silinir_digerlerine_dokunulmaz()
        {
            Directory.CreateDirectory(klasor);
            File.WriteAllText(Path.Combine(klasor, "hatalar-2026-01-01.log"), "eski");
            File.WriteAllText(Path.Combine(klasor, "hatalar-2026-01-01-2.log"), "eski parça");
            File.WriteAllText(Path.Combine(klasor, "hatalar-2026-01-25.log"), "yeni");
            File.WriteAllText(Path.Combine(klasor, "hatalar.log"), "eski biçim, dokunulmaz");
            File.WriteAllText(Path.Combine(klasor, "baska.log"), "başka dosya");

            new GunlukDinleyici(klasor, saklamaGunu: 10, saat: () => new DateTime(2026, 1, 30));

            var kalan = Directory.GetFiles(klasor).Select(Path.GetFileName).OrderBy(x => x, StringComparer.Ordinal).ToArray();
            CollectionAssert.AreEqual(new[] { "baska.log", "hatalar-2026-01-25.log", "hatalar.log" }, kalan);
        }
    }
}

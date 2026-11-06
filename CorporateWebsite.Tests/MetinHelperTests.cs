using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class MetinHelperTests
    {
        [TestMethod]
        public void Ozet_etiketleri_kaldirir_ve_HTML_varliklarini_cozer()
        {
            Assert.AreEqual("Merhaba dünya & herkes", MetinHelper.Ozet("<p>Merhaba <strong>dünya</strong> &amp; herkes</p>"));
        }

        [TestMethod]
        public void Ozet_bosluklari_tek_bosluga_indirir()
        {
            Assert.AreEqual("bir iki üç", MetinHelper.Ozet("<p>bir</p>\n\n<p>iki</p>   <br/>üç"));
        }

        [TestMethod]
        public void Ozet_uzun_metni_kelime_sinirinda_keser()
        {
            var metin = "Lorem ipsum dolor sit amet consectetur adipiscing elit";
            var ozet = MetinHelper.Ozet(metin, 20);

            Assert.IsTrue(ozet.EndsWith("…"));
            Assert.IsTrue(ozet.Length <= 21);
            Assert.AreEqual("Lorem ipsum dolor…", ozet);
        }

        [TestMethod]
        public void Ozet_null_ve_bos_icin_bos_doner()
        {
            Assert.AreEqual("", MetinHelper.Ozet(null));
            Assert.AreEqual("", MetinHelper.Ozet(""));
        }

        [TestMethod]
        public void Ozet_script_icerigini_etiket_olarak_birakmaz()
        {
            var ozet = MetinHelper.Ozet("<script>alert(1)</script>Metin");
            Assert.IsFalse(ozet.Contains("<"), "Özet hiçbir HTML etiketi içermemeli");
        }
    }
}

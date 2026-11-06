using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class HtmlTemizleyiciTests
    {
        [TestMethod]
        public void Temizle_script_etiketini_kaldirir()
        {
            var sonuc = HtmlTemizleyici.Temizle("<p>Merhaba</p><script>alert(1)</script>");
            Assert.AreEqual("<p>Merhaba</p>", sonuc);
        }

        [TestMethod]
        public void Temizle_olay_ozniteliklerini_kaldirir()
        {
            var sonuc = HtmlTemizleyici.Temizle("<img src=\"/a.png\" onerror=\"alert(1)\"><p onclick=\"x()\">a</p>");
            Assert.IsFalse(sonuc.Contains("onerror"));
            Assert.IsFalse(sonuc.Contains("onclick"));
            StringAssert.Contains(sonuc, "src=\"/a.png\"");
        }

        [TestMethod]
        public void Temizle_javascript_baglantilarini_kaldirir_guvenlileri_korur()
        {
            var sonuc = HtmlTemizleyici.Temizle("<a href=\"javascript:alert(1)\">x</a><a href=\"https://ornek.com\">y</a>");
            Assert.IsFalse(sonuc.Contains("javascript:"));
            StringAssert.Contains(sonuc, "href=\"https://ornek.com\"");
        }

        [TestMethod]
        public void Temizle_editor_bicimlendirmesini_ve_class_ozniteligini_korur()
        {
            var html = "<h2 class=\"baslik\">Başlık</h2><ul><li><strong>a</strong></li></ul><table><tbody><tr><td>1</td></tr></tbody></table>";
            Assert.AreEqual(html, HtmlTemizleyici.Temizle(html));
        }

        [TestMethod]
        public void Temizle_iframe_ve_style_icindeki_tehlikeli_cssi_kaldirir()
        {
            var sonuc = HtmlTemizleyici.Temizle("<iframe src=\"https://kotu.com\"></iframe><p style=\"background:url(javascript:alert(1))\">a</p>");
            Assert.IsFalse(sonuc.Contains("iframe"));
            Assert.IsFalse(sonuc.Contains("javascript"));
        }

        [TestMethod]
        public void Temizle_null_ve_bos_degeri_aynen_doner()
        {
            Assert.IsNull(HtmlTemizleyici.Temizle(null));
            Assert.AreEqual("", HtmlTemizleyici.Temizle(""));
        }
    }
}

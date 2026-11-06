using CorporateWebsite.Filters;
using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class SeoTests
    {
        private static string Metin(Microsoft.AspNetCore.Html.IHtmlContent icerik)
        {
            using (var w = new StringWriter())
            {
                icerik.WriteTo(w, HtmlEncoder.Default);
                return w.ToString();
            }
        }

        [TestMethod]
        public void JsonLd_icerikteki_script_kapanisi_blogu_kapatamaz()
        {
            Microsoft.AspNetCore.Html.IHtmlContent json0 = JsonLd.Yaz(new Dictionary<string, object> { { "headline", "Kötü </script><script>alert(1)</script> & \"tırnak\"" } });
            var json = Metin(json0);

            StringAssert.DoesNotMatch(json, new System.Text.RegularExpressions.Regex("</script", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
            Assert.IsFalse(json.Contains("<") || json.Contains(">") || json.Contains("&"), json);
            // Kaçışlanmış hâli yine aynı metne çözülür.
            var geri = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.AreEqual("Kötü </script><script>alert(1)</script> & \"tırnak\"", geri["headline"]);
            StringAssert.Contains(json, "Kötü", "Türkçe karakterler kaçışlanmadan yazılır");
        }

        [TestMethod]
        public void Sema_tarihi_saat_dilimi_farkiyla_ISO8601_yazar()
        {
            var t = new DateTime(2026, 11, 6, 10, 27, 41);
            var metin = Sema.Tarih(t);
            StringAssert.Matches(metin, new System.Text.RegularExpressions.Regex(@"^2026-11-06T10:27:41[+-]\d{2}:\d{2}$"));
            Assert.AreEqual(t, DateTimeOffset.Parse(metin).DateTime, "Saat değişmeden yazılır");
        }

        [TestMethod]
        public void Analitik_kimligi_yalnizca_GA4_bicimindeyse_kabul_edilir()
        {
            Assert.AreEqual("G-ABC123XYZ", GuvenlikBasliklariAttribute.AnalitikKimligiDogrula(" G-ABC123XYZ "));
            foreach (var gecersiz in new[] { null, "", "UA-12345-1", "G-abc'; script-src *", "G-<x>" })
            {
                Assert.IsNull(GuvenlikBasliklariAttribute.AnalitikKimligiDogrula(gecersiz), gecersiz);
            }
            Assert.IsNull(GuvenlikBasliklariAttribute.AnalitikKimligi, "Varsayılan ayar boş: analitik kapalı");
        }

        [TestMethod]
        public void CSP_Google_adreslerini_yalnizca_analitik_aciksa_icerir()
        {
            var kapali = GuvenlikBasliklariAttribute.SitePolitikasi(false);
            var acik = GuvenlikBasliklariAttribute.SitePolitikasi(true);

            Assert.IsFalse(kapali.Contains("google-analytics") || kapali.Contains("googletagmanager"));
            StringAssert.Contains(acik, "script-src 'self' https://www.googletagmanager.com;");
            StringAssert.Contains(acik, "https://*.google-analytics.com");
            StringAssert.DoesNotMatch(acik, new System.Text.RegularExpressions.Regex("script-src[^;]*unsafe-inline"), "Betiklerde inline izni olmamalı");
        }

        [TestMethod]
        public void CSP_localhost_baglantisi_yalnizca_gelistirmede_eklenir()
        {
            var canli = new[] { GuvenlikBasliklariAttribute.SitePolitikasi(false), GuvenlikBasliklariAttribute.SitePolitikasi(true), GuvenlikBasliklariAttribute.PanelPolitikasi };
            foreach (var p in canli)
            {
                Assert.IsFalse(p.Contains("localhost"), "Canlı politika localhost içermemeli: " + p);

                var gelistirme = GuvenlikBasliklariAttribute.GelistirmeyeUyarla(p);
                StringAssert.Contains(gelistirme, "connect-src 'self' http://localhost:* https://localhost:* ws://localhost:* wss://localhost:*");
                StringAssert.Contains(gelistirme, "script-src 'self'", "Betik politikası gevşetilmemeli");
            }
        }

        [TestMethod]
        public void Mutlak_adres_SiteAdresi_ayarini_kullanir()
        {
            var ctx = Sahte.HttpBaglami(yol: "/BlogPost");
            ctx.Request.Host = new Microsoft.AspNetCore.Http.HostString("ic-sunucu:8080");
            var istek = ctx.Request;
            Assert.AreEqual("http://ic-sunucu:8080/Hakkimizda", SeoAdres.Mutlak(istek, "/Hakkimizda", ""), "Ayar boşsa isteğin adresi");

            const string ayar = "https://www.ornek.com.tr/";
            Assert.AreEqual("https://www.ornek.com.tr/Hakkimizda", SeoAdres.Mutlak(istek, "/Hakkimizda", ayar));
            Assert.AreEqual("https://www.ornek.com.tr/Uploads/a.jpg", SeoAdres.Mutlak(istek, "~/Uploads/a.jpg", ayar));
            Assert.AreEqual("https://www.ornek.com.tr/a", SeoAdres.Mutlak(istek, "a", ayar));
            Assert.AreEqual("https://cdn.ornek.com/x.jpg", SeoAdres.Mutlak(istek, "https://cdn.ornek.com/x.jpg", ayar));
            Assert.IsNull(SeoAdres.Mutlak(istek, null, ayar));
        }
    }
}

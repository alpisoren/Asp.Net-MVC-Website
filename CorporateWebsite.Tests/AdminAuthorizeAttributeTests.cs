using CorporateWebsite.Filters;
using CorporateWebsite.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class AdminAuthorizeAttributeTests
    {
        /// <summary>Veritabanı yerine sözlükten okur ve kaç kez veritabanına gidildiğini sayar.</summary>
        private class TestEdilebilirFiltre : AdminAuthorizeAttribute
        {
            public readonly Dictionary<int, AktifYonetici> Yoneticiler = new Dictionary<int, AktifYonetici>();
            public int VeritabaniOkuma;

            /// <summary>Filtreyi çalıştırır; sonuç yoksa (null) erişim verilmiştir.</summary>
            public async Task<IActionResult> Calistir(HttpContext ctx)
            {
                var actionDescriptor = new ActionDescriptor { EndpointMetadata = new List<object>() };
                var baglam = new AuthorizationFilterContext(new ActionContext(ctx, new RouteData(), actionDescriptor), new List<IFilterMetadata>());
                await OnAuthorizationAsync(baglam);
                return baglam.Result;
            }

            public async Task<bool> Yetkili(HttpContext ctx)
            {
                return await Calistir(ctx) == null;
            }

            protected override AktifYonetici YoneticiBul(int adminId)
            {
                VeritabaniOkuma++;
                return Yoneticiler.TryGetValue(adminId, out var y) ? y : null;
            }
        }

        private static TestEdilebilirFiltre Filtre(bool sadeceAdmin = false)
        {
            var f = new TestEdilebilirFiltre { SadeceAdmin = sadeceAdmin };
            f.Yoneticiler[1] = new AktifYonetici { Id = 1, Eposta = "admin@test.local", Yetki = "Admin" };
            f.Yoneticiler[2] = new AktifYonetici { Id = 2, Eposta = "editor@test.local", Yetki = "Editor" };
            return f;
        }

        /// <summary>Çerezden okunan kimliği taklit eder; null = oturum yok.</summary>
        private static HttpContext Baglam(string kimlikDegeri, string yol = "/Blog", string yontem = "GET")
        {
            return Sahte.HttpBaglami(kimlikDegeri == null ? null : Sahte.Yonetici(kimlikDegeri), yol, yontem);
        }

        [TestMethod]
        public async Task Oturum_cerezi_yoksa_giris_sayfasina_donus_adresiyle_yonlendirilir()
        {
            var sonuc = await Filtre().Calistir(Baglam(null, "/Blog/Edit/5?x=1")) as RedirectResult;
            Assert.IsNotNull(sonuc);
            Assert.AreEqual("/yonetimpaneli/giris?returnUrl=%2FBlog%2FEdit%2F5%3Fx%3D1", sonuc.Url);
        }

        [TestMethod]
        public async Task POST_isteginde_donus_adresi_eklenmez()
        {
            var sonuc = await Filtre().Calistir(Baglam(null, "/Blog/Delete/5", "POST")) as RedirectResult;
            Assert.AreEqual("/yonetimpaneli/giris", sonuc?.Url);
        }

        [TestMethod]
        public async Task Bozuk_kimlik_verisi_reddedilir()
        {
            Assert.IsFalse(await Filtre().Yetkili(Baglam("abc")));
            Assert.IsFalse(await Filtre().Yetkili(Baglam("")));
            Assert.IsFalse(await Filtre().Yetkili(Baglam("-5")));
        }

        [TestMethod]
        public async Task Gecerli_oturumla_normal_sayfalara_erisilir_ve_yonetici_istege_eklenir()
        {
            var ctx = Baglam("2");
            Assert.IsTrue(await Filtre().Yetkili(ctx));
            Assert.AreEqual("editor@test.local", YoneticiOturumu.Aktif(ctx).Eposta);
        }

        [TestMethod]
        public async Task Silinmis_hesabin_cerezi_gecersizdir()
        {
            Assert.IsFalse(await Filtre().Yetkili(Baglam("99")));
        }

        [TestMethod]
        public async Task SadeceAdmin_sayfalarina_yetki_veritabanindan_okunarak_karar_verilir()
        {
            Assert.IsTrue(await Filtre(sadeceAdmin: true).Yetkili(Baglam("1")));
            var editor = await Filtre(sadeceAdmin: true).Calistir(Baglam("2")) as StatusCodeResult;
            Assert.AreEqual(StatusCodes.Status403Forbidden, editor?.StatusCode, "Giriş yapmış ama yetkisiz: 403");
        }

        [TestMethod]
        public async Task Yetki_degisikligi_mevcut_oturuma_aninda_yansir()
        {
            var f = Filtre(sadeceAdmin: true);
            Assert.IsTrue(await f.Yetkili(Baglam("1")));

            f.Yoneticiler[1].Yetki = "Editor"; // veritabanında yetki düşürüldü
            Assert.IsFalse(await f.Yetkili(Baglam("1")), "Çerez aynı kalsa da yeni istek güncel yetkiyi kullanmalı");
        }

        [TestMethod]
        public async Task Ayni_istekte_veritabanina_bir_kez_gidilir()
        {
            var f = Filtre();
            var ctx = Baglam("1");
            Assert.IsTrue(await f.Yetkili(ctx));
            Assert.IsTrue(await f.Yetkili(ctx)); // ör. sınıf + aksiyon özniteliği birlikte
            Assert.AreEqual(1, f.VeritabaniOkuma);
        }
    }
}

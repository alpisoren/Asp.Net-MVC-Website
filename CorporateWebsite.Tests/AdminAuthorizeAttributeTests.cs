using CorporateWebsite.Filters;
using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class AdminAuthorizeAttributeTests
    {
        /// <summary>
        /// AuthorizeCore korumalı olduğu için test amaçlı dışarı açılır; veritabanı yerine
        /// sözlükten okur ve kaç kez veritabanına gidildiğini sayar.
        /// </summary>
        private class TestEdilebilirFiltre : AdminAuthorizeAttribute
        {
            public readonly Dictionary<int, AktifYonetici> Yoneticiler = new Dictionary<int, AktifYonetici>();
            public int VeritabaniOkuma;

            public bool Yetkili(HttpContextBase ctx)
            {
                return AuthorizeCore(ctx);
            }

            protected override AktifYonetici YoneticiBul(int adminId)
            {
                VeritabaniOkuma++;
                AktifYonetici y;
                return Yoneticiler.TryGetValue(adminId, out y) ? y : null;
            }
        }

        private static TestEdilebilirFiltre Filtre(bool sadeceAdmin = false)
        {
            var f = new TestEdilebilirFiltre { SadeceAdmin = sadeceAdmin };
            f.Yoneticiler[1] = new AktifYonetici { Id = 1, Eposta = "admin@test.local", Yetki = "Admin" };
            f.Yoneticiler[2] = new AktifYonetici { Id = 2, Eposta = "editor@test.local", Yetki = "Editor" };
            return f;
        }

        /// <summary>Forms Authentication'ın cookie'den ürettiği kimliği taklit eder.</summary>
        private static SahteHttpContext Baglam(string userData)
        {
            var ctx = new SahteHttpContext(new SahteSession());
            if (userData != null)
            {
                var bilet = new FormsAuthenticationTicket(1, "kullanici", DateTime.Now, DateTime.Now.AddHours(1), false, userData);
                ctx.User = new GenericPrincipal(new FormsIdentity(bilet), new string[0]);
            }
            else
            {
                ctx.User = new GenericPrincipal(new GenericIdentity(""), new string[0]); // anonim
            }
            return ctx;
        }

        [TestMethod]
        public void Oturum_cookiesi_yoksa_erisim_reddedilir()
        {
            Assert.IsFalse(Filtre().Yetkili(Baglam(null)));
        }

        [TestMethod]
        public void Bozuk_bilet_verisi_reddedilir()
        {
            Assert.IsFalse(Filtre().Yetkili(Baglam("abc")));
            Assert.IsFalse(Filtre().Yetkili(Baglam("")));
            Assert.IsFalse(Filtre().Yetkili(Baglam("-5")));
        }

        [TestMethod]
        public void Gecerli_oturumla_normal_sayfalara_erisilir_ve_yonetici_istege_eklenir()
        {
            var ctx = Baglam("2");
            Assert.IsTrue(Filtre().Yetkili(ctx));
            Assert.AreEqual("editor@test.local", YoneticiOturumu.Aktif(ctx).Eposta);
        }

        [TestMethod]
        public void Silinmis_hesabin_cookiesi_gecersizdir()
        {
            Assert.IsFalse(Filtre().Yetkili(Baglam("99")));
        }

        [TestMethod]
        public void SadeceAdmin_sayfalarina_yetki_veritabanindan_okunarak_karar_verilir()
        {
            Assert.IsTrue(Filtre(sadeceAdmin: true).Yetkili(Baglam("1")));
            Assert.IsFalse(Filtre(sadeceAdmin: true).Yetkili(Baglam("2")));
        }

        [TestMethod]
        public void Yetki_degisikligi_mevcut_oturuma_aninda_yansir()
        {
            var f = Filtre(sadeceAdmin: true);
            Assert.IsTrue(f.Yetkili(Baglam("1")));

            f.Yoneticiler[1].Yetki = "Editor"; // veritabanında yetki düşürüldü
            Assert.IsFalse(f.Yetkili(Baglam("1")), "Cookie aynı kalsa da yeni istek güncel yetkiyi kullanmalı");
        }

        [TestMethod]
        public void Ayni_istekte_veritabanina_bir_kez_gidilir()
        {
            var f = Filtre();
            var ctx = Baglam("1");
            Assert.IsTrue(f.Yetkili(ctx));
            Assert.IsTrue(f.Yetkili(ctx)); // ör. layout'taki child action
            Assert.AreEqual(1, f.VeritabaniOkuma);
        }
    }
}

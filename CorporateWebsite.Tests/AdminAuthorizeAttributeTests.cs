using CorporateWebsite.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class AdminAuthorizeAttributeTests
    {
        /// <summary>AuthorizeCore korumalı olduğu için test amaçlı dışarı açılır.</summary>
        private class TestEdilebilirFiltre : AdminAuthorizeAttribute
        {
            public bool Yetkili(HttpContextBase ctx)
            {
                return AuthorizeCore(ctx);
            }
        }

        private static HttpContextBase Baglam(object adminId, string yetki)
        {
            var session = new SahteSession();
            session["adminid"] = adminId;
            session["yetki"] = yetki;
            return new SahteHttpContext(session);
        }

        [TestMethod]
        public void Oturum_yoksa_erisim_reddedilir()
        {
            Assert.IsFalse(new TestEdilebilirFiltre().Yetkili(Baglam(null, null)));
            Assert.IsFalse(new TestEdilebilirFiltre().Yetkili(new SahteHttpContext(null)), "Session'ı olmayan istek");
        }

        [TestMethod]
        public void Oturum_varsa_normal_sayfalara_erisilir()
        {
            Assert.IsTrue(new TestEdilebilirFiltre().Yetkili(Baglam(5, "Editor")));
        }

        [TestMethod]
        public void SadeceAdmin_sayfalarina_yalnizca_Admin_yetkisi_erisir()
        {
            var filtre = new TestEdilebilirFiltre { SadeceAdmin = true };

            Assert.IsTrue(filtre.Yetkili(Baglam(1, "Admin")));
            Assert.IsFalse(filtre.Yetkili(Baglam(2, "Editor")));
            Assert.IsFalse(filtre.Yetkili(Baglam(3, null)));
            Assert.IsFalse(filtre.Yetkili(Baglam(null, "Admin")), "Oturumsuz yetki değeri geçersiz");
        }
    }
}

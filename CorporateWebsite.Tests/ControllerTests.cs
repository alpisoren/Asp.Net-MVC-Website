using CorporateWebsite.Controllers;
using CorporateWebsite.Models.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace CorporateWebsite.Tests
{
    /// <summary>Panel controller'larının sonuç türleri (404, yönlendirme, açık yönlendirme koruması).</summary>
    [TestClass]
    [TestCategory("Veritabani")]
    public class ControllerTests
    {
        [ClassInitialize]
        public static void Baslat(TestContext _)
        {
            VeritabaniTestOrtami.Hazirla();
        }

        private static T Hazirla<T>(T controller) where T : Controller
        {
            var ctx = Sahte.HttpBaglami();
            var eylem = new ActionContext(ctx, new RouteData(), new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor());
            controller.ControllerContext = new ControllerContext(eylem);
            controller.Url = new UrlHelper(eylem);
            controller.TempData = new TempDataDictionary(ctx, new BellekTempDataSaglayici());
            return controller;
        }

        private static int YorumEkle()
        {
            using (var db = VeritabaniTestOrtami.YeniBaglam())
            {
                var b = new Blog { Baslik = "Controller testi " + Guid.NewGuid().ToString("N"), YayinTarihi = DateTime.Now };
                db.Blog.Add(b);
                db.SaveChanges();
                var y = new Yorum { AdSoyad = "Test", Icerik = "x", BlogId = b.BlogId };
                db.Yorum.Add(y);
                db.SaveChanges();
                return y.YorumId;
            }
        }

        [TestMethod]
        public void Olmayan_kayit_icin_silme_ve_duzenleme_404_doner()
        {
            using (var c = Hazirla(new KategoriController()))
            {
                Assert.IsInstanceOfType(c.DeleteConfirmed(int.MaxValue), typeof(NotFoundResult));
                Assert.IsInstanceOfType(c.Edit((int?)null), typeof(NotFoundResult));
                Assert.IsInstanceOfType(c.Edit(int.MaxValue), typeof(NotFoundResult));
            }
            using (var c = Hazirla(new YorumController()))
            {
                Assert.IsInstanceOfType(c.DeleteConfirmed(int.MaxValue), typeof(NotFoundResult));
                Assert.IsInstanceOfType(c.OnayDegistir(int.MaxValue), typeof(NotFoundResult));
            }
            using (var c = Hazirla(new IletisimController()))
            {
                Assert.IsInstanceOfType(c.DeleteConfirmed(int.MaxValue), typeof(NotFoundResult));
            }
        }

        [TestMethod]
        public void Yorum_onayinda_yalnizca_yerel_adrese_donulur()
        {
            var id = YorumEkle();
            using (var c = Hazirla(new YorumController()))
            {
                var yerel = c.OnayDegistir(id, "/yonetimpaneli") as RedirectResult;
                Assert.IsNotNull(yerel);
                Assert.AreEqual("/yonetimpaneli", yerel.Url);

                var dis = c.OnayDegistir(id, "https://kotu-site.example/");
                Assert.IsInstanceOfType(dis, typeof(RedirectToActionResult), "Dış adrese yönlendirme yapılmamalı");
                Assert.AreEqual("Index", ((RedirectToActionResult)dis).ActionName);

                var protokolsuz = c.OnayDegistir(id, "//kotu-site.example/");
                Assert.IsInstanceOfType(protokolsuz, typeof(RedirectToActionResult));
            }
        }

        [TestMethod]
        public void Kategori_gecersiz_formda_ayni_sayfa_hatayla_doner()
        {
            string ad;
            using (var db = VeritabaniTestOrtami.YeniBaglam())
            {
                ad = "Var olan " + Guid.NewGuid().ToString("N").Substring(0, 6);
                db.Kategori.Add(new Kategori { KategoriAd = ad });
                db.SaveChanges();
            }
            using (var c = Hazirla(new KategoriController()))
            {
                var sonuc = c.Create(new Kategori { KategoriAd = ad }) as ViewResult;
                Assert.IsNotNull(sonuc, "Hata varsa form yeniden gösterilir");
                Assert.IsFalse(c.ModelState.IsValid);
                Assert.IsTrue(c.ModelState["KategoriAd"].Errors.Any());
            }
        }
    }
}

using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace CorporateWebsite.Controllers
{
    /// <summary>
    /// Hata sayfaları. Gerçek durum kodunu döner (404 / 403 / 500): Google "200 dönen hata sayfası"nı (soft 404)
    /// içerik sanıp dizine almaya çalışır. Program.cs → UseStatusCodePagesWithReExecute ve UseExceptionHandler buraya yönlendirir.
    /// </summary>
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HataController : Controller
    {
        /// <summary>Durum kodu sayfaları (Program.cs: /Hata/Durum/{kod}).</summary>
        [Route("Hata/Durum/{kod:int}")]
        public ActionResult Durum(int kod)
        {
            if (kod == StatusCodes.Status404NotFound)
            {
                return Bulunamadi();
            }
            LayoutVerisi();
            ViewBag.Seo = new SeoBilgisi { Robots = "noindex, nofollow" };
            ViewBag.Kod = kod;
            ViewBag.Mesaj = kod == StatusCodes.Status403Forbidden
                ? "Bu sayfayı görüntüleme yetkiniz yok."
                : "İsteğiniz işlenemedi.";
            Response.StatusCode = kod;
            return View("Durum");
        }

        [Route("Hata/Bulunamadi")]
        public ActionResult Bulunamadi()
        {
            LayoutVerisi();
            ViewBag.Seo = new SeoBilgisi { Robots = "noindex, follow" };
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("Bulunamadi");
        }

        /// <summary>Yakalanmayan hata (yalnızca canlıda; geliştirmede ayrıntılı hata sayfası görünür).</summary>
        [Route("Hata/Sunucu")]
        public ActionResult Sunucu()
        {
            var hata = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (hata?.Error != null)
            {
                Trace.TraceError("[{0:u}] {1} -> {2}", DateTime.UtcNow, hata.Path, hata.Error);
            }
            // Veritabanı hatası olabileceği için burada site kimliği okunmaz.
            ViewBag.Seo = new SeoBilgisi { Robots = "noindex, nofollow" };
            ViewBag.Kod = StatusCodes.Status500InternalServerError;
            ViewBag.Mesaj = "Beklenmeyen bir hata oluştu. Lütfen biraz sonra tekrar deneyin.";
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return View("Durum");
        }

        /// <summary>Layout için site kimliği; veritabanına ulaşılamasa da sayfa çizilmeli.</summary>
        private void LayoutVerisi()
        {
            try
            {
                using (var db = new KurumsalDBContext())
                {
                    ViewBag.Kimlik = db.Kimlik.AsNoTracking().FirstOrDefault();
                    ViewBag.Iletisim = db.Iletisim.AsNoTracking().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Hata sayfası için site kimliği okunamadı: " + ex.Message);
            }
        }
    }
}

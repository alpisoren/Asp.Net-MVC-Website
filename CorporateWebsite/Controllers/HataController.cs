using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    /// <summary>
    /// Hata sayfaları. Gerçek durum kodunu döner (404 / 500): Google "200 dönen hata sayfası"nı (soft 404)
    /// içerik sanıp dizine almaya çalışır. Global.asax (Application_Error) ve Web.config (httpErrors) buraya yönlendirir.
    /// </summary>
    public class HataController : Controller
    {
        // Not: [Route] verilmez; Global.asax bu aksiyonu geleneksel rota verisiyle doğrudan çalıştırır.
        public ActionResult Bulunamadi()
        {
            // Layout için site kimliği; veritabanına ulaşılamasa da sayfa çizilmeli.
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
                Trace.TraceError("404 sayfası için site kimliği okunamadı: " + ex.Message);
            }
            ViewBag.Seo = new SeoBilgisi { Robots = "noindex, follow" };
            Response.StatusCode = 404;
            // Statik dosya işleyicisinden gelen 404'lerde içerik türü HTML olmayabilir.
            Response.ContentType = "text/html";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }
    }
}

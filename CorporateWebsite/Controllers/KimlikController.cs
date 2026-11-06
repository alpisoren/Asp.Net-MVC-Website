using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class KimlikController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private AyarServisi servis;

        /// <summary>İlk kullanımda oluşturulur (Server bu noktada hazırdır; testlerde de çalışır).</summary>
        private AyarServisi Servis
        {
            get { return servis ?? (servis = new AyarServisi(db)); }
        }

        public ActionResult Index()
        {
            return View(Servis.KimlikListele());
        }

        public ActionResult Edit(int? id)
        {
            var kimlik = id.HasValue ? Servis.KimlikBul(id.Value) : null;
            return kimlik == null ? (ActionResult)NotFound() : View(kimlik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Title,Keywords,Description,Unvan")] Kimlik kimlik, IFormFile LogoURL)
        {
            if (ModelState.IsValid)
            {
                var sonuc = Servis.KimlikGuncelle(id, kimlik, LogoURL);
                if (sonuc == null)
                {
                    return NotFound();
                }
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Index");
                }
                sonuc.ModelStateAktar(ModelState);
            }
            kimlik.KimlikId = id;
            kimlik.LogoUrl = Servis.KimlikBul(id)?.LogoUrl;
            return View(kimlik);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

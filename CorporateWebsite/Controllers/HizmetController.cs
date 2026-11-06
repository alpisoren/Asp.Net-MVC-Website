using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.ViewModel;
using CorporateWebsite.Services;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize]
    public class HizmetController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private HizmetServisi servis;

        /// <summary>İlk kullanımda oluşturulur (Server bu noktada hazırdır; testlerde de çalışır).</summary>
        private HizmetServisi Servis
        {
            get { return servis ?? (servis = new HizmetServisi(db)); }
        }

        public ActionResult Index()
        {
            return View(Servis.Listele());
        }

        public ActionResult Create()
        {
            return View(new HizmetFormModeli());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HizmetFormModeli model, IFormFile ResimUrl)
        {
            if (ModelState.IsValid)
            {
                var sonuc = Servis.Olustur(model, ResimUrl);
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Index");
                }
                sonuc.ModelStateAktar(ModelState);
            }
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            var model = id.HasValue ? Servis.FormModeliGetir(id.Value) : null;
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, HizmetFormModeli model, IFormFile ResimUrl)
        {
            if (ModelState.IsValid)
            {
                var sonuc = Servis.Guncelle(id, model, ResimUrl);
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

            var mevcut = Servis.FormModeliGetir(id);
            if (mevcut == null)
            {
                return NotFound();
            }
            model.HizmetId = id;
            model.ResimUrl = mevcut.ResimUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!Servis.Sil(id))
            {
                return NotFound();
            }
            return RedirectToAction("Index");
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

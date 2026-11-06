using Microsoft.AspNetCore.Mvc;
using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize]
    public class KategoriController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private readonly KategoriServisi servis;

        public KategoriController()
        {
            servis = new KategoriServisi(db);
        }

        public ActionResult Index()
        {
            return View(servis.Listele());
        }

        public ActionResult Details(int? id)
        {
            var kategori = id.HasValue ? servis.Bul(id.Value) : null;
            return kategori == null ? (ActionResult)NotFound() : View(kategori);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("KategoriAd,Aciklama")] Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                var sonuc = servis.Olustur(kategori);
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Index");
                }
                sonuc.ModelStateAktar(ModelState);
            }
            return View(kategori);
        }

        public ActionResult Edit(int? id)
        {
            var kategori = id.HasValue ? servis.Bul(id.Value) : null;
            return kategori == null ? (ActionResult)NotFound() : View(kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("KategoriAd,Aciklama")] Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                var sonuc = servis.Guncelle(id, kategori);
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
            kategori.KategoriId = id;
            return View(kategori);
        }

        public ActionResult Delete(int? id)
        {
            var kategori = id.HasValue ? servis.Bul(id.Value) : null;
            return kategori == null ? (ActionResult)NotFound() : View(kategori);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var ad = servis.Sil(id);
            if (ad == null)
            {
                return NotFound();
            }
            TempData["Basari"] = "\"" + ad + "\" kategorisi silindi.";
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

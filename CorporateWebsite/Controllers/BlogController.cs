using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.ViewModel;
using CorporateWebsite.Services;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize]
    public class BlogController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private BlogServisi servis;

        /// <summary>İlk kullanımda oluşturulur (Server bu noktada hazırdır; testlerde de çalışır).</summary>
        private BlogServisi Servis
        {
            get { return servis ?? (servis = new BlogServisi(db)); }
        }

        public ActionResult Index()
        {
            return View(Servis.Listele());
        }

        public ActionResult Create()
        {
            KategorileriYukle(null);
            return View(new BlogFormModeli { YayinTarihi = System.DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlogFormModeli model, IFormFile ResimURL)
        {
            if (ModelState.IsValid)
            {
                var sonuc = Servis.Olustur(model, ResimURL);
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Index");
                }
                sonuc.ModelStateAktar(ModelState);
            }

            KategorileriYukle(model.KategoriId);
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            var model = id.HasValue ? Servis.FormModeliGetir(id.Value) : null;
            if (model == null)
            {
                return NotFound();
            }
            KategorileriYukle(model.KategoriId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, BlogFormModeli model, IFormFile ResimURL)
        {
            if (ModelState.IsValid)
            {
                var sonuc = Servis.Guncelle(id, model, ResimURL);
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

            // Formu yeniden gösterirken mevcut resmi kaybetme.
            var mevcut = Servis.FormModeliGetir(id);
            if (mevcut == null)
            {
                return NotFound();
            }
            model.BlogId = id;
            model.ResimURL = mevcut.ResimURL;
            KategorileriYukle(model.KategoriId);
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var b = Servis.Bul(id);
            if (b == null)
            {
                return NotFound();
            }
            return View(b);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Servis.Sil(id))
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        private void KategorileriYukle(int? secili)
        {
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAd", secili);
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

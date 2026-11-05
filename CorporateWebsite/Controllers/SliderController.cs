﻿using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;
using System.Web;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class SliderController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private SliderServisi servis;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            servis = new SliderServisi(db, Server);
        }

        public ActionResult Index()
        {
            return View(servis.Listele());
        }

        public ActionResult Details(int? id)
        {
            var slider = id.HasValue ? servis.Bul(id.Value) : null;
            return slider == null ? (ActionResult)HttpNotFound() : View(slider);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Baslik,Aciklama")] Slider slider, HttpPostedFileBase ResimURL)
        {
            if (ModelState.IsValid)
            {
                var sonuc = servis.Olustur(slider, ResimURL);
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Index");
                }
                sonuc.ModelStateAktar(ModelState);
            }
            return View(slider);
        }

        public ActionResult Edit(int? id)
        {
            var slider = id.HasValue ? servis.Bul(id.Value) : null;
            return slider == null ? (ActionResult)HttpNotFound() : View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "Baslik,Aciklama")] Slider slider, HttpPostedFileBase ResimURL)
        {
            if (ModelState.IsValid)
            {
                var sonuc = servis.Guncelle(id, slider, ResimURL);
                if (sonuc == null)
                {
                    return HttpNotFound();
                }
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Index");
                }
                sonuc.ModelStateAktar(ModelState);
            }
            // Form yeniden gösterilirken mevcut resim önizlemesi kaybolmasın
            slider.SliderId = id;
            slider.ResimURL = servis.Bul(id)?.ResimURL;
            return View(slider);
        }

        public ActionResult Delete(int? id)
        {
            var slider = id.HasValue ? servis.Bul(id.Value) : null;
            return slider == null ? (ActionResult)HttpNotFound() : View(slider);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!servis.Sil(id))
            {
                return HttpNotFound();
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

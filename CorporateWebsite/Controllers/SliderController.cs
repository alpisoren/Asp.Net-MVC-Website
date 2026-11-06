﻿using CorporateWebsite.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class SliderController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private SliderServisi servis;

        /// <summary>İlk kullanımda oluşturulur (Server bu noktada hazırdır; testlerde de çalışır).</summary>
        private SliderServisi Servis
        {
            get { return servis ?? (servis = new SliderServisi(db)); }
        }

        public ActionResult Index()
        {
            return View(Servis.Listele());
        }

        public ActionResult Details(int? id)
        {
            var slider = id.HasValue ? Servis.Bul(id.Value) : null;
            return slider == null ? (ActionResult)NotFound() : View(slider);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Baslik,Aciklama")] Slider slider, IFormFile ResimURL)
        {
            if (ModelState.IsValid)
            {
                var sonuc = Servis.Olustur(slider, ResimURL);
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
            var slider = id.HasValue ? Servis.Bul(id.Value) : null;
            return slider == null ? (ActionResult)NotFound() : View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Baslik,Aciklama")] Slider slider, IFormFile ResimURL)
        {
            if (ModelState.IsValid)
            {
                var sonuc = Servis.Guncelle(id, slider, ResimURL);
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
            // Form yeniden gösterilirken mevcut resim önizlemesi kaybolmasın
            slider.SliderId = id;
            slider.ResimURL = Servis.Bul(id)?.ResimURL;
            return View(slider);
        }

        public ActionResult Delete(int? id)
        {
            var slider = id.HasValue ? Servis.Bul(id.Value) : null;
            return slider == null ? (ActionResult)NotFound() : View(slider);
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

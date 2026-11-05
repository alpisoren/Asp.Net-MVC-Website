using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;

using CorporateWebsite.Filters;
using CorporateWebsite.Helper;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class HakkimizdaController : Controller
    {
        KurumsalDBContext db = new KurumsalDBContext();

        // GET: Hakkimizda
        public ActionResult Index()
        {
            var h = db.Hakkimizda.ToList();
            return View(h);

        }

        public ActionResult Edit(int id)
        {
            var h = db.Hakkimizda.Where(x => x.HakkimizdaId == id).FirstOrDefault();
            return View(h);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
       public ActionResult Edit(int id, Hakkimizda hakkimizda)
        {
            if (ModelState.IsValid)
            {
                var h = db.Hakkimizda.Where(x => x.HakkimizdaId == id).SingleOrDefault();
                if (h == null)
                {
                    return HttpNotFound();
                }
                h.Aciklama = HtmlTemizleyici.Temizle(hakkimizda.Aciklama);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hakkimizda);


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
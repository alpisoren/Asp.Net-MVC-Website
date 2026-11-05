using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

using CorporateWebsite.Filters;
using CorporateWebsite.Helper;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class KimlikController : Controller
    {
        KurumsalDBContext db = new KurumsalDBContext();
        // GET: Kimlik
        public ActionResult Index()
        {
            return View(db.Kimlik.ToList());
        }

        // GET: Kimlik/Details/5
       
        // GET: Kimlik/Edit/5
        public ActionResult Edit(int id)
        {

            var kimlik = db.Kimlik.Where(x => x.KimlikId == id).SingleOrDefault();
            if (kimlik == null)
            {
                return HttpNotFound();
            }
            return View(kimlik);
        }

        // POST: Kimlik/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int id, [Bind(Exclude = "LogoUrl")] Kimlik kimlik, HttpPostedFileBase LogoURL)
        {
            if (ModelState.IsValid)
            {
                var kimlikinfo = db.Kimlik.Where(x => x.KimlikId == id).SingleOrDefault();
                if (kimlikinfo == null)
                {
                    return HttpNotFound();
                }
                if (LogoURL != null)
                {
                    string hata;
                    var url = ResimHelper.Kaydet(LogoURL, "Kimlik", 300, 300, out hata);
                    if (url == null)
                    {
                        ModelState.AddModelError("", hata);
                        kimlik.KimlikId = id;
                        kimlik.LogoUrl = kimlikinfo.LogoUrl;
                        return View(kimlik);
                    }
                    ResimHelper.Sil(Server, kimlikinfo.LogoUrl);
                    kimlikinfo.LogoUrl = url;
                }
                kimlikinfo.Title = kimlik.Title;
                kimlikinfo.Keywords = kimlik.Keywords;
                kimlikinfo.Description = kimlik.Description;
                kimlikinfo.Unvan = kimlik.Unvan;

                db.SaveChanges();
                return RedirectToAction("Index");

            }
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

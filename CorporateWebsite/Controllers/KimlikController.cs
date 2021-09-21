using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
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

            var kimlik = db.Kimlik.Where(x => x.Kimlikd == id).SingleOrDefault();
            return View(kimlik);
        }

        // POST: Kimlik/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Kimlik kimlik,HttpPostedFileBase LogoURL)
        {
            if (ModelState.IsValid)
            {
                var kimlikinfo = db.Kimlik.Where(x => x.Kimlikd == id).SingleOrDefault();
                if (LogoURL!=null)
                {
                    if (System.IO.File.Exists(Server.MapPath(kimlikinfo.LogoUrl)))
                    {
                        System.IO.File.Delete(Server.MapPath(kimlikinfo.LogoUrl));
                    }
                    WebImage img = new WebImage(LogoURL.InputStream);
                    FileInfo imginfo = new FileInfo(LogoURL.FileName);

                    string logoname = LogoURL.FileName+imginfo.Extension;
                    img.Resize(300, 300);
                    img.Save("~/Uploads/Kimlik/" + logoname);
                    kimlikinfo.LogoUrl = "/Uploads/Kimlik/" + logoname;
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

       
    }
}

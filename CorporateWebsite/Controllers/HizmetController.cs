using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    public class HizmetController : Controller
    {
        // GET: Hizmet

        private KurumsalDBContext db = new KurumsalDBContext();
        public ActionResult Index()
        {
            return View(db.Hizmet.ToList());
        }

        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Hizmet hizmet,HttpPostedFileBase ResimUrl)
        {
            if (ModelState.IsValid)
            {

                if (ResimUrl != null)
                {
                   
                    WebImage img = new WebImage(ResimUrl.InputStream);
                    FileInfo imginfo = new FileInfo(ResimUrl.FileName);

                    string logoname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(500, 500);
                    img.Save("~/Uploads/Hizmet/" + logoname);
                    hizmet.ResimUrl = "/Uploads/Hizmet/" + logoname;
                }
                db.Hizmet.Add(hizmet);
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(hizmet);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.Uyari = "Güncellenecek Veri bulunamadı.";
            }
            var hizmet = db.Hizmet.Find(id);
            if (hizmet == null)
            {
                return HttpNotFound();
            }
            return View(hizmet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int? id,Hizmet hizmet,HttpPostedFileBase ResimUrl)
        {

          
            //var h = db.Hizmet.Find(id);
            
            if (ModelState.IsValid)
            {
                var h = db.Hizmet.Where(x=>x.HizmetId==id).SingleOrDefault();

                if (ResimUrl != null)
                {
                    
                        if (System.IO.File.Exists(Server.MapPath(h.ResimUrl)))
                        {
                            System.IO.File.Delete(Server.MapPath(h.ResimUrl));
                        }
                        WebImage img = new WebImage(ResimUrl.InputStream);
                        FileInfo imginfo = new FileInfo(ResimUrl.FileName);

                        string hizmetName = Guid.NewGuid().ToString() + imginfo.Extension;
                        img.Resize(300, 300);
                        img.Save("~/Uploads/Hizmet/" + hizmetName);
                        h.ResimUrl = "/Uploads/Hizmet/" + hizmetName;
                    
                }
                h.Baslik = hizmet.Baslik;
                h.Aciklama = hizmet.Aciklama;
               // db.Entry(hizmet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return null;
            
        }

        public ActionResult Delete(int id)
        {

            if (id==null)
            {
                return HttpNotFound();
            }
            var h = db.Hizmet.Find(id);
            if (h==null)
            {
                return HttpNotFound();
            }
            db.Hizmet.Remove(h);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
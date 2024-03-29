﻿using CorporateWebsite.Models.DataContext;
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
    public class BlogController : Controller
    {

        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Blog

        public ActionResult Index()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return View(db.Blog.Include("Kategori").ToList().OrderByDescending(x=>x.BlogId));
        }

        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId","KategoriAd");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog, HttpPostedFileBase ResimURL)
        {
            if (ModelState.IsValid)
            {

                if (ResimURL != null)
                {

                    WebImage img = new WebImage(ResimURL.InputStream);
                    FileInfo imginfo = new FileInfo(ResimURL.FileName);

                    string blogName = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 400);
                    img.Save("~/Uploads/Blog/" + blogName);
                    blog.ResimURL = "/Uploads/Blog/" + blogName;
                }
                db.Blog.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(blog);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var blog = db.Blog.Where(x=>x.BlogId==id).SingleOrDefault();
            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAd", blog.KategoriId);
            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int? id, Blog blog, HttpPostedFileBase ResimURL)
        {


            if (ModelState.IsValid)
            {
                var h = db.Blog.Where(x => x.BlogId == id).SingleOrDefault();

                if (ResimURL != null)
                {

                    if (System.IO.File.Exists(Server.MapPath(h.ResimURL)))
                    {
                        System.IO.File.Delete(Server.MapPath(h.ResimURL));
                    }
                    WebImage img = new WebImage(ResimURL.InputStream);
                    FileInfo imginfo = new FileInfo(ResimURL.FileName);

                    string blogName = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 400);
                    img.Save("~/Uploads/Blog/" + blogName);
                    h.ResimURL = "/Uploads/Blog/" + blogName;

                }
                h.Baslik = blog.Baslik;
                h.Icerik = blog.Icerik;
                h.KategoriId = blog.KategoriId;
                // db.Entry(hizmet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);

        }

        public ActionResult Delete(int id)
        {

            //if (id == null)
            //{
            //    return HttpNotFound();
            //}
            var b = db.Blog.Find(id);
            if (b == null)
            {
                return HttpNotFound();
            }

            //if (System.IO.File.Exists(Server.MapPath(b.ResimURL)))
            //{
            //    System.IO.File.Delete(Server.MapPath(b.ResimURL));
            //}

            //db.Blog.Remove(b);
            //db.SaveChanges();

            return View(b);
        }

        // POST: Kategori/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Blog blog = db.Blog.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            if (System.IO.File.Exists(Server.MapPath(blog.ResimURL)))
            {
                System.IO.File.Delete(Server.MapPath(blog.ResimURL));
            }

            db.Blog.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

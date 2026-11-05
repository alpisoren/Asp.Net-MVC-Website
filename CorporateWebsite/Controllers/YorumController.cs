using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;

using CorporateWebsite.Filters;
using CorporateWebsite.Helper;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize]
    public class YorumController : Controller
    {
        private KurumsalDBContext db = new KurumsalDBContext();

        // GET: Yorum
        public ActionResult Index()
        {
            // Önce onay bekleyenler, sonra en yeniler
            var yorum = db.Yorum.Include(y => y.Blog).OrderBy(y => y.Onayli).ThenByDescending(y => y.YorumId);
            return View(yorum.ToList());
        }

        /// <summary>Listeden tek tıkla onaylama / onayı kaldırma.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnayDegistir(int id, string donus = null)
        {
            var yorum = db.Yorum.Find(id);
            if (yorum == null)
            {
                return HttpNotFound();
            }
            yorum.Onayli = !yorum.Onayli;
            db.SaveChanges();
            TempData["Basari"] = yorum.Onayli ? "Yorum onaylandı ve sitede yayında." : "Yorumun onayı kaldırıldı.";

            if (!string.IsNullOrEmpty(donus) && Url.IsLocalUrl(donus))
            {
                return Redirect(donus);
            }
            return RedirectToAction("Index");
        }

        // GET: Yorum/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yorum yorum = db.Yorum.Find(id);
            if (yorum == null)
            {
                return HttpNotFound();
            }
            return View(yorum);
        }

        // GET: Yorum/Create
        public ActionResult Create()
        {
            ViewBag.BlogId = new SelectList(db.Blog, "BlogId", "Baslik");
            return View();
        }

        // POST: Yorum/Create
        // Aşırı gönderim saldırılarından korunmak için bağlamak istediğiniz belirli özellikleri etkinleştirin. 
        // Daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "YorumId,AdSoyad,Icerik,Eposta,Onayli,BlogId")] Yorum yorum)
        {
            if (ModelState.IsValid)
            {
                db.Yorum.Add(yorum);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BlogId = new SelectList(db.Blog, "BlogId", "Baslik", yorum.BlogId);
            return View(yorum);
        }

        // GET: Yorum/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yorum yorum = db.Yorum.Find(id);
            if (yorum == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogId = new SelectList(db.Blog, "BlogId", "Baslik", yorum.BlogId);
            return View(yorum);
        }

        // POST: Yorum/Edit/5
        // Aşırı gönderim saldırılarından korunmak için bağlamak istediğiniz belirli özellikleri etkinleştirin. 
        // Daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "YorumId,AdSoyad,Icerik,Eposta,Onayli,BlogId")] Yorum yorum)
        {
            if (ModelState.IsValid)
            {
                db.Entry(yorum).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BlogId = new SelectList(db.Blog, "BlogId", "Baslik", yorum.BlogId);
            return View(yorum);
        }

        // GET: Yorum/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yorum yorum = db.Yorum.Find(id);
            if (yorum == null)
            {
                return HttpNotFound();
            }
            return View(yorum);
        }

        // POST: Yorum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Yorum yorum = db.Yorum.Find(id);
            db.Yorum.Remove(yorum);
            db.SaveChanges();
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

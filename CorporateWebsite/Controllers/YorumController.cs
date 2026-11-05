using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize]
    public class YorumController : Controller
    {
        private const string Alanlar = "AdSoyad,Icerik,Eposta,Onayli,BlogId";

        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private readonly YorumServisi servis;

        public YorumController()
        {
            servis = new YorumServisi(db);
        }

        /// <summary>Onay bekleyenler önce, sonra en yeniler.</summary>
        public ActionResult Index()
        {
            return View(servis.Listele());
        }

        /// <summary>Onayı tek tıkla aç/kapat. "donus" yerel bir adresse oraya geri döner (ör. panel ana sayfası).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnayDegistir(int id, string donus = null)
        {
            var onayli = servis.OnayDegistir(id);
            if (onayli == null)
            {
                return HttpNotFound();
            }
            TempData["Basari"] = onayli.Value ? "Yorum onaylandı ve sitede yayında." : "Yorumun onayı kaldırıldı.";

            if (!string.IsNullOrEmpty(donus) && Url.IsLocalUrl(donus))
            {
                return Redirect(donus);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            var yorum = id.HasValue ? servis.Bul(id.Value) : null;
            return yorum == null ? (ActionResult)HttpNotFound() : View(yorum);
        }

        public ActionResult Create()
        {
            ViewBag.BlogId = servis.BlogSecimListesi(null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // Yorum metni her yerde kodlanarak (encode) basılır; "<" içeren yorumlar düzenlenebilsin
        public ActionResult Create([Bind(Include = Alanlar)] Yorum yorum)
        {
            if (ModelState.IsValid)
            {
                servis.Olustur(yorum);
                return RedirectToAction("Index");
            }
            ViewBag.BlogId = servis.BlogSecimListesi(yorum.BlogId);
            return View(yorum);
        }

        public ActionResult Edit(int? id)
        {
            var yorum = id.HasValue ? servis.Bul(id.Value) : null;
            if (yorum == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogId = servis.BlogSecimListesi(yorum.BlogId);
            return View(yorum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // Yorum metni her yerde kodlanarak (encode) basılır; "<" içeren yorumlar düzenlenebilsin
        public ActionResult Edit(int id, [Bind(Include = Alanlar)] Yorum yorum)
        {
            if (ModelState.IsValid)
            {
                if (!servis.Guncelle(id, yorum))
                {
                    return HttpNotFound();
                }
                return RedirectToAction("Index");
            }
            yorum.YorumId = id;
            ViewBag.BlogId = servis.BlogSecimListesi(yorum.BlogId);
            return View(yorum);
        }

        public ActionResult Delete(int? id)
        {
            var yorum = id.HasValue ? servis.Bul(id.Value) : null;
            return yorum == null ? (ActionResult)HttpNotFound() : View(yorum);
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

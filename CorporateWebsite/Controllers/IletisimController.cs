using Microsoft.AspNetCore.Mvc;
using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class IletisimController : Controller
    {
        private const string Alanlar = "Adres,Telefon,Eposta,Whatsapp,Facebook,Twitter,Instagram";

        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private AyarServisi servis;

        /// <summary>İlk kullanımda oluşturulur (Server bu noktada hazırdır; testlerde de çalışır).</summary>
        private AyarServisi Servis
        {
            get { return servis ?? (servis = new AyarServisi(db)); }
        }

        // "/Iletisim" sitenin ziyaretçi sayfasıdır (HomeController); panel listesi bu yüzden açık adla yönlendirilir.
        [Route("Iletisim/Index")]
        public ActionResult Index()
        {
            return View(Servis.IletisimListele());
        }

        public ActionResult Details(int? id)
        {
            var iletisim = id.HasValue ? Servis.IletisimBul(id.Value) : null;
            return iletisim == null ? (ActionResult)NotFound() : View(iletisim);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Alanlar)] Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                Servis.IletisimOlustur(iletisim);
                return RedirectToAction("Index");
            }
            return View(iletisim);
        }

        public ActionResult Edit(int? id)
        {
            var iletisim = id.HasValue ? Servis.IletisimBul(id.Value) : null;
            return iletisim == null ? (ActionResult)NotFound() : View(iletisim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Alanlar)] Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                if (!Servis.IletisimGuncelle(id, iletisim))
                {
                    return NotFound();
                }
                return RedirectToAction("Index");
            }
            iletisim.IletisimId = id;
            return View(iletisim);
        }

        public ActionResult Delete(int? id)
        {
            var iletisim = id.HasValue ? Servis.IletisimBul(id.Value) : null;
            return iletisim == null ? (ActionResult)NotFound() : View(iletisim);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Servis.IletisimSil(id))
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

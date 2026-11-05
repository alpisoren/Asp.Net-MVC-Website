using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class IletisimController : Controller
    {
        private const string Alanlar = "Adres,Telefon,Fax,Whatsapp,Facebook,Twitter,Instagram";

        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private AyarServisi servis;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            servis = new AyarServisi(db, Server);
        }

        // "/Iletisim" sitenin ziyaretçi sayfasıdır (HomeController); panel listesi bu yüzden açık adla yönlendirilir.
        [Route("Iletisim/Index")]
        public ActionResult Index()
        {
            return View(servis.IletisimListele());
        }

        public ActionResult Details(int? id)
        {
            var iletisim = id.HasValue ? servis.IletisimBul(id.Value) : null;
            return iletisim == null ? (ActionResult)HttpNotFound() : View(iletisim);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = Alanlar)] Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                servis.IletisimOlustur(iletisim);
                return RedirectToAction("Index");
            }
            return View(iletisim);
        }

        public ActionResult Edit(int? id)
        {
            var iletisim = id.HasValue ? servis.IletisimBul(id.Value) : null;
            return iletisim == null ? (ActionResult)HttpNotFound() : View(iletisim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = Alanlar)] Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                if (!servis.IletisimGuncelle(id, iletisim))
                {
                    return HttpNotFound();
                }
                return RedirectToAction("Index");
            }
            iletisim.IletisimId = id;
            return View(iletisim);
        }

        public ActionResult Delete(int? id)
        {
            var iletisim = id.HasValue ? servis.IletisimBul(id.Value) : null;
            return iletisim == null ? (ActionResult)HttpNotFound() : View(iletisim);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!servis.IletisimSil(id))
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

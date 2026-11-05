using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;
using System.Web;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class KimlikController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private AyarServisi servis;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            servis = new AyarServisi(db, Server);
        }

        public ActionResult Index()
        {
            return View(servis.KimlikListele());
        }

        public ActionResult Edit(int? id)
        {
            var kimlik = id.HasValue ? servis.KimlikBul(id.Value) : null;
            return kimlik == null ? (ActionResult)HttpNotFound() : View(kimlik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "Title,Keywords,Description,Unvan")] Kimlik kimlik, HttpPostedFileBase LogoURL)
        {
            if (ModelState.IsValid)
            {
                var sonuc = servis.KimlikGuncelle(id, kimlik, LogoURL);
                if (sonuc == null)
                {
                    return HttpNotFound();
                }
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Index");
                }
                sonuc.ModelStateAktar(ModelState);
            }
            kimlik.KimlikId = id;
            kimlik.LogoUrl = servis.KimlikBul(id)?.LogoUrl;
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

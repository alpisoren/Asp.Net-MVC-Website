using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Services;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize(SadeceAdmin = true)]
    public class HakkimizdaController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private AyarServisi servis;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            servis = new AyarServisi(db, Server);
        }

        // "/Hakkimizda" sitenin ziyaretçi sayfasıdır (HomeController); panel listesi bu yüzden açık adla yönlendirilir.
        [Route("Hakkimizda/Index")]
        public ActionResult Index()
        {
            return View(servis.HakkimizdaListele());
        }

        public ActionResult Edit(int? id)
        {
            var h = id.HasValue ? servis.HakkimizdaBul(id.Value) : null;
            return h == null ? (ActionResult)HttpNotFound() : View(h);
        }

        /// <summary>Zengin metin (HTML) gönderilir; sunucuda HtmlTemizleyici'den geçirilerek kaydedilir.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int id, [Bind(Include = "Aciklama")] Hakkimizda hakkimizda)
        {
            if (ModelState.IsValid)
            {
                if (!servis.HakkimizdaGuncelle(id, hakkimizda.Aciklama))
                {
                    return HttpNotFound();
                }
                return RedirectToAction("Index");
            }
            hakkimizda.HakkimizdaId = id;
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

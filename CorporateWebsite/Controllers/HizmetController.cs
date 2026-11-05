using CorporateWebsite.Filters;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.ViewModel;
using CorporateWebsite.Services;
using System.Web;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize]
    public class HizmetController : Controller
    {
        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private HizmetServisi servis;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            servis = new HizmetServisi(db, Server);
        }

        public ActionResult Index()
        {
            return View(servis.Listele());
        }

        public ActionResult Create()
        {
            return View(new HizmetFormModeli());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HizmetFormModeli model, HttpPostedFileBase ResimUrl)
        {
            if (ModelState.IsValid)
            {
                var sonuc = servis.Olustur(model, ResimUrl);
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Index");
                }
                sonuc.ModelStateAktar(ModelState);
            }
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            var model = id.HasValue ? servis.FormModeliGetir(id.Value) : null;
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, HizmetFormModeli model, HttpPostedFileBase ResimUrl)
        {
            if (ModelState.IsValid)
            {
                var sonuc = servis.Guncelle(id, model, ResimUrl);
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

            var mevcut = servis.FormModeliGetir(id);
            if (mevcut == null)
            {
                return HttpNotFound();
            }
            model.HizmetId = id;
            model.ResimUrl = mevcut.ResimUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
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

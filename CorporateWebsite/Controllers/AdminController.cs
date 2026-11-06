using CorporateWebsite.Filters;
using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Models.ViewModel;
using CorporateWebsite.Services;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private const string SifirlamaMesaji = "Bu e-posta adresi kayıtlıysa şifre sıfırlama bağlantısı gönderildi. Bağlantı 1 saat geçerlidir.";
        private const string GecersizBaglanti = "Bağlantı geçersiz ya da süresi dolmuş. Lütfen yeniden şifre sıfırlama isteyin.";

        private readonly KurumsalDBContext db = new KurumsalDBContext();
        private readonly YoneticiServisi servis;

        public AdminController()
        {
            servis = new YoneticiServisi(db);
        }

        private int AktifAdminId
        {
            get { return YoneticiOturumu.Aktif(HttpContext)?.Id ?? 0; }
        }

        [Route("yonetimpaneli")]
        public ActionResult Index()
        {
            ViewBag.BlogSay = db.Blog.Count();
            ViewBag.KategoriSay = db.Kategori.Count();
            ViewBag.HizmetSay = db.Hizmet.Count();
            ViewBag.YorumSay = db.Yorum.Count();
            ViewBag.BekleyenYorumlar = db.Yorum.AsNoTracking().Include(y => y.Blog)
                .Where(y => !y.Onayli).OrderByDescending(y => y.YorumId).Take(5).ToList();
            return View(db.Kategori.AsNoTracking().ToList());
        }

        /// <summary>Layout'taki bildirim rozeti için bekleyen (onaysız) yorum sayısı.</summary>
        [ChildActionOnly]
        public ActionResult BekleyenYorumSayisi()
        {
            return Content(servis.BekleyenYorumSayisi().ToString());
        }

        // ---------------------------------------------------------------- Giriş / çıkış

        [AllowAnonymous]
        [Route("yonetimpaneli/giris")]
        public ActionResult Login(string returnUrl = null)
        {
            // Zaten giriş yapılmışsa panele geç
            if (YoneticiOturumu.KimlikId(User) != null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string eposta, string sifre, bool beniHatirla = false, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            Admin admin;
            switch (servis.GirisYap(eposta, sifre, Request.UserHostAddress, out admin))
            {
                case GirisDurumu.Basarili:
                    // Şifreli, HttpOnly oturum cookie'si; uygulama yeniden başlasa da geçerli kalır.
                    Session.Clear();
                    YoneticiOturumu.GirisYap(Response, admin.AdminId, admin.Eposta, beniHatirla);
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Admin");

                case GirisDurumu.GeciciOlarakEngellendi:
                    ViewBag.Uyari = "Çok fazla hatalı giriş denemesi. Lütfen " + (int)YoneticiServisi.GirisEngelSuresi.TotalMinutes + " dakika sonra tekrar deneyin.";
                    return View();

                default:
                    ViewBag.Uyari = "Kullanıcı adı ya da şifre yanlış";
                    return View();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            YoneticiOturumu.CikisYap();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
        }

        // ---------------------------------------------------------------- Şifre sıfırlama

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string eposta)
        {
            servis.SifirlamaBaslat(eposta, token => Url.Action("YeniSifre", "Admin", new { token = token }, Request.Url.Scheme));
            // Hesabın var olup olmadığını belli etmemek için her durumda aynı mesaj.
            ViewBag.Uyari = SifirlamaMesaji;
            return View();
        }

        [AllowAnonymous]
        public ActionResult YeniSifre(string token)
        {
            if (servis.TokenIleBul(token) == null)
            {
                ViewBag.Hata = GecersizBaglanti;
            }
            ViewBag.Token = token;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniSifre(string token, string sifre, string sifreTekrar)
        {
            ViewBag.Token = token;
            if (servis.TokenIleBul(token) == null)
            {
                ViewBag.Hata = GecersizBaglanti;
                return View();
            }

            var sonuc = servis.SifreyiYenile(token, sifre, sifreTekrar);
            if (!sonuc.Basarili)
            {
                ViewBag.Uyari = sonuc.Hatalar.First().Value;
                return View();
            }

            ViewBag.Basarili = "Şifreniz güncellendi. Yeni şifrenizle giriş yapabilirsiniz.";
            return View();
        }

        // ---------------------------------------------------------------- Yönetici yönetimi

        [AdminAuthorize(SadeceAdmin = true)]
        public ActionResult Adminler()
        {
            return View(servis.Listele());
        }

        [AdminAuthorize(SadeceAdmin = true)]
        public ActionResult Create()
        {
            ViewBag.YetkiListesi = Yetkiler.SecimListesi(Yetkiler.Editor);
            return View(new YoneticiFormModeli { Yetki = Yetkiler.Editor });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(SadeceAdmin = true)]
        public ActionResult Create(YoneticiFormModeli model)
        {
            if (string.IsNullOrEmpty(model.Sifre))
            {
                ModelState.AddModelError("Sifre", "Şifre zorunludur.");
            }
            if (ModelState.IsValid)
            {
                var sonuc = servis.Olustur(model);
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Adminler");
                }
                sonuc.ModelStateAktar(ModelState);
            }

            model.Sifre = null;
            ViewBag.YetkiListesi = Yetkiler.SecimListesi(model.Yetki);
            return View(model);
        }

        [AdminAuthorize(SadeceAdmin = true)]
        public ActionResult Edit(int id)
        {
            var model = servis.FormModeliGetir(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.YetkiListesi = Yetkiler.SecimListesi(model.Yetki);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(SadeceAdmin = true)]
        public ActionResult Edit(int id, YoneticiFormModeli model)
        {
            if (ModelState.IsValid)
            {
                var sonuc = servis.Guncelle(id, model, AktifAdminId);
                if (sonuc == null)
                {
                    return HttpNotFound();
                }
                if (sonuc.Basarili)
                {
                    return RedirectToAction("Adminler");
                }
                sonuc.ModelStateAktar(ModelState);
            }

            model.AdminId = id;
            model.Sifre = null;
            ViewBag.YetkiListesi = Yetkiler.SecimListesi(model.Yetki);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(SadeceAdmin = true)]
        public ActionResult Delete(int id)
        {
            var sonuc = servis.Sil(id, AktifAdminId);
            if (!sonuc.Basarili)
            {
                TempData["Uyari"] = sonuc.Hatalar.First().Value;
            }
            return RedirectToAction("Adminler");
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

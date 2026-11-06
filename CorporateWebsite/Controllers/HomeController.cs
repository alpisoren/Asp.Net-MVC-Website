using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml.Linq;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace CorporateWebsite.Controllers
{
    public class HomeController : Controller
    {
        private const int SayfaBoyutu = 5;
        private const int SonYaziSayisi = 5;

        private const int RssYaziSayisi = 20;

        private KurumsalDBContext db = new KurumsalDBContext();
        private BlogServisi blogServisi;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            blogServisi = new BlogServisi(db, Server);
        }

        [Route()]
        [Route("Anasayfa")]
        public ActionResult Index()
        {
            FooterLoader();
            ViewBag.Hizmetler = db.Hizmet.AsNoTracking().OrderByDescending(x => x.HizmetId).Take(6).ToList();
            ViewBag.HakkimizdaOzet = MetinHelper.Ozet(db.Hakkimizda.AsNoTracking().Select(x => x.Aciklama).FirstOrDefault(), 320);
            return View(db.Slider.AsNoTracking().OrderBy(x => x.SliderId).ToList());
        }

        /// <summary>Layout'un (başlık, meta, footer) ihtiyaç duyduğu verileri yükler.</summary>
        private void FooterLoader()
        {
            ViewBag.Kimlik = db.Kimlik.AsNoTracking().FirstOrDefault();
            ViewBag.Iletisim = db.Iletisim.AsNoTracking().FirstOrDefault();
            ViewBag.Blog = blogServisi.Yayindakiler()
                .Take(SonYaziSayisi)
                .ToList();
        }

        [Route("Hakkimizda")]
        public ActionResult Hakkimizda()
        {
            FooterLoader();
            return View(db.Hakkimizda.AsNoTracking().FirstOrDefault());
        }

        [Route("Hizmetlerimiz")]
        public ActionResult Hizmetlerimiz()
        {
            FooterLoader();
            return View(db.Hizmet.AsNoTracking().OrderByDescending(x => x.HizmetId).ToList());
        }

        [Route("Iletisim")]
        public ActionResult Iletisim()
        {
            FooterLoader();
            return View(db.Iletisim.AsNoTracking().FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Iletisim(string name = null, string email = null, string subject = null, string message = null, string website = null)
        {
            string mesaj;
            var basarili = IletisimMesajiGonder(name, email, subject, message, website, out mesaj);

            // iletisim.js formu AJAX ile gönderir; JavaScript kapalıysa sayfa normal şekilde yeniden çizilir.
            if (Request.IsAjaxRequest())
            {
                return Json(new { basarili, mesaj });
            }

            FooterLoader();
            ViewBag.Uyari = mesaj;
            ViewBag.UyariBasarili = basarili;
            return View(db.Iletisim.AsNoTracking().FirstOrDefault());
        }

        private bool IletisimMesajiGonder(string name, string email, string subject, string message, string website, out string mesaj)
        {
            // Bot tuzağı (honeypot): gerçek kullanıcı gizli "website" alanını doldurmaz; bota başarılı gibi görün.
            if (!string.IsNullOrEmpty(website))
            {
                mesaj = "Mesajınız alındı. En kısa sürede size dönüş yapacağız.";
                return true;
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message)
                || name.Length > 100 || email.Length > 100 || subject.Length > 200 || message.Length > 5000
                || !SpamKorumasi.GecerliEposta(email))
            {
                mesaj = "Lütfen tüm alanları eksiksiz ve doğru doldurun.";
                return false;
            }

            if (!SpamKorumasi.IzinVer("iletisim", Request.UserHostAddress, 3, TimeSpan.FromMinutes(10)))
            {
                mesaj = "Kısa sürede çok fazla mesaj gönderdiniz. Lütfen biraz sonra tekrar deneyin.";
                return false;
            }

            var alici = ConfigurationManager.AppSettings["IletisimAliciMail"];
            // Kullanıcıdan gelen veriler düz metin olarak gönderilir (HTML/script çalıştırılmaz).
            var govde = "Gönderen: " + name + Environment.NewLine + "E-posta: " + email + Environment.NewLine + Environment.NewLine + message;

            try
            {
                MailHelper.Gonder(alici, "İletişim Formu: " + subject, govde, html: false, yanitAdresi: email);
                mesaj = "Mesajınız alındı. En kısa sürede size dönüş yapacağız.";
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("İletişim maili gönderilemedi: " + ex);
                mesaj = "Mesajınız şu anda gönderilemedi. Lütfen daha sonra tekrar deneyin ya da bizi telefonla arayın.";
                return false;
            }
        }

        /// <summary>Blog listesi; "q" verilirse başlık, içerik ve etiketlerde arar.</summary>
        /// <remarks>Arama metni her yerde kodlanarak basılır; "&lt;" içeren aramalar istek doğrulamasına takılmasın.</remarks>
        [Route("BlogPost")]
        [ValidateInput(false)]
        public ActionResult Blog(int Page = 1, string q = null)
        {
            FooterLoader();
            q = MetinHelper.Kirp(q);
            if (q != null && q.Length > 100)
            {
                q = q.Substring(0, 100);
            }
            ViewBag.Arama = q;
            return View(blogServisi.Ara(q).ToPagedList(Math.Max(Page, 1), SayfaBoyutu));
        }

        [Route("BlogPost/etiket/{etiket}")]
        public ActionResult Etiket(string etiket, int Page = 1)
        {
            etiket = (EtiketHelper.Duzenle(etiket) ?? "").Split(',')[0].Trim();
            if (etiket.Length == 0)
            {
                return HttpNotFound();
            }
            var yazilar = blogServisi.EtiketliYazilar(etiket).ToPagedList(Math.Max(Page, 1), SayfaBoyutu);
            if (yazilar.TotalItemCount == 0)
            {
                return HttpNotFound();
            }
            FooterLoader();
            ViewBag.Etiket = etiket;
            ViewBag.Canonical = MutlakUrl(EtiketHelper.Url(etiket));
            return View(yazilar);
        }

        /// <summary>Son 20 yazının RSS 2.0 akışı.</summary>
        [Route("rss.xml")]
        [OutputCache(Duration = 900, VaryByParam = "none")]
        public ActionResult Rss()
        {
            var kimlik = db.Kimlik.AsNoTracking().FirstOrDefault();
            var siteAdi = kimlik?.Title ?? kimlik?.Unvan ?? "Blog";
            var yazilar = blogServisi.Yayindakiler().Take(RssYaziSayisi).ToList();
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var kanal = new XElement("channel",
                new XElement("title", siteAdi),
                new XElement("link", MutlakUrl("/BlogPost")),
                new XElement("description", kimlik?.Description ?? siteAdi),
                new XElement("language", "tr-TR"),
                new XElement(atom + "link", new XAttribute("href", MutlakUrl("/rss.xml")), new XAttribute("rel", "self"), new XAttribute("type", "application/rss+xml")));
            if (yazilar.Count > 0)
            {
                kanal.Add(new XElement("lastBuildDate", RssTarih(yazilar.Max(x => x.GuncellemeTarihi ?? x.YayinTarihi))));
            }
            foreach (var y in yazilar)
            {
                var link = MutlakUrl(SeoHelper.BlogUrl(y.Baslik, y.BlogId));
                var oge = new XElement("item",
                    new XElement("title", y.Baslik),
                    new XElement("link", link),
                    new XElement("guid", new XAttribute("isPermaLink", "true"), link),
                    new XElement("pubDate", RssTarih(y.YayinTarihi)),
                    new XElement("description", MetinHelper.Ozet(y.Icerik, 300)));
                if (y.Kategori != null)
                {
                    oge.Add(new XElement("category", y.Kategori.KategoriAd));
                }
                foreach (var e in EtiketHelper.Ayir(y.Etiketler))
                {
                    oge.Add(new XElement("category", e));
                }
                kanal.Add(oge);
            }

            var xml = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement("rss", new XAttribute("version", "2.0"), new XAttribute(XNamespace.Xmlns + "atom", atom), kanal));
            return Content(xml.Declaration + Environment.NewLine + xml.Root, "application/rss+xml", Encoding.UTF8);
        }

        /// <summary>RFC 822 biçimi (RSS'in istediği): sunucu saatini yerel saat dilimi farkıyla yazar.</summary>
        private static string RssTarih(DateTime t)
        {
            var ofset = TimeZoneInfo.Local.GetUtcOffset(t);
            return t.ToString("ddd, dd MMM yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
                + " " + (ofset < TimeSpan.Zero ? "-" : "+") + ofset.ToString("hhmm");
        }

        [ChildActionOnly]
        public ActionResult BlogKategoriPartial()
        {
            return PartialView(db.Kategori.AsNoTracking().Include(x => x.Blogs)
                .OrderByDescending(x => x.KategoriId)
                .ToList());
        }

        [ChildActionOnly]
        public ActionResult BlogSonKayitPartial()
        {
            return PartialView(blogServisi.Yayindakiler()
                .Take(SonYaziSayisi)
                .ToList());
        }

        [Route("BlogPost/{kategoriad}/{id:int}")]
        public ActionResult KategoriBlog(string kategoriad, int id, int Page = 1)
        {
            var kategori = db.Kategori.AsNoTracking().FirstOrDefault(x => x.KategoriId == id);
            if (kategori == null)
            {
                return HttpNotFound();
            }

            // Yanlış/eski slug ile gelen istekleri tek kalıcı adrese yönlendir (yinelenen içerik olmasın).
            var dogruUrl = SeoHelper.KategoriUrl(kategori.KategoriAd, kategori.KategoriId);
            if (!string.Equals(Request.Path, dogruUrl, StringComparison.Ordinal))
            {
                return RedirectPermanent(dogruUrl + (Page > 1 ? "?Page=" + Page : ""));
            }

            FooterLoader();
            ViewBag.Canonical = MutlakUrl(dogruUrl);
            ViewBag.KategoriAd = kategori.KategoriAd;
            var b = blogServisi.Yayindakiler()
                .Where(x => x.KategoriId == id)
                .ToPagedList(Math.Max(Page, 1), SayfaBoyutu);
            return View(b);
        }

        [Route("BlogPost/{baslik}-{id:int}")]
        public ActionResult BlogDetay(int id)
        {
            var blog = db.Blog.AsNoTracking().Include(x => x.Kategori).Include(x => x.Yorumlar)
                .FirstOrDefault(x => x.BlogId == id);
            // Planlı (yayın tarihi gelmemiş) yazıyı yalnızca oturum açmış yönetici önizleyebilir.
            if (blog == null || (blog.YayinTarihi > DateTime.Now && !YoneticiOturumAcik()))
            {
                return HttpNotFound();
            }

            // Başlık değişse ya da slug elle yazılsa bile tek kalıcı adres kullanılır (301).
            var dogruUrl = SeoHelper.BlogUrl(blog.Baslik, blog.BlogId);
            if (!string.Equals(Request.Path, dogruUrl, StringComparison.Ordinal))
            {
                return RedirectPermanent(dogruUrl);
            }

            FooterLoader();
            ViewBag.Canonical = MutlakUrl(dogruUrl);
            ViewBag.Aciklama = MetinHelper.Ozet(blog.Icerik, 160);
            return View(blog);
        }

        [Route("sitemap.xml")]
        [OutputCache(Duration = 3600, VaryByParam = "none")]
        public ActionResult Sitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var adresler = new List<string> { "/", "/Hakkimizda", "/Hizmetlerimiz", "/Iletisim", "/BlogPost" };

            adresler.AddRange(db.Kategori.AsNoTracking()
                .Select(x => new { x.KategoriId, x.KategoriAd }).ToList()
                .Select(x => SeoHelper.KategoriUrl(x.KategoriAd, x.KategoriId)));

            var ogeler = adresler.Select(a => new XElement(ns + "url", new XElement(ns + "loc", MutlakUrl(a)))).ToList();
            ogeler.AddRange(blogServisi.Yayindakiler()
                .Select(x => new { x.BlogId, x.Baslik, x.YayinTarihi, x.GuncellemeTarihi }).ToList()
                .Select(x => new XElement(ns + "url",
                    new XElement(ns + "loc", MutlakUrl(SeoHelper.BlogUrl(x.Baslik, x.BlogId))),
                    new XElement(ns + "lastmod", (x.GuncellemeTarihi ?? x.YayinTarihi).ToString("yyyy-MM-dd")))));

            var xml = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement(ns + "urlset", ogeler));

            return Content(xml.Declaration + Environment.NewLine + xml.Root, "application/xml", Encoding.UTF8);
        }

        [Route("robots.txt")]
        public ActionResult Robots()
        {
            var metin = "User-agent: *\n"
                        + "Disallow: /yonetimpaneli\n"
                        + "Disallow: /Admin/\n"
                        + "Sitemap: " + MutlakUrl("/sitemap.xml") + "\n";
            return Content(metin, "text/plain", Encoding.UTF8);
        }

        /// <summary>Panel dışındaki sayfalarda yetki filtresi çalışmaz; çerezdeki kimlik veritabanında hâlâ var mı diye bakılır.</summary>
        private bool YoneticiOturumAcik()
        {
            var id = YoneticiOturumu.KimlikId(User);
            return id.HasValue && db.Admin.Any(x => x.AdminId == id.Value);
        }

        private string MutlakUrl(string yol)
        {
            return Request.Url.GetLeftPart(UriPartial.Authority) + yol;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult YorumYap(string adsoyad, string eposta, string icerik, int? blogId, string website = null)
        {
            // true = hata, false = başarılı (view'daki mevcut sözleşme korunuyor)

            // Bot tuzağı: sessizce başarılı gibi davran.
            if (!string.IsNullOrEmpty(website))
            {
                return Json(false);
            }

            if (string.IsNullOrWhiteSpace(adsoyad) || string.IsNullOrWhiteSpace(icerik)
                || adsoyad.Length > 50 || icerik.Length > 2000
                || (!string.IsNullOrWhiteSpace(eposta) && (eposta.Length > 100 || !SpamKorumasi.GecerliEposta(eposta)))
                || blogId == null || !db.Blog.Any(x => x.BlogId == blogId))
            {
                return Json(true);
            }

            if (!SpamKorumasi.IzinVer("yorum", Request.UserHostAddress, 5, TimeSpan.FromMinutes(10)))
            {
                return Json(true);
            }

            db.Yorum.Add(new Models.Model.Yorum
            {
                AdSoyad = adsoyad.Trim(),
                Eposta = string.IsNullOrWhiteSpace(eposta) ? null : eposta.Trim(),
                Icerik = icerik.Trim(),
                BlogId = blogId,
                Onayli = false
            });
            db.SaveChanges();

            return Json(false);
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

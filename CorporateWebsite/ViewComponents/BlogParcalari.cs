using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Linq;

namespace CorporateWebsite.ViewComponents
{
    /// <summary>Blog kenar çubuğu: kategoriler ve yazı sayıları (Views/Shared/Components/BlogKategorileri).</summary>
    public class BlogKategorileriViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            using (var db = new KurumsalDBContext())
            {
                return View(db.Kategori.AsNoTracking().Include(x => x.Blogs)
                    .OrderByDescending(x => x.KategoriId)
                    .ToList());
            }
        }
    }

    /// <summary>Blog kenar çubuğu: son gönderiler (Views/Shared/Components/BlogSonYazilar).</summary>
    public class BlogSonYazilarViewComponent : ViewComponent
    {
        public const int YaziSayisi = 5;

        public IViewComponentResult Invoke()
        {
            using (var db = new KurumsalDBContext())
            {
                return View(new BlogServisi(db).Yayindakiler().Take(YaziSayisi).ToList());
            }
        }
    }

    /// <summary>Panel üst çubuğu ve menüsündeki "onay bekleyen yorum" rozeti.</summary>
    public class BekleyenYorumSayisiViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            using (var db = new KurumsalDBContext())
            {
                return Content(new YoneticiServisi(db).BekleyenYorumSayisi().ToString());
            }
        }
    }
}

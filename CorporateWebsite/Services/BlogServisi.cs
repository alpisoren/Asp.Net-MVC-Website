using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Models.ViewModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CorporateWebsite.Services
{
    /// <summary>Blog yazılarının kaydı: HTML temizleme, resim yükleme/silme ve doğrulama burada.</summary>
    public class BlogServisi
    {
        private const string ResimKlasoru = "Blog";
        private const int ResimGenislik = 600;
        private const int ResimYukseklik = 400;

        private readonly KurumsalDBContext db;
        private readonly HttpServerUtilityBase server;

        public BlogServisi(KurumsalDBContext db, HttpServerUtilityBase server)
        {
            this.db = db;
            this.server = server;
        }

        public List<Blog> Listele()
        {
            return db.Blog.AsNoTracking().Include(x => x.Kategori).OrderByDescending(x => x.BlogId).ToList();
        }

        public Blog Bul(int id)
        {
            return db.Blog.Find(id);
        }

        public BlogFormModeli FormModeliGetir(int id)
        {
            var b = db.Blog.AsNoTracking().SingleOrDefault(x => x.BlogId == id);
            if (b == null)
            {
                return null;
            }
            return new BlogFormModeli
            {
                BlogId = b.BlogId,
                Baslik = b.Baslik,
                Icerik = b.Icerik,
                KategoriId = b.KategoriId,
                ResimURL = b.ResimURL
            };
        }

        public IslemSonucu Olustur(BlogFormModeli model, HttpPostedFileBase resim)
        {
            var sonuc = KategoriKontrol(model);
            if (!sonuc.Basarili)
            {
                return sonuc;
            }

            string url = null;
            if (resim != null)
            {
                string hata;
                url = ResimHelper.Kaydet(resim, ResimKlasoru, ResimGenislik, ResimYukseklik, out hata);
                if (url == null)
                {
                    return IslemSonucu.Hata("", hata);
                }
            }

            db.Blog.Add(new Blog
            {
                Baslik = model.Baslik.Trim(),
                Icerik = HtmlTemizleyici.Temizle(model.Icerik),
                KategoriId = model.KategoriId,
                ResimURL = url
            });
            db.SaveChanges();
            return sonuc;
        }

        /// <summary>Kayıt yoksa null döner.</summary>
        public IslemSonucu Guncelle(int id, BlogFormModeli model, HttpPostedFileBase resim)
        {
            var b = db.Blog.SingleOrDefault(x => x.BlogId == id);
            if (b == null)
            {
                return null;
            }

            var sonuc = KategoriKontrol(model);
            if (!sonuc.Basarili)
            {
                return sonuc;
            }

            if (resim != null)
            {
                string hata;
                var url = ResimHelper.Kaydet(resim, ResimKlasoru, ResimGenislik, ResimYukseklik, out hata);
                if (url == null)
                {
                    return IslemSonucu.Hata("", hata);
                }
                ResimHelper.Sil(server, b.ResimURL);
                b.ResimURL = url;
            }

            b.Baslik = model.Baslik.Trim();
            b.Icerik = HtmlTemizleyici.Temizle(model.Icerik);
            b.KategoriId = model.KategoriId;
            db.SaveChanges();
            return sonuc;
        }

        public bool Sil(int id)
        {
            var b = db.Blog.Include(x => x.Yorumlar).SingleOrDefault(x => x.BlogId == id);
            if (b == null)
            {
                return false;
            }
            ResimHelper.Sil(server, b.ResimURL);
            // Yorumlar bloga bağlı; önce onları kaldır ki yabancı anahtar hatası olmasın.
            if (b.Yorumlar != null)
            {
                db.Yorum.RemoveRange(b.Yorumlar.ToList());
            }
            db.Blog.Remove(b);
            db.SaveChanges();
            return true;
        }

        private IslemSonucu KategoriKontrol(BlogFormModeli model)
        {
            if (model.KategoriId.HasValue && !db.Kategori.Any(x => x.KategoriId == model.KategoriId))
            {
                return IslemSonucu.Hata("KategoriId", "Geçerli bir kategori seçiniz.");
            }
            return IslemSonucu.Tamam();
        }
    }
}

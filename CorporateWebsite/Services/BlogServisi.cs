using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Models.ViewModel;
using System;
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
            return db.Blog.AsNoTracking().Include(x => x.Kategori)
                .OrderByDescending(x => x.YayinTarihi).ThenByDescending(x => x.BlogId).ToList();
        }

        // ---------------------------------------------------------------- site tarafı sorguları

        /// <summary>Sitede görünen yazılar: yayın tarihi gelmiş olanlar, en yeni önce.</summary>
        public IQueryable<Blog> Yayindakiler()
        {
            var simdi = DateTime.Now;
            return db.Blog.AsNoTracking().Include(x => x.Kategori)
                .Where(x => x.YayinTarihi <= simdi)
                .OrderByDescending(x => x.YayinTarihi).ThenByDescending(x => x.BlogId);
        }

        /// <summary>Başlık, içerik ve etiketlerde arar (büyük/küçük harf duyarsız; SQL Server harmanlamasına göre).</summary>
        public IQueryable<Blog> Ara(string sorgu)
        {
            sorgu = (sorgu ?? "").Trim();
            if (sorgu.Length == 0)
            {
                return Yayindakiler();
            }
            return Yayindakiler().Where(x => x.Baslik.Contains(sorgu) || x.Icerik.Contains(sorgu) || x.Etiketler.Contains(sorgu));
        }

        /// <summary>Etiketi tam olarak taşıyan yazılar ("seo", "seo araçları"nı eşlemez).</summary>
        public IQueryable<Blog> EtiketliYazilar(string etiket)
        {
            var e = (EtiketHelper.Duzenle(etiket) ?? "").Split(',')[0].Trim();
            var bas = e + ", ";
            var son = ", " + e;
            var orta = ", " + e + ",";
            return Yayindakiler().Where(x => x.Etiketler == e || x.Etiketler.StartsWith(bas)
                || x.Etiketler.EndsWith(son) || x.Etiketler.Contains(orta));
        }

        /// <summary>Formdaki tarih yalnızca gün içerir; bugünse ve yeni kayıtsa şu anki saat kullanılır.</summary>
        private static DateTime YayinZamani(DateTime? formTarihi, DateTime? mevcut)
        {
            var simdi = DateTime.Now;
            if (!formTarihi.HasValue)
            {
                return mevcut ?? simdi;
            }
            var gun = formTarihi.Value.Date;
            if (mevcut.HasValue && mevcut.Value.Date == gun)
            {
                return mevcut.Value; // gün değişmediyse saat korunur
            }
            return gun == simdi.Date ? simdi : gun;
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
                ResimURL = b.ResimURL,
                YayinTarihi = b.YayinTarihi,
                Yazar = b.Yazar,
                Etiketler = b.Etiketler
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
                ResimURL = url,
                YayinTarihi = YayinZamani(model.YayinTarihi, null),
                Yazar = MetinHelper.Kirp(model.Yazar),
                Etiketler = EtiketHelper.Duzenle(model.Etiketler)
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
            b.YayinTarihi = YayinZamani(model.YayinTarihi, b.YayinTarihi);
            b.Yazar = MetinHelper.Kirp(model.Yazar);
            b.Etiketler = EtiketHelper.Duzenle(model.Etiketler);
            b.GuncellemeTarihi = DateTime.Now;
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

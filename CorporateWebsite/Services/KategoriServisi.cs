using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using System.Collections.Generic;
using System.Linq;

namespace CorporateWebsite.Services
{
    /// <summary>Blog kategorileri. Silinen kategorinin yazıları silinmez, kategorisiz kalır.</summary>
    public class KategoriServisi
    {
        private readonly KurumsalDBContext db;

        public KategoriServisi(KurumsalDBContext db)
        {
            this.db = db;
        }

        public List<Kategori> Listele()
        {
            return db.Kategori.AsNoTracking().OrderBy(x => x.KategoriAd).ToList();
        }

        public Kategori Bul(int id)
        {
            return db.Kategori.AsNoTracking().SingleOrDefault(x => x.KategoriId == id);
        }

        public IslemSonucu Olustur(Kategori model)
        {
            var sonuc = AdKontrol(model.KategoriAd, 0);
            if (!sonuc.Basarili)
            {
                return sonuc;
            }
            db.Kategori.Add(new Kategori { KategoriAd = model.KategoriAd.Trim(), Aciklama = MetinHelper.Kirp(model.Aciklama) });
            db.SaveChanges();
            return sonuc;
        }

        /// <summary>Kayıt yoksa null döner.</summary>
        public IslemSonucu Guncelle(int id, Kategori model)
        {
            var k = db.Kategori.SingleOrDefault(x => x.KategoriId == id);
            if (k == null)
            {
                return null;
            }
            var sonuc = AdKontrol(model.KategoriAd, id);
            if (!sonuc.Basarili)
            {
                return sonuc;
            }
            k.KategoriAd = model.KategoriAd.Trim();
            k.Aciklama = MetinHelper.Kirp(model.Aciklama);
            db.SaveChanges();
            return sonuc;
        }

        /// <summary>Silinen kategorinin adını, kayıt yoksa null döner.</summary>
        public string Sil(int id)
        {
            var k = db.Kategori.Find(id);
            if (k == null)
            {
                return null;
            }
            // Kategoriye bağlı yazılar silinmez, kategorisiz kalır (aksi halde yabancı anahtar hatası oluşur).
            foreach (var blog in db.Blog.Where(b => b.KategoriId == id).ToList())
            {
                blog.KategoriId = null;
            }
            db.Kategori.Remove(k);
            db.SaveChanges();
            return k.KategoriAd;
        }

        private IslemSonucu AdKontrol(string ad, int haricId)
        {
            var sonuc = IslemSonucu.Tamam();
            ad = (ad ?? "").Trim();
            if (ad.Length > 0 && db.Kategori.Any(x => x.KategoriAd == ad && x.KategoriId != haricId))
            {
                sonuc.HataEkle("KategoriAd", "Bu adla bir kategori zaten var.");
            }
            return sonuc;
        }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CorporateWebsite.Services
{
    /// <summary>Blog yorumlarının yönetimi: onay, düzenleme, silme. Sitede yalnızca Onayli = true olanlar görünür.</summary>
    public class YorumServisi
    {
        private readonly KurumsalDBContext db;

        public YorumServisi(KurumsalDBContext db)
        {
            this.db = db;
        }

        /// <summary>Onay bekleyenler önce, sonra en yeniler.</summary>
        public List<Yorum> Listele()
        {
            return db.Yorum.AsNoTracking().Include(y => y.Blog)
                .OrderBy(y => y.Onayli).ThenByDescending(y => y.YorumId).ToList();
        }

        public Yorum Bul(int id)
        {
            return db.Yorum.AsNoTracking().Include(y => y.Blog).SingleOrDefault(y => y.YorumId == id);
        }

        public SelectList BlogSecimListesi(int? secili)
        {
            var bloglar = db.Blog.AsNoTracking().OrderByDescending(b => b.BlogId)
                .Select(b => new { b.BlogId, b.Baslik }).ToList();
            return new SelectList(bloglar, "BlogId", "Baslik", secili);
        }

        /// <summary>Onayı tersine çevirir; yeni durumu, kayıt yoksa null döner.</summary>
        public bool? OnayDegistir(int id)
        {
            var y = db.Yorum.Find(id);
            if (y == null)
            {
                return null;
            }
            y.Onayli = !y.Onayli;
            db.SaveChanges();
            return y.Onayli;
        }

        public void Olustur(Yorum model)
        {
            db.Yorum.Add(new Yorum
            {
                AdSoyad = model.AdSoyad.Trim(),
                Eposta = MetinHelper.Kirp(model.Eposta),
                Icerik = MetinHelper.Kirp(model.Icerik),
                Onayli = model.Onayli,
                BlogId = model.BlogId
            });
            db.SaveChanges();
        }

        /// <summary>Kayıt yoksa false döner.</summary>
        public bool Guncelle(int id, Yorum model)
        {
            var y = db.Yorum.Find(id);
            if (y == null)
            {
                return false;
            }
            y.AdSoyad = model.AdSoyad.Trim();
            y.Eposta = MetinHelper.Kirp(model.Eposta);
            y.Icerik = MetinHelper.Kirp(model.Icerik);
            y.Onayli = model.Onayli;
            y.BlogId = model.BlogId;
            db.SaveChanges();
            return true;
        }

        public bool Sil(int id)
        {
            var y = db.Yorum.Find(id);
            if (y == null)
            {
                return false;
            }
            db.Yorum.Remove(y);
            db.SaveChanges();
            return true;
        }
    }
}

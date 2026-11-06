using Microsoft.AspNetCore.Http;
using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using System.Collections.Generic;
using System.Linq;

namespace CorporateWebsite.Services
{
    /// <summary>Ana sayfa slaytları: resim yükleme (1920×700'e sığdırılır) ve silme.</summary>
    public class SliderServisi
    {
        public const string ResimKlasoru = "Slider";
        public const int ResimGenislik = 1920;
        public const int ResimYukseklik = 700;

        private readonly KurumsalDBContext db;

        public SliderServisi(KurumsalDBContext db)
        {
            this.db = db;
        }

        public List<Slider> Listele()
        {
            return db.Slider.AsNoTracking().OrderBy(x => x.SliderId).ToList();
        }

        public Slider Bul(int id)
        {
            return db.Slider.AsNoTracking().SingleOrDefault(x => x.SliderId == id);
        }

        public IslemSonucu Olustur(Slider model, IFormFile resim)
        {
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

            db.Slider.Add(new Slider
            {
                Baslik = MetinHelper.Kirp(model.Baslik),
                Aciklama = MetinHelper.Kirp(model.Aciklama),
                ResimURL = url
            });
            db.SaveChanges();
            return IslemSonucu.Tamam();
        }

        /// <summary>Kayıt yoksa null döner.</summary>
        public IslemSonucu Guncelle(int id, Slider model, IFormFile resim)
        {
            var s = db.Slider.SingleOrDefault(x => x.SliderId == id);
            if (s == null)
            {
                return null;
            }

            if (resim != null)
            {
                string hata;
                var url = ResimHelper.Kaydet(resim, ResimKlasoru, ResimGenislik, ResimYukseklik, out hata);
                if (url == null)
                {
                    return IslemSonucu.Hata("", hata);
                }
                ResimHelper.Sil(s.ResimURL);
                s.ResimURL = url;
            }

            s.Baslik = MetinHelper.Kirp(model.Baslik);
            s.Aciklama = MetinHelper.Kirp(model.Aciklama);
            db.SaveChanges();
            return IslemSonucu.Tamam();
        }

        public bool Sil(int id)
        {
            var s = db.Slider.Find(id);
            if (s == null)
            {
                return false;
            }
            ResimHelper.Sil(s.ResimURL);
            db.Slider.Remove(s);
            db.SaveChanges();
            return true;
        }
    }
}

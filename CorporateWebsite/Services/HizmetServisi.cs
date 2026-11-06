using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateWebsite.Services
{
    /// <summary>Hizmet kayıtları: HTML temizleme, resim yükleme/silme.</summary>
    public class HizmetServisi
    {
        private const string ResimKlasoru = "Hizmet";
        private const int ResimBoyutu = 500;

        private readonly KurumsalDBContext db;
        private readonly HttpServerUtilityBase server;

        public HizmetServisi(KurumsalDBContext db, HttpServerUtilityBase server)
        {
            this.db = db;
            this.server = server;
        }

        public List<Hizmet> Listele()
        {
            return db.Hizmet.AsNoTracking().OrderByDescending(x => x.HizmetId).ToList();
        }

        public HizmetFormModeli FormModeliGetir(int id)
        {
            var h = db.Hizmet.AsNoTracking().SingleOrDefault(x => x.HizmetId == id);
            if (h == null)
            {
                return null;
            }
            return new HizmetFormModeli { HizmetId = h.HizmetId, Baslik = h.Baslik, Aciklama = h.Aciklama, ResimUrl = h.ResimUrl };
        }

        public IslemSonucu Olustur(HizmetFormModeli model, HttpPostedFileBase resim)
        {
            string url = null;
            if (resim != null)
            {
                string hata;
                url = ResimHelper.Kaydet(resim, ResimKlasoru, ResimBoyutu, ResimBoyutu, out hata);
                if (url == null)
                {
                    return IslemSonucu.Hata("", hata);
                }
            }

            db.Hizmet.Add(new Hizmet
            {
                Baslik = model.Baslik.Trim(),
                Aciklama = HtmlTemizleyici.Temizle(model.Aciklama),
                ResimUrl = url
            });
            db.SaveChanges();
            return IslemSonucu.Tamam();
        }

        /// <summary>Kayıt yoksa null döner.</summary>
        public IslemSonucu Guncelle(int id, HizmetFormModeli model, HttpPostedFileBase resim)
        {
            var h = db.Hizmet.SingleOrDefault(x => x.HizmetId == id);
            if (h == null)
            {
                return null;
            }

            if (resim != null)
            {
                string hata;
                var url = ResimHelper.Kaydet(resim, ResimKlasoru, ResimBoyutu, ResimBoyutu, out hata);
                if (url == null)
                {
                    return IslemSonucu.Hata("", hata);
                }
                ResimHelper.Sil(server, h.ResimUrl);
                h.ResimUrl = url;
            }

            h.Baslik = model.Baslik.Trim();
            h.Aciklama = HtmlTemizleyici.Temizle(model.Aciklama);
            db.SaveChanges();
            return IslemSonucu.Tamam();
        }

        public bool Sil(int id)
        {
            var h = db.Hizmet.Find(id);
            if (h == null)
            {
                return false;
            }
            ResimHelper.Sil(server, h.ResimUrl);
            db.Hizmet.Remove(h);
            db.SaveChanges();
            return true;
        }
    }
}

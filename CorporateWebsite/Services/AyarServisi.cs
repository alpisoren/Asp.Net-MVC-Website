using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateWebsite.Services
{
    /// <summary>
    /// Tek kayıtlı site ayarları: Kimlik (başlık, SEO, logo), Hakkımızda metni ve İletişim bilgileri.
    /// Sitede her tablonun ilk kaydı kullanılır.
    /// </summary>
    public class AyarServisi
    {
        public const string LogoKlasoru = "Kimlik";
        public const int LogoBoyutu = 300;

        private readonly KurumsalDBContext db;
        private readonly HttpServerUtilityBase server;

        public AyarServisi(KurumsalDBContext db, HttpServerUtilityBase server)
        {
            this.db = db;
            this.server = server;
        }

        // ---------------------------------------------------------------- Kimlik

        public List<Kimlik> KimlikListele()
        {
            return db.Kimlik.AsNoTracking().ToList();
        }

        public Kimlik KimlikBul(int id)
        {
            return db.Kimlik.AsNoTracking().SingleOrDefault(x => x.KimlikId == id);
        }

        /// <summary>Kayıt yoksa null döner.</summary>
        public IslemSonucu KimlikGuncelle(int id, Kimlik model, HttpPostedFileBase logo)
        {
            var k = db.Kimlik.SingleOrDefault(x => x.KimlikId == id);
            if (k == null)
            {
                return null;
            }

            if (logo != null)
            {
                string hata;
                var url = ResimHelper.Kaydet(logo, LogoKlasoru, LogoBoyutu, LogoBoyutu, out hata);
                if (url == null)
                {
                    return IslemSonucu.Hata("", hata);
                }
                ResimHelper.Sil(server, k.LogoUrl);
                k.LogoUrl = url;
            }

            k.Title = model.Title.Trim();
            k.Keywords = model.Keywords.Trim();
            k.Description = model.Description.Trim();
            k.Unvan = MetinHelper.Kirp(model.Unvan);
            db.SaveChanges();
            return IslemSonucu.Tamam();
        }

        // ---------------------------------------------------------------- Hakkımızda

        public List<Hakkimizda> HakkimizdaListele()
        {
            return db.Hakkimizda.AsNoTracking().ToList();
        }

        public Hakkimizda HakkimizdaBul(int id)
        {
            return db.Hakkimizda.AsNoTracking().SingleOrDefault(x => x.HakkimizdaId == id);
        }

        /// <summary>HTML temizlenerek kaydedilir. Kayıt yoksa false döner.</summary>
        public bool HakkimizdaGuncelle(int id, string aciklama)
        {
            var h = db.Hakkimizda.SingleOrDefault(x => x.HakkimizdaId == id);
            if (h == null)
            {
                return false;
            }
            h.Aciklama = HtmlTemizleyici.Temizle(aciklama);
            db.SaveChanges();
            return true;
        }

        // ---------------------------------------------------------------- İletişim

        public List<Iletisim> IletisimListele()
        {
            return db.Iletisim.AsNoTracking().ToList();
        }

        public Iletisim IletisimBul(int id)
        {
            return db.Iletisim.AsNoTracking().SingleOrDefault(x => x.IletisimId == id);
        }

        public void IletisimOlustur(Iletisim model)
        {
            var i = new Iletisim();
            IletisimAktar(model, i);
            db.Iletisim.Add(i);
            db.SaveChanges();
        }

        /// <summary>Kayıt yoksa false döner.</summary>
        public bool IletisimGuncelle(int id, Iletisim model)
        {
            var i = db.Iletisim.Find(id);
            if (i == null)
            {
                return false;
            }
            IletisimAktar(model, i);
            db.SaveChanges();
            return true;
        }

        public bool IletisimSil(int id)
        {
            var i = db.Iletisim.Find(id);
            if (i == null)
            {
                return false;
            }
            db.Iletisim.Remove(i);
            db.SaveChanges();
            return true;
        }

        private static void IletisimAktar(Iletisim kaynak, Iletisim hedef)
        {
            hedef.Adres = MetinHelper.Kirp(kaynak.Adres);
            hedef.Telefon = MetinHelper.Kirp(kaynak.Telefon);
            hedef.Eposta = MetinHelper.Kirp(kaynak.Eposta);
            hedef.Whatsapp = MetinHelper.Kirp(kaynak.Whatsapp);
            hedef.Facebook = MetinHelper.Kirp(kaynak.Facebook);
            hedef.Twitter = MetinHelper.Kirp(kaynak.Twitter);
            hedef.Instagram = MetinHelper.Kirp(kaynak.Instagram);
        }
    }
}

using CorporateWebsite.Helper;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CorporateWebsite.Services
{
    public enum GirisDurumu
    {
        Basarili,
        HataliBilgi,
        GeciciOlarakEngellendi
    }

    /// <summary>Yönetici girişi, şifre sıfırlama ve yönetici CRUD iş kuralları.</summary>
    public class YoneticiServisi
    {
        public const int MaksimumHataliGiris = 5;
        public static readonly TimeSpan GirisEngelSuresi = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan SifirlamaGecerlilik = TimeSpan.FromHours(1);

        private const string GirisIslemi = "giris";
        private readonly KurumsalDBContext db;

        public YoneticiServisi(KurumsalDBContext db)
        {
            this.db = db;
        }

        // ---------------------------------------------------------------- Giriş

        public GirisDurumu GirisYap(string eposta, string sifre, string ip, out Admin admin)
        {
            admin = null;

            // Hem IP hem e-posta bazında sınır: tek IP'den çok hesap ya da çok IP'den tek hesap denenemesin.
            var epostaAnahtari = (eposta ?? "").Trim().ToLowerInvariant();
            if (SpamKorumasi.SinirAsildi(GirisIslemi, ip, MaksimumHataliGiris)
                || SpamKorumasi.SinirAsildi(GirisIslemi, "eposta:" + epostaAnahtari, MaksimumHataliGiris))
            {
                return GirisDurumu.GeciciOlarakEngellendi;
            }

            var kayit = string.IsNullOrWhiteSpace(eposta) ? null : db.Admin.SingleOrDefault(x => x.Eposta == eposta);

            bool yenidenHashle;
            if (kayit == null || !SifreHelper.Dogrula(kayit.Sifre, sifre, out yenidenHashle))
            {
                SpamKorumasi.BasarisizDenemeEkle(GirisIslemi, ip, GirisEngelSuresi);
                SpamKorumasi.BasarisizDenemeEkle(GirisIslemi, "eposta:" + epostaAnahtari, GirisEngelSuresi);
                return GirisDurumu.HataliBilgi;
            }

            if (yenidenHashle)
            {
                // Eski MD5 hash'ini PBKDF2'ye yükselt.
                kayit.Sifre = SifreHelper.Hashle(sifre);
                db.SaveChanges();
            }

            SpamKorumasi.Sifirla(GirisIslemi, ip);
            SpamKorumasi.Sifirla(GirisIslemi, "eposta:" + epostaAnahtari);
            admin = kayit;
            return GirisDurumu.Basarili;
        }

        // ---------------------------------------------------------------- Şifre sıfırlama

        /// <summary>
        /// Kayıtlı e-posta için token üretip sıfırlama bağlantısını gönderir. Hesabın var olup olmadığı
        /// dışarıya belli edilmez; mail hataları günlüğe yazılır.
        /// </summary>
        public void SifirlamaBaslat(string eposta, Func<string, string> baglantiUret)
        {
            var admin = string.IsNullOrWhiteSpace(eposta) ? null : db.Admin.SingleOrDefault(x => x.Eposta == eposta);
            if (admin == null)
            {
                return;
            }

            var token = SifreHelper.TokenUret();
            admin.SifreSifirlamaToken = SifreHelper.TokenOzeti(token);
            admin.SifreSifirlamaSonTarih = DateTime.Now.Add(SifirlamaGecerlilik);
            db.SaveChanges();

            var govde = "Yönetim paneli şifrenizi sıfırlamak için aşağıdaki bağlantıyı 1 saat içinde açın:\n\n"
                        + baglantiUret(token)
                        + "\n\nBu isteği siz yapmadıysanız bu e-postayı dikkate almayın.";
            try
            {
                MailHelper.Gonder(admin.Eposta, "Şifre Sıfırlama", govde);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Şifre sıfırlama maili gönderilemedi: " + ex);
            }
        }

        public Admin TokenIleBul(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }
            var ozet = SifreHelper.TokenOzeti(token);
            var simdi = DateTime.Now;
            return db.Admin.SingleOrDefault(x => x.SifreSifirlamaToken == ozet && x.SifreSifirlamaSonTarih > simdi);
        }

        public IslemSonucu SifreyiYenile(string token, string sifre, string sifreTekrar)
        {
            var admin = TokenIleBul(token);
            if (admin == null)
            {
                return IslemSonucu.Hata("", "Bağlantı geçersiz ya da süresi dolmuş. Lütfen yeniden şifre sıfırlama isteyin.");
            }

            var hata = SifreKontrol(sifre, sifreTekrar);
            if (hata != null)
            {
                return IslemSonucu.Hata("sifre", hata);
            }

            admin.Sifre = SifreHelper.Hashle(sifre);
            admin.SifreSifirlamaToken = null;
            admin.SifreSifirlamaSonTarih = null;
            db.SaveChanges();
            return IslemSonucu.Tamam();
        }

        // ---------------------------------------------------------------- Yönetici CRUD

        public List<Admin> Listele()
        {
            return db.Admin.AsNoTracking().OrderBy(x => x.Eposta).ToList();
        }

        public YoneticiFormModeli FormModeliGetir(int id)
        {
            var a = db.Admin.AsNoTracking().SingleOrDefault(x => x.AdminId == id);
            if (a == null)
            {
                return null;
            }
            // Şifre hash'i forma asla gönderilmez.
            return new YoneticiFormModeli { AdminId = a.AdminId, Eposta = a.Eposta, Yetki = a.Yetki };
        }

        public IslemSonucu Olustur(YoneticiFormModeli model)
        {
            var sonuc = OrtakKontrol(model, 0);
            var sifreHatasi = SifreKontrol(model.Sifre, model.Sifre);
            if (sifreHatasi != null)
            {
                sonuc.HataEkle("Sifre", sifreHatasi);
            }
            if (!sonuc.Basarili)
            {
                return sonuc;
            }

            db.Admin.Add(new Admin
            {
                Eposta = model.Eposta.Trim(),
                Sifre = SifreHelper.Hashle(model.Sifre),
                Yetki = model.Yetki
            });
            db.SaveChanges();
            return sonuc;
        }

        /// <summary>Kayıt yoksa null döner. Şifre boş bırakılırsa mevcut şifre korunur.</summary>
        public IslemSonucu Guncelle(int id, YoneticiFormModeli model, int aktifAdminId)
        {
            var a = db.Admin.SingleOrDefault(x => x.AdminId == id);
            if (a == null)
            {
                return null;
            }

            var sonuc = OrtakKontrol(model, id);
            bool sifreDegisiyor = !string.IsNullOrEmpty(model.Sifre);
            if (sifreDegisiyor)
            {
                var sifreHatasi = SifreKontrol(model.Sifre, model.Sifre);
                if (sifreHatasi != null)
                {
                    sonuc.HataEkle("Sifre", sifreHatasi);
                }
            }
            if (id == aktifAdminId && model.Yetki != Yetkiler.Admin)
            {
                sonuc.HataEkle("Yetki", "Kendi Admin yetkinizi kaldıramazsınız.");
            }
            if (!sonuc.Basarili)
            {
                return sonuc;
            }

            if (sifreDegisiyor)
            {
                a.Sifre = SifreHelper.Hashle(model.Sifre);
            }
            a.Eposta = model.Eposta.Trim();
            a.Yetki = model.Yetki;
            db.SaveChanges();
            return sonuc;
        }

        public IslemSonucu Sil(int id, int aktifAdminId)
        {
            if (id == aktifAdminId)
            {
                return IslemSonucu.Hata("", "Kendi hesabınızı silemezsiniz.");
            }

            var a = db.Admin.SingleOrDefault(x => x.AdminId == id);
            if (a == null)
            {
                return IslemSonucu.Hata("", "Yönetici bulunamadı.");
            }
            if (a.Yetki == Yetkiler.Admin && db.Admin.Count(x => x.Yetki == Yetkiler.Admin) <= 1)
            {
                return IslemSonucu.Hata("", "Son Admin yetkili hesap silinemez.");
            }

            db.Admin.Remove(a);
            db.SaveChanges();
            return IslemSonucu.Tamam();
        }

        public int BekleyenYorumSayisi()
        {
            return db.Yorum.Count(x => !x.Onayli);
        }

        // ---------------------------------------------------------------- yardımcılar

        private IslemSonucu OrtakKontrol(YoneticiFormModeli model, int haricId)
        {
            var sonuc = IslemSonucu.Tamam();
            if (!Yetkiler.Gecerli(model.Yetki))
            {
                sonuc.HataEkle("Yetki", "Geçerli bir yetki seçiniz.");
            }
            var eposta = (model.Eposta ?? "").Trim();
            if (eposta.Length > 0 && db.Admin.Any(x => x.Eposta == eposta && x.AdminId != haricId))
            {
                sonuc.HataEkle("Eposta", "Bu e-posta ile kayıtlı başka bir yönetici var.");
            }
            return sonuc;
        }

        public static string SifreKontrol(string sifre, string sifreTekrar)
        {
            if (string.IsNullOrEmpty(sifre) || sifre.Length < SifreHelper.MinimumUzunluk)
            {
                return "Şifre en az " + SifreHelper.MinimumUzunluk + " karakter olmalıdır.";
            }
            if (sifre.Length > 100)
            {
                return "Şifre en fazla 100 karakter olabilir.";
            }
            if (sifre != sifreTekrar)
            {
                return "Şifreler eşleşmiyor.";
            }
            return null;
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace CorporateWebsite.Helper
{
    public static class ResimHelper
    {
        private const string YuklemeKoku = "/Uploads/";
        private const int MaksimumBoyut = 4 * 1024 * 1024; // 4 MB
        private static readonly string[] IzinliUzantilar = { ".jpg", ".jpeg", ".png" };

        /// <summary>
        /// Yüklenen resmi doğrular, boyutlandırır ve ~/Uploads/{klasor}/ altına rastgele bir adla kaydeder.
        /// Başarılıysa site içi URL'yi, değilse null döner ve hata mesajını doldurur.
        /// </summary>
        public static string Kaydet(HttpPostedFileBase dosya, string klasor, int genislik, int yukseklik, out string hata)
        {
            hata = null;

            if (dosya == null || dosya.ContentLength == 0)
            {
                hata = "Dosya seçilmedi.";
                return null;
            }

            if (dosya.ContentLength > MaksimumBoyut)
            {
                hata = "Resim en fazla 4 MB olabilir.";
                return null;
            }

            var uzanti = (Path.GetExtension(dosya.FileName) ?? "").ToLowerInvariant();
            if (!IzinliUzantilar.Contains(uzanti))
            {
                hata = "Sadece .jpg, .jpeg ve .png dosyaları yüklenebilir.";
                return null;
            }

            WebImage img;
            try
            {
                img = new WebImage(dosya.InputStream);
            }
            catch (Exception)
            {
                hata = "Geçerli bir resim dosyası değil.";
                return null;
            }

            var dosyaAdi = Guid.NewGuid().ToString("N") + uzanti;
            var url = YuklemeKoku + klasor + "/" + dosyaAdi;

            // En-boy oranı korunur ve resim kutuya sığdırılır; küçük resimler büyütülmez (bulanıklaşmasın).
            img.Resize(genislik, yukseklik, preserveAspectRatio: true, preventEnlarge: true);
            var klasorYolu = System.Web.Hosting.HostingEnvironment.MapPath("~" + YuklemeKoku + klasor);
            if (klasorYolu != null)
            {
                Directory.CreateDirectory(klasorYolu); // ilk yüklemede klasör yoksa oluştur
            }
            img.Save("~" + url);

            return url;
        }

        /// <summary>Sadece ~/Uploads/ altındaki dosyaları siler; boş ya da dışarıyı gösteren yolları yok sayar.</summary>
        public static void Sil(HttpServerUtilityBase server, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(YuklemeKoku, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var kok = Path.GetFullPath(server.MapPath("~" + YuklemeKoku));
            var tamYol = Path.GetFullPath(server.MapPath("~" + url));
            if (!tamYol.StartsWith(kok, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(tamYol))
            {
                File.Delete(tamYol);
            }
        }
    }
}

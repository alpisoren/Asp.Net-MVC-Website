using Microsoft.AspNetCore.Http;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// Resim yükleme: doğrular, kutuya sığdırır (en-boy oranı korunur, küçükler büyütülmez) ve
    /// wwwroot/Uploads/{klasor}/ altına GUID adla kaydeder. Görsel işleme SkiaSharp (MIT) ile yapılır.
    /// </summary>
    public static class ResimHelper
    {
        private const string YuklemeKoku = "/Uploads/";
        private const int MaksimumBoyut = 4 * 1024 * 1024; // 4 MB
        private const int JpegKalitesi = 88;
        private static readonly string[] IzinliUzantilar = { ".jpg", ".jpeg", ".png" };

        /// <summary>wwwroot klasörünün tam yolu (Program.cs açılışta ayarlar; testler geçici klasör verir).</summary>
        public static string KokKlasor { get; set; }

        /// <summary>Başarılıysa site içi URL'yi (/Uploads/...), değilse null döner ve hata mesajını doldurur.</summary>
        public static string Kaydet(IFormFile dosya, string klasor, int genislik, int yukseklik, out string hata)
        {
            hata = null;

            if (dosya == null || dosya.Length == 0)
            {
                hata = "Dosya seçilmedi.";
                return null;
            }

            if (dosya.Length > MaksimumBoyut)
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

            SKBitmap kaynak;
            using (var akis = dosya.OpenReadStream())
            {
                kaynak = SKBitmap.Decode(akis); // içerik gerçekten resim değilse null
            }
            if (kaynak == null)
            {
                hata = "Geçerli bir resim dosyası değil.";
                return null;
            }

            using (kaynak)
            {
                var oran = Math.Min(1.0, Math.Min((double)genislik / kaynak.Width, (double)yukseklik / kaynak.Height));
                var yeniGenislik = Math.Max(1, (int)Math.Round(kaynak.Width * oran));
                var yeniYukseklik = Math.Max(1, (int)Math.Round(kaynak.Height * oran));

                var dosyaAdi = Guid.NewGuid().ToString("N") + uzanti;
                var url = YuklemeKoku + klasor + "/" + dosyaAdi;
                var hedefKlasor = Path.Combine(KokKlasorGerekli(), "Uploads", klasor);
                Directory.CreateDirectory(hedefKlasor); // ilk yüklemede klasör yoksa oluştur

                var bicim = uzanti == ".png" ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
                using (var hedef = oran < 1.0
                           ? kaynak.Resize(new SKImageInfo(yeniGenislik, yeniYukseklik), new SKSamplingOptions(SKCubicResampler.Mitchell))
                           : null)
                using (var resim = SKImage.FromBitmap(hedef ?? kaynak))
                using (var veri = resim.Encode(bicim, JpegKalitesi))
                using (var cikis = File.Create(Path.Combine(hedefKlasor, dosyaAdi)))
                {
                    veri.SaveTo(cikis);
                }
                return url;
            }
        }

        /// <summary>Sadece /Uploads/ altındaki dosyaları siler; boş ya da dışarıyı gösteren yolları yok sayar.</summary>
        public static void Sil(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(YuklemeKoku, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var kok = Path.GetFullPath(Path.Combine(KokKlasorGerekli(), "Uploads")) + Path.DirectorySeparatorChar;
            var tamYol = Path.GetFullPath(Path.Combine(KokKlasorGerekli(), url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
            if (!tamYol.StartsWith(kok, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(tamYol))
            {
                File.Delete(tamYol);
            }
        }

        private static string KokKlasorGerekli()
        {
            return KokKlasor ?? throw new InvalidOperationException("ResimHelper.KokKlasor ayarlanmamış.");
        }
    }
}

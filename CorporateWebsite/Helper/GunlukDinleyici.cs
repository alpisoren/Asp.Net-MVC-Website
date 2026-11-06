using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// Trace çıktısını App_Data/Logs altına günlük dosyalara yazar ve kendini temizler:
    /// <list type="bullet">
    /// <item>Her gün yeni dosya: hatalar-2026-11-06.log</item>
    /// <item>Bir günün dosyası <see cref="MaksimumBoyut"/>'u aşarsa sonraki parça: hatalar-2026-11-06-2.log</item>
    /// <item><see cref="SaklamaGunu"/> günden eski günlük dosyaları silinir (açılışta ve gün değişiminde).</item>
    /// </list>
    /// </summary>
    public class GunlukDinleyici : TraceListener
    {
        public const string OnEk = "hatalar-";

        private readonly string klasor;
        private readonly Func<DateTime> saat;
        private readonly object kilit = new object();
        private DateTime acikGun = DateTime.MinValue;
        private string acikDosya;

        public long MaksimumBoyut { get; set; }
        public int SaklamaGunu { get; set; }

        public GunlukDinleyici(string klasor, int saklamaGunu = 30, long maksimumBoyut = 5 * 1024 * 1024, Func<DateTime> saat = null)
            : base("gunlukDinleyici")
        {
            this.klasor = klasor;
            this.saat = saat ?? (() => DateTime.Now);
            SaklamaGunu = saklamaGunu;
            MaksimumBoyut = maksimumBoyut;
            Directory.CreateDirectory(klasor);
            EskileriSil();
        }

        /// <summary>Şu an yazılan dosyanın tam yolu.</summary>
        public string GecerliDosya
        {
            get { lock (kilit) { return DosyaSec(); } }
        }

        public override void Write(string message)
        {
            Yaz(message);
        }

        public override void WriteLine(string message)
        {
            Yaz(message + Environment.NewLine);
        }

        private void Yaz(string metin)
        {
            try
            {
                lock (kilit)
                {
                    File.AppendAllText(DosyaSec(), metin, Encoding.UTF8);
                }
            }
            catch (IOException)
            {
                // Günlük yazılamıyorsa uygulama çalışmaya devam etmeli.
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private string DosyaSec()
        {
            var bugun = saat().Date;
            if (bugun != acikGun)
            {
                acikGun = bugun;
                acikDosya = null;
                EskileriSil();
            }
            if (acikDosya == null || Boyut(acikDosya) >= MaksimumBoyut)
            {
                var temel = Path.Combine(klasor, OnEk + bugun.ToString("yyyy-MM-dd"));
                var parca = 1;
                string aday;
                do
                {
                    aday = temel + (parca == 1 ? "" : "-" + parca) + ".log";
                    parca++;
                }
                while (Boyut(aday) >= MaksimumBoyut);
                acikDosya = aday;
            }
            return acikDosya;
        }

        private static long Boyut(string yol)
        {
            var bilgi = new FileInfo(yol);
            return bilgi.Exists ? bilgi.Length : 0;
        }

        private void EskileriSil()
        {
            if (SaklamaGunu <= 0)
            {
                return;
            }
            var sinir = saat().Date.AddDays(-SaklamaGunu);
            try
            {
                foreach (var dosya in Directory.GetFiles(klasor, OnEk + "*.log"))
                {
                    var ad = Path.GetFileNameWithoutExtension(dosya);
                    if (ad.Length < OnEk.Length + 10)
                    {
                        continue;
                    }
                    DateTime gun;
                    if (DateTime.TryParseExact(ad.Substring(OnEk.Length, 10), "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out gun)
                        && gun < sinir)
                    {
                        File.Delete(dosya);
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}

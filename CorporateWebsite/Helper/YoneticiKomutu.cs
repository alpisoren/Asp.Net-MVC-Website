using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using CorporateWebsite.Models.ViewModel;
using CorporateWebsite.Services;
using System;
using System.Linq;
using System.Text;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// "dotnet CorporateWebsite.dll yonetici-ekle &lt;eposta&gt;" — Admin yetkili yönetici oluşturur.
    /// Şifre ekranda gösterilmeden sorulur (ya da girdi yönlendirilmişse tek satır olarak okunur).
    /// Docker: docker compose run --rm -it web yonetici-ekle ornek@alanadi.com
    /// </summary>
    public static class YoneticiKomutu
    {
        public static int Calistir(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // Windows konsolunda Türkçe karakterler
            if (args.Length < 2 || !SpamKorumasi.GecerliEposta(args[1]))
            {
                Console.Error.WriteLine("Kullanım: yonetici-ekle <eposta>");
                return 2;
            }
            var eposta = args[1].Trim();

            using (var db = new KurumsalDBContext())
            {
                if (db.Admin.Any(a => a.Eposta == eposta))
                {
                    Console.Error.WriteLine("Bu e-posta ile bir yönetici zaten var: " + eposta);
                    return 1;
                }

                var sifre = SifreOku("Şifre: ");
                var tekrar = SifreOku("Şifre (tekrar): ");
                var hata = YoneticiServisi.SifreKontrol(sifre, tekrar);
                if (hata != null)
                {
                    Console.Error.WriteLine(hata);
                    return 1;
                }

                db.Admin.Add(new Admin { Eposta = eposta, Sifre = SifreHelper.Hashle(sifre), Yetki = Yetkiler.Admin });
                db.SaveChanges();
            }

            Console.WriteLine("Yönetici oluşturuldu: " + eposta + " (Admin). Panel: /yonetimpaneli");
            return 0;
        }

        private static string SifreOku(string istem)
        {
            Console.Write(istem);
            if (Console.IsInputRedirected)
            {
                var satir = Console.ReadLine() ?? "";
                Console.WriteLine();
                return satir;
            }

            var sb = new StringBuilder();
            while (true)
            {
                var tus = Console.ReadKey(intercept: true);
                if (tus.Key == ConsoleKey.Enter)
                {
                    break;
                }
                if (tus.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                    }
                    continue;
                }
                if (!char.IsControl(tus.KeyChar))
                {
                    sb.Append(tus.KeyChar);
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }
    }
}

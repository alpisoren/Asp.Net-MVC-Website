using System;
using System.Net;
using System.Net.Mail;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// E-posta gönderimi. SMTP ayarları appsettings → "Smtp" bölümünden gelir (bkz. SmtpAyarlari);
    /// şifre depoya girmez: geliştirmede user-secrets, sunucuda ortam değişkeni ya da appsettings.Production.json.
    /// </summary>
    public static class MailHelper
    {
        public static void Gonder(string alici, string konu, string govde, bool html = false, string yanitAdresi = null)
        {
            var ayar = Ayarlar.Smtp;
            if (string.IsNullOrWhiteSpace(ayar.Sunucu) || string.IsNullOrWhiteSpace(ayar.Kullanici))
            {
                throw new InvalidOperationException("SMTP ayarları eksik (appsettings → Smtp).");
            }

            using (var mail = new MailMessage())
            using (var smtp = new SmtpClient(ayar.Sunucu, ayar.Port))
            {
                smtp.EnableSsl = ayar.Ssl;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(ayar.Kullanici, ayar.Sifre);

                mail.From = new MailAddress(string.IsNullOrWhiteSpace(ayar.Gonderen) ? ayar.Kullanici : ayar.Gonderen);
                mail.To.Add(alici);
                mail.Subject = TekSatir(konu);
                mail.Body = govde;
                mail.IsBodyHtml = html;

                if (!string.IsNullOrWhiteSpace(yanitAdresi))
                {
                    try
                    {
                        mail.ReplyToList.Add(new MailAddress(yanitAdresi));
                    }
                    catch (FormatException)
                    {
                        // Geçersiz yanıt adresi yok sayılır.
                    }
                }

                smtp.Send(mail);
            }
        }

        private static string TekSatir(string metin)
        {
            return (metin ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
        }
    }
}

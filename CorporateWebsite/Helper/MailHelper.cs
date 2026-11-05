using System.Net.Mail;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// SMTP ayarları (sunucu, port, kullanıcı, şifre, gönderen) Web.config içindeki
    /// system.net/mailSettings/smtp bölümünden okunur. Bu bölüm git'e girmeyen smtp.config dosyasındadır.
    /// </summary>
    public static class MailHelper
    {
        public static void Gonder(string alici, string konu, string govde, bool html = false, string yanitAdresi = null)
        {
            using (var mail = new MailMessage())
            using (var smtp = new SmtpClient())
            {
                // From, smtp.config içindeki "from" niteliğinden gelir.
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
                    catch (System.FormatException)
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

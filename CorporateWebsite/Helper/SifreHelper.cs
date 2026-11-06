using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Helpers;

namespace CorporateWebsite.Helper
{
    public static class SifreHelper
    {
        public const int MinimumUzunluk = 8;

        private static readonly Regex Md5Desen = new Regex("^[0-9a-fA-F]{32}$", RegexOptions.Compiled);

        /// <summary>Şifreyi PBKDF2 (tuzlu, 1000 tur) ile hash'ler.</summary>
        public static string Hashle(string sifre)
        {
            return Crypto.HashPassword(sifre);
        }

        /// <summary>
        /// Şifreyi doğrular. Eski MD5 hash'leri de kabul edilir; bu durumda
        /// yenidenHashle = true döner ve çağıran tarafın hash'i PBKDF2'ye yükseltmesi beklenir.
        /// </summary>
        public static bool Dogrula(string kayitliHash, string sifre, out bool yenidenHashle)
        {
            yenidenHashle = false;
            if (string.IsNullOrEmpty(kayitliHash) || string.IsNullOrEmpty(sifre))
            {
                return false;
            }

            if (Md5Desen.IsMatch(kayitliHash))
            {
                var md5 = Crypto.Hash(sifre, "MD5");
                bool eslesti = SabitZamanliEsit(md5.ToUpperInvariant(), kayitliHash.ToUpperInvariant());
                yenidenHashle = eslesti;
                return eslesti;
            }

            try
            {
                return Crypto.VerifyHashedPassword(kayitliHash, sifre);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>URL'de kullanılabilecek, kriptografik olarak güvenli rastgele token üretir.</summary>
        public static string TokenUret()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        /// <summary>Token'ın veritabanında saklanacak SHA-256 özetini döner.</summary>
        public static string TokenOzeti(string token)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token ?? ""));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static bool SabitZamanliEsit(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            int fark = 0;
            for (int i = 0; i < a.Length; i++)
            {
                fark |= a[i] ^ b[i];
            }
            return fark == 0;
        }
    }
}

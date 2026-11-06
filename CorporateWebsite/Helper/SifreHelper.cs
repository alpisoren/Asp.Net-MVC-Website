using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CorporateWebsite.Helper
{
    /// <summary>
    /// Şifre hash'leme. Kayıtlı biçimler (base64, 49 bayt: [sürüm][16 bayt tuz][32 bayt anahtar]):
    ///  - Sürüm 0x01 (güncel): PBKDF2-HMAC-SHA256, 600.000 tur.
    ///  - Sürüm 0x00 (eski): System.Web.Helpers.Crypto.HashPassword ile aynı — PBKDF2-HMAC-SHA1, 1000 tur.
    ///  - 32 hane onaltılık: çok eski MD5 kayıtları.
    /// Eski biçimler doğrulanır ve çağırana "yeniden hash'le" denir; giriş sırasında güncel biçime yükseltilir.
    /// </summary>
    public static class SifreHelper
    {
        public const int MinimumUzunluk = 8;

        private const byte SurumSha256 = 0x01;
        private const byte SurumEskiSha1 = 0x00;
        private const int TuzUzunlugu = 16;
        private const int AnahtarUzunlugu = 32;
        private const int Sha256Tur = 600_000;
        private const int EskiSha1Tur = 1000;

        private static readonly Regex Md5Desen = new Regex("^[0-9a-fA-F]{32}$", RegexOptions.Compiled);

        /// <summary>Şifreyi güncel biçimde (PBKDF2-HMAC-SHA256, tuzlu) hash'ler.</summary>
        public static string Hashle(string sifre)
        {
            if (sifre == null)
            {
                throw new ArgumentNullException(nameof(sifre));
            }
            var tuz = RandomNumberGenerator.GetBytes(TuzUzunlugu);
            var anahtar = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(sifre), tuz, Sha256Tur, HashAlgorithmName.SHA256, AnahtarUzunlugu);
            var cikti = new byte[1 + TuzUzunlugu + AnahtarUzunlugu];
            cikti[0] = SurumSha256;
            Buffer.BlockCopy(tuz, 0, cikti, 1, TuzUzunlugu);
            Buffer.BlockCopy(anahtar, 0, cikti, 1 + TuzUzunlugu, AnahtarUzunlugu);
            return Convert.ToBase64String(cikti);
        }

        /// <summary>
        /// Şifreyi doğrular. Eski biçimler (MD5, PBKDF2-SHA1) de kabul edilir; bu durumda
        /// yenidenHashle = true döner ve çağıran tarafın hash'i güncel biçime yükseltmesi beklenir.
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
                bool eslesti = CryptographicOperations.FixedTimeEquals(
                    Encoding.ASCII.GetBytes(Md5Hex(sifre)), Encoding.ASCII.GetBytes(kayitliHash.ToUpperInvariant()));
                yenidenHashle = eslesti;
                return eslesti;
            }

            byte[] veri;
            try
            {
                veri = Convert.FromBase64String(kayitliHash);
            }
            catch (FormatException)
            {
                return false;
            }
            if (veri.Length != 1 + TuzUzunlugu + AnahtarUzunlugu)
            {
                return false;
            }

            var tuz = new byte[TuzUzunlugu];
            var beklenen = new byte[AnahtarUzunlugu];
            Buffer.BlockCopy(veri, 1, tuz, 0, TuzUzunlugu);
            Buffer.BlockCopy(veri, 1 + TuzUzunlugu, beklenen, 0, AnahtarUzunlugu);
            var sifreBaytlari = Encoding.UTF8.GetBytes(sifre);

            switch (veri[0])
            {
                case SurumSha256:
                    return CryptographicOperations.FixedTimeEquals(beklenen,
                        Rfc2898DeriveBytes.Pbkdf2(sifreBaytlari, tuz, Sha256Tur, HashAlgorithmName.SHA256, AnahtarUzunlugu));

                case SurumEskiSha1:
                    bool dogru = CryptographicOperations.FixedTimeEquals(beklenen,
                        Rfc2898DeriveBytes.Pbkdf2(sifreBaytlari, tuz, EskiSha1Tur, HashAlgorithmName.SHA1, AnahtarUzunlugu));
                    yenidenHashle = dogru;
                    return dogru;

                default:
                    return false;
            }
        }

        /// <summary>URL'de kullanılabilecek, kriptografik olarak güvenli rastgele token üretir.</summary>
        public static string TokenUret()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        /// <summary>Token'ın veritabanında saklanacak SHA-256 özetini döner.</summary>
        public static string TokenOzeti(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token ?? ""))).ToLowerInvariant();
        }

        /// <summary>Eski MD5 kayıtlarıyla aynı biçim (büyük harf onaltılık).</summary>
        internal static string Md5Hex(string metin)
        {
#pragma warning disable CA5351 // yalnızca eski kayıtları doğrulamak için
            return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(metin)));
#pragma warning restore CA5351
        }
    }
}

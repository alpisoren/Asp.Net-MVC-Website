using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace CorporateWebsite.Tests
{
    [TestClass]
    public class SifreHelperTests
    {
        [TestMethod]
        public void Hashle_ve_Dogrula_dogru_sifreyi_kabul_eder()
        {
            var hash = SifreHelper.Hashle("GucluSifre123");
            bool yukselt;

            Assert.IsTrue(SifreHelper.Dogrula(hash, "GucluSifre123", out yukselt));
            Assert.IsFalse(yukselt, "PBKDF2 hash'i yükseltme gerektirmemeli");
        }

        [TestMethod]
        public void Dogrula_yanlis_sifreyi_reddeder()
        {
            var hash = SifreHelper.Hashle("GucluSifre123");
            bool yukselt;

            Assert.IsFalse(SifreHelper.Dogrula(hash, "gucluSifre123", out yukselt));
            Assert.IsFalse(SifreHelper.Dogrula(hash, "", out yukselt));
            Assert.IsFalse(SifreHelper.Dogrula(hash, null, out yukselt));
        }

        [TestMethod]
        public void Hashle_ayni_sifre_icin_her_seferinde_farkli_tuzlu_hash_uretir()
        {
            Assert.AreNotEqual(SifreHelper.Hashle("ayni-sifre"), SifreHelper.Hashle("ayni-sifre"));
        }

        [TestMethod]
        public void Dogrula_eski_MD5_hashini_kabul_eder_ve_yukseltme_ister()
        {
            var eskiHash = EskiMd5_eskiSifre;
            bool yukselt;

            Assert.IsTrue(SifreHelper.Dogrula(eskiHash, "eskiSifre", out yukselt));
            Assert.IsTrue(yukselt);
            Assert.IsTrue(SifreHelper.Dogrula(eskiHash.ToLowerInvariant(), "eskiSifre", out yukselt), "Büyük/küçük harf farkı önemsiz olmalı");
        }

        [TestMethod]
        public void Dogrula_eski_MD5_hashinde_yanlis_sifreyi_reddeder()
        {
            var eskiHash = EskiMd5_eskiSifre;
            bool yukselt;

            Assert.IsFalse(SifreHelper.Dogrula(eskiHash, "baskaSifre", out yukselt));
            Assert.IsFalse(yukselt);
        }

        [TestMethod]
        public void Dogrula_bozuk_hash_icin_hata_firlatmaz_false_doner()
        {
            bool yukselt;
            Assert.IsFalse(SifreHelper.Dogrula("bu-gecerli-bir-hash-degil", "x", out yukselt));
            Assert.IsFalse(SifreHelper.Dogrula(null, "x", out yukselt));
        }

        [TestMethod]
        public void TokenUret_url_guvenli_ve_benzersizdir()
        {
            var tokenlar = new HashSet<string>();
            for (int i = 0; i < 200; i++)
            {
                var t = SifreHelper.TokenUret();
                StringAssert.Matches(t, new Regex("^[A-Za-z0-9_-]{43}$"));
                Assert.IsTrue(tokenlar.Add(t), "Token tekrar etmemeli");
            }
        }

        [TestMethod]
        public void TokenOzeti_deterministik_64_karakter_hex()
        {
            var a = SifreHelper.TokenOzeti("abc");
            Assert.AreEqual(a, SifreHelper.TokenOzeti("abc"));
            Assert.AreNotEqual(a, SifreHelper.TokenOzeti("abd"));
            StringAssert.Matches(a, new Regex("^[0-9a-f]{64}$"));
            // SHA-256("abc") bilinen değer
            Assert.AreEqual("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", a);
        }
        // ------------------------------------------------------------------------------------------------
        // .NET Framework sürümündeki System.Web.Helpers.Crypto ile ÜRETİLMİŞ gerçek değerler (6 Kasım 2026).
        // Taşımadan sonra mevcut yönetici şifrelerinin çalışmaya devam ettiğinin kanıtı.
        private const string EskiMd5_eskiSifre = "D7723FBB12FCA136590DCA306BE02F83";

        private static readonly (string Sifre, string Hash)[] EskiPbkdf2Ornekleri =
        {
            ("GucluSifre123", "AETu0O/P+mb2cMKXTvGkw5A4C83rFaalbE1PDluK9sYm32kqHb39BIC3PYxE1FbcJw=="),
            ("Türkçe-Şifre-ığü", "AMIGt5QE5YT402Vqx8EdnlJGqjwPa5hhDzEsA4M4cSnWmpy01NnyIBt0+SXv5hQiwg=="),
            ("a", "AIISgPzXrj8c9cYUAxeyH7qUwYT4BXSz8plq9/vK8SHUnNuSslvAklE4pWr91ziiWg=="),
        };

        [TestMethod]
        public void Dogrula_System_Web_Helpers_ile_uretilmis_eski_hashleri_kabul_eder_ve_yukseltme_ister()
        {
            foreach (var (sifre, hash) in EskiPbkdf2Ornekleri)
            {
                Assert.IsTrue(SifreHelper.Dogrula(hash, sifre, out var yukselt), sifre);
                Assert.IsTrue(yukselt, "Eski PBKDF2-SHA1 kaydı güncel biçime yükseltilmeli: " + sifre);
                Assert.IsFalse(SifreHelper.Dogrula(hash, sifre + "x", out yukselt), "yanlış şifre: " + sifre);
                Assert.IsFalse(yukselt);
            }
        }

        [TestMethod]
        public void Hashle_guncel_bicimi_uretir_ve_eski_alan_uzunluguna_sigar()
        {
            var hash = SifreHelper.Hashle("GucluSifre123");
            var veri = Convert.FromBase64String(hash);
            Assert.AreEqual(49, veri.Length);
            Assert.AreEqual(0x01, veri[0], "Sürüm 1: PBKDF2-HMAC-SHA256");
            Assert.IsTrue(hash.Length <= 100, "Admin.Sifre alanı 100 karakter");
        }
    }
}

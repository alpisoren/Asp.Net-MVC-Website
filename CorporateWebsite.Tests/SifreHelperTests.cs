using CorporateWebsite.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Helpers;

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
            var eskiHash = Crypto.Hash("eskiSifre", "MD5");
            bool yukselt;

            Assert.IsTrue(SifreHelper.Dogrula(eskiHash, "eskiSifre", out yukselt));
            Assert.IsTrue(yukselt);
            Assert.IsTrue(SifreHelper.Dogrula(eskiHash.ToLowerInvariant(), "eskiSifre", out yukselt), "Büyük/küçük harf farkı önemsiz olmalı");
        }

        [TestMethod]
        public void Dogrula_eski_MD5_hashinde_yanlis_sifreyi_reddeder()
        {
            var eskiHash = Crypto.Hash("eskiSifre", "MD5");
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
    }
}

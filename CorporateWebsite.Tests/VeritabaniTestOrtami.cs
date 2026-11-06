using CorporateWebsite.Models.DataContext;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;

namespace CorporateWebsite.Tests
{
    /// <summary>
    /// Veritabanı testleri için geçici LocalDB veritabanı (KurumsalBirimTest).
    /// İlk kullanımda %TEMP%\KurumsalBirimTest altında sıfırdan oluşturulur ve TÜM migration'lar uygulanır;
    /// test çalıştırması bitince silinir. Geliştiricinin KurumsalDB veritabanına dokunulmaz.
    /// </summary>
    [TestClass]
    public class VeritabaniTestOrtami
    {
        public const string VeritabaniAdi = "KurumsalBirimTest";
        private static readonly object kilit = new object();
        private static bool hazir;

        private static string Klasor
        {
            get { return Path.Combine(Path.GetTempPath(), VeritabaniAdi); }
        }

        public static string BaglantiMetni
        {
            get
            {
                return "Server=(localdb)\\MSSQLLocalDB;Database=" + VeritabaniAdi
                       + ";AttachDbFilename=" + Path.Combine(Klasor, VeritabaniAdi + ".mdf")
                       + ";Integrated Security=True;Connect Timeout=60;TrustServerCertificate=True";
            }
        }

        public static void Hazirla()
        {
            lock (kilit)
            {
                if (hazir)
                {
                    return;
                }

                Directory.CreateDirectory(Klasor);
                Sil();
                KurumsalDBContext.BaglantiMetni = BaglantiMetni;

                // Boş veritabanına bütün migration'lar uygulanır (Baslangic dahil).
                new DbMigrator(new Migrations.Configuration()).Update();

                // Testlerde EF yeniden başlatıcı çalıştırmasın; şema migration'larla hazır.
                Database.SetInitializer<KurumsalDBContext>(null);
                hazir = true;
            }
        }

        public static KurumsalDBContext YeniBaglam()
        {
            Hazirla();
            return new KurumsalDBContext();
        }

        [AssemblyCleanup]
        public static void Temizle()
        {
            if (!hazir)
            {
                return;
            }
            SqlConnection.ClearAllPools();
            Sil();
        }

        private static void Sil()
        {
            var master = new SqlConnectionStringBuilder(BaglantiMetni) { InitialCatalog = "master", AttachDBFilename = "" };
            using (var b = new SqlConnection(master.ConnectionString))
            {
                b.Open();
                using (var k = b.CreateCommand())
                {
                    k.CommandText =
                        "IF DB_ID(N'" + VeritabaniAdi + "') IS NOT NULL BEGIN " +
                        "ALTER DATABASE [" + VeritabaniAdi + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                        "DROP DATABASE [" + VeritabaniAdi + "]; END";
                    k.ExecuteNonQuery();
                }
            }
            if (Directory.Exists(Klasor))
            {
                foreach (var dosya in Directory.GetFiles(Klasor, VeritabaniAdi + "*"))
                {
                    File.Delete(dosya);
                }
            }
        }
    }
}

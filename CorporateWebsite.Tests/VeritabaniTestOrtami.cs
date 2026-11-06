using CorporateWebsite.Models.DataContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;

namespace CorporateWebsite.Tests
{
    /// <summary>
    /// Veritabanı testleri için geçici LocalDB veritabanı (App.config: KurumsalBirimTest).
    /// İlk kullanımda %TEMP%\KurumsalBirimTest altında sıfırdan oluşturulur ve TÜM migration'lar uygulanır;
    /// test çalıştırması bitince silinir. Geliştiricinin KurumsalDB veritabanına dokunulmaz.
    /// </summary>
    [TestClass]
    public class VeritabaniTestOrtami
    {
        public const string VeritabaniAdi = "KurumsalBirimTest";
        private static readonly object kilit = new object();
        private static bool hazir;

        public static string BaglantiMetni
        {
            get { return ConfigurationManager.ConnectionStrings["KurumsalDB"].ConnectionString; }
        }

        public static void Hazirla()
        {
            lock (kilit)
            {
                if (hazir)
                {
                    return;
                }

                var klasor = Path.Combine(Path.GetTempPath(), VeritabaniAdi);
                Directory.CreateDirectory(klasor);
                AppDomain.CurrentDomain.SetData("DataDirectory", klasor);

                Sil(klasor);

                // Boş veritabanına bütün migration'lar uygulanır (Baslangic dahil).
                var migrator = new DbMigrator(new Migrations.Configuration());
                migrator.Update();

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
            Sil(Path.Combine(Path.GetTempPath(), VeritabaniAdi));
        }

        private static void Sil(string klasor)
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
            foreach (var dosya in Directory.GetFiles(klasor, VeritabaniAdi + "*"))
            {
                File.Delete(dosya);
            }
        }
    }
}

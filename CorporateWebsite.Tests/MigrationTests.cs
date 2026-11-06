using CorporateWebsite.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Migrations;
using System.Linq;

namespace CorporateWebsite.Tests
{
    [TestClass]
    [TestCategory("Veritabani")]
    public class MigrationTests
    {
        [ClassInitialize]
        public static void Baslat(TestContext _)
        {
            VeritabaniTestOrtami.Hazirla();
        }

        [TestMethod]
        public void Tum_migrationlar_bos_veritabanina_uygulanir_ve_model_ile_uyumludur()
        {
            var migrator = new DbMigrator(new Configuration());
            CollectionAssert.AreEqual(migrator.GetLocalMigrations().ToList(), migrator.GetDatabaseMigrations().OrderBy(x => x).ToList());
            Assert.AreEqual(0, migrator.GetPendingMigrations().Count());

            using (var db = VeritabaniTestOrtami.YeniBaglam())
            {
                // Model, son migration'ın şemasıyla birebir aynı olmalı (aksi halde Add-Migration unutulmuştur).
                Assert.IsTrue(db.Database.CompatibleWithModel(true), "Model değişmiş ama migration eklenmemiş");
            }
        }

        [TestMethod]
        public void IletisimEposta_migrationi_Fax_kolonundaki_adresi_korur()
        {
            var migrator = new DbMigrator(new Configuration());
            var oncesi = migrator.GetLocalMigrations().Last(x => x.EndsWith("_GirisKilidi"));
            migrator.Update(oncesi); // IletisimEposta geri alınır: kolon yeniden "Fax"
            try
            {
                using (var db = VeritabaniTestOrtami.YeniBaglam())
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO Iletisim (Adres, Fax) VALUES (N'migration testi', N'bilgi@ornek.com')");
                }
            }
            finally
            {
                new DbMigrator(new Configuration()).Update();
            }

            using (var db = VeritabaniTestOrtami.YeniBaglam())
            {
                var kayit = db.Iletisim.Single(x => x.Adres == "migration testi");
                Assert.AreEqual("bilgi@ornek.com", kayit.Eposta);
                db.Iletisim.Remove(kayit);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void Otomatik_migration_doneminden_kalan_veritabani_Baslangica_baglanir()
        {
            using (var db = VeritabaniTestOrtami.YeniBaglam())
            {
                // Eski veritabanını taklit et: Baslangic kaydı yok, onun yerine otomatik bir kayıt var.
                db.Database.ExecuteSqlCommand(
                    "INSERT INTO __MigrationHistory (MigrationId, ContextKey, Model, ProductVersion) " +
                    "SELECT '201901010000000_AutomaticMigration', ContextKey, Model, ProductVersion FROM __MigrationHistory WHERE MigrationId = @p0",
                    MevcutVeritabaniUyumu.BaslangicId);
                db.Database.ExecuteSqlCommand("DELETE FROM __MigrationHistory WHERE MigrationId = @p0", MevcutVeritabaniUyumu.BaslangicId);

                MevcutVeritabaniUyumu.Hazirla();
                MevcutVeritabaniUyumu.Hazirla(); // ikinci çağrı bir şey değiştirmemeli

                var sayi = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM __MigrationHistory WHERE MigrationId = @p0", MevcutVeritabaniUyumu.BaslangicId).Single();
                Assert.AreEqual(1, sayi);

                var migrator = new DbMigrator(new Configuration());
                Assert.AreEqual(0, migrator.GetPendingMigrations().Count(), "Baslangic tekrar uygulanmaya çalışılmamalı");

                db.Database.ExecuteSqlCommand("DELETE FROM __MigrationHistory WHERE MigrationId = '201901010000000_AutomaticMigration'");
            }
        }
    }
}

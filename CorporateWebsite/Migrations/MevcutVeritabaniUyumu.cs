using System;
using System.Data.Common;
using CorporateWebsite.Models.DataContext;

namespace CorporateWebsite.Migrations
{
    /// <summary>
    /// Proje önceden otomatik migration ile çalışıyordu; tablolar "Baslangic" migration'ı olmadan oluşmuş
    /// veritabanları var. Böyle bir veritabanında Baslangic'i yeniden çalıştırmak "tablo zaten var" hatası verir.
    /// Bu sınıf açılışta bir kez bakar: tablolar varsa ama Baslangic geçmişte yoksa, son otomatik kaydın model
    /// anlık görüntüsünü kopyalayarak Baslangic'i "uygulanmış" olarak işaretler. Yeni (boş) veritabanında hiçbir şey yapmaz.
    /// </summary>
    /// <remarks>
    /// Bilerek ham ADO.NET bağlantısı kullanılır: DbContext üzerinden sorgu çalıştırmak EF'in uygulama başına bir kez
    /// yapılan veritabanı başlatmasını tetikler ve ardından kurulan MigrateDatabaseToLatestVersion hiç çalışmaz.
    /// </remarks>
    public static class MevcutVeritabaniUyumu
    {
        public const string BaslangicId = "202611052225471_Baslangic";

        public static void Hazirla()
        {
            using (var db = new KurumsalDBContext())
            {
                // Database.Exists ve Database.Connection başlatmayı tetiklemez.
                if (!db.Database.Exists())
                {
                    return;
                }

                var baglanti = db.Database.Connection;
                baglanti.Open();
                try
                {
                    if (Sayi(baglanti, "SELECT CASE WHEN OBJECT_ID(N'dbo.__MigrationHistory') IS NULL OR OBJECT_ID(N'dbo.Admin') IS NULL THEN 0 ELSE 1 END") == 0)
                    {
                        return;
                    }
                    if (Sayi(baglanti, "SELECT COUNT(*) FROM dbo.__MigrationHistory WHERE MigrationId = @id", BaslangicId) > 0)
                    {
                        return;
                    }
                    using (var komut = baglanti.CreateCommand())
                    {
                        komut.CommandText =
                            "INSERT INTO dbo.__MigrationHistory (MigrationId, ContextKey, Model, ProductVersion) " +
                            "SELECT TOP 1 @id, ContextKey, Model, ProductVersion FROM dbo.__MigrationHistory " +
                            "WHERE ContextKey = @anahtar ORDER BY MigrationId DESC";
                        Parametre(komut, "@id", BaslangicId);
                        Parametre(komut, "@anahtar", typeof(Configuration).FullName);
                        komut.ExecuteNonQuery();
                    }
                }
                finally
                {
                    baglanti.Close();
                }
            }
        }

        private static int Sayi(DbConnection baglanti, string sql, string id = null)
        {
            using (var komut = baglanti.CreateCommand())
            {
                komut.CommandText = sql;
                if (id != null)
                {
                    Parametre(komut, "@id", id);
                }
                return Convert.ToInt32(komut.ExecuteScalar());
            }
        }

        private static void Parametre(DbCommand komut, string ad, object deger)
        {
            var p = komut.CreateParameter();
            p.ParameterName = ad;
            p.Value = deger;
            komut.Parameters.Add(p);
        }
    }
}

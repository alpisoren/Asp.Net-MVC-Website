using System.Data.Entity;
using System.Linq;
using CorporateWebsite.Models.DataContext;

namespace CorporateWebsite.Migrations
{
    /// <summary>
    /// Proje önceden otomatik migration ile çalışıyordu; tablolar "Baslangic" migration'ı olmadan oluşmuş
    /// veritabanları var. Böyle bir veritabanında Baslangic'i yeniden çalıştırmak "tablo zaten var" hatası verir.
    /// Bu sınıf açılışta bir kez bakar: tablolar varsa ama Baslangic geçmişte yoksa, son otomatik kaydın model
    /// anlık görüntüsünü kopyalayarak Baslangic'i "uygulanmış" olarak işaretler. Yeni (boş) veritabanında hiçbir şey yapmaz.
    /// </summary>
    public static class MevcutVeritabaniUyumu
    {
        public const string BaslangicId = "202611052225471_Baslangic";

        public static void Hazirla()
        {
            // Bu kontrol sırasında başlatıcı çalışmasın (Global.asax ardından MigrateDatabaseToLatestVersion'ı kurar).
            Database.SetInitializer<KurumsalDBContext>(null);

            using (var db = new KurumsalDBContext())
            {
                if (!db.Database.Exists())
                {
                    return;
                }

                bool gecmisVar = db.Database.SqlQuery<int>(
                    "SELECT CASE WHEN OBJECT_ID(N'dbo.__MigrationHistory') IS NULL THEN 0 ELSE 1 END").Single() == 1;
                bool tablolarVar = db.Database.SqlQuery<int>(
                    "SELECT CASE WHEN OBJECT_ID(N'dbo.Admin') IS NULL THEN 0 ELSE 1 END").Single() == 1;
                if (!gecmisVar || !tablolarVar)
                {
                    return;
                }

                bool baslangicVar = db.Database.SqlQuery<int>(
                    "SELECT COUNT(*) FROM dbo.__MigrationHistory WHERE MigrationId = @p0", BaslangicId).Single() > 0;
                if (baslangicVar)
                {
                    return;
                }

                db.Database.ExecuteSqlCommand(
                    "INSERT INTO dbo.__MigrationHistory (MigrationId, ContextKey, Model, ProductVersion) " +
                    "SELECT TOP 1 @p0, ContextKey, Model, ProductVersion FROM dbo.__MigrationHistory " +
                    "WHERE ContextKey = @p1 ORDER BY MigrationId DESC",
                    BaslangicId, typeof(Configuration).FullName);
            }
        }
    }
}

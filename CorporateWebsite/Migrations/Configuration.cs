namespace CorporateWebsite.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<CorporateWebsite.Models.DataContext.KurumsalDBContext>
    {
        public Configuration()
        {
            // Şema değişiklikleri yalnızca kod tabanlı migration ile yapılır (Add-Migration <Ad>).
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(CorporateWebsite.Models.DataContext.KurumsalDBContext context)
        {
            // Başlangıç verisi yok; ilk yönetici README'deki SQL ile eklenir.
        }
    }
}

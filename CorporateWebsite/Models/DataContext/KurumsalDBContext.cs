using CorporateWebsite.Models.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace CorporateWebsite.Models.DataContext
{
    /// <summary>
    /// Entity Framework 6 bağlamı. Microsoft.Data.SqlClient sağlayıcısını kullanır (MicrosoftSqlDbConfiguration).
    /// Bağlantı dizesi açılışta appsettings.json → ConnectionStrings:KurumsalDB'den <see cref="BaglantiMetni"/>'ne yazılır.
    /// </summary>
    [DbConfigurationType(typeof(MicrosoftSqlDbConfiguration))]
    public class KurumsalDBContext : DbContext
    {
        /// <summary>Uygulama genelindeki bağlantı dizesi (Program.cs ya da testler ayarlar).</summary>
        public static string BaglantiMetni { get; set; }

        public KurumsalDBContext()
            : base(BaglantiMetni ?? KurumsalDBContextFabrikasi.TasarimBaglantisi())
        {
        }

        public KurumsalDBContext(string baglantiMetni)
            : base(baglantiMetni)
        {
        }

        public DbSet<Admin> Admin { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<Hakkimizda> Hakkimizda { get; set; }
        public DbSet<Hizmet> Hizmet { get; set; }
        public DbSet<Iletisim> Iletisim { get; set; }
        public DbSet<Kategori> Kategori { get; set; }
        public DbSet<Kimlik> Kimlik { get; set; }
        public DbSet<Slider> Slider { get; set; }
        public DbSet<Yorum> Yorum { get; set; }
    }
}

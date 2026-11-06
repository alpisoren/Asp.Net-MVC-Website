using Microsoft.Extensions.Configuration;
using System;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace CorporateWebsite.Models.DataContext
{
    /// <summary>
    /// Tasarım zamanı (EF6 komut satırı aracı: migrations add/list) için bağlam fabrikası.
    /// Uygulama çalışırken kullanılmaz; bağlantıyı sırasıyla ortam değişkeninden (ConnectionStrings__KurumsalDB)
    /// ya da çalışma klasöründeki appsettings.json / appsettings.Development.json'dan okur.
    /// </summary>
    public class KurumsalDBContextFabrikasi : IDbContextFactory<KurumsalDBContext>
    {
        public KurumsalDBContext Create()
        {
            return new KurumsalDBContext(KurumsalDBContext.BaglantiMetni ?? TasarimBaglantisi());
        }

        /// <summary>Uygulama dışında (EF6 aracı) bağlantı dizesi: ortam değişkeni ya da appsettings(.Development).json.</summary>
        public static string TasarimBaglantisi()
        {
            var ayarlar = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            var baglanti = ayarlar.GetConnectionString("KurumsalDB");
            if (string.IsNullOrWhiteSpace(baglanti))
            {
                throw new InvalidOperationException("Tasarım zamanı bağlantısı bulunamadı: CorporateWebsite klasöründe çalıştırın ya da ConnectionStrings__KurumsalDB ortam değişkenini verin.");
            }
            return baglanti;
        }
    }
}

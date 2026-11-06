namespace CorporateWebsite.Helper
{
    /// <summary>appsettings.json → "Site" bölümü.</summary>
    public class SiteAyarlari
    {
        /// <summary>Kalıcı alan adı (ör. https://www.alanadi.com.tr). Canonical, Open Graph, sitemap, RSS, robots.txt bunu kullanır.</summary>
        public string SiteAdresi { get; set; } = "";

        /// <summary>Google Search Console "HTML etiketi" doğrulama kodu.</summary>
        public string GoogleSiteDogrulama { get; set; } = "";

        /// <summary>GA4 ölçüm kimliği (G-XXXXXXXXXX). Boşsa analitik ve çerez bandı kapalı.</summary>
        public string GoogleAnalyticsId { get; set; } = "";

        /// <summary>İletişim formundan gelen mesajların gönderileceği adres.</summary>
        public string IletisimAliciMail { get; set; } = "";

        /// <summary>App_Data/Logs altındaki günlük dosyalarının saklanacağı gün (0 = silme).</summary>
        public int HataGunluguSaklamaGun { get; set; } = 30;

        /// <summary>
        /// Uygulama bir ters vekilin (Caddy, Nginx, IIS ARR) arkasındaysa true: X-Forwarded-For/Proto başlıkları
        /// okunur (gerçek ziyaretçi IP'si ve https). Doğrudan internete açıksa false bırakın.
        /// </summary>
        public bool ArkadaVekil { get; set; }
    }

    /// <summary>appsettings.json → "Smtp" bölümü. Şifre depoya girmez: user-secrets ya da ortam değişkeni.</summary>
    public class SmtpAyarlari
    {
        public string Sunucu { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool Ssl { get; set; } = true;
        public string Kullanici { get; set; } = "";
        public string Sifre { get; set; } = "";

        /// <summary>Gönderen, ör. "Kurumsal &lt;bilgi@alanadi.com.tr&gt;". Boşsa Kullanici.</summary>
        public string Gonderen { get; set; } = "";
    }

    /// <summary>Açılışta (Program.cs) yüklenen ayarlar. Görünümler ve statik yardımcılar buradan okur.</summary>
    public static class Ayarlar
    {
        public static SiteAyarlari Site { get; set; } = new SiteAyarlari();
        public static SmtpAyarlari Smtp { get; set; } = new SmtpAyarlari();
    }
}

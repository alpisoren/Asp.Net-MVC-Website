# CorporateWebsite

Alp İŞÖREN

**ASP.NET Core (.NET 10)** ile yazılmış kurumsal web sitesi ve yönetim paneli. Windows'ta ve Linux'ta (Docker) çalışır.

- **Ziyaretçi sitesi:** Ana sayfa, Hakkımızda, Hizmetlerimiz, Blog (kategoriler, etiketler, arama, yorumlar, RSS), İletişim
- **Yönetim paneli** (`/yonetimpaneli`): içerik yönetimi, yorum onayı, site kimliği, yöneticiler (Admin / Editör)

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Geliştirme ortamı: **Visual Studio 2026** (ASP.NET ve web geliştirme iş yükü), VS Code ya da Rider. *Visual Studio 2022 .NET 10 projelerini açamaz.*
- SQL Server: geliştirmede **LocalDB** (Visual Studio ile gelir), canlıda SQL Server / SQL Server Express (Docker'da hazır)

## Geliştirme ortamında çalıştırma

1. Depoyu klonlayın, `CorporateWebsite/CorporateWebsite.sln` çözümünü açın (ya da `dotnet build`).
2. **Veritabanı:** `CorporateWebsite/appsettings.Development.json` LocalDB'yi (`KurumsalDB`) gösterir; değiştirmeniz gerekmez.
3. **E-posta (SMTP) şifresi depoya girmez**, .NET'in kullanıcıya özel *user-secrets* deposunda tutulur:
   ```powershell
   cd CorporateWebsite
   dotnet user-secrets set "Smtp:Kullanici" "ornek@yandex.com"
   dotnet user-secrets set "Smtp:Sifre" "<yandex-uygulama-sifresi>"
   dotnet user-secrets set "Smtp:Gonderen" "Kurumsal <ornek@yandex.com>"
   ```
   Yandex için hesapta **Tüm ayarlar → Posta programları** sayfasında IMAP ve "Uygulama şifreleri" açık olmalı; şifre olarak **Yandex ID → Güvenlik → Uygulama şifreleri → Posta** ile oluşturulan 16 harflik uygulama şifresi kullanılır.
4. `dotnet run --project CorporateWebsite` (ya da F5). Veritabanı ve tablolar açılışta `Migrations/` altındaki migration'larla oluşturulur/güncellenir.

### İlk yönetici hesabı

Boş bir veritabanında yönetici yoktur. Uygulamanın kendi komutuyla oluşturun (şifre ekranda gösterilmeden sorulur):

```powershell
dotnet run --project CorporateWebsite -- yonetici-ekle ornek@alanadi.com
```

Sonraki yöneticileri panelden (**Yöneticiler → Yeni Yönetici**) ekleyebilirsiniz. Şifreler PBKDF2-HMAC-SHA256 (600.000 tur) ile saklanır; .NET Framework sürümünden kalan eski kayıtlar (PBKDF2-SHA1, MD5) ilk başarılı girişte otomatik yükseltilir.

## Ayarlar (`appsettings.json`)

| Ayar | Ne işe yarar |
|---|---|
| `ConnectionStrings:KurumsalDB` | SQL Server bağlantısı. Microsoft.Data.SqlClient kullanıldığı için yerel/self-signed sertifikada `TrustServerCertificate=True` ekleyin |
| `Site:SiteAdresi` | Kalıcı alan adı (ör. `https://www.alanadi.com.tr`). Canonical, Open Graph, sitemap, RSS ve robots.txt bunu kullanır. **Canlıda mutlaka doldurun** |
| `Site:IletisimAliciMail` | İletişim formundan gelen mesajların gideceği adres |
| `Site:GoogleSiteDogrulama` | Google Search Console "HTML etiketi" doğrulama kodu |
| `Site:GoogleAnalyticsId` | GA4 ölçüm kimliği (`G-XXXXXXXXXX`). Boşsa analitik ve çerez bandı tamamen kapalı |
| `Site:HataGunluguSaklamaGun` | `App_Data/Logs` günlüklerinin saklanacağı gün (varsayılan 30) |
| `Site:ArkadaVekil` | Uygulama Caddy/Nginx/IIS gibi bir ters vekilin arkasındaysa `true` (gerçek IP ve https) |
| `Smtp:*` | `Sunucu`, `Port`, `Ssl`, `Kullanici`, `Sifre`, `Gonderen` |

Gizli değerler (`Smtp:Sifre`, bağlantı şifresi) **depoya girmez**: geliştirmede user-secrets, sunucuda ortam değişkeni (`Smtp__Sifre`, `ConnectionStrings__KurumsalDB`) ya da git'e eklenmeyen `appsettings.Production.json`.

## Linux sunucuda yayın (Docker) — önerilen

Depo kökündeki `docker-compose.yml` siteyi, **SQL Server 2022 Express**'i ve otomatik HTTPS veren **Caddy**'yi tek komutla kurar.

```bash
# Ubuntu 24.04 VPS (2 vCPU / 4 GB RAM yeterli); Docker kurulu olmalı: https://docs.docker.com/engine/install/ubuntu/
git clone <depo-adresi> kurumsal && cd kurumsal
cp .env.example .env      # ALAN_ADI, SQL_SA_SIFRE, SMTP_* değerlerini doldurun
docker compose up -d --build
docker compose run --rm -it web yonetici-ekle ornek@alanadi.com
```

- Alan adının DNS **A kayıtları** (`@` ve `www`) sunucu IP'sini göstermeli; sertifikayı Caddy kendisi alır ve yeniler.
- Kalıcı veriler Docker volume'larında: veritabanı (`sql-verisi`), yüklenen resimler (`yuklemeler`), günlükler ve oturum şifreleme anahtarları (`uygulama-verisi`).
- Yedek: `docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SQL_SA_SIFRE" -C -Q "BACKUP DATABASE KurumsalDB TO DISK='/var/opt/mssql/yedek/KurumsalDB.bak' WITH INIT"` → dosya sunucuda `./yedek/` altında; sunucu dışına kopyalayın. `yuklemeler` volume'unu da yedekleyin.
- Güncelleme: `git pull && docker compose up -d --build` (migration'lar açılışta uygulanır; önce yedek alın).
- Mevcut veriyi taşımak: yerel `KurumsalDB`'den `.bak` alıp `./yedek/`'e koyun, `RESTORE DATABASE` ile geri yükleyin; `wwwroot/Uploads` içeriğini `yuklemeler` volume'una kopyalayın.

## Windows sunucuda yayın (IIS)

1. Sunucuya **ASP.NET Core 10 Hosting Bundle** ve SQL Server Express kurun.
2. `dotnet publish CorporateWebsite -c Release -o yayin` → klasörü sunucuya kopyalayın (yayın paketi `web.config`'i kendisi üretir).
3. IIS'te site oluşturun; uygulama havuzu **No Managed Code**. `App_Data` ve `wwwroot/Uploads` klasörlerine havuz kimliğine yazma izni verin.
4. Ayarları sunucudaki `appsettings.Production.json`'a ya da ortam değişkenlerine yazın; HTTPS için win-acme (Let's Encrypt).

## Veritabanı değişiklikleri (migration)

Şema yalnızca kod tabanlı migration ile değişir (Entity Framework 6.5, otomatik migration kapalı). Varlığı değiştirip derledikten sonra:

```powershell
.\tools\Migration.ps1 add KisaAd    # ya da Visual Studio → Package Manager Console: Add-Migration KisaAd
.\tools\Migration.ps1 list          # veritabanına uygulanmış migration'lar
```

Üretilen dosyayı gözden geçirin (ör. yeniden adlandırmada EF "ekle + sil" üretir, `RenameColumn` ile değiştirin). Uygulama açılışta bekleyen migration'ları uygular. Otomatik migration döneminden kalan veritabanları ilk açılışta `Baslangic` migration'ına bağlanır (`Migrations/MevcutVeritabaniUyumu.cs`).

## Testler

```powershell
dotnet test CorporateWebsite.Tests
```

MSTest; 77 test. `Veritabani` kategorisindeki testler LocalDB üzerinde `%TEMP%` altında geçici bir veritabanı oluşturur, tüm migration'ları uygular ve sonunda siler; geliştirme veritabanınıza dokunmaz.

## Proje yapısı

| Klasör | İçerik |
|---|---|
| `Program.cs` | Açılış: ayarlar, migration'lar, kimlik doğrulama, hata sayfaları, güvenlik başlıkları, statik dosyalar |
| `Controllers/` | Site (`HomeController`), panel controller'ları, hata sayfaları (`HataController`) |
| `ViewComponents/` | Blog kenar çubuğu parçaları, panel yorum rozeti |
| `Services/` | İş kuralları ve veri erişimi |
| `Migrations/` | Kod tabanlı EF6 migration'ları |
| `Models/` | EF varlıkları, form modelleri, `KurumsalDBContext` |
| `Filters/` | `AdminAuthorize` (yetki), `GuvenlikBasliklari` (CSP) |
| `Helper/` | Şifre, oturum, resim (SkiaSharp), HTML temizleme, SEO, etiket, istek sınırı, günlük, e-posta, ayarlar |
| `wwwroot/Content/SiteLayout/` | Ziyaretçi sitesi teması, yazı tipleri, betikler |
| `wwwroot/Content/AdminLayout/` | Panel teması (`css/panel.css`, `js/admin.js`, `vendor/`) |
| `wwwroot/Uploads/` | Yüklenen resimler (yayın paketine girmez) |
| `Dockerfile`, `docker-compose.yml`, `deploy/Caddyfile` | Linux/Docker yayını |
| `design-system/kurumsal-web/MASTER.md` | Renk, yazı tipi ve bileşen kuralları |

## SEO ve Google araçları

- Her sayfada başlık, açıklama, canonical, Open Graph / Twitter etiketleri; ana sayfada `Organization` + `WebSite` (site içi arama), yazılarda `BlogPosting`, iç sayfalarda `BreadcrumbList` JSON-LD.
- Arama sonuçları `noindex`; olmayan sayfalar tasarımlı ve **gerçek 404** döner; `/Anasayfa` → `/` (301).
- Google Analytics yalnızca ziyaretçi çerez bandında **Kabul et** derse yüklenir (KVKK); reklam izinleri kapalı. Çerez politikası: `/cerez-politikasi`.
- Yayından sonra: Search Console'da alan adını doğrulayın ve `/sitemap.xml`'i gönderin; [PageSpeed Insights](https://pagespeed.web.dev/) ve [Zengin Sonuçlar Testi](https://search.google.com/test/rich-results) ile canlı adresi test edin.

## Güvenlik notları

- Panel oturumu şifreli, `HttpOnly` bir çerezle tutulur (`.KurumsalPanel`, ASP.NET Core cookie authentication). "Beni hatırla" 14 gün; yetki her istekte veritabanından okunur.
- Çerez/CSRF şifreleme anahtarları `App_Data/Keys` altında (Windows'ta DPAPI ile şifreli). Birden fazla sunucuda bu klasör paylaşılmalıdır.
- Canlıda HTTPS zorunlu (HSTS); çerezler yalnızca HTTPS üzerinden gider.
- Art arda 5 hatalı şifre hesabı 10 dakika kilitler; kilit veritabanında tutulur.
- Hatalar `App_Data/Logs/hatalar-YYYY-AA-GG.log` dosyalarına yazılır; dosya başına 5 MB, eski dosyalar silinir.
- Zengin metin editörü [Jodit](https://xdsoft.net/jodit/) (MIT); kaydedilen HTML HtmlSanitizer ile temizlenir. Resim işleme [SkiaSharp](https://github.com/mono/SkiaSharp) (MIT).

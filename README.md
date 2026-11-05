# CorporateWebsite

Alp İŞÖREN

ASP.NET MVC 5 (.NET Framework 4.8) ile yazılmış kurumsal web sitesi ve yönetim paneli.

- **Ziyaretçi sitesi:** Ana sayfa, Hakkımızda, Hizmetlerimiz, Blog (kategoriler, yorumlar), İletişim
- **Yönetim paneli** (`/yonetimpaneli`): içerik yönetimi, yorum onayı, site kimliği, yöneticiler (Admin / Editör)

## Gereksinimler

- Visual Studio 2022 (ASP.NET ve web geliştirme iş yükü)
- .NET Framework 4.8
- SQL Server ya da SQL Server Express LocalDB

## Kurulum

1. Depoyu klonlayın ve `CorporateWebsite/CorporateWebsite.sln` çözümünü Visual Studio ile açın. NuGet paketleri ilk derlemede geri yüklenir.
2. **Veritabanı bağlantısı:** `CorporateWebsite/connectionStrings.config.example` dosyasını aynı klasöre `connectionStrings.config` adıyla kopyalayıp kendi SQL Server bilgilerinizi girin.
3. **E-posta (SMTP):** `CorporateWebsite/smtp.config.example` dosyasını `smtp.config` adıyla kopyalayıp SMTP bilgilerinizi girin.
   - Yandex için hesapta **Tüm ayarlar → Posta programları** sayfasında IMAP ve "Uygulama şifreleri ve OAuth token'ları" açık olmalı; şifre olarak **Yandex ID → Güvenlik → Uygulama şifreleri → Posta** ile oluşturulan 16 harflik uygulama şifresi kullanılır (normal hesap şifresi çalışmaz). Port 587.
   - İletişim formundan gelen mesajların gideceği adres `Web.config` içindeki `IletisimAliciMail` ayarıdır.
4. F5 ile çalıştırın. Veritabanı ve tablolar ilk açılışta Entity Framework migration'larıyla otomatik oluşturulur/güncellenir.

> `connectionStrings.config` ve `smtp.config` gizli bilgi içerir ve **git'e eklenmez** (`.gitignore`). Yayın sunucusunda bu iki dosyayı elle oluşturun.

### İlk yönetici hesabı

Boş bir veritabanında yönetici yoktur. `Admin` tablosuna bir kayıt ekleyin; `Sifre` alanına düz şifre değil, PBKDF2 hash'i yazılmalıdır. Hash'i örneğin PowerShell ile üretebilirsiniz:

```powershell
Add-Type -Path ".\CorporateWebsite\bin\System.Web.Helpers.dll"
[System.Web.Helpers.Crypto]::HashPassword("GüçlüBirŞifre123")
```

```sql
INSERT INTO Admin (Eposta, Sifre, Yetki) VALUES (N'ornek@alanadi.com', N'<üretilen hash>', N'Admin');
```

Sonraki yöneticileri panelden (**Yöneticiler → Yeni Yönetici**) ekleyebilirsiniz. Eski MD5 şifreler ilk başarılı girişte otomatik olarak PBKDF2'ye yükseltilir.

## Testler

Birim testleri `CorporateWebsite.Tests` projesindedir (MSTest). Visual Studio'da **Test → Tüm Testleri Çalıştır** ya da komut satırından:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" .\CorporateWebsite.Tests\bin\Debug\CorporateWebsite.Tests.dll
```

## Proje yapısı

| Klasör | İçerik |
|---|---|
| `Controllers/` | Site (`HomeController`) ve panel controller'ları |
| `Services/` | İş kuralları (yönetici girişi/şifre sıfırlama, blog, hizmet) |
| `Models/Model/` | Entity Framework varlıkları |
| `Models/ViewModel/` | Panel formlarının modelleri |
| `Filters/` | `AdminAuthorize` (yetki), `GuvenlikBasliklari` (CSP) |
| `Helper/` | Şifre, oturum, resim yükleme, HTML temizleme, SEO, istek sınırı yardımcıları |
| `Content/SiteLayout/` | Ziyaretçi sitesi teması (`css/tema.css`, `js/`) |
| `Content/AdminLayout/` | Panel teması (`css/panel.css`, `js/admin.js`, `vendor/`) |
| `design-system/kurumsal-web/MASTER.md` | Renk, yazı tipi ve bileşen kuralları |

## Güvenlik notları

- Panel oturumu şifreli, `HttpOnly` bir cookie ile tutulur (Forms Authentication); uygulama yeniden başlasa da düşmez. "Beni hatırla" 14 gün geçerlidir. Yetki her istekte veritabanından okunur.
- Yayında (Release) HTTPS zorunludur ve cookie'ler yalnızca HTTPS üzerinden gönderilir; sunucuda SSL sertifikası olmalıdır.
- **Birden fazla sunucu / web farm** kullanılacaksa `Web.config`'e sabit bir `<machineKey>` eklenmelidir; aksi halde oturum cookie'leri sunucular arasında çözülemez. Bu anahtar gizlidir, git'e eklemeyin.
- Hatalar `CorporateWebsite/App_Data/Logs/hatalar.log` dosyasına yazılır (dışarıdan erişilemez).
- Zengin metin editörü [Jodit](https://xdsoft.net/jodit/) (MIT). Kaydedilen HTML sunucuda HtmlSanitizer ile temizlenir.

## Geliştirme ipuçları

- **LocalDB "Cannot create file ... KurumsalDB.mdf because it already exists" hatası:** Veritabanı dosyası diskte var ama kullandığınız LocalDB örneğinde kayıtlı değil (genelde LocalDB'nin farklı bir süreçte/oturumda başlatılmasından). Çözüm: SQL Server Object Explorer ya da `sqlcmd` ile aynı örneğe bağlanıp dosyaları ekleyin:
  ```sql
  CREATE DATABASE KurumsalDB ON (FILENAME = N'C:\Users\<kullanıcı>\KurumsalDB.mdf'), (FILENAME = N'C:\Users\<kullanıcı>\KurumsalDB_log.ldf') FOR ATTACH;
  ```
  LocalDB bağlantı cümlesine `Connect Timeout=60` eklemek ilk açılıştaki zaman aşımı kaynaklı benzer hataları önler (örnek dosyada mevcut).

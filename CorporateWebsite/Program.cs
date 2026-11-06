using CorporateWebsite.Filters;
using CorporateWebsite.Helper;
using CorporateWebsite.Migrations;
using CorporateWebsite.Models.DataContext;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.WebEncoders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var ortam = builder.Environment;

// ---------------------------------------------------------------- Ayarlar
Ayarlar.Site = builder.Configuration.GetSection("Site").Get<SiteAyarlari>() ?? new SiteAyarlari();
Ayarlar.Smtp = builder.Configuration.GetSection("Smtp").Get<SmtpAyarlari>() ?? new SmtpAyarlari();
KurumsalDBContext.BaglantiMetni = builder.Configuration.GetConnectionString("KurumsalDB");
if (string.IsNullOrWhiteSpace(KurumsalDBContext.BaglantiMetni))
{
    throw new InvalidOperationException("Bağlantı dizesi yok: appsettings / ortam değişkeni ConnectionStrings__KurumsalDB.");
}
ResimHelper.KokKlasor = ortam.WebRootPath;

// ---------------------------------------------------------------- Günlük (App_Data/Logs/hatalar-YYYY-AA-GG.log)
var veriKlasoru = Path.Combine(ortam.ContentRootPath, "App_Data");
try
{
    Trace.Listeners.Add(new GunlukDinleyici(Path.Combine(veriKlasoru, "Logs"), Ayarlar.Site.HataGunluguSaklamaGun));
    Trace.AutoFlush = true;
}
catch (Exception)
{
    // Günlük açılamazsa uygulama yine de çalışmaya devam etmeli.
}
builder.Logging.AddProvider(new TraceHataGunlukSaglayici());

// ---------------------------------------------------------------- Servisler
builder.Services.AddControllersWithViews(o =>
{
    o.Filters.Add<GuvenlikBasliklariAttribute>();
});
builder.Services.AddOutputCache();

// Türkçe karakterler HTML'e olduğu gibi yazılsın ("Hakk&#x131;m&#x131;zda" yerine "Hakkımızda").
// Kullanıcı girdisi yine kodlanır: <, >, &, " ve ' her zaman kaçışlanır.
builder.Services.Configure<WebEncoderOptions>(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

// Yönetim paneli oturumu: şifreli, HttpOnly çerez. İçinde yalnızca yönetici kimliği var;
// yetki her istekte veritabanından okunur (AdminAuthorizeAttribute).
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = ".KurumsalPanel";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.Cookie.SecurePolicy = ortam.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
        o.ExpireTimeSpan = YoneticiOturumu.OturumSuresi;
        o.SlidingExpiration = true;
        o.LoginPath = "/yonetimpaneli/giris";
    });

builder.Services.AddAntiforgery(o =>
{
    o.Cookie.Name = ".KurumsalForm";
    o.Cookie.SecurePolicy = ortam.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
});

// Çerez/CSRF şifreleme anahtarları diskte: yeniden başlatmada ve container yeniden oluşturulduğunda oturumlar düşmez.
// Birden fazla sunucuda bu klasör paylaşılmalı (eski <machineKey> ayarının karşılığı).
var korumaAnahtarlari = builder.Services.AddDataProtection()
    .SetApplicationName("KurumsalWeb")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(veriKlasoru, "Keys")));
if (OperatingSystem.IsWindows())
{
    korumaAnahtarlari.ProtectKeysWithDpapi(); // Windows'ta anahtarlar diskte şifreli; Linux'ta klasör izinleriyle korunur
}

if (Ayarlar.Site.ArkadaVekil)
{
    builder.Services.Configure<ForwardedHeadersOptions>(o =>
    {
        o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // Vekil aynı makinede ya da aynı Docker ağında; bilinen adres listesi temizlenir.
        o.KnownIPNetworks.Clear();
        o.KnownProxies.Clear();
    });
}

var app = builder.Build();

// ---------------------------------------------------------------- Veritabanı: bekleyen migration'lar
VeritabaniniGuncelle(app.Logger);

// Komut satırı: ilk yöneticiyi oluşturur ve çıkar (web sunucusu başlamaz).
//   dotnet CorporateWebsite.dll yonetici-ekle ornek@alanadi.com
if (args.Length > 0 && args[0] == "yonetici-ekle")
{
    Environment.ExitCode = YoneticiKomutu.Calistir(args);
    return;
}

// ---------------------------------------------------------------- İstek hattı
if (Ayarlar.Site.ArkadaVekil)
{
    app.UseForwardedHeaders();
}

if (ortam.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Hata/Sunucu");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Hata kodları (404, 403 ...) site tasarımındaki sayfalara; durum kodu korunur (soft 404 yok).
app.UseStatusCodePagesWithReExecute("/Hata/Durum/{0}");

// Güvenlik başlıkları (CSP ayrıca GuvenlikBasliklariAttribute'ta, sayfa türüne göre).
app.Use(async (context, next) =>
{
    var b = context.Response.Headers;
    b["X-Frame-Options"] = "SAMEORIGIN";
    b["X-Content-Type-Options"] = "nosniff";
    b["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

// Statik dosyalar 1 yıl önbellekte; kendi CSS/JS dosyalarımız asp-append-version ile sürümlenir.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = c => c.Context.Response.Headers["Cache-Control"] = "public,max-age=31536000"
});

app.UseRouting();
app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// <summary>
/// Açılışta veritabanını son migration'a getirir. Otomatik migration döneminden kalan veritabanları
/// önce Baslangic migration'ına bağlanır (MevcutVeritabaniUyumu).
/// </summary>
static void VeritabaniniGuncelle(ILogger logger)
{
    try
    {
        Database.SetInitializer<KurumsalDBContext>(null);
        MevcutVeritabaniUyumu.Hazirla();
        new DbMigrator(new Configuration()).Update();
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Veritabanı migration'ları uygulanamadı.");
        throw;
    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CorporateWebsite.Tests
{
    /// <summary>Testler için ASP.NET Core bağlamı ve dosya sahteleri.</summary>
    internal static class Sahte
    {
        /// <summary>Çerez kimlik doğrulaması kayıtlı servislerle bir HTTP bağlamı (çıkış/giriş çağrıları çalışsın).</summary>
        public static DefaultHttpContext HttpBaglami(ClaimsPrincipal kullanici = null, string yol = "/", string yontem = "GET")
        {
            var servisler = new ServiceCollection();
            servisler.AddLogging();
            servisler.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            var ctx = new DefaultHttpContext { RequestServices = servisler.BuildServiceProvider() };
            ctx.Request.Method = yontem;
            ctx.Request.Scheme = "http";
            ctx.Request.Host = new HostString("localhost");
            var soru = yol.IndexOf('?');
            ctx.Request.Path = soru < 0 ? yol : yol.Substring(0, soru);
            ctx.Request.QueryString = soru < 0 ? QueryString.Empty : new QueryString(yol.Substring(soru));
            ctx.User = kullanici ?? new ClaimsPrincipal(new ClaimsIdentity()); // anonim
            return ctx;
        }

        /// <summary>Çerezden okunmuş gibi, belirtilen yönetici kimliği talebini taşıyan kullanıcı.</summary>
        public static ClaimsPrincipal Yonetici(string kimlikDegeri)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(Helper.YoneticiOturumu.KimlikTalebi, kimlikDegeri)
            }, CookieAuthenticationDefaults.AuthenticationScheme));
        }

        public static IFormFile Dosya(string ad, byte[] veri, long? bildirilenBoyut = null)
        {
            return new SahteDosya(ad, veri ?? new byte[0], bildirilenBoyut);
        }

        private sealed class SahteDosya : IFormFile
        {
            private readonly byte[] veri;

            public SahteDosya(string ad, byte[] veri, long? bildirilenBoyut)
            {
                FileName = ad;
                Name = "dosya";
                this.veri = veri;
                Length = bildirilenBoyut ?? veri.Length;
            }

            public string ContentType { get; } = "application/octet-stream";
            public string ContentDisposition { get; } = "";
            public IHeaderDictionary Headers { get; } = new HeaderDictionary();
            public long Length { get; }
            public string Name { get; }
            public string FileName { get; }

            public Stream OpenReadStream()
            {
                return new MemoryStream(veri, false);
            }

            public void CopyTo(Stream target)
            {
                target.Write(veri, 0, veri.Length);
            }

            public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
            {
                return target.WriteAsync(veri, 0, veri.Length, cancellationToken);
            }
        }
    }

    /// <summary>TempData'yı bellekte tutar (controller testlerinde TempData["Basari"] vb.).</summary>
    internal sealed class BellekTempDataSaglayici : ITempDataProvider
    {
        private IDictionary<string, object> veri = new Dictionary<string, object>();

        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            return veri;
        }

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
            veri = values;
        }
    }

    internal static class GeciciKlasor
    {
        public static string Olustur()
        {
            var yol = Path.Combine(Path.GetTempPath(), "CorporateWebsiteTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(yol);
            return yol;
        }
    }
}

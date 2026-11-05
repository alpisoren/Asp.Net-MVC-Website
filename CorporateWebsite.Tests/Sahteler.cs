using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace CorporateWebsite.Tests
{
    /// <summary>Testler için basit HttpContext/Session/Server/HttpPostedFile sahteleri.</summary>
    internal class SahteSession : HttpSessionStateBase
    {
        private readonly Dictionary<string, object> degerler = new Dictionary<string, object>();

        public override object this[string name]
        {
            get { object v; return degerler.TryGetValue(name, out v) ? v : null; }
            set { degerler[name] = value; }
        }
    }

    internal class SahteHttpContext : HttpContextBase
    {
        private readonly HttpSessionStateBase session;

        public SahteHttpContext(HttpSessionStateBase session)
        {
            this.session = session;
        }

        public override HttpSessionStateBase Session
        {
            get { return session; }
        }
    }

    /// <summary>"~/..." yollarını verilen kök klasöre eşler.</summary>
    internal class SahteServer : HttpServerUtilityBase
    {
        private readonly string kok;

        public SahteServer(string kok)
        {
            this.kok = kok;
        }

        public override string MapPath(string path)
        {
            var goreli = path.TrimStart('~').TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(kok, goreli);
        }
    }

    internal class SahteDosya : HttpPostedFileBase
    {
        private readonly string ad;
        private readonly Stream icerik;
        private readonly int boyut;

        public SahteDosya(string ad, byte[] veri, int? bildirilenBoyut = null)
        {
            this.ad = ad;
            icerik = new MemoryStream(veri ?? new byte[0]);
            boyut = bildirilenBoyut ?? (veri == null ? 0 : veri.Length);
        }

        public override string FileName
        {
            get { return ad; }
        }

        public override int ContentLength
        {
            get { return boyut; }
        }

        public override Stream InputStream
        {
            get { return icerik; }
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

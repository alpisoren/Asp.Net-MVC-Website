using System;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using CorporateWebsite.Models.DataContext;

namespace CorporateWebsite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            HataGunluguBaslat();

            // Veritabanını açılışta Migrations/ altındaki son kod tabanlı migration'a getir.
            // Otomatik migration döneminden kalan veritabanları önce Baslangic migration'ına bağlanır.
            Migrations.MevcutVeritabaniUyumu.Hazirla();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<KurumsalDBContext, Migrations.Configuration>());

            // Yakalanmayan hatalarda Views/Shared/Error.cshtml gösterilir (customErrors açıkken).
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            GlobalFilters.Filters.Add(new Filters.GuvenlikBasliklariAttribute());

            // Canlı ortamda (Web.config'de debug kapalıyken) tüm istekleri HTTPS'e zorla.
            var derleme = (CompilationSection)ConfigurationManager.GetSection("system.web/compilation");
            if (derleme != null && !derleme.Debug)
            {
                GlobalFilters.Filters.Add(new RequireHttpsAttribute());
            }

            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var hata = Server.GetLastError();
            if (hata == null)
            {
                return;
            }

            // 404'leri günlüğe yazma; gürültü yaratır.
            var httpHata = hata as HttpException;
            if (httpHata != null && httpHata.GetHttpCode() == 404)
            {
                return;
            }

            var url = Context != null && Context.Request != null ? Context.Request.RawUrl : "";
            Trace.TraceError("[{0:u}] {1} -> {2}", DateTime.UtcNow, url, hata);
        }

        private void HataGunluguBaslat()
        {
            try
            {
                // App_Data/Logs/hatalar-YYYY-AA-GG.log: günlük dosya, dosya başına en fazla 5 MB,
                // "HataGunluguSaklamaGun" (varsayılan 30) günden eskiler silinir.
                int gun;
                if (!int.TryParse(ConfigurationManager.AppSettings["HataGunluguSaklamaGun"], out gun))
                {
                    gun = 30;
                }
                Trace.Listeners.Add(new Helper.GunlukDinleyici(HostingEnvironment.MapPath("~/App_Data/Logs"), gun));
                Trace.AutoFlush = true;
            }
            catch (Exception)
            {
                // Günlük açılamazsa uygulama yine de çalışmaya devam etmeli.
            }
        }
    }
}

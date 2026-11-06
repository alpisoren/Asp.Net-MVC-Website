using Microsoft.AspNetCore.Mvc;
using CorporateWebsite.Filters;
using CorporateWebsite.Helper;
using System.Collections.Generic;

namespace CorporateWebsite.Controllers
{
    /// <summary>
    /// Zengin metin editörünün (Jodit) resim yükleme ucu. Yanıt, Jodit'in varsayılan uploader biçimindedir:
    /// { success, data: { baseurl, files[], isImages[], messages[] } }.
    /// </summary>
    [AdminAuthorize]
    public class MedyaController : Controller
    {
        public const string Klasor = "Editor";
        public const int MaksimumKenar = 1600;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Yukle()
        {
            var dosyalar = new List<string>();
            var resimMi = new List<bool>();
            var mesajlar = new List<string>();

            for (int i = 0; i < Request.Form.Files.Count; i++)
            {
                var dosya = Request.Form.Files[i];
                string hata;
                var url = ResimHelper.Kaydet(dosya, Klasor, MaksimumKenar, MaksimumKenar, out hata);
                if (url == null)
                {
                    mesajlar.Add((dosya != null ? dosya.FileName + ": " : "") + hata);
                    continue;
                }
                dosyalar.Add(url);
                resimMi.Add(true);
            }

            if (Request.Form.Files.Count == 0)
            {
                mesajlar.Add("Dosya seçilmedi.");
            }

            // Hata durumunda da 200 dönülür: Jodit mesajı yalnızca başarılı HTTP yanıtından okuyup gösterir.
            bool basarili = dosyalar.Count > 0;
            return Json(new
            {
                success = basarili,
                data = new
                {
                    baseurl = "",
                    files = dosyalar,
                    isImages = resimMi,
                    messages = mesajlar,
                    code = basarili ? 220 : 400
                }
            });
        }
    }
}

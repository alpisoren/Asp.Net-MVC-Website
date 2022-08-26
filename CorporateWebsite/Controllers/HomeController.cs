using CorporateWebsite.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    public class HomeController : Controller
    {


        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Home
        public ActionResult Index()
        {

            FooterLoader();

            return View(db.Slider.ToList());
        }

        public void FooterLoader(){
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            ViewBag.Iletisim = db.Iletisim.ToList().SingleOrDefault();
            ViewBag.Blog = db.Blog.ToList().OrderByDescending(x => x.BlogId);
          
        }
       
        [Obsolete("Silinecek")]
        public ActionResult SliderPartial()
        {
            return View(db.Slider.ToList().OrderByDescending(x=>x.SliderId));
        }

        public ActionResult HizmetPartipal()
        {
            return View(db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId));

        }
        public ActionResult Hakkimizda()
        {
            FooterLoader();
            return View(db.Hakkimizda.FirstOrDefault());

        }

        public ActionResult Hizmetlerimiz()
        {
            FooterLoader();
            return View(db.Hizmet.ToList().OrderByDescending(x => x.HizmetId));


        }

        public ActionResult Iletisim()
        {
            FooterLoader();
            return View(db.Iletisim.ToList().FirstOrDefault());

        }
        [HttpPost]
        public ActionResult Iletisim(string name=null,string email=null,string subject=null,string message=null)
        {
            if (name!=null&& email!=null&&subject!=null&&message!=null)
            {
                //ssl ile ilgili mail gönderme sorunu çözümü :https://www.google.com/settings/security/lesssecureapps


                //WebMail.SmtpServer = "smtp.gmail.com";
                //WebMail.EnableSsl = true;

                //WebMail.UserName = "nasemltd@gmail.com";
                //WebMail.Password = "Nasem.9162036";
                //WebMail.SmtpPort = 587;
                //WebMail.SmtpUseDefaultCredentials = false;
                //WebMail.Send("nasemltd@gmail.com", subject, message);
                //ViewBag.Uyari = "Mesajınız başarı ile gönderilmiştir.";

                //using (MailMessage mail = new MailMessage())
                //{
                //    mail.From = new MailAddress("alpisorenblog@gmail.com");
                //    mail.To.Add("alpisorenblog@gmail.com");
                //    mail.Subject = subject;
                //    mail.Body = message;
                //    mail.IsBodyHtml = false;
                //   // mail.Attachments.Add(new Attachment("C:\\file.Zip"));

                //    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                //    {
                //        smtp.Credentials = new NetworkCredential("alpisorenblog@gmail.com", ".ALEX1907TUNCAY.");
                //        smtp.EnableSsl = true;
                //        smtp.Send(mail);
                //    }
                //}
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("exehiro@yandex.com", "Kurumsal");

                mail.To.Add("exehiro@yandex.com");
                // mail.Bcc.Add("exehiro@yandex.com");

                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = message;


                SmtpClient sc = new SmtpClient();
                sc.Port = 587;
                sc.EnableSsl = true;
                sc.Host = "smtp.yandex.com";
                sc.UseDefaultCredentials = false;
                sc.Credentials = new NetworkCredential("exehiro@yandex.com", "karahirolol123");

                sc.Send(mail);

            }
            else
            {
                ViewBag.Uyarı = "Hata Oluştu Tekrar Deneyiniz.";
            }
            return View();


        }


        public ActionResult Blog()
        {

            FooterLoader();
            return View(db.Blog.Include("Kategori").ToList().OrderByDescending(x => x.BlogId));

        }
        public ActionResult BlogKategoriPartial()
        {

            FooterLoader();
            return PartialView(db.Kategori.Include("Blogs").ToList().OrderByDescending(x => x.KategoriId));

        }
        public ActionResult BlogSonKayitPartial()
        {

            FooterLoader();
            return PartialView(db.Blog.ToList().OrderByDescending(x => x.BlogId));

        }

        public ActionResult BlogDetay(int id)
        {
            FooterLoader();
            return View(db.Blog.Include("Kategori").Where(x => x.BlogId==id).FirstOrDefault());


        }



    }
}
﻿using CorporateWebsite.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace CorporateWebsite.Controllers
{
    public class HomeController : Controller
    {


        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Home
        [Route()]
        [Route("Anasayfa")]
        public ActionResult Index()
        {

            FooterLoader();

            return View(db.Slider.ToList());
        }

        public void FooterLoader(){
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            ViewBag.Iletisim = db.Iletisim.ToList().FirstOrDefault();
            ViewBag.Blog = db.Blog.ToList().OrderByDescending(x => x.BlogId);
          
        }
       
        [Obsolete("Silinecek")]
        public ActionResult SliderPartial()
        {
            return View(db.Slider.ToList().OrderByDescending(x=>x.SliderId));
        }


        [Route("Hizmet")]
        public ActionResult HizmetPartipal()
        {
            return View(db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId));

        }


        [Route("Hakkimizda")]
        public ActionResult Hakkimizda()
        {
            FooterLoader();
            return View(db.Hakkimizda.FirstOrDefault());

        }


        [Route("Hizmetlerimiz")]
        public ActionResult Hizmetlerimiz()
        {
            FooterLoader();
            return View(db.Hizmet.ToList().OrderByDescending(x => x.HizmetId));


        }

        [Route("Iletisim")]
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

        [Route("BlogPost")]
        public ActionResult Blog(int Page = 1)
        {

            FooterLoader();
            return View(db.Blog.Include("Kategori").OrderByDescending(x => x.BlogId).ToPagedList(Page, 5));

        }
        [Route("BlogKategori")]
        public ActionResult BlogKategoriPartial()
        {

            FooterLoader();
            return PartialView(db.Kategori.Include("Blogs").ToList().OrderByDescending(x => x.KategoriId));

        }


        [Route("BlogSonKayit")]
        public ActionResult BlogSonKayitPartial()
        {

            FooterLoader();
            return PartialView(db.Blog.ToList().OrderByDescending(x => x.BlogId));

        }

        [Route("BlogPost/{kategoriad}/{id:int}")]
        public ActionResult KategoriBlog(int id,int Page = 1)
        {
            FooterLoader();
            var b = db.Blog.Include("Kategori").Where(x => x.Kategori.KategoriId == id).OrderByDescending(x => x.Kategori.KategoriId).ToPagedList(Page, 5);
            return View(b);
        }


        [Route("BlogPost/{baslik}-{id:int}")]
        public ActionResult BlogDetay(int id)
        {
            FooterLoader();
            return View(db.Blog.Include("Kategori").Include("Yorums").Where(x => x.BlogId==id).FirstOrDefault());


        }


        public  JsonResult YorumYap(string adsoyad,string eposta, string icerik,int? blogId)
        {
            if (icerik==null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            db.Yorum.Add(new Models.Model.Yorum
            {
                AdSoyad = adsoyad,
                Eposta = eposta,
                Icerik = icerik,
                BlogId = blogId,
                Onayli = false
            });
            db.SaveChanges();

            return Json(false, JsonRequestBehavior.AllowGet);
        }


    }
}
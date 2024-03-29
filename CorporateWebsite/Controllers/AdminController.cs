﻿using CorporateWebsite.Models;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
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
    public class AdminController : Controller
    {
        KurumsalDBContext db = new KurumsalDBContext();
        // GET: Admin
       
        [Route("yonetimpaneli")]
        public ActionResult Index()
        {
            ViewBag.BlogSay = db.Blog.Count();
            ViewBag.KategoriSay = db.Kategori.Count();
            ViewBag.HizmetSay = db.Hizmet.Count();
            ViewBag.YorumSay = db.Yorum.Count();

            ViewBag.YorumOnay = db.Yorum.Where(x => x.Onayli == false).Count();
            var sorgu = db.Kategori.ToList();
            return View(sorgu);
        }
        [Route("yonetimpaneli/giris")]
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            var login = db.Admin.Where(x => x.Eposta == admin.Eposta).SingleOrDefault();
            if (login.Eposta==admin.Eposta && login.Sifre== Crypto.Hash(admin.Sifre, "MD5"))
            {
                Session["adminid"] = login.AdminId;
                Session["eposta"] = login.Eposta;
                Session["yetki"] = login.Yetki;

                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Uyari = "Kullanıcı adı ya da şifre yanlış";
            return View(admin);
        }

        public ActionResult Logout()
        {
            Session["adminid"] = null;
            Session["eposta"] = null;
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
        }

        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string eposta)
        {
            var userAdmin = db.Admin.Where(x => x.Eposta == eposta).SingleOrDefault();
            if (userAdmin != null )
            {
                //ssl ile ilgili mail gönderme sorunu çözümü :https://www.google.com/settings/security/lesssecureapps
                //google hizmetlerinde mail gönderimi yapılamıyor bu tyüzden yandex üzerinden mail gönderildi.


                //var newpass = Guid.NewGuid();
                Random rnd = new Random();
                var newpass = rnd.Next().ToString();
                // Admin user = new Admin();
                userAdmin.Sifre = Crypto.Hash(newpass, "MD5");
                db.SaveChanges();
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("exehiro@yandex.com", "Kurumsal");

                mail.To.Add(eposta);
                // mail.Bcc.Add("exehiro@yandex.com");

                mail.Subject = "Şifre Sıfırlama";
                mail.IsBodyHtml = true;
                var body = String.Format("Admin panel Yeni giriş şifreniz :{0}", newpass);
                mail.Body = body;


                SmtpClient sc = new SmtpClient();
                sc.Port = 587;
                sc.EnableSsl = true;
                sc.Host = "smtp.yandex.com";
                sc.UseDefaultCredentials = false;
                sc.Credentials = new NetworkCredential("exehiro@yandex.com", "karahirolol123");

                sc.Send(mail);

                ViewBag.Uyarı = "Şifreniz başarı ile gönderildi.";
            }
            else
            {
                ViewBag.Uyarı = "Hata Oluştu Tekrar Deneyiniz.";
            }
            return View();
        }

        public ActionResult Adminler()
        {
            return View(db.Admin.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Admin admin,string sifre,string eposta)
        {

            if (ModelState.IsValid)
            {
                admin.Sifre = Crypto.Hash(sifre, "MD5");
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            return View();
        }
        [HttpPost]
        public ActionResult Edit(int id,Admin admin, string sifre,string eposta , string yetki)
        {
         
            if (ModelState.IsValid)
            {
                var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
                a.Sifre = Crypto.Hash(sifre, "MD5");
                a.Eposta = admin.Eposta;
                a.Yetki = admin.Yetki;


                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View();
        }
        public ActionResult Delete(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            if (a!=null)
            {
                db.Admin.Remove(a);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View();
        }
    }
}
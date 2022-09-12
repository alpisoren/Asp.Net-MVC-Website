using CorporateWebsite.Models;
using CorporateWebsite.Models.DataContext;
using CorporateWebsite.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    public class AdminController : Controller
    {
        KurumsalDBContext db = new KurumsalDBContext();
        // GET: Admin
       
        public ActionResult Index()
        {
            var sorgu = db.Kategori.ToList();
            return View(sorgu);
        }
        
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            var login = db.Admin.Where(x => x.Eposta == admin.Eposta).SingleOrDefault();
            if (login.Eposta==admin.Eposta && login.Sifre==admin.Sifre)
            {
                Session["adminid"] = login.AdminId;
                Session["eposta"] = login.Eposta;
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
    }
}
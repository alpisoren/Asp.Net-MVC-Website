using CorporateWebsite.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    public class HomeController : Controller
    {


        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            ViewBag.Iletisim = db.Iletisim.ToList().SingleOrDefault();
            ViewBag.Blog = db.Blog.ToList().OrderByDescending(x => x.BlogId);


            return View(db.Slider.ToList());
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

        
    }
}
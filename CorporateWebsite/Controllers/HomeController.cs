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
            return View(db.Slider.ToList().OrderByDescending(x => x.SliderId));
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
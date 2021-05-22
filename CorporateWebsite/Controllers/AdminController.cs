using CorporateWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateWebsite.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        CorporateDBEntities db = new CorporateDBEntities();
        public ActionResult Index()
        {
            var query = db.Tbl_Category.ToList();
            return View(query);
        }
    }
}
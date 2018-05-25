using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExampleMVC.Controllers
{
    public class HomeController : Controller
    {
        [Sanatana.Logs.Web.Filters.MvcTrackUsage("Home/Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Sanatana.Logs.Web.Filters.MvcTrackUsage("Home/About")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
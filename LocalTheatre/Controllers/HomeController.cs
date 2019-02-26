using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LocalTheatre.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Returns the "About" view from the Home controller
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Returns the "Contact" view from the Home controller
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            return View();
        }
    }
}
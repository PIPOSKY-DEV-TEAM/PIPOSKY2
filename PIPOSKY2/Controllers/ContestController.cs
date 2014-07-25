using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class ContestController : Controller
    {
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        public ActionResult Index()
        {
            return View(db.Contests.Find(int.Parse(RouteData.Values["id"].ToString())));
        }

        public ActionResult Edit()
        {
            return View();
        }
    }
}

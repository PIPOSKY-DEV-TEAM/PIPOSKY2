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
        // GET: /Contest/
        public ActionResult Index()
        {
            PIPOSKY2DbContext db = new PIPOSKY2DbContext();
            return View(db.Contests);
        }

        // GET: /Contest/Add
        public ActionResult Add()
        {
            return View();
        }
    }
}

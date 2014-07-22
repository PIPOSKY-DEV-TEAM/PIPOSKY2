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
        [HttpPost]
        public ActionResult Add(Contest contest)
        {
            PIPOSKY2DbContext db = new PIPOSKY2DbContext();
            Contest newContest = new Contest();
            newContest.ContestName = contest.ContestName;
            newContest.StartTime = DateTime.Parse(Request.Form["StartTime"]);
            newContest.EndTime = DateTime.Parse(Request.Form["EndTime"]);
            db.Contests.Add(newContest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

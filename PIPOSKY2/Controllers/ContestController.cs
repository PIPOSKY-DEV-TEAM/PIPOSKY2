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
        public ActionResult Add(AddContestFormModel addContest)
        {
            PIPOSKY2DbContext db = new PIPOSKY2DbContext();
            Contest contest = new Contest();
            contest.ContestName = addContest.ContestName;
            contest.StartTime = DateTime.Parse(addContest.StartTime);
            contest.EndTime = DateTime.Parse(addContest.EndTime);
            db.Contests.Add(contest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

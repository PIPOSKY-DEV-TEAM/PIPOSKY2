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
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();
        public ActionResult Index()
        {
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
            try
            {
                Contest contest = new Contest();
                contest.ContestName = addContest.ContestName;
                contest.StartTime = DateTime.Parse(addContest.StartTime);
                contest.EndTime = DateTime.Parse(addContest.EndTime);
                db.Contests.Add(contest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

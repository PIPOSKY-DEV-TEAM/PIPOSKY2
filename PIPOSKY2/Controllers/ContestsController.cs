using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class ContestsController : Controller
    {
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        public ActionResult Index()
        {
            return View(db.Contests);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(AddContestFormModel addContest, FormCollection form)
        {
            try
            {
                foreach (var i in db.Contests)
                {
                    if (i.ContestName == addContest.ContestName)
                        return View(addContest);
                }
                Contest contest = new Contest();
                contest.ContestName = addContest.ContestName;
                contest.StartTime = DateTime.Parse(addContest.StartTime);
                contest.EndTime = DateTime.Parse(addContest.EndTime);
                db.Contests.Add(contest);
                db.SaveChanges();
                PIPOSKY2DbContext dbtemp = new PIPOSKY2DbContext();
                foreach (var i in dbtemp.Problems)
                {
                    if (form[i.ProblemName] == "on")
                    {
                        ContestProblem contestProblem = new ContestProblem();
                        contestProblem.ContestID = db.Contests.First(c => c.ContestName == addContest.ContestName).ContestID;
                        contestProblem.ProblemID = db.Problems.First(p => p.ProblemName == i.ProblemName).ProblemID;
                        db.ContestProblems.Add(contestProblem);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Contest()
        {
            
            return View(db.Contests.Find(int.Parse(RouteData.Values["id"].ToString())));
        }
    }
}

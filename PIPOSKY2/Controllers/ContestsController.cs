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
            User tmp = Session["User"] as User;
            if (tmp != null)
            {
                if ((tmp.UserType == "admin") || (tmp.UserType == "editor"))
                    return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Add(ContestFormModel addContest, FormCollection form)
        {
            addContest.ContestName = addContest.ContestName.Trim();
            if (addContest.ContestName == "")
            {
                ModelState.AddModelError("ContestName", "比赛名不能为空");
                return View(addContest);
            }
            foreach (var i in db.Contests)
            {
                if (i.ContestName == addContest.ContestName)
                {
                    ModelState.AddModelError("ContestName", "比赛名已存在");
                    return View(addContest);
                }
            }
            Contest contest = new Contest();
            contest.ContestName = addContest.ContestName;
            try
            {
                contest.StartTime = DateTime.Parse(addContest.StartTime);
            }
            catch
            {
                ModelState.AddModelError("StartTime", "开始时间格式不正确");
                return View(addContest);
            }
            try
            {
                contest.EndTime = DateTime.Parse(addContest.EndTime);
            }
            catch
            {
                ModelState.AddModelError("EndTime", "结束时间格式不正确");
                return View(addContest);
            }
            db.Contests.Add(contest);
            db.SaveChanges();
            foreach (var i in db.Problems)
                if (form[i.ProblemID.ToString()] == "on")
                {
                    ContestProblem contestProblem = new ContestProblem();
                    contestProblem.ContestID = contest.ContestID;
                    contestProblem.ProblemID = i.ProblemID;
                    db.ContestProblems.Add(contestProblem);
                }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete()
        {
            User tmp = Session["User"] as User;
            if (tmp != null)
            {
                if ((tmp.UserType == "admin") || (tmp.UserType == "editor"))
                    return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            PIPOSKY2DbContext dbtemp = new PIPOSKY2DbContext();
            foreach (var i in dbtemp.Contests)
                if (form[i.ContestID.ToString()] == "on")
                {
                    foreach (var j in db.ContestProblems)
                        if (j.ContestID == i.ContestID)
                            db.ContestProblems.Remove(j);
                    db.Contests.Remove(db.Contests.Find(i.ContestID));
                }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

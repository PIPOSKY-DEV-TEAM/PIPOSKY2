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
            return View(db.ContestGroups.Find(int.Parse(RouteData.Values["id"].ToString())));
        }

        public ActionResult Add()
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            ContestFormModel addContest = new ContestFormModel();
            addContest.ContestGroupID = int.Parse(RouteData.Values["id"].ToString());
            return View(addContest);
        }

        [HttpPost]
        public ActionResult Add(ContestFormModel addContest, FormCollection form)
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            try
            {
                addContest.ContestName = addContest.ContestName.Trim();
            }
            catch
            {
                return View(addContest);
            }
            if (addContest.ContestName == "")
            {
                ModelState.AddModelError("ContestName", "作业名不能为空");
                return View(addContest);
            }
            foreach (var i in db.Contests.Where(c => c.ContestGroupID == addContest.ContestGroupID).Where(c => c.ContestName == addContest.ContestName))
            {
                ModelState.AddModelError("ContestName", "作业名已存在");
                return View(addContest);
            }
            Contest contest = new Contest();
            contest.ContestGroupID = addContest.ContestGroupID;
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
            return RedirectToAction("Index", new { id = contest.ContestGroupID });
        }

        public ActionResult Delete()
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            return View(db.ContestGroups.Find(int.Parse(RouteData.Values["id"].ToString())));
        }

        [HttpPost]
        public ActionResult Delete(ContestGroup contestGroup, FormCollection form)
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            PIPOSKY2DbContext dbtemp = new PIPOSKY2DbContext();
            foreach (var i in dbtemp.Contests.Where(c => c.ContestGroupID == contestGroup.ContestGroupID))
                if (form[i.ContestID.ToString()] == "on")
                {
                    foreach (var j in db.ContestProblems.Where(p => p.ContestID == i.ContestID))
                        db.ContestProblems.Remove(j);
                    db.Contests.Remove(db.Contests.Find(i.ContestID));
                }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = contestGroup.ContestGroupID });
        }
    }
}

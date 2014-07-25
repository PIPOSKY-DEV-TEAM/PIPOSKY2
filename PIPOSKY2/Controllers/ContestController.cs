using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;
using System.Data.Entity.Migrations;

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
            ContestFormModel editContest = new ContestFormModel();
            editContest.ContestID = int.Parse(RouteData.Values["id"].ToString());
            editContest.ContestName = db.Contests.Find(editContest.ContestID).ContestName;
            editContest.StartTime = db.Contests.Find(editContest.ContestID).StartTime.ToString();
            editContest.EndTime = db.Contests.Find(editContest.ContestID).EndTime.ToString();
            return View(editContest);
        }

        [HttpPost]
        public ActionResult Edit(ContestFormModel editContest, FormCollection form)
        {
            editContest.ContestName = editContest.ContestName.Trim();
            if (editContest.ContestName == "")
            {
                ModelState.AddModelError("ContestName", "比赛名不能为空");
                return View(editContest);
            }
            foreach (var i in db.Contests)
            {
                if ((i.ContestName == editContest.ContestName) && (i.ContestID != editContest.ContestID))
                {
                    ModelState.AddModelError("ContestName", "比赛名已存在");
                    return View(editContest);
                }
            }
            Contest contest = new Contest();
            contest.ContestID = editContest.ContestID;
            contest.ContestName = editContest.ContestName;
            try
            {
                contest.StartTime = DateTime.Parse(editContest.StartTime);
            }
            catch
            {
                ModelState.AddModelError("StartTime", "开始时间格式不正确");
                return View(editContest);
            }
            try
            {
                contest.EndTime = DateTime.Parse(editContest.EndTime);
            }
            catch
            {
                ModelState.AddModelError("EndTime", "结束时间格式不正确");
                return View(editContest);
            }
            db.Contests.AddOrUpdate(contest);
            db.SaveChanges();
            foreach (var i in db.ContestProblems)
                if (i.ContestID == editContest.ContestID)
                {
                    db.ContestProblems.Remove(i);
                }
            foreach (var i in db.Problems)
                if (form[i.ProblemID.ToString()] == "on")
                {
                    ContestProblem contestProblem = new ContestProblem();
                    contestProblem.ContestID = contest.ContestID;
                    contestProblem.ProblemID = i.ProblemID;
                    db.ContestProblems.Add(contestProblem);
                }
            db.SaveChanges();
            return RedirectToAction("Index", "Contests");
        }
    }
}

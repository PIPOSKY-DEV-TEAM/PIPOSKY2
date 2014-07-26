using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class ContestGroupsController : Controller
    {
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        public ActionResult Index()
        {
            return View(db.ContestGroups);
        }

        public ActionResult Add()
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public ActionResult Add(ContestGroup addContestGroup)
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            try
            {
                addContestGroup.ContestGroupName = addContestGroup.ContestGroupName.Trim();
            }
            catch
            {
                return View(addContestGroup);
            }
            if (addContestGroup.ContestGroupName == "")
            {
                ModelState.AddModelError("ContestGroupName", "课程名不能为空");
                return View(addContestGroup);
            }
            foreach (var i in db.ContestGroups.Where(g => g.ContestGroupName == addContestGroup.ContestGroupName))
            {
                ModelState.AddModelError("ContestGroupName", "课程名已存在");
                return View(addContestGroup);
            }
            ContestGroup contestGroup = new ContestGroup();
            contestGroup.ContestGroupName = addContestGroup.ContestGroupName;
            db.ContestGroups.Add(contestGroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete()
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public ActionResult Delete(ContestGroup contestGroup, FormCollection form)
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            PIPOSKY2DbContext dbtemp1 = new PIPOSKY2DbContext();
            PIPOSKY2DbContext dbtemp2 = new PIPOSKY2DbContext();
            foreach (var i in dbtemp1.ContestGroups)
                if (form[i.ContestGroupID.ToString()] == "on")
                {
                    foreach (var j in dbtemp2.Contests.Where(c => c.ContestGroupID == i.ContestGroupID))
                    {
                        foreach (var k in db.ContestProblems.Where(p => p.ContestID == j.ContestID))
                            db.ContestProblems.Remove(k);
                        db.Contests.Remove(db.Contests.Find(j.ContestID));
                    }
                    db.ContestGroups.Remove(db.ContestGroups.Find(i.ContestGroupID));
                }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

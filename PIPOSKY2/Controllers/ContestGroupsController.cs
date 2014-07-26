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
            addContestGroup.ContestGroupName = addContestGroup.ContestGroupName.Trim();
            if (addContestGroup.ContestGroupName == "")
            {
                ModelState.AddModelError("ContestGroupName", "比赛组名不能为空");
                return View(addContestGroup);
            }
            foreach (var i in db.ContestGroups.Where(g => g.ContestGroupName == addContestGroup.ContestGroupName))
            {
                ModelState.AddModelError("ContestGroupName", "比赛组名已存在");
                return View(addContestGroup);
            }
            ContestGroup contestGroup = new ContestGroup();
            contestGroup.ContestGroupName = addContestGroup.ContestGroupName;
            db.ContestGroups.Add(contestGroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

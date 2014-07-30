using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;
using System.Data.Entity.Migrations;

namespace PIPOSKY2.Controllers
{
    public class HomeworkController : Controller
    {
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        public ActionResult Index(int? id)
        {
            return View(db.Course.Find(id));
        }

        [CheckAdminOrEditor]
        public ActionResult Edit(int? id)
        {
            HomeworkFormModel editHomework = new HomeworkFormModel();
            editHomework.HomeworkID = (int)id;
            editHomework.HomeworkName = db.Course.Find(editHomework.HomeworkID).HomeworkName;
            editHomework.StartTime = db.Course.Find(editHomework.HomeworkID).StartTime.ToString();
            editHomework.EndTime = db.Course.Find(editHomework.HomeworkID).EndTime.ToString();
            return View(editHomework);
        }

        [HttpPost]
        [CheckAdminOrEditor]
        public ActionResult Edit(HomeworkFormModel editHomework, FormCollection form)
        {
            try
            {
                editHomework.HomeworkName = editHomework.HomeworkName.Trim();
            }
            catch
            {
                return View(editHomework);
            }
            if (editHomework.HomeworkName == "")
            {
                ModelState.AddModelError("HomeworkName", "作业名不能为空");
                return View(editHomework);
            }
            foreach (var i in db.Course.Where(c => c.HomeworkName == editHomework.HomeworkName).Where(c => c.HomeworkID != editHomework.HomeworkID))
            {
                ModelState.AddModelError("HomeworkName", "作业名已存在");
                return View(editHomework);
            }
            Homework Homework = new Homework();
            Homework.HomeworkID = editHomework.HomeworkID;
            Homework.HomeworkName = editHomework.HomeworkName;
            Homework.CourseID = db.Course.Find(Homework.HomeworkID).CourseID;
            try
            {
                Homework.StartTime = DateTime.Parse(editHomework.StartTime);
            }
            catch
            {
                ModelState.AddModelError("StartTime", "开始时间格式不正确");
                return View(editHomework);
            }
            try
            {
                Homework.EndTime = DateTime.Parse(editHomework.EndTime);
            }
            catch
            {
                ModelState.AddModelError("EndTime", "结束时间格式不正确");
                return View(editHomework);
            }
            db.Course.AddOrUpdate(Homework);
            db.SaveChanges();
            foreach (var i in db.HomeworkProblems.Where(p => p.HomeworkID == editHomework.HomeworkID))
            {
                db.HomeworkProblems.Remove(i);
            }
            foreach (var i in db.Problems)
                if (form[i.ProblemID.ToString()] == "on")
                {
                    HomeworkProblem HomeworkProblem = new HomeworkProblem();
                    HomeworkProblem.HomeworkID = Homework.HomeworkID;
                    HomeworkProblem.ProblemID = i.ProblemID;
                    db.HomeworkProblems.Add(HomeworkProblem);
                }
            db.SaveChanges();
            return RedirectToAction("Index", "Course", new { id = Homework.CourseID });
        }
    }
}

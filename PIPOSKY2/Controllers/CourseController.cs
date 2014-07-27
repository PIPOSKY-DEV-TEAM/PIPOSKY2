using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class CourseController : Controller
    {
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        public ActionResult Index()
        {
            return View(db.Courses.Find(int.Parse(RouteData.Values["id"].ToString())));
        }

        public ActionResult Add()
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            HomeworkFormModel addHomework = new HomeworkFormModel();
            addHomework.CourseID = int.Parse(RouteData.Values["id"].ToString());
            return View(addHomework);
        }

        [HttpPost]
        public ActionResult Add(HomeworkFormModel addHomework, FormCollection form)
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            try
            {
                addHomework.HomeworkName = addHomework.HomeworkName.Trim();
            }
            catch
            {
                return View(addHomework);
            }
            if (addHomework.HomeworkName == "")
            {
                ModelState.AddModelError("HomeworkName", "作业名不能为空");
                return View(addHomework);
            }
            foreach (var i in db.Course.Where(c => c.CourseID == addHomework.CourseID).Where(c => c.HomeworkName == addHomework.HomeworkName))
            {
                ModelState.AddModelError("HomeworkName", "作业名已存在");
                return View(addHomework);
            }
            Homework Homework = new Homework();
            Homework.CourseID = addHomework.CourseID;
            Homework.HomeworkName = addHomework.HomeworkName;
            try
            {
                Homework.StartTime = DateTime.Parse(addHomework.StartTime);
            }
            catch
            {
                ModelState.AddModelError("StartTime", "开始时间格式不正确");
                return View(addHomework);
            }
            try
            {
                Homework.EndTime = DateTime.Parse(addHomework.EndTime);
            }
            catch
            {
                ModelState.AddModelError("EndTime", "结束时间格式不正确");
                return View(addHomework);
            }
            db.Course.Add(Homework);
            db.SaveChanges();
            foreach (var i in db.Problems)
                if (form[i.ProblemID.ToString()] == "on")
                {
                    HomeworkProblem HomeworkProblem = new HomeworkProblem();
                    HomeworkProblem.HomeworkID = Homework.HomeworkID;
                    HomeworkProblem.ProblemID = i.ProblemID;
                    db.HomeworkProblems.Add(HomeworkProblem);
                }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = Homework.CourseID });
        }

        public ActionResult Delete()
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            return View(db.Courses.Find(int.Parse(RouteData.Values["id"].ToString())));
        }

        [HttpPost]
        public ActionResult Delete(Course Course, FormCollection form)
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            PIPOSKY2DbContext dbtemp = new PIPOSKY2DbContext();
            foreach (var i in dbtemp.Course.Where(c => c.CourseID == Course.CourseID))
                if (form[i.HomeworkID.ToString()] == "on")
                {
                    foreach (var j in db.HomeworkProblems.Where(p => p.HomeworkID == i.HomeworkID))
                        db.HomeworkProblems.Remove(j);
                    db.Course.Remove(db.Course.Find(i.HomeworkID));
                }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = Course.CourseID });
        }
    }
}

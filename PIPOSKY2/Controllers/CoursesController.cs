using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class CoursesController : Controller
    {
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        public ActionResult Index()
        {
            return View(db.Courses);
        }

        [CheckAdminOrEditor]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [CheckAdminOrEditor]
        public ActionResult Add(Course addCourse)
        {
            try
            {
                addCourse.CourseName = addCourse.CourseName.Trim();
            }
            catch
            {
                return View(addCourse);
            }
            if (addCourse.CourseName == "")
            {
                ModelState.AddModelError("CourseName", "课程名不能为空");
                return View(addCourse);
            }
            foreach (var i in db.Courses.Where(g => g.CourseName == addCourse.CourseName))
            {
                ModelState.AddModelError("CourseName", "课程名已存在");
                return View(addCourse);
            }
            Course Course = new Course();
            Course.CourseName = addCourse.CourseName;
            db.Courses.Add(Course);
            db.SaveChanges();
            Session["alertetype"] = "success";
            Session["alertetext"] = "添加成功";
            return RedirectToAction("Index");
        }

        [CheckAdminOrEditor]
        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [CheckAdminOrEditor]
        public ActionResult Delete(Course Course, FormCollection form)
        {
            PIPOSKY2DbContext dbtemp1 = new PIPOSKY2DbContext();
            PIPOSKY2DbContext dbtemp2 = new PIPOSKY2DbContext();
            foreach (var i in dbtemp1.Courses)
                if (form[i.CourseID.ToString()] == "on")
                {
                    foreach (var j in dbtemp2.Course.Where(c => c.CourseID == i.CourseID))
                    {
                        foreach (var k in db.HomeworkProblems.Where(p => p.HomeworkID == j.HomeworkID))
                            db.HomeworkProblems.Remove(k);
                        db.Course.Remove(db.Course.Find(j.HomeworkID));
                    }
                    db.Courses.Remove(db.Courses.Find(i.CourseID));
                }
            Session["alertetype"] = "success";
            Session["alertetext"] = "删除成功";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

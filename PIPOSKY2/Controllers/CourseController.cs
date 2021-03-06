﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;
using System.IO;

namespace PIPOSKY2.Controllers
{
    public class CourseController : Controller
    {
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        public ActionResult Index(int? id)
        {
            return View(db.Courses.Find(id));
        }

        [CheckAdminOrEditor]
        public ActionResult Add(int? id)
        {
            HomeworkFormModel addHomework = new HomeworkFormModel();
            addHomework.CourseID = (int)id;
            addHomework.StartTime = DateTime.Now.ToString();
            addHomework.EndTime = addHomework.StartTime;
            return View(addHomework);
        }

        [HttpPost]
        [CheckAdminOrEditor]
        public ActionResult Add(HomeworkFormModel addHomework, FormCollection form)
        {
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
            Session["alertetype"] = "success";
            Session["alertetext"] = "添加成功";
            return RedirectToAction("Index", new { id = Homework.CourseID });
        }

        [CheckAdminOrEditor]
        public ActionResult Delete(int? id)
        {
            return View(db.Courses.Find(id));
        }

        [HttpPost]
        [CheckAdminOrEditor]
        public ActionResult Delete(Course Course, FormCollection form)
        {
            PIPOSKY2DbContext dbtemp = new PIPOSKY2DbContext();
            foreach (var i in dbtemp.Course.Where(c => c.CourseID == Course.CourseID))
                if (form[i.HomeworkID.ToString()] == "on")
                {
                    foreach (var j in db.HomeworkProblems.Where(p => p.HomeworkID == i.HomeworkID))
                        db.HomeworkProblems.Remove(j);
                    db.Course.Remove(db.Course.Find(i.HomeworkID));
                }
            db.SaveChanges();
            Session["alertetype"] = "success";
            Session["alertetext"] = "删除成功";
            return RedirectToAction("Index", new { id = Course.CourseID });
        }

        [CheckAdminOrEditor]
        public ActionResult Score(int? id)
        {
            if (!Directory.Exists(Server.MapPath("~/Scores")))
                Directory.CreateDirectory(Server.MapPath("~/Scores"));
            string path = Server.MapPath("~/Scores") + "\\" + db.Courses.Find(id).CourseName + ".csv";
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("GB2312"));
            string s = "用户ID,用户学号,用户名";
            int sum;
            IQueryable<Submit> t;
            bool ok = true;
            foreach (var i in db.Course.Where(c => c.CourseID == id))
                s = s + "," + i.HomeworkName;
            sw.WriteLine(s);
            PIPOSKY2DbContext dbtemp1 = new PIPOSKY2DbContext();
            PIPOSKY2DbContext dbtemp2 = new PIPOSKY2DbContext();
            PIPOSKY2DbContext dbtemp3 = new PIPOSKY2DbContext();
            foreach (var i in dbtemp1.Users.Where(u => u.UserType == "normal"))
            {
                s = i.UserID.ToString() + "," + i.StudentNumber + "," + i.UserName;
                foreach (var j in dbtemp2.Course.Where(c => c.CourseID == id))
                {
                    sum = 0;
                    foreach (var k in dbtemp3.HomeworkProblems.Where(p => p.HomeworkID == j.HomeworkID))
                    {
                        t = db.Submits.Where(model => model.Prob.ProblemID == k.ProblemID && model.User.UserID == i.UserID && model.Time.CompareTo(j.EndTime) < 0);
                        if (t.Count() > 0)
                        {
                            sum += t.ToList().Last().Score;
                            if ((t.ToList().Last().State == "wait") || (t.ToList().Last().State == "run"))
                                ok = false;
                        }
                    }
                    s = s + "," + sum.ToString();
                }
                sw.WriteLine(s);
            }
            if (!ok)
                sw.WriteLine("警告：成绩中有未完成的评测");
            sw.Close();
            return File(path, "Application/x-csv", Path.GetFileName(path));
        }
    }
}

using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.IO.Compression;
using System.Data.Entity;
using SharpCompress.Common;
using SharpCompress.Reader;
using PIPOSKY2.Models;
using System.Data.Entity.Migrations;

namespace PIPOSKY2.Controllers
{
    public class ProblemController : Controller
    {
        //
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();
        public ActionResult Index()
        {
            return View(db.Problems.ToList());
        }
        public ActionResult Upload()
        {
            Problem problem = new Problem();
            return View(problem);
        }
        [HttpPost]
        public ActionResult Upload(UploadProblemFormModel form)
        {
            Problem problem = new Problem();
            if (DealWithForm(form, problem))
            {
                db.Problems.Add(problem);
                db.SaveChanges();
                return RedirectToAction("Index", "Problem");                
            };
            return View(problem);
        }

        public ActionResult Edit(int ?id)
        {
            Problem problem = db.Problems.Find(id);
            return View(problem);
        }
        [HttpPost]
        public ActionResult Edit(int ?id, UploadProblemFormModel form)
        {
            Problem problem = db.Problems.Find(id);
            if (DealWithForm(form, problem))
            {
                db.SaveChanges();
                return RedirectToAction("Index", "Problem");                
            }
            return View(problem);
        }

        public string OpenRar(HttpPostedFileBase file)
        {
            string content = "";
            Encoding encoding = System.Text.Encoding.GetEncoding("GB2312");
            using (Stream stream = file.InputStream)
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory && reader.Entry.FilePath.EndsWith("Content.txt"))
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        EntryStream entry = reader.OpenEntryStream();
                        StreamReader temp = new StreamReader(entry,enconding);
                        content = temp.ReadToEnd();
                    }
                }
            }
            return content;
        }
        public bool DealWithForm(UploadProblemFormModel form, Problem problem)
        {
            //题目名称
            problem.ProblemName = form.Name;
            //获取文件
            HttpPostedFileBase file = form.File;
            //题目是否公开
            if (form.visible == "on")
                problem.Visible = true;
            else problem.Visible = false;
            //上传用户
            problem.Creator = Session["User"] as User;
            //处理文件
            string ext = Path.GetExtension(file.FileName);
            if (ext == ".rar" || ext == ".zip")
            {
                //文件路径
                string filePath = Path.Combine(HttpContext.Server.MapPath("~/Problems"), problem.ProblemName + ext);
                problem.ProblemPath = filePath;
                problem.Content = OpenRar(file);
                if (problem.Content.Length > 0)
                {
                    //保存文件
                    file.SaveAs(filePath);
                    return true;
                }
            }
            ViewBag.mention = "文件格式错误！";
            return false;
        }

        public ActionResult Delete()
        {
            User tmp = Session["User"] as User;
            if ((tmp == null) || (tmp.UserType != "admin" && tmp.UserType != "editor"))
                return RedirectToAction("Index");
            return View(db.Problems.ToList());
        }
        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            User tmp = Session["User"] as User;
            if ((tmp == null) || (tmp.UserType != "admin" && tmp.UserType != "editor"))
                return RedirectToAction("Index"); ;
            foreach (var i in db.Problems)
                if (form[i.ProblemID.ToString()] == "on")
                {
                    db.Problems.Remove(i);
                }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Content(int? id)
        {
            return View(db.Problems.Find(id));
        }

    }
}
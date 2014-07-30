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
using Newtonsoft.Json.Linq;

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
            User tmp = Session["User"] as User;
            if ((tmp == null) || (tmp.UserType != "admin" && tmp.UserType != "editor"))
            {
                return RedirectToAction("Index", "Problem");
            }
            Problem problem = new Problem();
            return View(problem);
        }
        [HttpPost]
        public ActionResult Upload(UploadProblemFormModel form)
        {
            Problem problem = new Problem();
            if (DealWithForm(form, problem))
            {
                foreach (var i in db.Problems.Where(x => x.ProblemName == problem.ProblemName))
                {
                    ViewBag.mention = "题目已经存在！请更改题目名称！";
                    return View(problem);
                }
                db.Problems.Add(problem);
                db.SaveChanges();
                return RedirectToAction("Index", "Problem");                
            };
            return View(problem);
        }

        public ActionResult Edit(int ?id)
        {
            User tmp = Session["User"] as User;
            if ((tmp == null) || (tmp.UserType != "admin" && tmp.UserType != "editor"))
            {
                return RedirectToAction("Index", "Problem");
            }
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

        public bool OpenRar(HttpPostedFileBase file, Problem problem)
        {
            string filename = "";
            bool x1 = false, x2 = false, x3 = false;
            Encoding encoding = System.Text.Encoding.GetEncoding("GB2312");
            Stream stream = file.InputStream;
            var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                filename = reader.Entry.FilePath;
                if (!reader.Entry.IsDirectory)
                {
                    if (filename.EndsWith("Prob.html"))
                    {
                        EntryStream entry = reader.OpenEntryStream();
                        StreamReader temp = new StreamReader(entry, encoding);
                        problem.Description = temp.ReadToEnd();
                        x1 = true;
                    }
                    else if (filename.EndsWith("Solve.html"))
                    {
                        EntryStream entry = reader.OpenEntryStream();
                        StreamReader temp = new StreamReader(entry, encoding);
                        problem.Solution = temp.ReadToEnd();
                        x2 = true;
                    }
                    else if (filename.EndsWith("Config.json"))
                    {
                        EntryStream entry = reader.OpenEntryStream();
                        StreamReader temp = new StreamReader(entry);
                        problem.Config = temp.ReadToEnd();
                        x3 = true;
                        try
                        {
                            JObject obj = JObject.Parse(problem.Config);
                            problem.ProblemName = (string)obj["Title"];
                         }
                        catch
                        {
                            ViewBag.mention = "Config文件格式错误！";
                            return false;
                        }
                    }
                }
            }
            stream.Flush();
            if (!(x1 && x2 && x3))
            {
                ViewBag.mention = "压缩包内文件不足！";
                return false;
            }
            return true;
        }
        public bool DealWithForm(UploadProblemFormModel form, Problem problem)
        {
            //题目是否公开
            if (form.visible == "on")
                problem.Visible = true;
            else problem.Visible = false;
            //上传用户
            problem.Creator = db.Users.Find(Session["_UserID"] as int?);
            //获取文件
            HttpPostedFileBase file = form.File;
            //处理文件
            if (file == null)
            {
                return true;
            }
            if (!Directory.Exists(Server.MapPath("~/Problems")))
                Directory.CreateDirectory(Server.MapPath("~/Problems"));
            string ext = Path.GetExtension(file.FileName);
            if (ext == ".rar" || ext == ".zip")
            {
                //解压文件获取数据   
                if (OpenRar(file, problem))
                {
                    //保存文件
                    if (problem.ProblemPath != null)
                    {
                        if (System.IO.File.Exists(problem.ProblemPath))
                            System.IO.File.Delete(problem.ProblemPath);
                    }
                    string filePath = Path.Combine(HttpContext.Server.MapPath("~/Problems"), problem.ProblemName + ext);
                    problem.ProblemPath = "Problems/" + problem.ProblemName + ext;
                    file.SaveAs(filePath);
                    return true;
                }
                return false;
            }
            ViewBag.mention = "文件格式错误！请上传压缩包！";
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
            PIPOSKY2DbContext dbtemp = new PIPOSKY2DbContext();
            foreach (var i in dbtemp.Problems)
                if (form[i.ProblemID.ToString()] == "on")
                {
                    foreach (var j in db.HomeworkProblems.Where(p => p.ProblemID == i.ProblemID))
                    {
                        db.HomeworkProblems.Remove(j);
                    }
                    if (System.IO.File.Exists(i.ProblemPath))
                        System.IO.File.Delete(i.ProblemPath);
                    db.Problems.Remove(db.Problems.Find(i.ProblemID));
                }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Content(int? id)
        {
            User tmp = Session["User"] as User;
            bool CanRead = false;
            if ((tmp != null) && (tmp.UserType == "admin" || tmp.UserType == "editor"))
                CanRead = true;
            Problem problem = db.Problems.Find(id);
            if (problem.Visible || CanRead)
                return View(problem);
            else return RedirectToAction("index");
        }

        public FileStreamResult Download(int? id)
        {
            User tmp = Session["User"] as User;
            bool CanDown = false;
            if ((tmp != null) && (tmp.UserType == "admin" || tmp.UserType == "editor"))
                CanDown = true;
            Problem problem = db.Problems.Find(id);
            if (CanDown || problem.Downloadable)
            {
                FileStream filestream = new FileStream(Server.MapPath("~/") + problem.ProblemPath,
                    FileMode.Open, FileAccess.Read, FileShare.None);
                return File(filestream,
                    "text/plain", problem.ProblemName + Path.GetExtension(problem.ProblemPath));
            }
            else
                return null;
        }
    }
}
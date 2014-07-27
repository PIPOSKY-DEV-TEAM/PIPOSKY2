using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.IO.Compression;
using SharpCompress.Common;
using SharpCompress.Reader;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class ProblemController : Controller
    {
        //
        // GET: /Problem/
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();
        public ActionResult Index()
        {
            return View(db.Problems);
        }
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(HttpRequestBase Request)
        {
            //创建题目数据
            Problem problem = new Problem();
            //题目名称
            problem.ProblemName = Request["Name"];
            //获取文件
            HttpPostedFileBase file = Request.Files["File"];
            if (!DealWithFile(file, problem))
            {
                ViewBag.mention = "文件格式错误！";
                return View();
            }
            if (Request["visible"] == "on")
                problem.Visible = true;
            else problem.Visible = false;
            db.Problems.Add(problem);
            db.SaveChanges();
            return RedirectToAction("Index", "Problem");
        }
        public ActionResult Edit()
        {
            Problem problem = db.Problems.Find(int.Parse(RouteData.Values["id"].ToString()));
            return View(problem); ;
        }
        [HttpPost]
        public ActionResult Edit(UploadProblemFormModel form)
        {
            Problem problem = db.Problems.Find(int.Parse(RouteData.Values["id"].ToString()));
            problem.ProblemName = form.Name;
            //获取文件
            HttpPostedFileBase file = form.File;
            if (file != null)
                if (!DealWithFile(file, problem))
                {
                    return View();
                }
            db.SaveChanges();
            return RedirectToAction("Index", "Problem");
        }
        public ActionResult Delete()
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            return View(db.Problems);
        }
        [HttpPost]
        public ActionResult Delete(UploadProblemFormModel problem, FormCollection form)
        {
            User tmp = Session["User"] as User;
            if (tmp == null)
                return RedirectToAction("Index");
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
                return RedirectToAction("Index");
            foreach (var i in db.Problems)
                if (form[i.ProblemID.ToString()] == "on")
                {
                    db.Problems.Remove(i);
                }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Content()
        {
            return View(db.Problems.Find(int.Parse(RouteData.Values["id"].ToString())));
        }
        public bool DealWithFile(HttpPostedFileBase file, Problem problem)
        {
            if (CheckFormat(file))
            {
                //文件路径
                string filePath = Path.Combine(HttpContext.Server.MapPath("~/Problems"), Path.GetFileName(file.FileName));
                problem.ProblemPath = filePath;
                //文件类型zip/rar
                string content = "";
                if (filePath.EndsWith(".zip"))
                {
                    if (!OpenZip(file, content))
                        return false;
                    problem.Content = content;
                }
                else if (filePath.EndsWith(".rar"))
                {
                    if (!OpenRar(file, content))
                        return false;
                    problem.Content = content;
                }
                //保存文件
                file.SaveAs(filePath);
                return true;
            }
            else
            {
                ViewBag.text = "文件格式错误";
                return false;
            }
        }
        public bool CheckFormat(HttpPostedFileBase file)
        {
            string name = file.FileName;
            if (!(name.EndsWith(".zip") || name.EndsWith(".rar")))
            {
                return false;
            }
            return true;
        }
        public bool OpenZip(HttpPostedFileBase file, string content)
        {
            Encoding encoding = System.Text.Encoding.GetEncoding("GB2312");
            using (ZipArchive archive = new ZipArchive(file.InputStream, ZipArchiveMode.Read, false, encoding))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith("Content.txt"))
                    {
                        using (StreamReader reader = new StreamReader(entry.Open(), encoding))
                        {
                            content = reader.ReadToEnd();
                            reader.Close();
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public bool OpenRar(HttpPostedFileBase file,string content)
        {
            Encoding encoding = System.Text.Encoding.GetEncoding("GB2312");
            using (Stream stream = file.InputStream)
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        if (reader.Entry.FilePath.EndsWith("Content.txt"))
                        {
                            Console.WriteLine(reader.Entry.FilePath);
                            EntryStream entry = reader.OpenEntryStream();
                            StreamReader temp = new StreamReader(entry, encoding);
                            content = temp.ReadToEnd();
                            return true;
                        }

                    }
                }
            }
            return false;
        }
    }
}

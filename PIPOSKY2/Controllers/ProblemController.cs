using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.IO.Compression;
using SharpCompress;
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
        public ActionResult Upload(UploadProblemFormModel form)
        {
            //创建题目数据
            Problem problem = new Problem();
            //题目名称
            problem.ProblemName = form.Name;
            //获取文件
            HttpPostedFileBase file = form.File;
            if (!DealWithFile(file, problem))
            {
                return View();
            }
            db.Problems.Add(problem);
            db.SaveChanges();
            return RedirectToAction("Index", "Problem");
        }
        public bool CheckFormat(HttpPostedFileBase file)
        {
            string name = file.FileName;
            if (!(name.EndsWith(".zip")||name.EndsWith(".rar")))
            {
                return false;
            }
            return true;
        }
        public string OpenZip(HttpPostedFileBase file)
        {
            string content = "";
            Encoding encoding = System.Text.Encoding.GetEncoding("GB2312");
            using (ZipArchive archive = new ZipArchive(file.InputStream, ZipArchiveMode.Read, false, encoding))
            {
                ZipArchiveEntry entry = archive.GetEntry("9_27/Content.txt");
                using (StreamReader reader = new StreamReader(entry.Open(), encoding))
                {
                    content = reader.ReadToEnd();
                    reader.Close();
                }
            }
            return content;
        }
        public string OpenRar(HttpPostedFileBase file)
        {
            string content = "";           
            Encoding encoding = System.Text.Encoding.GetEncoding("GB2312");
            /*using (Stream stream = file.InputStream)
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        reader.WriteEntryToDirectory(@"C:\temp", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }*/
            return content;
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
            Problem problem = db.Problems.Find(int.Parse(RouteData.Values["id"].ToString()));
            db.Problems.Remove(problem);
            db.SaveChanges();
            return RedirectToAction("Index", "Problem");
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
                //文件类型
                file.GetType();
                if (filePath.EndsWith(".zip"))
                {
                    string content = OpenZip(file);
                    problem.Content = content;
                }
                else if (filePath.EndsWith(".rar"))
                {
                    string content = OpenRar(file);
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
    }
}

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
        // GET: /Problem/
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();
        public ActionResult Index()
        {
            return View(db.Problems.ToList());
        }

        public string OpenRar(HttpPostedFileBase file)
        {
            string content = "";
            Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");
            using (Stream stream = file.InputStream)
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory && reader.Entry.FilePath.EndsWith("Content.txt"))
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        EntryStream entry = reader.OpenEntryStream();
                        StreamReader temp = new StreamReader(entry, encoding);
                        content = temp.ReadToEnd();
                    }
                }
            }
            return content;
        }

        public ActionResult Edit(int? id)
        {
            Problem problem;
            if (id == null)
                problem = new Problem();
            else 
                problem = db.Problems.Find(id);
            return View(problem); ;
        }

        [HttpPost]
        public ActionResult Edit(int?id, UploadProblemFormModel form)
        {
            Problem problem;
            if (id == null)
                problem = new Problem();
            else 
                problem = db.Problems.Find(id);
            problem.ProblemName = form.Name;
            //获取文件
            HttpPostedFileBase file = form.File;
            if (file == null || !SaveFile(file, problem))
                return View(problem);
            db.Problems.AddOrUpdate(problem);
            db.SaveChanges();
            return RedirectToAction("Index", "Problem");
        }

        public ActionResult Delete(int? id)
        {
            Problem problem = db.Problems.Find(id);
            db.Problems.Remove(problem);
            db.SaveChanges();
            return RedirectToAction("Index", "Problem");
        }

        public ActionResult Content(int? id)
        {
            return View(db.Problems.Find(id));
        }

        public bool SaveFile(HttpPostedFileBase file, Problem problem)
        {
            string ext = Path.GetExtension(file.FileName);
            if (ext == ".rar" || ext ==".zip")
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

            ViewBag.text = "文件格式错误";
            return false;

        }
    }
}

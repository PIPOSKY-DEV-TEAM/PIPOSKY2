using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
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
            if (file != null && file.ContentLength > 0)
            {
                //文件路径
                string filePath = Path.Combine(HttpContext.Server.MapPath("~/Problems"), Path.GetFileName(file.FileName));
                file.SaveAs(filePath);
                problem.ProblemPath = filePath;
                //解压缩
                //获取题目内容
            }
            else return View();
            
            db.Problems.Add(problem);
            db.SaveChanges();
            return RedirectToAction("Index", "Problem");
        }
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Delete()
        {
            return View();
        }
        public ActionResult ShowContent()
        {
            return View();
        }
    }
}

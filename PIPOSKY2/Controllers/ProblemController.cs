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
            Problem problem = new Problem();
            problem.ProblemName = form.Name;
            HttpPostedFileBase file = form.File;
            if (file != null && file.ContentLength > 0)
            {
                string filePath = Path.Combine(HttpContext.Server.MapPath("~/Problems"), Path.GetFileName(file.FileName));
                file.SaveAs(filePath);
                problem.ProblemPath = filePath;
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

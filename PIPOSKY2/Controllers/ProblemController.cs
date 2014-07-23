using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace PIPOSKY2.Controllers
{
    public class ProblemController : Controller
    {
        //
        // GET: /Problem/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            if (Request.HttpMethod == "POST")
            {
                HttpPostedFileBase file = Request.Files["file"];
                if (file != null && file.ContentLength > 0)
                {
                    string filePath = Path.Combine(HttpContext.Server.MapPath("~/Problems"), Path.GetFileName(file.FileName));
                    file.SaveAs(filePath);
                    return RedirectToAction("Index", "Problem");
                }
            }
            return View();
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

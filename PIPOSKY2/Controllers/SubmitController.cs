using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class SubmitController : Controller
    {

		PIPOSKY2DbContext db = new PIPOSKY2DbContext();

        public ActionResult Index()
        {
			List<Submit> tmp = db.Submits.OrderBy(_ => _.SubmitID).Take(30).ToList();
            return View(tmp);
        }

        public ActionResult Details(int id)
        {
	        var tmp = db.Submits.Find(id);
	        if (tmp == null)
	        {
		        return HttpNotFound();
	        }
            return View(tmp);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
		public ActionResult Create(SubmitFormModel info)
        {
            try
            {
				var tmp = new Submit();
	            tmp.Lang = info.Lang;
	            tmp.Prob = db.Problems.Find(info.PID);
	            if (tmp.Prob == null)
	            {
					ModelState.AddModelError("PID","没有这样的题目");
	            }
	            tmp.User = db.Users.Find(Session["_User"]);
				
	            tmp.Time = DateTime.Now;
	            tmp.Source = info.Source;

				tmp.State = "wait";
	            if (ModelState.IsValid)
	            {
		            db.Submits.Add(tmp);
		            db.SaveChanges();
		            return RedirectToAction("Index");
	            }
            }
            catch
            {
            
            }
			return View();
        }
    }
}

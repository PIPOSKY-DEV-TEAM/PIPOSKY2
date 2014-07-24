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
		        return HttpNotFound();
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
				var tmp = new Submit {
					Lang = info.Lang, 
					Prob = db.Problems.Find(info.PID),
					Source = info.Source,
					Time = DateTime.Now,
					State = "wait",
					User = db.Users.Find(Session["_UserID"] as int?)
				};

	            if (tmp.Prob == null)
					ModelState.AddModelError("PID","没有这样的题目");
	            
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

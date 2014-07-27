using PIPOSKY2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace PIPOSKY2.Controllers
{
    public class SubmitController : Controller
    {

		PIPOSKY2DbContext db = new PIPOSKY2DbContext();
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

        public ActionResult Index()
        {
            var tmp = db.Submits.AsQueryable();
            if (Request.QueryString["p"] != null && Request.QueryString["p"].Length >0)
            {
                int x = Int32.Parse(Request.QueryString["p"]);
                tmp = tmp.Where(_ => _.Prob.ProblemID == x);
            }
            if (Request.QueryString["id"] != null && Request.QueryString["id"].Length > 0)
            {
                int x = Int32.Parse(Request.QueryString["id"]);
                tmp = tmp.Where(_ => _.SubmitID == x);
            }
            if (Request.QueryString["s"] != null && Request.QueryString["s"].Length > 0)
            {
                string x = Request.QueryString["s"];
                tmp = tmp.Where(_ => _.State == x);
            }
            
            if (Request.HttpMethod == "POST" && Request.Form ["rejudge"] != null)
            {
                foreach (var i in tmp.ToArray())
                {
                    i.State = "wait";
                    db.Submits.AddOrUpdate(i);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tmp.ToList());
        }

    }
}

using PIPOSKY2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json;

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
            if (tmp.Result == null || tmp.Result.Length == 0)
                tmp.Result = "[]";
            ViewBag.Res = JsonConvert.DeserializeObject<List< List<string> > >(tmp.Result);
            return View(tmp);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
		public ActionResult Create(SubmitFormModel info)
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
            if (tmp.Source == null || tmp.Source.Length == 0)
                ModelState.AddModelError("Source", "不能提交空的代码");

	        if (ModelState.IsValid)
	        {
		        db.Submits.Add(tmp);
		        db.SaveChanges();
		        return RedirectToAction("Index");
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
            if (Request.QueryString["u"] != null && Request.QueryString["u"].Length > 0)
            {
                string x = Request.QueryString["u"];
                tmp = tmp.Where(_ => _.User.UserName == x);
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
            if (Request.QueryString["rejudge"] == "on")
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

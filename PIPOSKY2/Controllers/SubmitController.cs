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
			List<Submit> tmp = db.Submits.OrderBy(_ => _.SubmitID).Take(10).ToList();
            return View(tmp);
        }

        public ActionResult Details(int id)
        {
            return View();
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
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

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
using System.Collections.Specialized;

namespace PIPOSKY2.Controllers
{
    public class ContestController : Controller
    {
        PIPOSKY2DbContext db = new PIPOSKY2DbContext();
        //
        // GET: /Contest/

        public ActionResult Index()
        {
            return View(db.Contest.ToList());
        }

        //
        // GET: /Contest/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Contest/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Contest/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
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

        //
        // GET: /Contest/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Contest/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Contest/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Contest/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

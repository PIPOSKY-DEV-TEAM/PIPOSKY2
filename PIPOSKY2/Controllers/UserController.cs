using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PIPOSKY2.Models;

namespace PIPOSKY2.Controllers
{
    public class UserController : Controller
    {
        private PIPOSKY2DbContext db = new PIPOSKY2DbContext();
        [HttpPost]
        public ActionResult Reg(RegFormModel info)
        {
            try
            {
                var tmp = new User();
                tmp.UserName = info.UserName;
                tmp.UserPwd = info.UserPwd;
                tmp.UserEmail = info.UserEmail;
                tmp.UserType = "";
                db.Users.Add(tmp);
                db.SaveChanges();
                Session.Add("_massage", "注册成功");
                Session.Add("_UserID",tmp.UserID);
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View(info);
            }
            
        }

        public ActionResult Reg()
        {
            return View();
        }

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

        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}

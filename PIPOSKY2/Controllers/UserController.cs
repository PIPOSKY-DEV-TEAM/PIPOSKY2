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
          
		//验证输入
			if (info.UserName!=null && db.Users.Any(_ => _.UserName == info.UserName))
			{
				ModelState.AddModelError("UserName","用户名已经存在");
			}
			if (info.UserEmail != null && db.Users.Any(_ => _.UserEmail == info.UserEmail))
			{
				ModelState.AddModelError("UserName", "Email已经存在");
			}
			if (info.UserPwd!=null && info.UserPwd.Length < 6)
			{
				ModelState.AddModelError("UserPwd","密码至少6位长");
			}
			if (info.UserPwd2 != null && info.UserPwd != info.UserPwd2)
			{
				ModelState.AddModelError("UserPwd2","两次密码不一致");
			}
			if (ModelState.IsValid)
			{
				var tmp = new User {UserName = info.UserName, UserPwd = info.UserPwd, UserEmail = info.UserEmail, UserType = ""};
				db.Users.Add(tmp);
				db.SaveChanges();
				Session.Add("_massage", "注册成功");
				Session.Add("_UserID", tmp.UserID);
				return RedirectToAction("Index", "Home");
			}
			return View(info);
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

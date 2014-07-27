using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;
using System.Data.Entity.Migrations;

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
			if (info.UserPwd2 != null && info.UserPwd != info.UserPwd2)
			{
				ModelState.AddModelError("UserPwd2","两次密码不一致");
			}
			if (ModelState.IsValid)
			{
				var tmp = new User {UserName = info.UserName, UserPwd = info.UserPwd, UserEmail = info.UserEmail, UserType = "admin"};
				db.Users.Add(tmp);
				db.SaveChanges();
                tmp = db.Users.FirstOrDefault(m => m.UserName == tmp.UserName);
                Session["User"] = tmp;
				//Session["_massage"] =  "注册成功";
				Session["_UserID"] = tmp.UserID;
                Session["_UserName"] = tmp.UserName;
                return RedirectToAction("Index","Courses");
			}
			return View(info);
        }

        public ActionResult Reg()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginFormModel currentLogin)
        {
            var tmp = db.Users.FirstOrDefault( m => m.UserName == currentLogin.UserName);
            if (tmp !=null )
            {
                if (tmp.UserPwd != currentLogin.UserPwd) {
                    ModelState.AddModelError("UserPwd", "密码错误，登陆失败！");
                    return View();
                }
                Session["User"] = tmp;
                Session["_UserID"] = tmp.UserID;
                Session["_UserName"] = tmp.UserName;
                return RedirectToAction("Index","Courses");
            }
            ModelState.AddModelError("UserName", "用户名不存在，登陆失败！");
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Exit() {
            Session["User"] = null;
            Session["_UserName"] = null;
            Session["_UserID"] = null;
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult EditInfo(RegFormModel EditUser)
        {
            var tmp = Session["User"] as User;
            
            if (tmp!= null)
            {
                if (EditUser.UserName != null && EditUser.UserName != "" && EditUser.UserName != " ")
                    tmp.UserName = EditUser.UserName;
                if (EditUser.UserEmail != null && EditUser.UserEmail != "" && EditUser.UserEmail != " ")
                    tmp.UserEmail = EditUser.UserEmail;

                db.Users.AddOrUpdate(tmp);
                db.SaveChanges();

                Session["User"] = tmp;
                Session["_UserID"] = tmp.UserID;
                Session["_UserName"] = tmp.UserName;
            }
            
            return RedirectToAction("info", "User");
        }

        public ActionResult EditInfo()
        {
            var tmp = Session["User"] as User;
            RegFormModel Edit = new RegFormModel();
            Edit.UserName = tmp.UserName;
            Edit.UserEmail = tmp.UserEmail;
            return View(Edit);
        }

        [HttpPost]
        public ActionResult EditPwd(ChangePasswordModel EditPwd)
        {
            var tmp = Session["User"] as User;
            if (tmp.UserPwd != EditPwd.OldPassword) {
                ModelState.AddModelError("OldPassword", "原密码错误！");
                return View();
            }
            if (EditPwd.NewPassword != EditPwd.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "新密码不匹配！");
            if (ModelState.IsValid)
            {
                tmp.UserPwd = EditPwd.NewPassword;
                db.Users.AddOrUpdate(tmp);
                db.SaveChanges();

                Session["User"] = tmp;
                return RedirectToAction("info", "User");
            }

            
            return View();
        }

        public ActionResult EditPwd()
        {
            return View();
        }

        public ActionResult AdministrateUsers() {
            return View();
        }

        [NonAction]
        private bool BatchAddUsers() {
            return false;
        }

        [NonAction]
        private bool Delete(int id)
        {
            return false;
        }

        public ActionResult Info()
        {
            if (Session["User"] != null)
            {
                User tmp = Session["User"] as User;
                return View(tmp);
            }
            return View();
        }
    }
}

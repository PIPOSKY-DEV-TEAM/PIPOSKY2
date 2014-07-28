using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIPOSKY2.Models;
using System.Data.Entity.Migrations;
using System.IO;
using System.Text.RegularExpressions;
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
                ModelState.AddModelError("UserEmail", "Email已经存在");
			}
			if (info.UserPwd2 != null && info.UserPwd != info.UserPwd2)
			{
				ModelState.AddModelError("UserPwd2","两次密码不一致");
			}
			if (ModelState.IsValid)
			{
				//var tmp = new User {UserName = info.UserName, UserPwd = info.UserPwd, UserEmail = info.UserEmail, UserType = "admin"};
                var tmp = new User { UserName = info.UserName, UserPwd = info.UserPwd, UserEmail = info.UserEmail, UserType = "normal" };
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
            if (EditUser.UserName != null && db.Users.Any(_ => _.UserName == EditUser.UserName))
            {
                ModelState.AddModelError("UserName", "用户名已经存在");
            }
            if (EditUser.UserEmail != null && db.Users.Any(_ => _.UserEmail == EditUser.UserEmail))
            {
                ModelState.AddModelError("UserEmail", "Email已经存在");
            }
            if (tmp!= null)
            {
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

        //public ActionResult AdministrateUsers() {
        //    return View(db.Users.ToList());
        //}

        //[HttpPost]
        public ActionResult AdministrateUsers()
        {
            int userid = (int)Session["_EditUserTypeID"];
            if (userid == -1) {
                return View(db.Users.ToList());
            }
            try
            {
                User tmp = db.Users.FirstOrDefault(_ => _.UserID == userid);
                tmp.UserType = Request.QueryString["editusertype"];
                db.Users.AddOrUpdate(tmp);
                db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("ErrorMessage", "保存失败，请再次修改。");
                return View(db.Users.ToList());
            }
            Session["_EditUserTypeID"] = -1;
            return View(db.Users.ToList());
        }

        [HttpPost]
        public ActionResult BatchAddUsers(FormCollection form)
        {
            //导入csv用户信息文件
            if (Request.Files.Count == 0)
            {
　　　　　　//Request.Files.Count 文件数为0上传不成功
　　　　　　  return View();　
　　　　　   }

            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength == 0)
            {
                //文件大小大（以字节为单位）为0时，做一些操作
　　　　　　  return View();
　　　　    }
　　　　    else
　　　　    {
　　　　　　  //文件大小不为0
              //上传文件名不变，保存在服务器上的BatchAddUser文件夹下　　
              file.SaveAs(Server.MapPath(@"~\") + System.IO.Path.GetFileName(file.FileName));
　　　　    }
            ViewBag.FileName = System.IO.Path.GetFileName(file.FileName);

            //读取文件内容，并添加用户
            string UserInfoStr = ReadFile(Server.MapPath(@"~\") + ViewBag.FileName);
            UserInfoStr = Regex.Replace(UserInfoStr, @"[\n\r]", ",");  
            UserInfoStr = UserInfoStr.TrimEnd((char[])"\n\r".ToCharArray());
            userInfo strUserName;
            userInfo strUserPwd;
            userInfo strUserEmail;
            int length = UserInfoStr.Length;
            int numofNewUser = 0;
            for (int beginIndex = 0; beginIndex < length; )
            {
                strUserName = getUserInfo(UserInfoStr, beginIndex);
                beginIndex += strUserName.lengthAdded;
                strUserPwd = getUserInfo(UserInfoStr, beginIndex);
                beginIndex += strUserPwd.lengthAdded;
                strUserEmail = getUserInfo(UserInfoStr, beginIndex);
                beginIndex += strUserEmail.lengthAdded;

                //验证输入
                if (strUserName.strU != null && db.Users.Any(_ => _.UserName == strUserName.strU))
                {
                    ModelState.AddModelError("ErrorMessage", "用户名" + strUserName.strU+"已经存在");
                    return View();
                }
                if (strUserEmail.strU != null && db.Users.Any(_ => _.UserEmail == strUserEmail.strU))
                {
                    ModelState.AddModelError("ErrorMessage", "Email" + strUserEmail.strU+"已经存在");
                    return View();
                }
                if (strUserPwd.strU == null || strUserPwd.strU.Length < 6 || strUserPwd.strU.Length > 20)
                {
                    ModelState.AddModelError("ErrorMessage", "密码"+ strUserPwd.strU+"的格式错误" );
                    return View();
                }
                if (strUserName.strU == null || strUserName.strU.Length < 4 || strUserName.strU.Length > 100)
                {
                    ModelState.AddModelError("ErrorMessage", "用户名" + strUserName.strU+"的格式错误");
                    return View();
                }
                string emailPatern = @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*$";
                //
                if (!Regex.IsMatch(strUserEmail.strU,emailPatern)) {
                    ModelState.AddModelError("ErrorMessage", "邮箱" + strUserEmail.strU + "的格式错误");
                    return View();
                }
                if (ModelState.IsValid)
                {
                    var tmp = new User { UserName = strUserName.strU, UserPwd = strUserPwd.strU, UserEmail = strUserEmail.strU, UserType = "normal" };
                    db.Users.Add(tmp);
                    db.SaveChanges();
                    numofNewUser++;
                }
            }
            ModelState.AddModelError("ErrorMessage", "成功添加"+numofNewUser+"名新用户！");
            return View();
        }
        
        public ActionResult BatchAddUsers() {
            return View();
        }

        public ActionResult EditUserType(int id) {
            Session["_EditUserTypeID"] = id;
            return RedirectToAction("AdministrateUsers", "User");
        }

        //public ActionResult SaveUserType(string newUserType)
        //{
        //    int userid = (int)Session["_EditUserTypeID"];
        //    try
        //    {
        //        User tmp = db.Users.FirstOrDefault(_ => _.UserID == userid);
        //        tmp.UserType = newUserType;
        //        db.Users.AddOrUpdate(tmp);
        //        db.SaveChanges();
        //    }
        //    catch
        //    {
        //        ModelState.AddModelError("ErrorMessage", "保存失败，请再次修改。");
        //        return RedirectToAction("AdministrateUsers", "User");
        //    }
        //    return RedirectToAction("AdministrateUsers", "User");
        //}

        public ActionResult Delete(int id)
        {
            try
            {
                User tmp = db.Users.FirstOrDefault(_ => _.UserID == id);
                db.Users.Remove(tmp);
                db.SaveChanges();
            }
            catch {
                ModelState.AddModelError("ErrorMessage", "删除失败，请再次删除。");
                return RedirectToAction("AdministrateUsers","User");
            }
            return RedirectToAction("AdministrateUsers", "User");
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

        [NonAction]
        public static string ReadFile(string Path)
        {
            string s = "";
            if (!System.IO.File.Exists(Path))
            {
                FileInfo file = new FileInfo(Path);

                //创建文件   

                FileStream fs = file.Create();

                //关闭文件流   

                fs.Close();
            }
            else
            {
                StreamReader f2 = new StreamReader(Path, System.Text.Encoding.GetEncoding("gb2312"));
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }
            return s;
        }

        [NonAction]
        public static userInfo getUserInfo(string userInfo,int beginIndex) {
            userInfo str = new userInfo();
            for (int i = beginIndex; i < userInfo.Length; i++)
            {
                if (userInfo[i] == ',' && userInfo[i + 1] != ',')
                {
                    str.lengthAdded = i - beginIndex + 1;
                    return str;
                }
                else if (userInfo[i] == ',' && userInfo[i + 1] == ',')
                {
                    str.lengthAdded = i - beginIndex + 2;
                    return str;
                }
                if (userInfo[i] == ' ')
                {
                    continue;
                }
                str.strU += userInfo[i];
            }
            return str;
        }

        public class userInfo {
            public string strU { get; set; }
            public int lengthAdded { get; set; }
        }
    }
}

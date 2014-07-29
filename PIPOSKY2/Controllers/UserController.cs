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
                if (currentLogin.KeepLogin) {
                    HttpCookie hc = new HttpCookie("_currentUser");
                    hc["UserName"] = currentLogin.UserName;
                    hc["UserPwd"] = currentLogin.UserPwd;
                    hc.Expires = DateTime.Today.AddDays(7);
                    Response.Cookies.Add(hc);
                }
                return RedirectToAction("Index","Courses");
            }
            ModelState.AddModelError("UserName", "用户名不存在，登陆失败！");
            return View();
        }

        public ActionResult Login()
        {
            if (Session["User"] != null) {
                return RedirectToAction("Index", "Courses");
            }
            try
            {
                string name = Request.Cookies["_currentUser"]["UserName"].ToString();
                string pwd = Request.Cookies["_currentUser"]["UserPwd"].ToString();
                if (name != null)
                {
                    User tmp = db.Users.FirstOrDefault(m => m.UserName == name);
                    if (tmp.UserPwd == pwd)
                    {
                        Session["User"] = tmp;
                        Session["_UserName"] = tmp.UserName;
                        Session["_UserID"] = tmp.UserID;
                        return RedirectToAction("Index", "Courses");
                    }
                }
            }
            catch {
                return View();
            }
            return View();
        }

        public ActionResult Exit() {
            Session.Abandon();
            HttpCookie hc = Request.Cookies["_currentUser"];
            try
            {
                hc.Expires = DateTime.Now.AddDays(-1);
                Response.AppendCookie(hc);
            }
            catch {
                return RedirectToAction("Login", "User");
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult EditInfo(RegFormModel EditUser)
        {
            var tmp = Session["User"] as User;
            if (EditUser.UserEmail != null && db.Users.Any(_ => _.UserEmail == EditUser.UserEmail))
            {
                ModelState.AddModelError("UserEmail", "Email已经存在");
            }
            if (tmp!= null)
            {
                tmp.UserEmail = EditUser.UserEmail;
                tmp.UserName = EditUser.UserName;
                db.Users.AddOrUpdate(tmp);
                db.SaveChanges();
                Session["User"] = tmp;
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

        public ActionResult AdministrateUsers()
        {
            return View(db.Users.ToList());
        }

        [HttpPost]
        public ActionResult AdministrateUsers(FormCollection info)
        {
            if (info["edittype"] != null)
            {
               try
               {
                    int userid = (int)Session["_EditUserTypeID"];
                    User tmp = db.Users.FirstOrDefault(_ => _.UserID == userid);
                    tmp.UserType = info["edittype"];
                    db.Users.AddOrUpdate(tmp);
                    db.SaveChanges();
                    Session["_EditUserTypeID"] = -1;
                }
               catch
               {
                   ModelState.AddModelError("ErrorMessage", "保存用户类型失败，请再次修改。");
                   return View(db.Users.ToList());
               }
            }
            if (info["item.StudentNumber"] != null)
            {
                try
                {
                    int stuNumID = (int)Session["_EditStuNumID"];
                    User tmp1 = db.Users.FirstOrDefault(_ => _.UserID == stuNumID);
                    tmp1.StudentNumber = info["item.StudentNumber"];
                    db.Users.AddOrUpdate(tmp1);
                    db.SaveChanges();
                    Session["_EditStuNumID"] = -1;
                }
                catch 
                {
                    ModelState.AddModelError("ErrorMessage", "保存学号失败，请再次修改。");
                    return View(db.Users.ToList());
                }
            }
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
            userInfo str;
            int length = UserInfoStr.Length;
            int numofNewUser = 0;
            for (int beginIndex = 0; beginIndex < length; )
            {
                str = getUserInfo(UserInfoStr, beginIndex);
                beginIndex = str.beginIndex;
                
                //验证输入
                if (str.strU.UserName != null && db.Users.Any(_ => _.UserName == str.strU.UserName))
                {
                    ModelState.AddModelError("ErrorMessage", "用户名" + str.strU.UserName + "已经存在");
                    return View();
                }
                if (str.strU.UserEmail != null && db.Users.Any(_ => _.UserEmail == str.strU.UserEmail))
                {
                    ModelState.AddModelError("ErrorMessage", "邮箱" + str.strU.UserEmail + "已经存在");
                    return View();
                }
                if (str.strU.StudentNumber != null && db.Users.Any(_ => _.StudentNumber == str.strU.StudentNumber))
                {
                    ModelState.AddModelError("ErrorMessage", "学号" + str.strU.StudentNumber + "已经存在");
                    return View();
                }
                if (str.strU.UserPwd == null || str.strU.UserPwd.Length < 6 || str.strU.UserPwd.Length > 20)
                {
                    ModelState.AddModelError("ErrorMessage", "密码" + str.strU.UserPwd + "的格式错误");
                    return View();
                }
                if (str.strU.UserName == null || str.strU.UserName.Length < 4 || str.strU.UserName.Length > 100)
                {
                    ModelState.AddModelError("ErrorMessage", "用户名" + str.strU.UserName + "的格式错误");
                    return View();
                }
                string emailPatern = @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*$";
                if (!Regex.IsMatch(str.strU.UserEmail, emailPatern))
                {
                    ModelState.AddModelError("ErrorMessage", "邮箱" + str.strU.UserEmail + "的格式错误");
                    return View();
                }
                if (ModelState.IsValid)
                {
                    var tmp = new User { UserName = str.strU.UserName, UserPwd = str.strU.UserPwd, UserEmail = str.strU.UserEmail, UserType = "normal", StudentNumber = str.strU.StudentNumber };
                    db.Users.Add(tmp);
                    db.SaveChanges();
                    numofNewUser++;
                }
            }
            ModelState.AddModelError("ErrorMessage", "成功添加"+numofNewUser+"名新用户！");
            return RedirectToAction("AdministrateUsers","User");
        }
        
        public ActionResult BatchAddUsers() {
            return View();
        }

        public ActionResult EditUserType(int id) {
            Session["_EditUserTypeID"] = id;
            return RedirectToAction("AdministrateUsers", "User");
        }

        public ActionResult EditStuNum(int id) {
            Session["_EditStuNumID"] = id;
            return RedirectToAction("AdministrateUsers", "User");
        }

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
            User tmp = Session["User"] as User;
            if (tmp != null)
            {
                return View(tmp);
            }
            return RedirectToAction("Login","User");
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
        public static userInfo getUserInfo(string Info,int beginIndex) {
            userInfo str = new userInfo();
            str.strU = new User();
            try
            {
                //user name
                for (; beginIndex < Info.Length; beginIndex++)
                {
                    if (Info[beginIndex] == ',' && Info[beginIndex + 1] != ',')
                    {
                        beginIndex += 1;
                        break;
                    }
                    if (Info[beginIndex] == ' ')
                    {
                        continue;
                    }
                    str.strU.UserName += Info[beginIndex];
                }
                //user password
                for (; beginIndex < Info.Length; beginIndex++)
                {
                    if (Info[beginIndex] == ',' && Info[beginIndex + 1] != ',')
                    {
                        beginIndex += 1;
                        break;
                    }
                    if (Info[beginIndex] == ' ')
                    {
                        continue;
                    }
                    str.strU.UserPwd += Info[beginIndex];
                }
                //user email
                for (; beginIndex < Info.Length; beginIndex++)
                {
                    if (Info[beginIndex] == ',' && (beginIndex + 1) < Info.Length && Info[beginIndex + 1] != ',')
                    {
                        beginIndex += 1;
                        break;
                    }
                    else if (Info[beginIndex] == ',' && (beginIndex + 1) < Info.Length && Info[beginIndex + 1] == ',')
                    {
                        if ((beginIndex+2) < Info.Length && Info[beginIndex + 2] == ',')
                        {
                            beginIndex += 3;
                            str.beginIndex = beginIndex;
                            return str;
                        }
                        beginIndex += 2;
                        str.beginIndex = beginIndex;
                        return str;
                    }
                    else if (Info[beginIndex] == ',' && (beginIndex + 1) == Info.Length)
                    {
                        beginIndex += 1;
                        break;
                    }
                    if (Info[beginIndex] == ' ')
                    {
                        continue;
                    }
                    str.strU.UserEmail += Info[beginIndex];
                }
                //user student number
                for (; beginIndex < Info.Length; beginIndex++)
                {
                    if (Info[beginIndex] == ',' && (beginIndex + 1) < Info.Length && Info[beginIndex + 1] != ',')
                    {
                        beginIndex += 1;
                        break;
                    }
                    else if (Info[beginIndex] == ',' && (beginIndex + 1) < Info.Length && Info[beginIndex + 1] == ',')
                    {
                        if ((beginIndex + 2) < Info.Length && Info[beginIndex + 2] == ',')
                        {
                            beginIndex += 3;
                            str.beginIndex = beginIndex;
                            return str;
                        }
                        beginIndex += 2;
                        str.beginIndex = beginIndex;
                        return str;
                    }
                    else if (Info[beginIndex] == ',' && (beginIndex + 1) == Info.Length) {
                        beginIndex += 1;
                        break;
                    }
                    if (Info[beginIndex] == ' ')
                    {
                        continue;
                    }
                    str.strU.StudentNumber += Info[beginIndex];
                }
            }
            catch { 
                
            }
            str.beginIndex = beginIndex;
            return str;
        }

        public class userInfo {
            public User strU { get; set; }
            public int beginIndex { get; set; }
        }
    }
}

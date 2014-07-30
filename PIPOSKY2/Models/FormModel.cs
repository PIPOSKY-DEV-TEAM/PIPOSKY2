using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PIPOSKY2.Models
{
    public class LoginFormModel
    {
        [Required]
        [Display(Name = "用户名")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "用户名长度不能大于{2} 且要小于{1}")]
        [RegularExpression(@"^[^\s]*$",
            ErrorMessage = "用户名格式有误，不能有空格")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码不能大于{2} 且要小于{1}")]
        [RegularExpression(@"^[^\s]*$",
            ErrorMessage = "密码格式有误，不能有空格")]
        public string UserPwd { get; set; }
        [Required]
        [Display(Name = "保持登录")]
        public bool KeepLogin { get; set; }
    }

    public class RegFormModel
    {
        [Required]
        [Display(Name = "用户名")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "用户名长度不能大于{2} 且要小于{1}")]
        [RegularExpression(@"^[^\s]*$",
            ErrorMessage = "用户名格式有误，不能有空格")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "密码")]
		[DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码不能大于{2} 且要小于{1}")]
        [RegularExpression(@"^[^\s]*$",
            ErrorMessage = "密码格式有误，不能有空格")]
        public string UserPwd { get; set; }

        [Required]
        [Display(Name = "确认密码")]
		[DataType(DataType.Password)]
        public string UserPwd2 { get; set; }

        [Required]
        [Display(Name = "Email")]
		[DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }
    }

    public class ChangePasswordModel {
        [Required]
        [Display(Name = "原密码")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "新密码")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码不能大于{2} 且要小于{1}")]
        [RegularExpression(@"^[^\s]*$",
            ErrorMessage = "密码格式有误，不能有空格")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "确认密码")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class CheckinLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["User"] == null)
            {
                filterContext.HttpContext.Response.Redirect("/User/Login");
            }
        }
    }

    public class CheckinLogOffAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["User"] != null)
            {
                filterContext.HttpContext.Response.Redirect("/User/info");
            }
        }
    }  

    public class CheckAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["User"] == null)
            {
                filterContext.HttpContext.Response.Redirect("/User/Login");
            }
            User tmp = filterContext.HttpContext.Session["User"] as User;
            if (tmp.UserType != "admin")
            {
                filterContext.HttpContext.Response.Redirect("/User/info");
            }
        }
    }

    public class CheckAdminOrEditorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["User"] == null)
            {
                filterContext.HttpContext.Response.Redirect("/User/Login");
            }
            User tmp = filterContext.HttpContext.Session["User"] as User;
            if ((tmp.UserType != "admin") && (tmp.UserType != "editor"))
            {
                filterContext.HttpContext.Response.Redirect("/User/info");
            }
        }
    } 

    public class HomeworkFormModel
    {
        [Required]
        [Display(Name = "作业名称")]
        public string HomeworkName { get; set; }
        [Required]
        [Display(Name = "开始时间")]
        public string StartTime { get; set; }
        [Required]
        [Display(Name = "结束时间")]
        public string EndTime { get; set; }
        [Required]
        public int HomeworkID { get; set; }
        [Required]
        public int CourseID { get; set; }
    }

    public class UploadProblemFormModel
    {
        [Required]
        [Display(Name = "题目名称")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "文件路径")]
        public HttpPostedFileBase File{ get; set; }
        [Display(Name = "是否公开")]
        public string visible { get; set; }
    }
	
	public class SubmitFormModel
	{
		[Required]
		[Display(Name = "语言")]
		public string Lang { get; set; }

		[Required]
		[Display(Name="题目")]
		public int? PID { get; set; }

		[Required]
		[Display(Name="提交代码")]
		public string Source { get; set; }
	}

}
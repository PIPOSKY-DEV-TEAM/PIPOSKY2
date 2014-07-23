using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PIPOSKY2.Models
{
    public class LoginFormModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "密码")]
        public string UserPwd { get; set; }
        [Required]
        [Display(Name = "保持登录")]
        public bool KeepLogin { get; set; }
    }

    public class RegFormModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "密码")]
		[DataType(DataType.Password)]
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


    public class AddContestFormModel
    {
        [Required]
        [Display(Name = "比赛名称")]
        public string ContestName { get; set; }
        [Required]
        [Display(Name = "开始时间")]
        public string StartTime { get; set; }
        [Required]
        [Display(Name = "结束时间")]
        public string EndTime { get; set; }
    }
    public class UploadProblemFormModel
    {
        [Required]
        [Display(Name = "题目名称")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "文件路径")]
        public HttpPostedFileBase File{ get; set; }
    }
}
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
        public string UserPwd { get; set; }
        [Required]
        [Display(Name = "确认密码")]
        public string UserPwd2 { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ts.ictu.Models
{
    public class PassWordModels
    {
        [Required(ErrorMessage = "The field is required")]
        //[StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "Password not match")]
        public string ConfirmPassword { get; set; }
    }
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Bạn phải nhập vào tên đăng nhập")]
        [Display(Name = "Mã số")]
        public string UserName { get; set; }
    }

    public class ResetPasswordModel : PassWordModels
    {
        [Required(ErrorMessage = "Bạn phải nhập vào tên đăng nhập")]
        [Display(Name = "Mã số")]
        public string UserName { get; set; }
    }

    public class ConfirmForgotPasswordModel : ResetPasswordModel
    {
        public string Code { get; set; }
    }
}
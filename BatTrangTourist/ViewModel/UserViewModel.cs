using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace BatTrangTourist.ViewModel
{
    public class RegisterViewModel
    {
        [Display(Name = "Tên đăng nhập"), Required(ErrorMessage = "Hãy nhập tên đăng nhập"),
         StringLength(50, ErrorMessage = "Tối đa 20 ký tự"), RegularExpression(@"[a-z0-9_.]{4,20}", ErrorMessage = "Từ 4 đến 20 ký tự, chỉ nhập chữ thường, số 0-9, dấu . và dấu _")]
        public string Username { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập lại mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Số điện thoại"), Required(ErrorMessage = "Hãy nhập số điện thoại"),
         RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Địa chỉ email"), Required(ErrorMessage = "Hãy nhập Email"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), EmailAddress(ErrorMessage = "Email không chính xác"), Remote("CheckEmail", "User")]
        public string Email { get; set; }
        public int UserId { get; set; }
    }
    public class UserLoginModel
    {
        [Display(Name = "Tên đăng nhập"), Required(ErrorMessage = "Hãy nhập tên đăng nhập"), RegularExpression(@"[a-z0-9_.]{4,20}", ErrorMessage = "Từ 4 đến 20 ký tự, chỉ nhập chữ thường, số 0-9, dấu . và dấu _")]
        public string Username { get; set; }
        [Display(Name = "Mật khẩu"), Required(ErrorMessage = "Hãy nhập mật khẩu")]
        public string Password { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Display(Name = "Email"), EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ"), Required(ErrorMessage = "Hãy nhập email của bạn")]
        public string Email { get; set; }
    }
    public class SetNewPasswordViewModel
    {
        public string Username { get; set; }
        public string Token { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập lại mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác")]
        public string ConfirmPassword { get; set; }
    }
}
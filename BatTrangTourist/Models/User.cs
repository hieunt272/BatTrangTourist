using Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BatTrangTourist.Models
{
    public class User
    {
        public int Id { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Vui lòng nhập họ tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string FullName { get; set; }
        [Display(Name = "Tên đăng nhập", Description = "Tên đăng nhập"), Required(ErrorMessage = "Hãy điền tên đăng nhập"), RegularExpression(@"[a-z0-9_.]{4,10}", ErrorMessage = "Chỉ nhập chữ thường và số 0-9, từ 4-10 ký tự"), UIHint("TextBox")]
        public string Username { get; set; }
        [Display(Name = "Ảnh đại diện"), StringLength(500)]
        public string Avatar { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Hãy nhập mật khẩu"), StringLength(60, ErrorMessage = "Tối đa 60 ký tự"), UIHint("Password")]
        public string Password { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Địa chỉ email"), Required(ErrorMessage = "Hãy nhập Email"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), EmailAddress(ErrorMessage = "Email không chính xác")]
        public string Email { get; set; }
        [Display(Name = "Hoạt động", Description = "Hoạt động")]
        [StringLength(50)]
        public string Token { get; set; }
        public bool Active { get; set; }
        public string Color { get; set; }
        public DateTime CreateDate { get; set; }
        public User()
        {
            CreateDate = DateTime.Now;
            Token = HtmlHelpers.RandomCode(50);
        }
    }
}
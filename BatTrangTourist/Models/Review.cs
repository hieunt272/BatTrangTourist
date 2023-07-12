using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BatTrangTourist.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Vote { get; set; }
        [Display(Name = "Hình ảnh"), StringLength(500)]
        public string Image { get; set; }
        [Display(Name = "Nội dung đánh giá"), Required(ErrorMessage = "Hãy nhập nội dung đánh giá"), UIHint("TextArea")]
        public string Content { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập họ tên"), UIHint("TextBox"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string FullName { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string Mobile { get; set; }
        [Display(Name = "Ảnh đại diện"), StringLength(500)]
        public string Avatar { get; set; }
        public DateTime CreateDate { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        public string Color { get; set; }
        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Review()
        {
            CreateDate = DateTime.Now;
            Active = false;

        }
    }
}
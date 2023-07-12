using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BatTrangTourist.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        [Display(Name = "Lời nhận xét"), StringLength(700, ErrorMessage = "Tối đa 700 ký tự"), UIHint("TextArea")]
        public string Content { get; set; }
        [Display(Name = "Ảnh đại diện"), StringLength(500)]
        public string Image { get; set; }
        [Display(Name = "Họ tên", Description = "Tên dài tối đa 100 ký tự"), Required(ErrorMessage = "Hãy nhập Họ tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"),
         UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Loại khách hàng", Description = "Dài tối đa 100 ký tự"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Classify { get; set; }
        [Display(Name = "Tên Tour", Description = "Dài tối đa 100 ký tự"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string TourName { get; set; }
        [Display(Name = "Đánh giá"), UIHint("NumberBox"), Range(1, 5, ErrorMessage = "Chỉ chọn từ 1 đến 5")]
        public int Star { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        public Feedback()
        {
            Active = true;
        }
    }
}
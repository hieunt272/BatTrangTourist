using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BatTrangTourist.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Display(Name = "Câu hỏi", Description = "Câu hỏi dài tối đa 150 ký tự"),
         Required(ErrorMessage = "Hãy nhập câu hỏi"), StringLength(150, ErrorMessage = "Tối đa 150 ký tự"),
         UIHint("TextBox")]
        public string QuestionName { get; set; }
        [Display(Name = "Nội dung"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"),
         RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Tour"), Required(ErrorMessage = "Hãy chọn Tour")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public Question()
        {
            Active = true;
        }
    }
}
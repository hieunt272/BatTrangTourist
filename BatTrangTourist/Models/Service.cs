using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BatTrangTourist.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Display(Name = "Tên dịch vụ"), Required(ErrorMessage = "Hãy nhập tên dịch vụ"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Lịch trình"), UIHint("EditorBox")]
        public string Schedule { get; set; }
        [Display(Name = "Bao gồm"), UIHint("EditorBox")]
        public string Include { get; set; }
        [Display(Name = "Lưu ý trước khi đặt"), UIHint("EditorBox")]
        public string Note { get; set; }
        [Display(Name = "Điều khoản chung"), UIHint("EditorBox")]
        public string Terms { get; set; }
        [Display(Name = "Hướng dẫn sử dụng"), UIHint("EditorBox")]
        public string Manual { get; set; }
        [Display(Name = "Giá người lớn"), DisplayFormat(DataFormatString = "{0:N0} ₫")]
        public decimal? AdultPrice { get; set; }
        [Display(Name = "Giá người lớn"), DisplayFormat(DataFormatString = "{0:N0} ₫")]
        public decimal? ChildPrice { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"),
         RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }

        [Display(Name = "Tour"), Required(ErrorMessage = "Hãy chọn Tour")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        public Service()
        {
            Active = true;
        }
    }
}
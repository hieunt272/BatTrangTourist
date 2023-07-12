using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BatTrangTourist.Models
{
    public class Subcribe
    {
        public int Id { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Hãy nhập email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        public Subcribe()
        {
            CreateDate = DateTime.Now;
        }
    }
}
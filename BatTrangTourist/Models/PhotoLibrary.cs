using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BatTrangTourist.Models
{
    public class PhotoLibrary
    {
        public int Id { get; set; }
        [Display(Name = "Tên Album"), Required(ErrorMessage = "Hãy nhập tên Album"), UIHint("TextBox")]
        public string AlbumName { get; set; }
        [Display(Name = "Danh sách ảnh"), UIHint("UploadMultiFile")]
        public string ListImage { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Ngày đăng")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"),
            RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Nội dung"), UIHint("EditorBox")]
        public string Body { get; set; }
        [StringLength(300)]
        public string Url { get; set; }

        public PhotoLibrary()
        {
            CreateDate = DateTime.Now;
            Active = true;
        }
    }
}
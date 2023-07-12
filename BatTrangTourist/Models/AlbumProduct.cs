using System.ComponentModel.DataAnnotations;

namespace BatTrangTourist.Models
{
    public class AlbumProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "Tiêu đề ảnh"), StringLength(500), UIHint("TextBox")]
        public string TitleImage { get; set; }
        [Display(Name = "Hình ảnh"), StringLength(500)]
        public string Image { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Số lượng ảnh"), Range(1, 50, ErrorMessage = "Chỉ chọn từ 1 đến 50"), UIHint("NumberBox")]
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
    }
}
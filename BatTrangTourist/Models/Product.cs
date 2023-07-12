using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BatTrangTourist.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Display(Name = "Tên sản phẩm", Description = "Tên sản phẩm dài tối đa 200 ký tự")
            , Required(ErrorMessage = "Hãy nhập Tên sản phẩm"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Ảnh Tour")]
        public string Image { get; set; }
        [Display(Name = "Điểm nổi bật"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Về dịch vụ này"), UIHint("EditorBox")]
        public string About { get; set; }
        [Display(Name = "Lưu ý"), UIHint("EditorBox")]
        public string Note { get; set; }
        [Display(Name = "Giá"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? Price { get; set; }
        [Display(Name = "Giá khuyến mãi"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? PriceSale { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Ngày đăng")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự")
            , RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool Home { get; set; }
        [Display(Name = "Tour Hot")]
        public bool Hot { get; set; }
        [Display(Name = "Tour yêu thích")]
        public bool Favorite { get; set; }
        [Display(Name = "Ảnh Đánh giá")]
        public string ImageReview { get; set; }
        [StringLength(300)]
        public string Url { get; set; }
        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? FinalPrice => PriceSale ?? Price;
        [Display(Name = "Danh mục sản phẩm"), Required(ErrorMessage = "Hãy chọn danh mục sản phẩm")]
        public int ProductCategoryId { get; set; }

        public double ResultReview()
        {
            double count = 0;
            if (Reviews.Any())
            {
                count = Reviews.Average(a => a.Vote);
            }
            return count;
        }

        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<AlbumProduct> AlbumProducts { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Service> Services { get; set; }

        public Product()
        {
            CreateDate = DateTime.Now;
            Active = true;
        }
    }
}
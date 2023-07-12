using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatTrangTourist.Models
{
    public class Order
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string OrderNumber { get; set; }
        public DateTime CreateDate { get; set; }
        [Display(Name = "Tình trạng thanh toán")]
        public bool Payment { get; set; }
        [Display(Name = "Ngày khởi hành"), UIHint("DateTimePicker"), Required(ErrorMessage = "Hãy chọn ngày khởi hành")]
        public DateTime TransportDate { get; set; }
        public int? AdultQuantity { get; set; }
        public int? AdultPrice { get; set; }
        public int? ChildQuantity { get; set; }
        public int? ChildPrice { get; set; }
        [Display(Name = "Trạng thái đơn hàng"), UIHint("DisplayOrderStatus")]
        public int Status { get; set; }
        public bool Viewed { get; set; }
        public string DiscountCode { get; set; }
        public decimal Total { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public int? OrderMemberId { get; set; }
        public int? ServiceId { get; set; }
        public virtual Service Service { get; set; }

        public Order()
        {
            CreateDate = DateTime.Now;
            Payment = false;
            Viewed = false;
        }
    }
    [ComplexType]
    public class CustomerInfo
    {
        [Display(Name = "Họ và tên *"), Required(ErrorMessage = "Hãy nhập họ và tên"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Fullname { get; set; }
        [Display(Name = "Địa chỉ *"), Required(ErrorMessage = "Hãy nhập địa chỉ"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string Address { get; set; }
        [Display(Name = "Điện thoại *"), Required(ErrorMessage = "Hãy nhập điện thoại"), StringLength(11, MinimumLength = 7, ErrorMessage = "Điện thoại từ 7, 11 ký tự"), UIHint("TextBox")]
        public string Mobile { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ"), Display(Name = "Email *"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Required(ErrorMessage = "Hãy nhập email"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Yêu cầu thêm"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), DataType(DataType.MultilineText)]
        public string Body { get; set; }


        [Display(Name = "Thành viên mới")]
        public bool IsNewMember { get; set; }
    }
}
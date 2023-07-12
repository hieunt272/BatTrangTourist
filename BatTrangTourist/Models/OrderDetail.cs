using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatTrangTourist.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; } 
        public int ServiceId { get; set; }
        public int? AdultQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? AdultPrice { get; set; }
        public int? ChildQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? ChildPrice { get; set; }

        public virtual Order Order { get; set; }
        public virtual Service Service { get; set; }
    }
}
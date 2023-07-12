using BatTrangTourist.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace BatTrangTourist.ViewModel
{
    public class InsertProductCatViewModel
    {
        [Display(Name = "Danh mục sản phẩm")]
        public int? ParentId { get; set; }
        public SelectList RootCats { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }

    public class ListProductViewModel
    {
        public PagedList.IPagedList<Product> Products { get; set; }
        public SelectList SelectCategories { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public SelectList ThirdCategoryList { get; set; }
        public int? ParentId { get; set; }
        public int? CatId { get; set; }
        public int? ThirdId { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }

        public ListProductViewModel()
        {
            ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
            ThirdCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
        }
    }

    public class InsertProductViewModel
    {
        public Product Product { get; set; }
        [Display(Name = "Danh mục sản phẩm con"), Required(ErrorMessage = "Hãy chọn danh mục sản phẩm")]
        public int ParentId { get; set; }
        [Display(Name = "Danh mục sản phẩm cha")]
        public int? CategoryId { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        [Display(Name = "Giá niêm yết"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string Price { get; set; }
        [Display(Name = "Giá khuyến mãi"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceSale { get; set; }
    }

    public class ListReviewViewModel
    {
        public PagedList.IPagedList<Review> Reviews { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public int ProductId { get; set; }
    }

    public class InsertAlbumProductViewModel
    {
        public IEnumerable<AlbumProduct> AlbumProducts { get; set; }
        public AlbumProduct AlbumProduct { get; set; }
        public int ProId { get; set; }
    }

    public class ListQuestionViewModel
    {
        public PagedList.IPagedList<Question> Questions { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public int ProductId { get; set; }
    }
    public class ListServiceViewModel
    {
        public PagedList.IPagedList<Service> Services { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public int ProductId { get; set; }
    }
    public class InsertServiceViewModel
    {
        public Service Service { get; set; }
        [Display(Name = "Danh sách Tour"), Required(ErrorMessage = "Hãy chọn Tour")]
        public int ProductId { get; set; }
        public IEnumerable<Product> Products { get; set; }
        [Display(Name = "Giá người lớn"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string AdultPrice { get; set; }
        [Display(Name = "Giá trẻ em"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string ChildPrice { get; set; }
        [UIHint("TextBox"), Display(Name = "Thẻ Tags")]
        public string Tags { get; set; }
    }

    public class OrderDetailModel
    {
        public Service Service { get; set; }
        public Order Order { get; set; }
    }
}
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatTrangTourist.Models;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace BatTrangTourist.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Product> ProductFavorites { get; set; }
        public IEnumerable<Product> ProductHots { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Feedback> Feedbacks { get; set; }
    }

    public class HeaderViewModel
    {
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
    }
    public class FooterViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
    }
    public class AllArticleViewModel
    {
        public IPagedList<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> Categories { get; set; }
        public string Sort { set; get; }
        public int BeginCount { get; set; }
        public int EndCount { get; set; }
    }
    public class ArticleCategoryViewModel
    {
        public ArticleCategory Category { get; set; }
        public IPagedList<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> Categories { get; set; }
        public string Sort { get; set; }
        public int BeginCount { get; set; }
        public int EndCount { get; set; }
    }
    public class ArticleDetailViewModel
    {
        public Article Article { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
    public class ArticleSearchViewModel
    {
        public string Keywords { get; set; }
        public IPagedList<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> Categories { get; set; }
        public string Sort { get; set; }
        public int BeginCount { get; set; }
        public int EndCount { get; set; }
    }
    public class MenuArticleViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
    }
    public class ProductDetailViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IPagedList<Review> Reviews { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Review Review { get; set; }
        public int ReviewCount { get; set; }
        public int MoreImage { get; set; }
        public AlbumProduct LargeImage { get; set; }
        public IEnumerable<AlbumProduct> AfterImages { get; set; }
        public AlbumProduct LastImage { get; set; }
        public IEnumerable<AlbumProduct> AlbumProducts { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public Service FirstService { get; set; }
    }
    public class CategoryProductViewModel
    {
        public ProductCategory Category { get; set; }
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
    }
    public class SearchProductViewModel
    {
        public string Keywords { get; set; }
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public int? CatId { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
    }

    public class PhotoLibraryViewModel
    {
        public IPagedList<PhotoLibrary> PhotoLibraries { get; set; }
    }

    public class AlbumDetailViewModel
    {
        public PhotoLibrary PhotoLibrary { get; set; }
        public IEnumerable<PhotoLibrary> PhotoLibraries { get; set; }
    }

    public class BookTourViewModel
    {
        public string ProductName { get; set; }
        public string ProducImg { get; set; }
        public decimal? AdultPrice { get; set; }
        public decimal? ChildPrice { get; set; }
        public int ProductStar { get; set; }
        public int CartTotal { get; set; }
        public double Rating { get; set; }
        public Order Order { get; set; }
        public int? AdultQuantity { get; set; }
        public int? ChildQuantity { get; set; }
        [Required]
        public string DateGo { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
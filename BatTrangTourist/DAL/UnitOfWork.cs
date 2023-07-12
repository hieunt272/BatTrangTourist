using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using BatTrangTourist.Models;
using WebGrease.Css.Ast.Selectors;

namespace BatTrangTourist.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly DataEntities _context = new DataEntities();
        private GenericRepository<Admin> _adminRepository;
        private GenericRepository<ArticleCategory> _articategoryRepository;
        private GenericRepository<Article> _articleRepository;
        private GenericRepository<Banner> _bannerRepository;
        private GenericRepository<Contact> _contactRepository;
        private GenericRepository<ConfigSite> _configRepository;
        private GenericRepository<Product> _productRepository;
        private GenericRepository<ProductCategory> _productCategoryRepository;
        private GenericRepository<Order> _orderRepository;
        private GenericRepository<OrderDetail> _orderdetailRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<Subcribe> _subcribeRepository;
        private GenericRepository<Review> _reviewRepository;
        private GenericRepository<AlbumProduct> _albumProductRepository;
        private GenericRepository<Feedback> _feedbackRepository;
        private GenericRepository<PhotoLibrary> _photoLibraryRepository;
        private GenericRepository<Question> _questionRepository;
        private GenericRepository<Service> _serviceRepository;
        private GenericRepository<Tag> _tagRepository;

        public GenericRepository<Service> ServiceRepository =>
            _serviceRepository ?? (_serviceRepository = new GenericRepository<Service>(_context));
        public GenericRepository<Tag> TagRepository =>
            _tagRepository ?? (_tagRepository = new GenericRepository<Tag>(_context));
        public GenericRepository<Question> QuestionRepository => 
            _questionRepository ?? (_questionRepository = new GenericRepository<Question>(_context));
        public GenericRepository<PhotoLibrary> PhotoLibraryRepository =>
            _photoLibraryRepository ?? (_photoLibraryRepository = new GenericRepository<PhotoLibrary>(_context));
        public GenericRepository<Feedback> FeedbackRepository =>
            _feedbackRepository ?? (_feedbackRepository = new GenericRepository<Feedback>(_context));
        public GenericRepository<AlbumProduct> AlbumProductRepository => 
            _albumProductRepository ?? (_albumProductRepository = new GenericRepository<AlbumProduct>(_context));
        public GenericRepository<Review> ReviewRepository =>
            _reviewRepository ?? (_reviewRepository = new GenericRepository<Review>(_context));
        public GenericRepository<Subcribe> SubcribeRepository =>
            _subcribeRepository ?? (_subcribeRepository = new GenericRepository<Subcribe>(_context));
        public GenericRepository<User> UserRepository =>
            _userRepository ?? (_userRepository = new GenericRepository<User>(_context));
        public GenericRepository<OrderDetail> OrderDetailRepository =>
            _orderdetailRepository ?? (_orderdetailRepository = new GenericRepository<OrderDetail>(_context));
        public GenericRepository<Order> OrderRepository =>
            _orderRepository ?? (_orderRepository = new GenericRepository<Order>(_context));
        public GenericRepository<Product> ProductRepository =>
            _productRepository ?? (_productRepository = new GenericRepository<Product>(_context));
        public GenericRepository<ProductCategory> ProductCategoryRepository =>
            _productCategoryRepository ?? (_productCategoryRepository = new GenericRepository<ProductCategory>(_context));
        public GenericRepository<ConfigSite> ConfigSiteRepository =>
            _configRepository ?? (_configRepository = new GenericRepository<ConfigSite>(_context));
        public GenericRepository<Contact> ContactRepository =>
            _contactRepository ?? (_contactRepository = new GenericRepository<Contact>(_context));
        public GenericRepository<Banner> BannerRepository =>
            _bannerRepository ?? (_bannerRepository = new GenericRepository<Banner>(_context));
        public GenericRepository<Article> ArticleRepository =>
            _articleRepository ?? (_articleRepository = new GenericRepository<Article>(_context));
        public GenericRepository<ArticleCategory> ArticleCategoryRepository =>
            _articategoryRepository ?? (_articategoryRepository = new GenericRepository<ArticleCategory>(_context));
        public GenericRepository<Admin> AdminRepository =>
            _adminRepository ?? (_adminRepository = new GenericRepository<Admin>(_context));
        public void Save()
        {
            _context.SaveChanges();
        }
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
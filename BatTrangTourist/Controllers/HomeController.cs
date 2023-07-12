using Helpers;
using BatTrangTourist.DAL;
using BatTrangTourist.Filters;
using BatTrangTourist.Models;
using BatTrangTourist.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.IO;
using System.Globalization;
using ImageResizer.ExtensionMethods;

namespace BatTrangTourist.Controllers
{
    [MemberLoginFilter]
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];
        private int UserId => Convert.ToInt32(RouteData.Values["Id"]);
        private User UserIdentify => _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Id == UserId).SingleOrDefault();
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];

        private IEnumerable<ArticleCategory> ArticleCategories =>
            _unitOfWork.ArticleCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<ProductCategory> ProductCategories =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));

        #region Home
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Banners = _unitOfWork.BannerRepository.GetQuery(b => b.Active, o => o.OrderBy(b => b.Sort)),
                ProductHots = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Hot, o => o.OrderBy(a => a.Sort), 12),
                ProductFavorites = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Favorite, o => o.OrderBy(a => a.Sort), 12),
                Articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Home, o => o.OrderByDescending(a => a.CreateDate), 9),
                Feedbacks = _unitOfWork.FeedbackRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort), 12),
            };

            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult Header()
        {
            var model = new HeaderViewModel
            {
                ArticleCategories = ArticleCategories.Where(a => a.ShowMenu),
                ProductCategories = ProductCategories.Where(a => a.ShowMenu)
            };
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var model = new FooterViewModel
            {
                Articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.ShowFooter &&
                    a.ArticleCategory.TypePost == TypePost.Policy, o => o.OrderByDescending(a => a.CreateDate))
            };
            return PartialView(model);
        }
        #endregion

        [Route("lien-he")]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ContactForm(Contact model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            _unitOfWork.ContactRepository.Insert(model);
            _unitOfWork.Save();
            var subject = "Email liên hệ từ website: " + Request.Url?.Host;
            var body = $"<p>Tên người liên hệ: {model.FullName},</p>" +
                       $"<p>Email: {model.Email},</p>" +
                       $"<p>Số điện thoại: {model.Mobile},</p>" +
                       $"<p>Nội dung: {model.Body}</p>" +
                       $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";
            Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, ConfigSite.Email, Email, Email, Password, "Bát Tràng Tourist"));

            return Json(new { status = true, msg = "Gửi liên hệ thành công.\nChúng tôi sẽ liên lạc với bạn sớm nhất có thể." });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SubcribeForm(string email)
        {
            var isEmail = new EmailAddressAttribute().IsValid(email);
            if (!isEmail || string.IsNullOrEmpty(email))
            {
                return Json(new { status = false, msg = "Email không hợp lệ, vui lòng thử lại!" });
            }

            Subcribe model = new Subcribe { Email = email };

            _unitOfWork.SubcribeRepository.Insert(model);
            _unitOfWork.Save();
            return Json(new { status = true, msg = "Đăng ký nhân bạn tin thành công!" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReviewForm(ProductDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            _unitOfWork.ReviewRepository.Insert(model.Review);
            _unitOfWork.Save();

            return Json(new { status = true, msg = "Gửi đánh giá thành công.\nChúng tôi sẽ xét duyệt đánh giá của bạn sớm nhất có thể." });
        }

        #region Article 
        [Route("blogs/{url}.html", Order = 1)]
        public ActionResult ArticleDetail(string url)
        {
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Url == url && a.Active).FirstOrDefault();
            if (article == null)
            {
                return RedirectToAction("Index");
            }

            var model = new ArticleDetailViewModel
            {
                Article = article,
                Articles = _unitOfWork.ArticleRepository.GetQuery(p =>
                p.Active && (p.ArticleCategoryId == article.ArticleCategoryId && p.Id != article.Id), q => q.OrderByDescending(a => a.CreateDate), 6),
            };
            return View(model);
        }
        [Route("blogs/{url:regex(^(?!.*(vcms|uploader|article|banner|contact|product|user)).*$)}", Order = 2)]
        public ActionResult ArticleCategory(int? page, string url, string sort)
        {
            var category = _unitOfWork.ArticleCategoryRepository.GetQuery(a => a.CategoryActive && a.Url == url).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            var articles = _unitOfWork.ArticleRepository.GetQuery(
                a => a.Active && (a.ArticleCategoryId == category.Id || a.ArticleCategory.ParentId == category.Id),
                q => q.OrderByDescending(a => a.CreateDate));
            var pageNumber = page ?? 1;
            var pageSize = 12;

            if (articles.Count() == 1)
            {
                var fi = articles.First();
                return RedirectToAction("ArticleDetail", new { url = fi.Url });
            }

            switch (sort)
            {
                case "date-asc":
                    articles = articles.OrderBy(a => a.CreateDate);
                    break;
                default:
                    articles = articles.OrderByDescending(a => a.CreateDate);
                    break;
            }

            var model = new ArticleCategoryViewModel
            {
                Category = category,
                Articles = articles.ToPagedList(pageNumber, pageSize),
                Categories = ArticleCategories.Where(a => a.TypePost == TypePost.Article),
                Sort = sort,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };
            return View(model);
        }
        public PartialViewResult GetArticleCategory(int? page, string url, string sort)
        {
            var category = _unitOfWork.ArticleCategoryRepository.GetQuery(a => a.CategoryActive && a.Url == url).FirstOrDefault();

            var articles = _unitOfWork.ArticleRepository.GetQuery(
                a => a.Active && (a.ArticleCategoryId == category.Id || a.ArticleCategory.ParentId == category.Id),
                q => q.OrderByDescending(a => a.CreateDate));
            var pageNumber = page ?? 1;
            var pageSize = 12;

            switch (sort)
            {
                case "date-asc":
                    articles = articles.OrderBy(a => a.CreateDate);
                    break;
                default:
                    articles = articles.OrderByDescending(a => a.CreateDate);
                    break;
            }

            var model = new ArticleCategoryViewModel
            {
                Category = category,
                Articles = articles.ToPagedList(pageNumber, pageSize),
                Sort = sort,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };
            return PartialView(model);
        }
        [Route("blogs")]
        public ActionResult AllArticle(int? page, string sort)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.ArticleCategory.TypePost == TypePost.Article);

            switch (sort)
            {
                case "date-asc":
                    articles = articles.OrderBy(a => a.CreateDate);
                    break;
                default:
                    articles = articles.OrderByDescending(a => a.CreateDate);
                    break;
            }

            var model = new AllArticleViewModel()
            {
                Articles = articles.ToPagedList(pageNumber, pageSize),
                Categories = ArticleCategories.Where(a => a.TypePost == TypePost.Article),
                Sort = sort,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };
            return View(model);
        }
        public PartialViewResult GetArticle(int? page, string sort)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.ArticleCategory.TypePost == TypePost.Article).AsNoTracking();

            switch (sort)
            {
                case "date-asc":
                    articles = articles.OrderBy(a => a.CreateDate);
                    break;
                default:
                    articles = articles.OrderByDescending(a => a.CreateDate);
                    break;
            }

            var model = new AllArticleViewModel()
            {
                Articles = articles.ToPagedList(pageNumber, pageSize),
                Sort = sort,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };

            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult MenuArticle()
        {
            var model = new MenuArticleViewModel
            {
                Articles = _unitOfWork.ArticleRepository.GetQuery(l => l.Active, a => a.OrderByDescending(c => c.CreateDate), 5),
                ArticleCategories = ArticleCategories.Where(a => a.ParentId == null),
            };
            return PartialView(model);
        }
        [Route("tim-kiem")]
        public ActionResult SearchArticle(int? page, string keywords, string sort)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var newkey = keywords.Trim();

            if (string.IsNullOrEmpty(newkey))
            {
                return RedirectToAction("Index");
            }

            var articles = _unitOfWork.ArticleRepository.GetQuery(l =>
                l.Active && l.Subject.Contains(newkey) && l.ArticleCategory.TypePost == TypePost.Article, c => c.OrderByDescending(a => a.CreateDate));

            switch (sort)
            {
                case "date-asc":
                    articles = articles.OrderBy(a => a.CreateDate);
                    break;
                default:
                    articles = articles.OrderByDescending(a => a.CreateDate);
                    break;
            }

            var model = new ArticleSearchViewModel
            {
                Articles = articles.ToPagedList(pageNumber, pageSize),
                Keywords = keywords,
                Categories = ArticleCategories.Where(a => a.TypePost == TypePost.Article),
                Sort = sort,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };

            return View(model);
        }
        public PartialViewResult GetSearchArticle(int? page, string keywords, string sort)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var newkey = keywords.Trim();
            var articles = _unitOfWork.ArticleRepository.GetQuery(l =>
                l.Active && l.Subject.Contains(newkey) && l.ArticleCategory.TypePost == TypePost.Article, c => c.OrderByDescending(a => a.CreateDate));

            switch (sort)
            {
                case "date-asc":
                    articles = articles.OrderBy(a => a.CreateDate);
                    break;
                default:
                    articles = articles.OrderByDescending(a => a.CreateDate);
                    break;
            }

            var model = new ArticleSearchViewModel
            {
                Articles = articles.ToPagedList(pageNumber, pageSize),
                Keywords = keywords,
                Sort = sort,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };

            return PartialView(model);
        }
        #endregion

        #region Product
        [Route("tour")]
        public ActionResult AllProduct(int? page, int price = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Sort));

            switch (price)
            {
                case 1:
                    products = products.Where(a => (a.PriceSale ?? a.Price) <= 1000000);
                    break;
                case 2:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 1000000 && (a.PriceSale ?? a.Price) <= 2000000);
                    break;
                case 3:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 2000000 && (a.PriceSale ?? a.Price) <= 3000000);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Count = products.Count(),
                Price = price,
            };

            return View(model);
        }
        public PartialViewResult GetProduct(int? page, int price = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var products = _unitOfWork.ProductRepository.GetQuery(l => l.Active, c => c.OrderBy(a => a.Sort)).AsNoTracking();

            switch (price)
            {
                case 1:
                    products = products.Where(a => (a.PriceSale ?? a.Price) <= 1000000);
                    break;
                case 2:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 1000000 && (a.PriceSale ?? a.Price) <= 2000000);
                    break;
                case 3:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 2000000 && (a.PriceSale ?? a.Price) <= 3000000);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Count = products.Count(),
                Price = price,
            };
            return PartialView(model);
        }
        [Route("{url:regex(^(?!.*(vcms|uploader|article|banner|contact|productvcms)).*$)}", Order = 2)]
        public ActionResult ProductCategory(int? page, string url, int price = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var category = ProductCategories.FirstOrDefault(a => a.Url == url);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && (p.ProductCategoryId == category.Id || p.ProductCategory.ParentId == category.Id),
                o => o.OrderByDescending(a => a.Sort));

            switch (price)
            {
                case 1:
                    products = products.Where(a => (a.PriceSale ?? a.Price) <= 1000000);
                    break;
                case 2:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 1000000 && (a.PriceSale ?? a.Price) <= 2000000);
                    break;
                case 3:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 2000000 && (a.PriceSale ?? a.Price) <= 3000000);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Category = category,
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Price = price,
                Count = products.Count()
            };
            return View(model);
        }
        public PartialViewResult GetProductCategory(int? page, string url, int price = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var category = ProductCategories.FirstOrDefault(a => a.Url == url);

            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && (p.ProductCategoryId == category.Id || p.ProductCategory.ParentId == category.Id),
                o => o.OrderByDescending(a => a.Sort));

            switch (price)
            {
                case 1:
                    products = products.Where(a => (a.PriceSale ?? a.Price) <= 1000000);
                    break;
                case 2:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 1000000 && (a.PriceSale ?? a.Price) <= 2000000);
                    break;
                case 3:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 2000000 && (a.PriceSale ?? a.Price) <= 3000000);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Category = category,
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Price = price,
                Count = products.Count()
            };

            return PartialView(model);
        }
        [Route("{url}.html", Order = 1)]
        public ActionResult ProductDetail(string url, string result)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(p => p.Url == url).FirstOrDefault();

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var products = _unitOfWork.ProductRepository.GetQuery(
                    p => p.Id != product.Id && p.Active && (p.ProductCategoryId == product.ProductCategoryId || p.ProductCategory.ParentId == product.ProductCategoryId),
                    o => o.OrderByDescending(p => Guid.NewGuid()), 8);

            var reviews = _unitOfWork.ReviewRepository.GetQuery(l => l.Active && l.ProductId == product.Id, c => c.OrderByDescending(a => a.CreateDate));
            var albumProducts = product.AlbumProducts.OrderBy(a => a.Sort).ToList();
            var services = product.Services.Where(a => a.Active).OrderBy(a => a.Sort);

            var model = new ProductDetailViewModel
            {
                Product = product,
                Products = products,
                ReviewCount = reviews.Count(),
                MoreImage = albumProducts.Count() - 5,
                LargeImage = albumProducts.FirstOrDefault(),
                AfterImages = albumProducts.Skip(1).Take(3),
                LastImage = albumProducts.Skip(4).FirstOrDefault(),
                AlbumProducts = albumProducts,
                Review = new Review { Active = false },
                Articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Subject), 4),
                Questions = product.Questions.Where(a => a.Active).OrderBy(a => a.Sort),
                FirstService = services.FirstOrDefault(),
                Services = services.Skip(1),
            };
            ViewBag.Result = result;
            return View(model);
        }
        [Route("{url}.html", Order = 1)]
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductDetail(ProductDetailViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Review.Avatar"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/reviews/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            model.Review.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                else
                {
                    var colorList = new List<string>() { "#4CAF50", "#0bba88", "#473db8", "#267cde", "#CF5555", "#FF6F22", "#7c32a1" };
                    var random = new Random().Next(0, colorList.Count);

                    model.Review.Color = colorList[random];
                }
                if (isPost)
                {
                    model.Review.Image = fc["Pictures"];
                    _unitOfWork.ReviewRepository.Insert(model.Review);
                    _unitOfWork.Save();
                    return RedirectToAction("ProductDetail", new { result = "success" });
                }
            }
            return View(model);
        }
        [Route("tim-san-pham")]
        public ActionResult SearchProduct(int? page, string keywords, int catId = 0, int price = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 2;

            if (string.IsNullOrEmpty(keywords))
            {
                return RedirectToAction("Index");
            }

            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.Name.Contains(keywords), o => o.OrderByDescending(a => a.Sort));

            if (catId > 0)
            {
                products = products.Where(p => p.ProductCategoryId == catId);
            }

            switch (price)
            {
                case 1:
                    products = products.Where(a => (a.PriceSale ?? a.Price) <= 1000000);
                    break;
                case 2:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 1000000 && (a.PriceSale ?? a.Price) <= 2000000);
                    break;
                case 3:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 2000000 && (a.PriceSale ?? a.Price) <= 3000000);
                    break;
            }

            var model = new SearchProductViewModel
            {
                Categories = ProductCategories,
                Products = products.ToPagedList(pageNumber, pageSize),
                Keywords = keywords,
                CatId = catId,
                Price = price,
                Count = products.Count(),
            };
            return View(model);
        }
        public PartialViewResult GetSearchProduct(int? page, string keywords, int catId = 0, int price = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 2;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.Name.Contains(keywords), o => o.OrderByDescending(a => a.Sort));

            if (catId > 0)
            {
                products = products.Where(p => p.ProductCategoryId == catId);
            }

            switch (price)
            {
                case 1:
                    products = products.Where(a => (a.PriceSale ?? a.Price) <= 1000000);
                    break;
                case 2:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 1000000 && (a.PriceSale ?? a.Price) <= 2000000);
                    break;
                case 3:
                    products = products.Where(a => (a.PriceSale ?? a.Price) >= 2000000 && (a.PriceSale ?? a.Price) <= 3000000);
                    break;
            }

            var model = new SearchProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Keywords = keywords,
                CatId = catId,
                Price = price,
                Count = products.Count()
            };
            return PartialView(model);
        }
        public PartialViewResult GetReview(int? page, int proId, string name)
        {
            var pageNumber = page ?? 1;
            var product = _unitOfWork.ProductRepository.GetById(proId);
            var reviews = product.Reviews.Where(a => a.Active).OrderByDescending(a => a.CreateDate);
            var count = reviews.Count();

            var model = new ProductDetailViewModel
            {
                Reviews = reviews.ToPagedList(pageNumber, 5),
                Review = new Review { Active = false },
                ReviewCount = reviews.Count(),
                Product = product
            };
            ViewBag.Name = name;
            return PartialView(model);
        }
        public PartialViewResult GetService(int serviceId)
        {
            var service = _unitOfWork.ServiceRepository.GetById(serviceId);
            if (service == null)
            {
                return null;
            }

            var model = new ProductDetailViewModel
            {
                FirstService = service,
            };

            return PartialView(model);
        }
        #endregion

        [Route("dat-tour/{name}")]
        public ActionResult BookTour(int? adultQuantity, int? childQuantity, int serviceId = 0, string startDate = "")
        {
            var service = _unitOfWork.ServiceRepository.GetById(serviceId);
            if (service == null || !service.Active)
            {
                return RedirectToActionPermanent("Index");
            }

            var priceAdult = service.AdultPrice * adultQuantity;
            var priceChild = service.ChildPrice * childQuantity;

            var model = new BookTourViewModel
            {
                ProducImg = service.Product.Image,
                ProductName = service.Product.Name,
                AdultQuantity = adultQuantity,
                ChildQuantity = childQuantity,
                AdultPrice = service.AdultPrice,
                ChildPrice = service.ChildPrice,
                DateGo = startDate,
                Order = new Order { OrderNumber = DateTime.Now.ToString("yyMMddHHmmss") },
                CartTotal = Convert.ToInt32(priceAdult + priceChild),
                ServiceId = serviceId,
                ServiceName = service.Name,
                Rating = service.Product.ResultReview(),
            };

            return View(model);
        }
        [Route("dat-tour/{name}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BookTour(BookTourViewModel model, FormCollection fc)
        {
            var service = _unitOfWork.ServiceRepository.GetById(model.ServiceId);
            if (service == null)
            {
                return RedirectToActionPermanent("Index");
            }

            var adultQuantity = fc["AdultQuantity"];
            var childQuantity = fc["ChildQuantity"];
            var cartTotal = Convert.ToInt32(fc["CartTotal"]);

            if (ModelState.IsValid)
            {
                if (UserId > 0)
                {
                    model.Order.OrderMemberId = UserId;
                }
                if (service.AdultPrice != null)
                {
                    model.Order.AdultQuantity = Convert.ToInt32(adultQuantity);
                    model.Order.AdultPrice = Convert.ToInt32(service.AdultPrice);
                }
                if (service.ChildPrice != null)
                {
                    model.Order.ChildQuantity = Convert.ToInt32(childQuantity);
                    model.Order.ChildPrice = Convert.ToInt32(service.ChildPrice);
                }

                model.Order.TransportDate = DateTime.TryParse(model.DateGo, new CultureInfo("Vi"), DateTimeStyles.None, out var tDate) ? tDate : DateTime.Now;
                model.Order.OrderNumber = DateTime.Now.ToString("yyMMddHHmmss");
                model.Order.ServiceId = model.ServiceId;
                model.Order.Total = cartTotal;
                model.Order.Status = 1;

                _unitOfWork.OrderRepository.Insert(model.Order);
                _unitOfWork.Save();

                var sb = "<p style='font-size:16px'>Thông tin đặt tour gửi từ website BatTrangTourist.vn</p>";
                sb += "<p>Mã đơn hàng: <strong>" + model.Order.OrderNumber + "</strong><br/>";
                sb += "Họ và tên: <strong>" + model.Order.CustomerInfo.Fullname + "</strong><br/>";
                sb += "Địa chỉ: <strong>" + model.Order.CustomerInfo.Address + "</strong><br/>";
                sb += "Email: <strong>" + model.Order.CustomerInfo.Email + "</strong><br/>";
                sb += "Điện thoại: <strong>" + model.Order.CustomerInfo.Mobile + "</strong><br/>";
                sb += "Yêu cầu thêm: <strong>" + model.Order.CustomerInfo.Body + "</strong><br/>";
                sb += "Ngày đặt tour: <strong>" + model.Order.CreateDate.ToString("dd-MM-yyyy HH:ss") + "</strong><br/>";
                sb += "Ngày khởi hành: <strong>" + model.Order.TransportDate.ToString("dd-MM-yyyy") + "</strong><br/>";
                sb += "Thông tin đặt tour</p>";
                sb += "<table border='1' cellpadding='10' style='border:1px #ccc solid;border-collapse: collapse'>" +
                      "<tr>" +
                      "<th>Ảnh Tour</th>" +
                      "<th>Tên tour</th>" +
                      "<th>Dịch vụ</th>" +
                      "<th>Thông tin</th>" +
                      "<th>Thành tiền</th>" +
                      "</tr>";

                var img = "NO PICTURE";
                if (service.Product.Image != null)
                {
                    img = "<img src='" + Request.Url?.GetLeftPart(UriPartial.Authority) + "/images/products/" + service.Product.Image + "?w=100' />";
                }
                string adult = null;
                if (model.Order.AdultQuantity > 0)
                {
                    adult = "<p>Người lớn: " + Convert.ToDecimal(service.AdultPrice).ToString("N0") + " x " + adultQuantity + "</p>";
                }
                string child = null;
                if (model.Order.ChildQuantity > 0)
                {
                    child = "<p>Trẻ em: " + Convert.ToDecimal(service.ChildPrice).ToString("N0") + " x " + childQuantity + "</p>";
                }
                sb += "<tr>" +
                      "<td>" + img + "</td>" +
                      "<td>" + service.Product.Name + "</td>" +
                      "<td>" + service.Name;

                sb += "</td>" +
                      "<td style='text-align:center'>" + adult + child + "</td>" +
                      "<td style='text-align:center'>" + cartTotal.ToString("N0") + "₫ </td>" +
                      "</tr>";
                sb += "<tr><td colspan='5' style='text-align:right'><strong>Tổng tiền: " + cartTotal.ToString("N0") + " ₫</strong></td></tr>";
                sb += "</table>";
                sb += "<p>Cảm ơn bạn đã tin tưởng và đặt tour của chúng tôi.<br/>";

                var orderId = DateTime.Now.ToString("yyMMddHHmmss");
                Task.Run(() => HtmlHelpers.SendEmail("gmail", "[" + orderId + "] Đơn đặt Tour từ website BatTrangTourist.vn", sb, model.Order.CustomerInfo.Email, Email, Email, Password, "Đặt Tour Online", model.Order.CustomerInfo.Email, ConfigSite.Email));

                return RedirectToAction("BookComplete", new { orderNumber = model.Order.OrderNumber });
            }

            return View(model);
        }
        [Route("dat-tour-thanh-cong")]
        public ActionResult BookComplete(string orderNumber)
        {
            ViewBag.OrderNumber = orderNumber;
            return View();
        }

        [Route("albums")]
        public ActionResult PhotoLibrary(int? page)
        {
            var pageNumber = page ?? 1;
            var photoLibraries = _unitOfWork.PhotoLibraryRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));

            var model = new PhotoLibraryViewModel()
            {
                PhotoLibraries = photoLibraries.ToPagedList(pageNumber, 12),
            };
            return View(model);
        }
        [Route("albums/{url}.html", Order = 1)]
        public ActionResult AlbumDetail(string url)
        {
            var photolibrary = _unitOfWork.PhotoLibraryRepository.GetQuery(a => a.Url == url && a.Active).FirstOrDefault();
            if (photolibrary == null)
            {
                return RedirectToAction("Index");
            }

            var model = new AlbumDetailViewModel
            {
                PhotoLibrary = photolibrary,
                PhotoLibraries = _unitOfWork.PhotoLibraryRepository.GetQuery(p => p.Active && p.Id != photolibrary.Id, q => q.OrderByDescending(a => a.CreateDate), 8),
            };
            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult FormLogin()
        {
            return PartialView();
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
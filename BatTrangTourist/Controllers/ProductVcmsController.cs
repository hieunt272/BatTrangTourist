using Helpers;
using BatTrangTourist.DAL;
using BatTrangTourist.Models;
using BatTrangTourist.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography.X509Certificates;
using Antlr.Runtime.Tree;

namespace BatTrangTourist.Controllers
{
    [Authorize]
    public class ProductVcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private IEnumerable<ProductCategory> ProductCategories =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private SelectList ParentProductCategoryList => new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");

        #region ProductCategory
        [ChildActionOnly]
        public ActionResult ListCategory()
        {
            var allcats = _unitOfWork.ProductCategoryRepository.Get(orderBy: q => q.OrderBy(a => a.CategorySort));
            return PartialView(allcats);
        }
        public ActionResult ProductCategory(string result = "")
        {
            ViewBag.ArticleCat = result;

            var model = new InsertProductCatViewModel
            {
                RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                ProductCategory = new ProductCategory { CategorySort = 1 }
            };

            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductCategory(InsertProductCatViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "ProductCategory.Image")
                    {
                        model.ProductCategory.Image = imgFile;
                    }
                }

                model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(null, model.ProductCategory.Url ?? model.ProductCategory.CategoryName);
                model.ProductCategory.ParentId = model.ParentId;
                _unitOfWork.ProductCategoryRepository.Insert(model.ProductCategory);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == model.ProductCategory.Url).Count();
                if (count > 1)
                {
                    model.ProductCategory.Url += "-" + model.ProductCategory.Id;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ProductCategory", new { result = "success" });
            }
            model.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(model);
        }
        public ActionResult UpdateCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            var parentCat = ProductCategories.FirstOrDefault(a => a.Id == category.ParentId);
            if (category == null)
            {
                return RedirectToAction("ProductCategory");
            }

            var model = new InsertProductCatViewModel
            {
                ParentId = 0,
                RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                ProductCategory = category
            };
            if (parentCat != null)
            {
                model.ParentId = Convert.ToInt32(parentCat.Id);
                if (parentCat.ParentId != null)
                {
                    model.ParentId = Convert.ToInt32(parentCat.ParentId);
                }
            }
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateCategory(InsertProductCatViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "ProductCategory.Image")
                    {
                        model.ProductCategory.Image = imgFile;
                    }
                }

                var file = Request.Files["ProductCategory.Image"];

                if (file != null && file.ContentLength == 0)
                {
                    model.ProductCategory.Image = fc["CurrentFile2"] == "" ? null : fc["CurrentFile2"];
                }

                model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(null, model.ProductCategory.Url ?? model.ProductCategory.CategoryName);
                model.ProductCategory.ParentId = model.ParentId;
                _unitOfWork.ProductCategoryRepository.Update(model.ProductCategory);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == model.ProductCategory.Url).Count();
                if (count > 1)
                {
                    model.ProductCategory.Url += "-" + model.ProductCategory.Id;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ProductCategory", new { result = "update" });
            }
            model.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(model);
        }
        [HttpPost]
        public bool DeleteCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            if (category == null)
            {
                return false;
            }
            _unitOfWork.ProductCategoryRepository.Delete(category);
            _unitOfWork.Save();
            return true;
        }
        public bool UpdateProductCat(int sort = 1, bool active = false, bool home = false, bool menu = false, int productCatId = 0)
        {
            var productCat = _unitOfWork.ProductCategoryRepository.GetById(productCatId);
            if (productCat == null)
            {
                return false;
            }
            productCat.CategorySort = sort;
            productCat.CategoryActive = active;
            productCat.ShowMenu = menu;
            productCat.Home = home;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Product
        public ActionResult ListProduct(int? page, string name, int? parentId, int catId = 0, int thirdId = 0, string sort = "sort-asc", string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var products = _unitOfWork.ProductRepository.GetQuery().AsNoTracking();

            if (thirdId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == thirdId);
            }
            else if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            else if (parentId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == parentId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(l => l.Name.Contains(name));
            }

            switch (sort)
            {
                case "date-desc":
                    products = products.OrderByDescending(a => a.CreateDate);
                    break;
                case "sort-desc":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "hot":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "date-asc":
                    products = products.OrderBy(a => a.CreateDate);
                    break;
                default:
                    products = products.OrderBy(a => a.Sort);
                    break;
            }
            var model = new ListProductViewModel
            {
                SelectCategories = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                Products = products.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                ParentId = parentId,
                ThirdId = thirdId,
                Name = name,
                Sort = sort
            };
            if (parentId.HasValue)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == parentId), "Id", "CategoryName");
            }
            if (catId > 0)
            {
                model.ThirdCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == catId), "Id", "CategoryName");
            }
            return View(model);
        }
        public ActionResult Product()
        {
            var model = new InsertProductViewModel
            {
                Product = new Product { Sort = 1, Active = true },
                Categories = ProductCategories,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Product(InsertProductViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                if (model.Price != null)
                {
                    model.Product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
                }
                if (model.PriceSale != null)
                {
                    model.Product.PriceSale = Convert.ToDecimal(model.PriceSale.Replace(",", ""));
                }

                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/products/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Product.Image")
                    {
                        model.Product.Image = imgFile;
                    }
                }

                model.Product.ImageReview = fc["Pictures"];
                model.Product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                model.Product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                _unitOfWork.ProductRepository.Insert(model.Product);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == model.Product.Url).Count();
                if (count > 1)
                {
                    model.Product.Url += "-" + DateTime.Now.Millisecond;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ListProduct", new { result = "success" });
            }

            model.Categories = ProductCategories;
            return View(model);
        }
        public ActionResult UpdateProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }

            var model = new InsertProductViewModel
            {
                Product = product,
                Categories = ProductCategories,
                Price = product.Price?.ToString("N0"),
                PriceSale = product.PriceSale?.ToString("N0")
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateProduct(InsertProductViewModel model, FormCollection fc)
        {
            var product = _unitOfWork.ProductRepository.GetById(model.Product.Id);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }

            if (ModelState.IsValid)
            {
                if (model.Price != null)
                {
                    product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
                }
                else
                {
                    product.Price = null;
                }
                if (model.PriceSale != null)
                {
                    product.PriceSale = Convert.ToDecimal(model.PriceSale.Replace(",", ""));
                }
                else
                {
                    product.PriceSale = null;
                }

                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/products/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Product.Image")
                    {
                        product.Image = imgFile;
                    }
                }

                var file = Request.Files["Product.Image"];

                if (file != null && file.ContentLength == 0)
                {
                    product.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                }

                product.ImageReview = fc["Pictures"];
                product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                product.Name = model.Product.Name;
                product.Body = model.Product.Body;
                product.About = model.Product.About;
                product.Note = model.Product.Note;
                product.Active = model.Product.Active;
                product.Favorite = model.Product.Favorite;
                product.Hot = model.Product.Hot;
                product.TitleMeta = model.Product.TitleMeta;
                product.DescriptionMeta = model.Product.DescriptionMeta;
                product.Sort = model.Product.Sort;
                product.Description = model.Product.Description;

                _unitOfWork.Save();

                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == product.Url).Count();
                if (count > 1)
                {
                    product.Url += "-" + DateTime.Now.Millisecond;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ListProduct", new { result = "update" });
            }

            model.Categories = ProductCategories;
            return View(model);
        }
        [HttpPost]
        public bool DeleteProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            _unitOfWork.ProductRepository.Delete(product);
            _unitOfWork.Save();
            return true;
        }
        public bool QuickUpdate(int sort = 1, int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            product.Sort = sort;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region AlbumProduct
        public ActionResult AlbumProduct(int proId)
        {
            var albumProducts = _unitOfWork.AlbumProductRepository.Get(a => a.ProductId == proId);
            var quantity = 10;
            var count = albumProducts.Count();
            var model = new InsertAlbumProductViewModel
            {
                AlbumProducts = albumProducts,
                ProId = proId,
                AlbumProduct = new AlbumProduct { Quantity = quantity }
            };
            if (count > 0)
            {
                model.AlbumProduct.Quantity = count;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AlbumProduct(InsertAlbumProductViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var albumProducts = _unitOfWork.AlbumProductRepository.Get(a => a.ProductId == model.ProId);

                var pictures = fc.GetValues("Pictures_insert");
                var titleImages = fc.GetValues("TitleImage");

                var id = fc.GetValues("Id");
                var picturesUpdate = fc.GetValues("Pictures_update");
                var titleImageUpdate = fc.GetValues("TitleImageUpdate");

                if (!albumProducts.Any())
                {
                    for (var i = 0; i < pictures.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(pictures[i]))
                        {
                            var albumProduct = new AlbumProduct
                            {
                                ProductId = model.ProId,
                                TitleImage = titleImages[i],
                                Sort = i,
                                Image = pictures[i],
                                Quantity = model.AlbumProduct.Quantity,
                            };
                            _unitOfWork.AlbumProductRepository.Insert(albumProduct);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < albumProducts.Count(); i++)
                    {
                        var _id = Convert.ToInt32(id[i]);
                        var albumProduct = _unitOfWork.AlbumProductRepository.GetById(_id);

                        if (!string.IsNullOrEmpty(picturesUpdate[i]))
                        {
                            albumProduct.ProductId = model.ProId;
                            albumProduct.TitleImage = titleImageUpdate[i];
                            albumProduct.Quantity = model.AlbumProduct.Quantity;
                            albumProduct.Sort = i;
                            albumProduct.Image = picturesUpdate[i];
                            _unitOfWork.Save();
                        }
                        else
                        {
                            _unitOfWork.AlbumProductRepository.Delete(albumProduct);
                            _unitOfWork.Save();
                        }
                    }
                    if (pictures != null)
                    {
                        for (var i = 0; i < pictures.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(pictures[i]))
                            {
                                var albumProduct = new AlbumProduct
                                {
                                    ProductId = model.ProId,
                                    TitleImage = titleImages[i],
                                    Quantity = model.AlbumProduct.Quantity,
                                    Sort = i + albumProducts.Count(),
                                    Image = pictures[i],
                                };
                                _unitOfWork.AlbumProductRepository.Insert(albumProduct);
                            }
                        }
                    }
                }
                _unitOfWork.Save();
                return RedirectToAction("ListProduct");
            }
            return View(model);
        }
        #endregion

        #region Review
        public ActionResult ListReview(int? page, int productId = 0)
        {
            var pageNumber = page ?? 1;
            const int pageSize = 10;
            var reviews = _unitOfWork.ReviewRepository.GetQuery(orderBy: q => q.OrderByDescending(a => a.CreateDate));
            if (productId > 0)
            {
                reviews = reviews.Where(a => a.ProductId == productId);
            }

            var model = new ListReviewViewModel
            {
                Reviews = reviews.ToPagedList(pageNumber, pageSize),
                Products = _unitOfWork.ProductRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                ProductId = productId,
            };
            return View(model);
        }
        [HttpPost]
        public bool DeleteReview(int reviewId = 0)
        {

            var review = _unitOfWork.ReviewRepository.GetById(reviewId);
            if (review == null)
            {
                return false;
            }
            _unitOfWork.ReviewRepository.Delete(review);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool UpdateReview(bool active = false, int reviewId = 0)
        {
            var review = _unitOfWork.ReviewRepository.GetById(reviewId);
            if (review == null)
            {
                return false;
            }

            review.Active = active;
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Question
        [ChildActionOnly]
        public PartialViewResult ListQuestion()
        {
            var questions = _unitOfWork.QuestionRepository.Get(orderBy: q => q.OrderBy(a => a.Sort));
            return PartialView(questions);
        }
        public ActionResult AddOrUpdateQuestion(int? questionId, int result = 0, int proId = 0)
        {
            ViewBag.Result = result;
            var question = new Question
            {
                Sort = 1,
                ProductId = proId
            };
            if (questionId.HasValue)
            {
                question = _unitOfWork.QuestionRepository.GetById(questionId);
                if (question == null)
                {
                    return RedirectToAction("AddOrUpdateQuestion");
                }
            }
            return View(question);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult AddOrUpdateQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.QuestionRepository.AddOrUpdate(question);
                _unitOfWork.Save();

                return RedirectToAction("AddOrUpdateQuestion", new { proId = question.ProductId, result = 1 });
            }
            return View(question);
        }
        [HttpPost]
        public bool DeleteQuestion(int questionId = 0)
        {
            var question = _unitOfWork.QuestionRepository.GetById(questionId);
            if (question == null)
            {
                return false;
            }

            _unitOfWork.QuestionRepository.Delete(question);
            _unitOfWork.Save();

            return true;
        }
        #endregion

        #region Service
        public ActionResult ListService(int? page, int productId = 0, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 10;
            var services = _unitOfWork.ServiceRepository.GetQuery(orderBy: q => q.OrderBy(a => a.Sort));
            if (productId > 0)
            {
                services = services.Where(a => a.ProductId == productId);
            }

            var model = new ListServiceViewModel
            {
                Services = services.ToPagedList(pageNumber, pageSize),
                Products = _unitOfWork.ProductRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                ProductId = productId,
            };
            return View(model);
        }
        public ActionResult Service()
        {
            var model = new InsertServiceViewModel
            {
                Service = new Service { Sort = 1, Active = true },
                Products = _unitOfWork.ProductRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate)),
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Service(InsertServiceViewModel model, FormCollection fc)
        {
            if (model.ProductId == 0)
            {
                ModelState.AddModelError("", @"Hãy chọn Tour");
            }
            if (ModelState.IsValid)
            {
                if (model.AdultPrice != null)
                {
                    model.Service.AdultPrice = Convert.ToDecimal(model.AdultPrice.Replace(",", ""));
                }
                if (model.ChildPrice != null)
                {
                    model.Service.ChildPrice = Convert.ToDecimal(model.ChildPrice.Replace(",", ""));
                }

                model.Service.ProductId = model.ProductId;
                _unitOfWork.ServiceRepository.Insert(model.Service);
                _unitOfWork.Save();

                var tags = fc["hidden-Tags"];
                if (!string.IsNullOrEmpty(tags))
                {
                    model.Service.Tags = new List<Tag>();
                    var items = tags.Split(',');
                    foreach (var item in items)
                    {
                        var tagWord = item;
                        var tag = new Tag { Name = tagWord.Trim() };
                        var tagExist = _unitOfWork.TagRepository.Get(a => a.Name == tagWord).FirstOrDefault();
                        if (tagExist == null)
                        {
                            model.Service.Tags.Add(tag);
                        }
                        else
                        {
                            tagExist.Services.Add(model.Service);
                        }
                    }
                    _unitOfWork.Save();
                }

                return RedirectToAction("ListService", new { result = "success" });
            }

            model.Products = _unitOfWork.ProductRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            return View(model);
        }
        public ActionResult UpdateService(int serviceId = 0)
        {
            var service = _unitOfWork.ServiceRepository.GetById(serviceId);
            if (service == null)
            {
                return RedirectToAction("ListService");
            }
            var tags = _unitOfWork.TagRepository.Get(a => a.Services.Any(t => t.Id == serviceId)).ToList();
            var allTag = "";
            var i = 1;
            foreach (var tag in tags)
            {
                allTag += i == tags.Count ? tag.Name : tag.Name + ", ";
                i++;
            }
            var model = new InsertServiceViewModel
            {
                Service = service,
                Products = _unitOfWork.ProductRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate)),
                AdultPrice = service.AdultPrice?.ToString("N0"),
                ChildPrice = service.ChildPrice?.ToString("N0"),
                Tags = allTag
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateService(InsertServiceViewModel model, FormCollection fc)
        {
            var service = _unitOfWork.ServiceRepository.GetById(model.Service.Id);
            if (service == null)
            {
                return RedirectToAction("ListService");
            }

            if (model.ProductId == 0)
            {
                ModelState.AddModelError("", @"Hãy chọn Tour");
            }

            model.Tags = fc["hidden-Tags"];
            if (ModelState.IsValid)
            {
                if (model.AdultPrice != null)
                {
                    service.AdultPrice = Convert.ToDecimal(model.AdultPrice.Replace(",", ""));
                }
                else
                {
                    service.AdultPrice = null;
                }
                if (model.ChildPrice != null)
                {
                    service.ChildPrice = Convert.ToDecimal(model.ChildPrice.Replace(",", ""));
                }
                else
                {
                    service.ChildPrice = null;
                }

                service.ProductId = model.ProductId;
                service.Name = model.Service.Name;
                service.Schedule = model.Service.Schedule;
                service.Include = model.Service.Include;
                service.Note = model.Service.Note;
                service.Terms = model.Service.Terms;
                service.Manual = model.Service.Manual;
                service.Sort = model.Service.Sort;
                service.Active = model.Service.Active;

                if (service.Tags.Any())
                {
                    service.Tags.Clear();
                }
                _unitOfWork.Save();

                var tags = fc["hidden-Tags"];
                if (!string.IsNullOrEmpty(tags))
                {
                    var items = tags.Split(',');
                    foreach (var item in items)
                    {
                        var tagWord = item;
                        var tag = new Tag { Name = tagWord.Trim() };
                        var tagExist = _unitOfWork.TagRepository.GetQuery(a => a.Name == tagWord).FirstOrDefault();
                        if (tagExist == null)
                        {
                            service.Tags.Add(tag);
                        }
                        else
                        {
                            tagExist.Services.Add(service);
                        }
                    }

                    _unitOfWork.Save();
                }

                return RedirectToAction("ListService", new { result = "update" });
            }

            model.Products = _unitOfWork.ProductRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            return View(model);
        }
        [HttpPost]
        public bool DeleteService(int serviceId)
        {
            var service = _unitOfWork.ServiceRepository.GetById(serviceId);
            if (service == null)
            {
                return false;
            }
            _unitOfWork.ServiceRepository.Delete(service);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Order
        public ActionResult ListOrder(int? page, int status = -1)
        {
            var pageNumber = page ?? 1;
            const int pageSize = 20;
            var orders = _unitOfWork.OrderRepository.GetQuery(orderBy: q => q.OrderByDescending(a => a.CreateDate)).AsNoTracking();
            if (status >= 0)
            {
                orders = orders.Where(a => a.Status == status);
            }
            ViewBag.Status = status;
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult LoadOrder(int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);

            var model = new OrderDetailModel
            {
                Order = order,
                Service = order.Service,
            };
            return PartialView(model);
        }
        [HttpPost]
        public bool UpdateOrder(int status = 0, int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = status;
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool DeleteOrder(int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = 3;
            _unitOfWork.Save();
            return true;
        }
        #endregion

        public JsonResult GetProductCategory(int? parentId)
        {
            var categories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(categories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChildCategory(int parentId = 0)
        {
            var childCategories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(childCategories.Select(a => new { a.Id, a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTour(int categoryId = 0)
        {
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.ProductCategoryId == categoryId);
            return Json(products.Select(a => new { a.Id, a.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTags(string term)
        {
            var tags = _unitOfWork.TagRepository.GetQuery(a => a.Name.Contains(term));
            return Json(tags.Select(a => new { label = a.Name }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
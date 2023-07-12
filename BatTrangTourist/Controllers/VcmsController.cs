using Helpers;
using BatTrangTourist.DAL;
using BatTrangTourist.Models;
using BatTrangTourist.ViewModel;
using PagedList;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

namespace BatTrangTourist.Controllers
{
    [Authorize]
    public class VcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(AdminLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.Get(a => a.Username == model.Username && a.Active).SingleOrDefault();

                if (admin != null && HtmlHelpers.VerifyHash(model.Password, "SHA256", admin.Password))
                {
                    var ticket = new FormsAuthenticationTicket(1, model.Username.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        admin.ToString(), FormsAuthentication.FormsCookiePath);

                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    // Create the cookie.
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Vcms");
                }
                ModelState.AddModelError("", @"Tên đăng nhập hoặc mật khẩu không chính xác.");
            }
            return View(model);
        }
        public RedirectToRouteResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Vcms");
        }
        #endregion

        #region Admin
        public ActionResult Index()
        {
            var model = new InfoAdminViewModel
            {
                Admins = _unitOfWork.AdminRepository.GetQuery().Count(),
                Articles = _unitOfWork.ArticleRepository.GetQuery().Count(),
                Contacts = _unitOfWork.ContactRepository.GetQuery().Count(),
                Banners = _unitOfWork.BannerRepository.GetQuery().Count(),
                Products = _unitOfWork.ProductRepository.GetQuery().Count(),
                Orders = _unitOfWork.OrderRepository.GetQuery().Count(),
            };
            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult ListAdmin()
        {
            var admins = _unitOfWork.AdminRepository.Get();
            return PartialView("ListAdmin", admins);
        }
        public ActionResult CreateAdmin(string result = "")
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost]
        public ActionResult CreateAdmin(Admin model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                if (admin != null)
                {
                    ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                }
                else
                {
                    var hashPass = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.AdminRepository.Insert(new Admin { Username = model.Username, Password = hashPass, Active = model.Active });
                    _unitOfWork.Save();
                    return RedirectToAction("CreateAdmin", new { result = "success" });
                }
            }
            return View();
        }
        public ActionResult EditAdmin(int adminId = 0)
        {
            var admin = _unitOfWork.AdminRepository.GetById(adminId);
            if (admin == null)
            {
                return RedirectToAction("CreateAdmin");
            }

            var model = new EditAdminViewModel
            {
                Id = admin.Id,
                Username = admin.Username,
                Active = admin.Active,
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult EditAdmin(EditAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetById(model.Id);
                if (admin == null)
                {
                    return RedirectToAction("CreateAdmin");
                }
                if (admin.Username != model.Username)
                {
                    var exists = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                    if (exists != null)
                    {
                        ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                        return View(model);
                    }
                    admin.Username = model.Username;
                }
                admin.Active = model.Active;
                if (model.Password != null)
                {
                    admin.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                }
                _unitOfWork.Save();
                return RedirectToAction("CreateAdmin", new { result = "update" });
            }
            return View(model);
        }
        public bool DeleteAdmin(string username)
        {
            var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(username)).SingleOrDefault();
            if (admin == null)
            {
                return false;
            }
            if (username == "admin")
            {
                return false;
            }
            _unitOfWork.AdminRepository.Delete(admin);
            _unitOfWork.Save();
            return true;
        }
        public ActionResult ChangePassword(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(User.Identity.Name,
                StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (admin == null)
                {
                    return HttpNotFound();
                }
                if (HtmlHelpers.VerifyHash(model.OldPassword, "SHA256", admin.Password))
                {
                    admin.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.Save();
                    return RedirectToAction("ChangePassword", new { result = 1 });
                }
                else
                {
                    ModelState.AddModelError("", @"Mật khẩu hiện tại không đúng!");
                    return View();
                }
            }
            return View(model);
        }
        #endregion

        #region ConfigSite
        public ActionResult ConfigSite(string result = "")
        {
            ViewBag.Result = result;
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();

            return View(config);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ConfigSite(ConfigSite model, FormCollection fc)
        {
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();
            if (config == null)
            {
                _unitOfWork.ConfigSiteRepository.Insert(model);
            }
            else
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/configs/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Image")
                    {
                        config.Image = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "Favicon")
                    {
                        config.Favicon = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "AboutImage")
                    {
                        config.AboutImage = imgFile;
                    }
                }

                config.ListImage = fc["Pictures"];
                config.Facebook = model.Facebook;
                config.GoogleMap = model.GoogleMap;
                config.Youtube = model.Youtube;
                config.Instagram = model.Instagram;
                config.Twitter = model.Twitter;
                config.Telegram = model.Telegram;
                config.Title = model.Title;
                config.Description = model.Description;
                config.GoogleAnalytics = model.GoogleAnalytics;
                config.Hotline = model.Hotline;
                config.Email = model.Email;
                config.LiveChat = model.LiveChat;
                config.Place = model.Place;
                config.AboutText = model.AboutText;
                config.InfoFooter = model.InfoFooter;
                config.InfoContact = model.InfoContact;
                config.AboutUrl = model.AboutUrl;
                config.BankInfo = model.BankInfo;
                config.VideoUrl = model.VideoUrl;

                if (model.Zalo != null)
                {
                    config.Zalo = model.Zalo.Replace(" ", string.Empty);
                }

                _unitOfWork.Save();

                HttpContext.Application["ConfigSite"] = config;
                return RedirectToAction("ConfigSite", "Vcms", new { result = "success" });
            }
            return View("ConfigSite", model);
        }
        #endregion

        #region User
        public ActionResult ListUser(int? page, string name, string result)
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var users = _unitOfWork.UserRepository.Get(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(a => a.Username.Contains(name));
            }
            var model = new ListUserViewModel
            {
                Users = users.ToPagedList(pageNumber, pageSize),
                Name = name,
            };
            return View(model);
        }
        public ActionResult EditUser(int userId = 0)
        {
            var user = _unitOfWork.UserRepository.GetById(userId);
            if (user == null)
            {
                return RedirectToAction("ListUser");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Active = user.Active,
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.GetById(model.Id);
                if (user == null)
                {
                    return RedirectToAction("ListUser");
                }
                if (user.Username != model.Username)
                {
                    var exists = _unitOfWork.UserRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                    if (exists != null)
                    {
                        ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                        return View(model);
                    }
                    user.Username = model.Username;
                }
                user.Active = model.Active;
                if (model.Password != null)
                {
                    user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                }
                _unitOfWork.Save();
                return RedirectToAction("ListUser", new { result = "update" });
            }
            return View(model);
        }
        public bool DeleteUser(int userId)
        {
            var user = _unitOfWork.UserRepository.GetById(userId);
            if (user == null)
            {
                return false;
            }
            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region PhotoLibrary
        public ActionResult ListPhotoLibrary(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 10;
            var photolibraries = _unitOfWork.PhotoLibraryRepository.Get(orderBy: l => l.OrderBy(a => a.Sort));

            if (!string.IsNullOrEmpty(name))
            {
                photolibraries = photolibraries.Where(l => l.AlbumName.Contains(name.Trim()));
            }

            var model = new ListPhotoLibrary
            {
                Photolibraries = photolibraries.ToPagedList(pageNumber, pageSize),
                Name = name,
            };
            return View(model);
        }
        public ActionResult PhotoLibrary(string result = "")
        {
            ViewBag.Result = result;

            var model = new InsertPhotoLibrary
            {
                Photolibrary = new PhotoLibrary { Sort = 1 }
            };

            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PhotoLibrary(InsertPhotoLibrary model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.Photolibrary.ListImage = fc["Pictures"];

                model.Photolibrary.Url = HtmlHelpers.ConvertToUnSign(null, model.Photolibrary.Url ?? model.Photolibrary.AlbumName);
                var photolibraries = _unitOfWork.PhotoLibraryRepository.GetQuery().AsNoTracking();
                if (photolibraries.Any(p => p.Url.Trim() == model.Photolibrary.Url.Trim()))
                {
                    model.Photolibrary.Url += "-" + DateTime.Now.Millisecond;
                }
                _unitOfWork.PhotoLibraryRepository.Insert(model.Photolibrary);
                _unitOfWork.Save();

                return RedirectToAction("ListPhotoLibrary", new { result = "success" });
            }
            return View(model);
        }
        public ActionResult UpdatePhotoLibrary(int photoId = 0)
        {
            var photoLibrary = _unitOfWork.PhotoLibraryRepository.GetById(photoId);
            if (photoLibrary == null)
            {
                return RedirectToAction("ListPhotoLibrary");
            }
            var model = new InsertPhotoLibrary
            {
                Photolibrary = photoLibrary,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdatePhotoLibrary(InsertPhotoLibrary model, FormCollection fc)
        {
            var photoLibrary = _unitOfWork.PhotoLibraryRepository.GetById(model.Photolibrary.Id);
            if (photoLibrary == null)
            {
                return RedirectToAction("ListPhotoLibrary");
            }
            if (ModelState.IsValid)
            {
                photoLibrary.ListImage = fc["Pictures"] == "" ? null : fc["Pictures"];
                photoLibrary.Url = HtmlHelpers.ConvertToUnSign(null, model.Photolibrary.Url ?? model.Photolibrary.AlbumName);
                photoLibrary.AlbumName = model.Photolibrary.AlbumName;
                photoLibrary.Sort = model.Photolibrary.Sort;
                photoLibrary.Active = model.Photolibrary.Active;
                photoLibrary.Body = model.Photolibrary.Body;

                var photolibraries = _unitOfWork.PhotoLibraryRepository.GetQuery().AsNoTracking();
                if (photolibraries.Any(p => p.Url.ToLower().Trim() == model.Photolibrary.Url.ToLower().Trim() && p.Id != model.Photolibrary.Id))
                {
                    photoLibrary.Url += "-" + DateTime.Now.Millisecond;
                }

                _unitOfWork.Save();

                return RedirectToAction("ListPhotoLibrary", new { result = "update" });
            }
            return View(model);
        }
        [HttpPost]
        public bool DeletePhotoLibrary(int photoId = 0)
        {
            var photolibrary = _unitOfWork.PhotoLibraryRepository.GetById(photoId);
            if (photolibrary == null)
            {
                return false;
            }
            _unitOfWork.PhotoLibraryRepository.Delete(photolibrary);
            _unitOfWork.Save();
            return true;
        }
        public bool QuickUpdate(int sort = 1, bool active = false, int photoId = 0)
        {
            var photolibrary = _unitOfWork.PhotoLibraryRepository.GetById(photoId);
            if (photolibrary == null)
            {
                return false;
            }
            photolibrary.Sort = sort;
            photolibrary.Active = active;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
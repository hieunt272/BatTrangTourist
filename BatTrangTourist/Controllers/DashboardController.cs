using Helpers;
using BatTrangTourist.DAL;
using BatTrangTourist.Filters;
using BatTrangTourist.Models;
using BatTrangTourist.ViewModel;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Hytyky.Controllers
{
    [MemberLoginFilter, RoutePrefix("dashboard")]
    public class DashboardController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private string Username => RouteData.Values["Username"].ToString();
        private string Email => RouteData.Values["Email"].ToString();
        private new User User => _unitOfWork.UserRepository.Get(a => (a.Email == Email || a.Username == Username)).SingleOrDefault();


        [ChildActionOnly]
        public PartialViewResult UserNav()
        {
            return PartialView(User);
        }
        #region UserInfo
        [Route("thong-tin-ca-nhan")]
        public ActionResult UserInfo(string result = "")
        {
            ViewBag.Result = result;

            if (User == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _unitOfWork.UserRepository.GetQuery(a => a.Id == User.Id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
        [Route("thong-tin-ca-nhan")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserInfo(User model)
        {
            var user = _unitOfWork.UserRepository.Get(a => a.Id == User.Id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Avatar"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        isPost = false;
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            isPost = false;
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/users/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            model.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                            user.Avatar = model.Avatar;
                        }
                    }

                }
                if (isPost)
                {
                    user.FullName = model.FullName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    _unitOfWork.Save();

                    var userData = user.Username + "|" + user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.Color;
                    var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);

                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    // Create the cookie.
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));

                    return RedirectToAction("UserInfo", "Dashboard", new { result = "success" });
                }
            }
            return View(model);
        }
        [Route("doi-mat-khau")]
        public ActionResult ChangePassword(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost, Route("doi-mat-khau"), ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.GetById(User.Id);
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (HtmlHelpers.VerifyHash(model.OldPassword, "SHA256", user.Password))
                {
                    user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
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

        #region UserOrder        
        [Route("don-hang-cua-toi")]
        public ActionResult UserOrder()
        {
            return View();
        }
        public PartialViewResult GetUserOrder(int status = 1)
        {
            var orders = _unitOfWork.OrderRepository.GetQuery(a => a.OrderMemberId == User.Id && a.Status == status, o => o.OrderBy(a => a.Id));

            var model = new UserOrderViewModel
            {
                Orders = orders,
            };

            return PartialView(model);
        }
        [HttpPost]
        public bool CancelOrder(int orderId = 0)
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

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
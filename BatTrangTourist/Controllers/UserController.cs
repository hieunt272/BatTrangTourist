using Helpers;
using BatTrangTourist.DAL;
using BatTrangTourist.Filters;
using BatTrangTourist.Models;
using BatTrangTourist.ViewModel;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Collections.Generic;

namespace BatTrangTourist.Controllers
{
    [MemberFilter, RoutePrefix("user")]
    public class UserController : Controller
    {
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private int UserId => Convert.ToInt32(RouteData.Values["UserId"]);

        [OverrideActionFilters, Route("dang-ky")]
        public ActionResult Register()
        {
            return View();
        }
        [Route("dang-ky")]
        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var phoneVal = model.PhoneNumber.Trim();

                var checkUser = _unitOfWork.UserRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                if (checkUser != null)
                {
                    ModelState.AddModelError("", @"Tên đăng nhập đã tồn tại!! Vui lòng nhập tên đăng nhập khác");
                }
                else
                {
                    var hashPass = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);

                    var colorList = new List<string>() { "#4CAF50", "#0bba88", "#473db8", "#267cde", "#CF5555", "#FF6F22", "#7c32a1" };
                    var random = new Random().Next(0, colorList.Count);

                    var user = new User
                    {
                        FullName = model.Username,
                        Username = model.Username,
                        Password = hashPass,
                        Email = model.Email,
                        PhoneNumber = phoneVal,
                        Active = true,
                        Color = colorList[random]
                    };
                    _unitOfWork.UserRepository.Insert(user);
                    _unitOfWork.Save();
                    var userData = user.Username + "|" + user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.Color;
                    var ticket = new FormsAuthenticationTicket(2, model.Username.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);

                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    // Create the cookie.
                    Response.Cookies.Add(new HttpCookie(".MEMBERAUTH", encTicket));
                    //if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    //    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
        [OverrideActionFilters, Route("dang-nhap")]
        public ActionResult Login(string result = "")
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost, Route("dang-nhap"), OverrideActionFilters]
        public ActionResult Login(UserLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.Get(a => a.Username == model.Username).SingleOrDefault();
                if (user == null || !HtmlHelpers.VerifyHash(model.Password, "SHA256", user.Password))
                {
                    ModelState.AddModelError("", @"Tên đăng nhập hoặc mật khẩu không chính xác.");
                    return View(model);
                }
                if (!user.Active)
                {
                    ModelState.AddModelError("", @"Tài khoản tạm thời bị khóa. Vui lòng liên hệ với quản trị viên.");
                    return View(model);
                }
                var userData = user.Username + "|" + user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.Color;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);

                var encTicket = FormsAuthentication.Encrypt(ticket);
                // Create the cookie.
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        public RedirectToRouteResult LogOut()
        {
            var cookie = Request.Cookies[".ASPXAUTHMEMBER"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Home");
        }
        [OverrideActionFilters, Route("quen-mat-khau")]
        public ActionResult ForgotPassword(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }
        [Route("quen-mat-khau")]
        [OverrideActionFilters , HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = _unitOfWork.UserRepository.GetQuery(a => a.Email == model.Email).SingleOrDefault();
                if (email == null)
                {
                    ModelState.AddModelError("", @"Email không chính xác.");
                    return View(model);
                }

                var setNewPasswordUrl = Request.Url?.GetLeftPart(UriPartial.Authority) + Url.Action("SetNewPassword", new { token = email.Token });
                var emailTemp = System.IO.File.ReadAllText(Server.MapPath("/EmailTemplates/ForgotPassword.html"));
                emailTemp = emailTemp.Replace("[FULLNAME]", email.Username).Replace("[USERNAME]", email.Username).Replace("[URL]", setNewPasswordUrl);

                Task.Run(() => HtmlHelpers.SendEmail("gmail", "Yêu cầu lấy lại mật khẩu từ BatTrangTourist.vn", emailTemp, model.Email, Email, Email, Password, "BatTrangTourist.vn"));

                return RedirectToAction("ForgotPassword", new { result = 1 });
            }
            return View(model);
        }
        [OverrideActionFilters, Route("dat-mat-khau-moi")]
        public ActionResult SetNewPassword(string token, int result = 0)
        {
            ViewBag.Result = result;

            var model = new SetNewPasswordViewModel
            {
                Token = token
            };

            if (result == 0)
            {
                var user = _unitOfWork.UserRepository.GetQuery(a => a.Token == token).SingleOrDefault();
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                model.Username = user.Username;
            }
            return View(model);
        }
        [Route("dat-mat-khau-moi")]
        [OverrideActionFilters, HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetNewPassword(SetNewPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.GetQuery(a => a.Token == model.Token).SingleOrDefault();
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                user.Token = HtmlHelpers.RandomCode(50);
                _unitOfWork.Save();

                return RedirectToAction("SetNewPassword", new { result = 1 });
            }
            return View(model);
        }

        [Route("doi-mat-khau")]
        public ActionResult UserChangePassword(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost, Route("doi-mat-khau")]
        public ActionResult UserChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.GetById(UserId);
                if (user == null)
                {
                    return RedirectToAction("ListUser", "Vcms");
                }
                if (HtmlHelpers.VerifyHash(model.OldPassword, "SHA256", user.Password))
                {
                    user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.Save();
                    return RedirectToAction("UserChangePassword", new { result = 1 });
                }
                else
                {
                    ModelState.AddModelError("", @"Mật khẩu hiện tại không đúng!");
                    return View();
                }
            }
            return View(model);
        }

        [OverrideActionFilters]
        public JsonResult CheckEmail(string email)
        {
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Email == email).SingleOrDefault();

            if (user == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("Email đã được sử dụng, vui lòng thử lại", JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
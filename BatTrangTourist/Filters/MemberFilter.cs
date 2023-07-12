using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace BatTrangTourist.Filters
{
    public class MemberFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies[".ASPXAUTHMEMBER"];
            if (cookie == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {{"action", "Login"}, {"controller", "User"}});
            }
            else
            {
                var ticketInfo = FormsAuthentication.Decrypt(cookie.Value);
                if (ticketInfo != null)
                {
                    var data = ticketInfo.UserData;
                    var arrData = data.Split('|');
                    filterContext.RouteData.Values["UserName"] = arrData[0];
                    if (arrData.Length > 1)
                    {
                        filterContext.RouteData.Values["Avatar"] = arrData[1];
                    }
                    if (arrData.Length > 2)
                    {
                        filterContext.RouteData.Values["Id"] = arrData[2];
                    }
                    filterContext.RouteData.Values["Email"] = ticketInfo.Name;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class MemberLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies[".ASPXAUTHMEMBER"];
            if (cookie == null)
            {
                filterContext.RouteData.Values["Username"] = "";
                filterContext.RouteData.Values["Avatar"] = "";
                filterContext.RouteData.Values["Id"] = null;
                filterContext.RouteData.Values["Email"] = "";
                filterContext.RouteData.Values["Color"] = "";
            }
            else
            {
                var ticketInfo = FormsAuthentication.Decrypt(cookie.Value);
                if (ticketInfo != null)
                {
                    var arrData = ticketInfo.UserData.Split('|');

                    filterContext.RouteData.Values["Username"] = arrData[0];
                    if (arrData.Length > 1)
                    {
                        filterContext.RouteData.Values["Avatar"] = arrData[1];
                    }
                    if (arrData.Length > 2)
                    {
                        filterContext.RouteData.Values["Id"] = arrData[2];
                    }
                    if (arrData.Length > 4)
                    {
                        filterContext.RouteData.Values["Color"] = arrData[4];
                    }
                    filterContext.RouteData.Values["Email"] = ticketInfo.Name;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
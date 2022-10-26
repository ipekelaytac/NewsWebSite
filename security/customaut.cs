using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newswebsite.security
{
    public class CustomAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["login"] == null || string.IsNullOrEmpty(filterContext.HttpContext.Session["login"].ToString()))
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "controller", "admin" }, { "action", "login" } });

            }
            base.OnActionExecuting(filterContext);
        }
    }
}